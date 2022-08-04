using DeviceConfig;
using DeviceConfig.Core;
using DisplayBorder.ViewModel;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
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
using TextBox = System.Windows.Controls.TextBox;

namespace DisplayBorder.Controls
{
    /// <summary>
    /// OperationControl.xaml 的交互逻辑
    /// </summary>
    public partial class OperationControl : UserControl
    {
        public OperationControl()
        {
            InitializeComponent();
            Init(new DataBaseOperation());
        }

        public void Init(OperationBase operation)
        {
            DataContext = operationView = new OperationViewModel(operation);
            uniforms[0] = sv1;
            uniforms[1] = sv2;
        }
        private OperationViewModel operationView;
        private UniformSpacingPanel[] uniforms = new UniformSpacingPanel[2];

        public event Action OnCreate;
        public event Action OnCancel;

        //创建这个操作
        private void Btn_Click_CreateOperation(object sender, RoutedEventArgs e)
        {
            if(sender is Button btn)
            {
                if(btn.Content.ToString() == "创建")
                {

                }
                else if( btn.Content.ToString() == "取消")
                {

                }
            }
        }

        //获取连接
        private void Btn_Click_CreateConn(object sender, RoutedEventArgs e)
        {
            WindowHelper.GetObject<ConnectionConfigBase>((conn) =>
            {
                operationView.Conn = conn;
                CreateControl (operationView.Conn ,1);
            });
        }


        //获取指令
        private void Btn_Click_CreateCommd(object sender, RoutedEventArgs e)
        {
            WindowHelper.GetObject<CommandBase>((commd) =>
            {
                operationView.Command = commd;
                CreateControl(operationView.Command ,2);

            });
        }


        //测试连接
        private void Btn_Click_TestConn(object sender, RoutedEventArgs e)
        {
            if (operationView.Operation.ConnectConfig == null)
            {
                Growl.Warning("创建连接先!");
                return;
            }

            try
            {
                var result = operationView.Operation.Connect();

                Growl.Info($"连接{(result ? "成功" : "失败")}");

            }
            catch (Exception ex)
            {
                Growl.Error($"连接失败,出现错误'{ex.Message}'");
            }
            finally
            {
                operationView.Operation.Disconnected();
            }
        }

        private void CreateControl<T> (T target, int index) where T : class
        {
            if(index <=-1 || index >= uniforms.Length)
            {
                return;
            }
            var container = uniforms[index-1];
            container.Children.Clear();
            Type objType =target.GetType();
            foreach (var propInfo in objType.GetProperties())
            {
                if (!propInfo.DeclaringType.IsPublic) return;
                //获取所有特性
                object[] objAttrs = propInfo.GetCustomAttributes(typeof(ControlAttribute), true);
                if (objAttrs.Length > 0)
                {
                    foreach (var att in objAttrs)
                    { 
                        if ( att is ControlAttribute conattr)
                        {
                            TextBlock textBlock = new TextBlock();
                            textBlock.Text = conattr.Name;

                            switch (conattr.ControlType)
                            {
                                case ControlType.Label:
                                    break;
                                case ControlType.TextBox:

                                    TextBox txtBox = new TextBox();
                                    txtBox.Tag = conattr.Name;
                                    txtBox.Width = 240;
                                    //根据值发生变化时 自动将值附上去
                                    txtBox.LostFocus += (sender, e) =>
                                    {
                                        if (sender is TextBox t)
                                        {
                                            if (string.IsNullOrEmpty(t.Text)) return;
                                            if (t.Tag.ToString() == propInfo.Name)
                                            {
                                                object str = t.Text;
                                                propInfo.SetValue(target, str );
                                                var value = objType.GetProperty(propInfo.Name).GetValue(target, null)?.ToString();
                                                Growl.Info($"名称{propInfo.Name},值:{value}");
                                            }
                                        }
                                    }; 
                                    container.Children.Add(textBlock);
                                    container.Children.Add(txtBox);
                                    break;
                                case ControlType.ComboBox:
                                    break;
                                case ControlType.ComboBoxSerialPort:
                                    break;
                                case ControlType.ComboBoxEnum:


                                    break;
                                default:
                                    break;
                            }
                   
                        }

                        //if (att is TextBoxAttribute txt)
                        //{
                        //    textBlock.Text = txt.LabelText;

                        //    TextBox txtBox = new TextBox();
                        //    txtBox.Tag = txt.Name;
                        //    txtBox.TextInput  += (sender, e) =>
                        //    {
                              
                        //        if (sender is TextBox t)
                        //        {
                        //            if (t.Tag.ToString() == propInfo.Name)
                        //            {
                        //                propInfo.SetValue(t, txtBox.Text);
                        //                var value = objType.GetProperty(propInfo.Name).GetValue(target, null)?.ToString();
                        //                Growl.Info($"名称{propInfo.Name},值:{value}");
                        //            }
                        //        }
                        //    }; 
                        //}
                        //else if (att is ComboBoxAttribute combo)
                        //{

                        //    textBlock.Text = combo.LabelText;
                        //}
                    }
                }
            }


        }

        private void TxtBox_TextInput(object sender, TextCompositionEventArgs e)
        {
            
        }
    }
}
