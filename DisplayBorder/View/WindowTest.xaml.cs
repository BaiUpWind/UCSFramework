using DeviceConfig.Core;
using DisplayBorder.Controls;
using HandyControl.Controls;
using HandyControl.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CommonApi; 
using DeviceConfig;
using DisplayBorder.Events;
using System.Windows;
using Window = HandyControl.Controls.Window;

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
            //--------------------事件系统测试

            GlobalPara.EventManager.Subscribe(OnOpenNewWindowArgs.EventID, OnOpenNewWindow);
            //-----------------------

            Growl.SetToken(this, "wode");
        }

        private void OnOpenNewWindow(object sender, BaseEventArgs e)
        {
            if (e is OnOpenNewWindowArgs args && args.NewWindow != null)
            {
                //设置弹窗新的父类容器
                Growl.SetGrowlParent(args.NewWindow, true);
                //Growl.SetToken(args.NewWindow, args.NewWindow.GetType().GetHashCode().ToString());

                //if (args.ParentWindow != null)
                //{
                //    var cache = args.ParentWindow;
                //    //当关闭时
                //    args.NewWindow.Closing += (s, ee) =>
                //    {
                //        Growl.SetGrowlParent(cache, false);

                //    };
                //}
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //WindowHelper.GetObject<ConnectionConfigBase>(OnCreate);
            Growl.Info("创建成功!", "wode");
        }

        //private void OnCreate(ConnectionConfigBase obj)
        //{
        //    Growl.Info("创建成功!");
        //}

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            WindowHelper.GetObject<OperationBase>((obj) =>
            {
                Growl.Info($"创建成功!{obj.GetType().Name}");
            }, para: new DataBaseConnectCfg());
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Window1 window = new Window1();
            //WindowOperation window = new WindowOperation();

            GlobalPara.EventManager.Fire(this, OnOpenNewWindowArgs.Create(window, this));
            //window.Init(new DataBaseOperation(), new DeviceInfo()
            //{
            //    RefreshInterval = 50,
            //});
            window.Show();
        }
    }
}
