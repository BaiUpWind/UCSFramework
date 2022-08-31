using DeviceConfig;
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
using ComboBox = System.Windows.Controls.ComboBox;
using TextBox = System.Windows.Controls.TextBox;

namespace DisplayBorder.Controls
{
    /// <summary>
    /// DataControl.xaml 的交互逻辑
    /// </summary>
    public partial class DataControl : UserControl, ISingleOpen, IControlHelper
    {


        public DataControl()
        {
            InitializeComponent();
            canDispose = false;
        }

        private object Ttype;
        private bool canDispose;
        public bool CanDispose => canDispose;

        public event Action<object> OnEnter;
        public event Action<object> OnCancel;


        public void Dispose()
        {

        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                canDispose = true;
                if (btn.Content.ToString() == "确认")
                {
                    OnEnter?.Invoke(Ttype);
                }
                else if (btn.Content.ToString() == "关闭")
                {
                    OnCancel?.Invoke(Ttype);
                }
            }
        }

        public void CreateType (Type targetType, object traget) 
        {
            Ttype = traget;
            WindowHelper.CreateControl(targetType, traget, container);
        }

      

        public object GetType(params object[] paras) => Ttype;
    }

    /// <summary>
    /// 控件帮助接口
    /// 创建对应的控件,和创建对应实例
    /// </summary>
    public interface IControlHelper
    {

        void CreateType(Type targetType,object target);
         
        object GetType(params object[] paras);

        event Action<object> OnEnter;
        event Action<object> OnCancel;

    }
    /// <summary>
    /// 控件数据接口
    /// </summary>
    public interface IControlData
    {
        bool OnlyWacth { get; set; }
        object TypeData { get; set; }
        /// <summary>
        /// 设置控件的数据
        /// </summary>
        /// <param name="data"></param>
        void SetData(object data);

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        object GetData();

        event Action<object> OnSet;
        event Action OnClose;
    }

}
