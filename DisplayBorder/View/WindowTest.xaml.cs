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
using System.Windows.Threading;
using System.Threading;
using ScrollViewer = System.Windows.Controls.ScrollViewer;

namespace DisplayBorder.View
{
    /// <summary>
    /// WindowTest.xaml 的交互逻辑
    /// </summary>
    public partial class WindowTest :Window
    {
        public WindowTest()
        {
            InitializeComponent();
            //--------------------事件系统测试

            GlobalPara.EventManager.Subscribe(OnOpenNewWindowArgs.EventID, OnOpenNewWindow);
        
            GlobalPara.EventManager.Subscribe(OnGroupChooseArgs.EventID, OnGroupChoose);
            //-----------------------
             
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
                groupMixers.Add(mc);
            }
            InitTimer();
      


            Activated += (s, e) =>
            {
                Growl.SetGrowlParent(this, true);

            };
            Deactivated += (s, e) =>
            {
                Growl.SetGrowlParent(this, false);

            };

            ClassControl cc = new ClassControl(typeof(List<Group>), true,GlobalPara.Groups);
            sp.Children.Add(cc);


        }

        private List<MixerControl> groupMixers = new List<MixerControl>();

        private DispatcherTimer mDataTimer = null; //定时器
        private long timerExeCount = 0; //定时器执行次数

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

        private int gmIndex = 0;


        private void InitTimer()
        {
            if (mDataTimer == null)
            {
                mDataTimer = new DispatcherTimer();
                mDataTimer.Tick += MDataTimer_Tick; ;
                mDataTimer.Interval = TimeSpan.FromSeconds(1);
            }
        }

        private void MDataTimer_Tick(object sender, EventArgs e)
        {
            if(gmIndex < 0) gmIndex = 0;
            if (gmIndex >= groupMixers.Count)
            {
                gmIndex = 0;
            }
            var target = groupMixers[gmIndex++];
            target.IsChoose = true;

            var currentScrollPosition = sv.VerticalOffset;
            //获取目标控件相对scrollViewer位置
            var point = new Point(0, currentScrollPosition);

            var tarPos = target.TransformToVisual(sv).Transform(point);

            //垂直方向上的定位
            sv.ScrollToVerticalOffset(tarPos.Y);
        }

        private void OnOpenNewWindow(object sender, BaseEventArgs e)
        {
            if (e is OnOpenNewWindowArgs args && args.NewWindow != null)
            {
                //设置弹窗新的父类容器
                //Growl.SetGrowlParent(args.NewWindow, false); 
            }
        }
         
        

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            WindowHelper.CreateComboBox<OperationBase>((obj) =>
            {
                Growl.Info($"创建成功!{obj.GetType().Name}");
            }, para: new DataBaseConnectCfg());
        }
        string token = "wode";
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //WindowEmpty window = new WindowEmpty();
            //Window1 window = new Window1();
            
            //Growl.SetToken(window, token);

            //var result = Growl.GetToken(window);
            //GlobalPara.EventManager.Fire(this, OnOpenNewWindowArgs.Create(window, this));

            //window.Show();
        }

        private void OnGroupChoose(object sender, BaseEventArgs e)
        {
            if (e is OnGroupChooseArgs args)
            {
                CurrentGroup = args.Group;
                GroupMixer = args.GroupMixer; 
            }
        }

        private void Button_Start(object sender, RoutedEventArgs e)
        {
            if(sender is Button btn)
            {
                if(btn.Content.ToString() == "开始")
                {
                    btn.Content = "停止"; 
                    mDataTimer.Start();
                }
                else if(btn.Content.ToString() == "停止")
                { 
                    mDataTimer.Stop(); 
                    btn.Content = "开始";
                }
            }
        }

        private void Btn_ShowInfo(object sender, RoutedEventArgs e)
        {
            Growl.Info("创建成功!"  );
        }
    }
}

namespace MyWindows
{
    public class MyWindow : System.Windows.Window
    {
        public MyWindow()
        { 
            Activated += (s, e) =>
            {
                Growl.SetGrowlParent(this, true);

            };
            Deactivated += (s, e) =>
            {
                Growl.SetGrowlParent(this, false);

            }; 
        }
     
    }
}

