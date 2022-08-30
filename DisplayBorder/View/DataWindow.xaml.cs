using DisplayBorder.Controls;
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
using System.Windows.Shapes;

namespace DisplayBorder.View
{
    /// <summary>
    /// DataWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DataWindow : Window, IControlHelper
    {
        public DataWindow()
        {
            InitializeComponent();
        }
        private object Ttype;
        public event Action<object> OnEnter;
        public event Action OnCancel;

        public void CreateType(Type targetType, object target)
        {
            Ttype = target;
            WindowHelper.CreateContrl(targetType, target, container,this);
        } 
        public object GetType(params object[] paras) => Ttype;

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            { 
                if (btn.Content.ToString() == "确认")
                {
                    OnEnter?.Invoke(Ttype);
                }
                else if (btn.Content.ToString() == "关闭")
                {
                    OnCancel?.Invoke();
                }
            }
        }
    }
}
