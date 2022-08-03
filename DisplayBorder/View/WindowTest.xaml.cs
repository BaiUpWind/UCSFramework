using DeviceConfig.Core;
using DisplayBorder.Controls;
using HandyControl.Controls;
using HandyControl.Tools;
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
using CommonApi;
using Window = System.Windows.Window;
using DeviceConfig;

namespace DisplayBorder.View
{
    /// <summary>
    /// WindowTest.xaml 的交互逻辑
    /// </summary>
    public partial class WindowTest : Window
    {
        public WindowTest()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
          WindowHelper.GetObject<ConnectionConfigBase>(OnCreate);
        }

        private void OnCreate(ConnectionConfigBase obj)
        {
            Growl.Info("创建成功!");
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            WindowHelper.GetObject<OperationBase>((obj) =>
            {
                Growl.Info($"创建成功!{obj.GetType().Name}");
            }, new DataBaseConnectCfg());
        }
    }
}
