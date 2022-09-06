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
using System.Windows.Shapes;

namespace DisplayBorder.View
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : HandyControl.Controls.Window
    {
        public Window1()
        {
            InitializeComponent();
            Name = "window1";
            Activated += (s, e) =>
            {
                Growl.SetGrowlParent(this, true);

            };
            Deactivated += (s, e) =>
            {
                Growl.SetGrowlParent(this, false);

            };

        }

        private void Btn_ShowInfo(object sender, RoutedEventArgs e)
        {
            Growl.Info("显示信息" );
        }
    }
}
