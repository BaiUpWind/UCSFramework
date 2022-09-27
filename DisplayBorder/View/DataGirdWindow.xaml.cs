using DisplayBorder.Controls;
using HandyControl.Controls;
using System;
using System.Collections;
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
using System.Windows.Shapes;
using MessageBox = HandyControl.Controls.MessageBox;
using Window = System.Windows.Window;

namespace DisplayBorder.View
{
    /// <summary>
    /// DataGirdWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DataGirdWindow : Window, IControlData
    {

        public DataGirdWindow()
        {
            InitializeComponent();
            dg1.SelectionChanged += Dg1_SelectionChanged;
            selectionTemp = null;
            KeyDown += (s, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    Close();
                }
            };
        }

        private Type genericArgument;
        private Type collType;
        private object selectionTemp;
        private object typeData;

        public object TypeData
        {
            get => typeData; set
            {
                typeData = value;
            }
        }
        public bool OnlyWacth { get; set; }

        /// <summary>
        /// 集合的泛型类型
        /// <para> 从'<see cref="IControlData.TypeData"/>'找第一位</para>
        /// </summary>
        public Type GenericArgument => genericArgument;
        /// <summary>
        /// 获取到集合类型
        /// </summary>
        public Type CollType => collType;


        private void Dg1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (genericArgument == null)
            {
                //todo:提示
                return;
            }
            var sitem = dg1.SelectedItem;
            if (sitem != null && sitem.GetType() == genericArgument)
            {
                selectionTemp = dg1.SelectedItem;
            }
            else
            {
                selectionTemp = null;
            }
        }

        public event Action<object> OnSet;
        public event Action OnClose;

        public object GetData() => TypeData;

        public void SetData(object data)
        {
            TypeData = data;

            if (TypeData is IEnumerable list)
            {
                //获取到泛型类型
                if (genericArgument == null)
                {
                    var arg = TypeData.GetType().GetGenericArguments()[0];
                    if (arg != null)
                    {

                        genericArgument = arg;
                    }
                    else
                    {
                        //todo:错误的类型
                    }
                }
                //获取集合类型
                if (collType == null)
                {
                    foreach (var item in TypeData.GetType().GetInterfaces())
                    {
                        if (item.Name == typeof(ICollection<>).Name)
                        {
                            collType = item;
                            break;
                        }
                    }
                    if (collType == null)
                    {
                        //todo:显示错误哦
                    }
                }
                dg1.ItemsSource = list;
            }
            else if (data is DataTable dt)
            {
                DataView dv = new DataView(dt);
                dg1.ItemsSource = dv;
                //强制不允许操作
                OnlyWacth = false;
            }
            foreach (UIElement item in opSP.Children)
            {
                item.Visibility = OnlyWacth ? Visibility.Hidden : Visibility.Visible;
            }
            OnSet?.Invoke(TypeData);
            dg1.DataContext = TypeData;
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            OnClose?.Invoke();
        }

        private void Btn_AddElement(object sender, RoutedEventArgs e)
        {
            OpenDataWindow(null);
        }

        private void Btn_DeleteElement(object sender, RoutedEventArgs e)
        {
            if (selectionTemp == null)
            {
                //todo:提示没有选择
                return;
            }
         
            var value = new object[] { selectionTemp };
            var result = MessageBox.Ask($"是否确定删除该元素");
            if(result == MessageBoxResult.OK)
            {
                var check = (bool)collType.GetMethod("Contains").Invoke(TypeData, value);
                if (!check)
                {
                    //todo:提示当前没有这个元素
                    return;
                }
                if ((bool)collType.GetMethod("Remove").Invoke(TypeData, value))
                {
                    SetDg();
                    MessageBox.Show("删除成功!");
                }
            }
            else
            {
                Growl.InfoGlobal("取消删除");
            } 

        }

        private void Btn_ModifyElement(object sender, RoutedEventArgs e)
        {
            if(selectionTemp == null)
            {
                Growl.InfoGlobal("请选择一个元素!");
                return;
            }
            OpenDataWindow(selectionTemp);
        }

        private void OpenDataWindow(object target = null)
        {
            if (collType == null || genericArgument == null || TypeData == null)
            {
                //todo:提示错误
                return;
            }
            WindowHelper.CreateWindow<DataWindow>(genericArgument, (data) =>
            {
                if (genericArgument == data.GetType())
                {
                    if(target == null)
                    {
                        //添加
                        collType.GetMethod("Add").Invoke(TypeData, new object[] { data });
                        Growl.InfoGlobal( "添加成功"  );
                    }
                    else
                    {
                        //修改


                        Growl.InfoGlobal( "修改成功!");
                    }

                    SetDg();


                }
                else
                {
                    //todo:提升错误的类型
                }
            }, target, $"{(target == null ?"建立元素":"修改元素")}'{genericArgument.Name}'",this);
        }

        private void SetDg()
        {
            if(TypeData is IEnumerable list)
            {
                dg1.ItemsSource = null;
                dg1.ItemsSource = list;
            }
          
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
