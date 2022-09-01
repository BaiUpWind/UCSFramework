using DeviceConfig;
using DeviceConfig.Core;
using DisplayBorder.ViewModel;
using HandyControl.Controls;
using HandyControl.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ComboBox = System.Windows.Controls.ComboBox;
using TextBox = System.Windows.Controls.TextBox;
using Window = HandyControl.Controls.Window;

namespace DisplayBorder.View
{
    /// <summary>
    /// OperationControl.xaml 的交互逻辑
    /// </summary>
    public partial class WindowOperation : Window 
 
    {
        public WindowOperation()
        {
            InitializeComponent();
            uniforms[0] = sv1;
            uniforms[1] = sv2;
            //Growl.SetGrowlParent(this,true);
          
        }

        private OperationViewModel operationView;
        private UniformSpacingPanel[] uniforms = new UniformSpacingPanel[2];
        private DeviceInfo temp;


        public event Action OnEnter;
        public event Action OnCancel;

        public void Init(OperationBase operation,DeviceInfo info = null)
        {
            temp = info;
            CreateNew(operation);
         
        }

        public OperationBase GetResult() => operationView.Operation;
         
     
        public void Clear()
        {
            sv1.Children.Clear();
            sv2.Children.Clear();
            operationView.Conn = null;
            operationView.Command = null;
        }

        //创建这个操作
        private void Btn_Click_CreateOperation(object sender, RoutedEventArgs e)
        {
            WindowHelper.CreateComboBox<OperationBase>((operation) =>
            {
                Clear();
                CreateNew(operation);
            },this); 
        }

        private void CreateNew(OperationBase operation)
        {
            DataContext = operationView = new OperationViewModel(operation, temp);
            operationView.Conn = operation.ConnectConfig;
            CreateControl(operationView.Operation.ConnectConfig, 1);
            //operationView.Command = operation.Command;
            //CreateControl(operationView.Operation.Command, 2);
        }

       
        //测试连接
        private void Btn_Click_TestConn(object sender, RoutedEventArgs e)
        { 
            TryConn(); 
        } 

        private void TryConn()
        {
            if (operationView.Operation.ConnectConfig == null)
            {
                Growl.Warning("创建连接先!");
                return;
            }

            try
            {
                operationView.Operation.SetConn(operationView.Operation.ConnectConfig);
                var result = operationView.Operation.Connect();

                Growl.Info($"连接{(result ? "成功" : "失败")}");
 

               
             

            }
            catch (Exception ex)
            {
                Growl.Error($"连接失败,出现错误'{ex.Message}'" );
            }
            finally
            {
                operationView.Operation.Disconnected();
            }
        }

 
 
        private void Btn_Click_Enter(object sender, RoutedEventArgs e)
        {
            OnEnter?.Invoke();
        }
        private void Btn_Click_Cancel(object sender, RoutedEventArgs e)
        {
            OnCancel?.Invoke();
        }

     

        private void Btn_Click_Read(object sender, RoutedEventArgs e)
        {
            //var data =  operationView.Operation.Read(operationView.Operation.Command);

            //string heads = "";
            //foreach (var item in data.Tables[0].Keys)
            //{
            //    heads += item + ",";
            //}

            //Growl.InfoGlobal($"列头:"+ heads);
            
            //DataView dv = new DataView((DataTable) data.Data);
            //dgv1.ItemsSource = dv;
        }

        private void CreateControl<T>(T target, int index) where T : class
        {
            if (index <= -1 || index > uniforms.Length)
            {
                return;
            }
            var container = uniforms[index - 1];
            container.Children.Clear();
            Type objType = target.GetType();
            foreach (var propInfo in objType.GetProperties())
            {
                if (!propInfo.DeclaringType.IsPublic) return;
                //获取所有特性
                object[] objAttrs = propInfo.GetCustomAttributes(typeof(ControlAttribute), true);
                if (objAttrs.Length > 0)
                {
                    foreach (var att in objAttrs)
                    {
                        if (att is ControlAttribute conattr)
                        {
                            TextBlock textBlock = new TextBlock();
                            textBlock.Text = conattr.LabelName;

                            switch (conattr.ControlType)
                            {
                                //case ControlType.Label:
                                //    break;
                                case ControlType.TextBox:

                                    TextBox txtBox = new TextBox();
                                    txtBox.Tag = conattr.Name;
                                    txtBox.Width = 240;
                                    txtBox.Text = objType.GetProperty(propInfo.Name).GetValue(target, null)?.ToString();
                                    //根据值发生变化时 自动将值附上去
                                    txtBox.LostFocus += (sender, e) =>
                                    {
                                        if (sender is TextBox t)
                                        {
                                            if (string.IsNullOrEmpty(t.Text)) return;
                                            if (t.Tag.ToString() == propInfo.Name)
                                            {
                                                propInfo.SetValue(target, t.Text);
                                                var value = objType.GetProperty(propInfo.Name).GetValue(target, null)?.ToString();
                                                Growl.Info($"名称{propInfo.Name},值:{value}" ); 
                                            }
                                        }
                                    };
                                    container.Children.Add(textBlock);
                                    container.Children.Add(txtBox);
                                    break;
                                case ControlType.ComboBox:
                                    ComboBox cmb = new ComboBox();
                                    cmb.Tag = conattr.Name;
                                    cmb.SelectedIndex = int.Parse( objType.GetProperty(propInfo.Name).GetValue(target, null)?.ToString());
                                    cmb.SelectionChanged += (sender, e) =>
                                    {
                                        if (sender is ComboBox c)
                                        {
                                            if (c.SelectedIndex <= -1) return;
                                            if (c.Tag.ToString() == propInfo.Name)
                                            {
                                                propInfo.SetValue(target, c.SelectedIndex);
                                                var value = objType.GetProperty(propInfo.Name).GetValue(target, null)?.ToString();
                                                Growl.Info($"名称{propInfo.Name},值:{value}" );
                                            }
                                        }
                                    };

                                    foreach (var cbItem in conattr.Items)
                                    {
                                        cmb.Items.Add(cbItem);
                                    }
                                    container.Children.Add(textBlock);
                                    container.Children.Add(cmb);
                                    break;
                                case ControlType.FilePathSelector:



                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }


        }
    }
}
