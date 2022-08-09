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
using DisplayBorder.ViewModel;

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
        
            GlobalPara.EventManager.Subscribe(OnGroupChooseArgs.EventID, OnGroupChoose);
            //-----------------------

            Growl.SetToken(this, "wode");

            for (int i = 0; i < 20; i++)
            {
                MixerControl mc = new MixerControl(new GroupViewModel(new Group()
                {
                    GroupID = 100 + i,
                    GroupName = $"第{i}台",
                    DeviceConfigs = new List<Device>()
                })) ;
                mc.BorderThickness = new Thickness(5);
                if (i == 0) mc.IsChoose = true;
                wpGroups.Children.Add(mc);
            }


           
        }
 
        private Group currentGroup;
        private MixerControl groupMixer; 
        /// <summary>
        /// 当前在读取的组
        /// </summary>
        public Group CurrentGroup
        {
            get => currentGroup; set
            {
                currentGroup = value; 
            }
        }

        public MixerControl GroupMixer
        {
            get => groupMixer; set
            {

                if (groupMixer != null)
                {
                    groupMixer.IsChoose = false;
                }
                groupMixer = value;

            }
        }
        private void OnOpenNewWindow(object sender, BaseEventArgs e)
        {
            if (e is OnOpenNewWindowArgs args && args.NewWindow != null)
            {
                //设置弹窗新的父类容器
                Growl.SetGrowlParent(args.NewWindow, true); 
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

        private void OnGroupChoose(object sender, BaseEventArgs e)
        {
            if (e is OnGroupChooseArgs args)
            {
                CurrentGroup = args.Group;
                GroupMixer = args.GroupMixer; 
            }
        }
    }
}
