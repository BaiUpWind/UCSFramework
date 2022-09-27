using CommonApi;
using DeviceConfig;
using DisplayBorder.Controls;
using DisplayBorder.Events;
using DisplayBorder.View;
using DisplayBorder.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
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
using System.Windows.Threading;

namespace DisplayBorder
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = viewModel = new GroupsViewModel();
            Init();
        }
        private GroupsViewModel viewModel; 
        private Group currentGroup;
        private MixerControl groupMixer;
        private MixerControl deviceMixer;
        private Device currenDevice;
        //private bool auto = false;

        //private List<MixerControl> groupMixers = new List<MixerControl>();
        //private List<MixerControl> deviceMixers = new List<MixerControl>();

        //------------- 定时器之类的

        /// <summary>
        /// 组轮询定时器
        /// </summary>
        //private DispatcherTimer groupDataTimer = null; //定时器
       
        //private DispatcherTimer deviceDataTimer = null;

        //----------------


        /// <summary>
        /// 当前在读取的组
        /// </summary>
        public Group CurrentGroup
        {
            get => currentGroup; set
            { 
                currentGroup = value;
                viewModel.CurrentGroup = value;
            } 
        }

        /// <summary>
        /// 当前选择的设备
        /// </summary>
        public Device CurrenDevice
        {
            get => currenDevice; set
            {
                currenDevice = value;
                viewModel.CurretnDevice = value;
            } 
        }
        public MixerControl GroupMixer
        {
            get => groupMixer; set { 
                
                if(groupMixer!= null)
                {
                    groupMixer.IsChoose = false;
                }
                groupMixer = value; 
            
            } 
        } 
        public MixerControl DeviceMixer
        {
            get => deviceMixer; set {
                if(deviceMixer!= null)
                {
                    deviceMixer.IsChoose = false;
                    //当设备发生变化时 所有设备信息读取都进行关闭
                    foreach (var item in deviceMixer.Dvm.CurrentDevice.DeviceInfos)
                    {
                        item.Operation.Disconnected();
                    } 
                }
                deviceMixer = value;  
            } 
        }

        public void Init()
        {
            //---------------事件注册
            GlobalPara.EventManager.Subscribe(OnGroupChooseArgs.EventID, OnGroupChoose);
            GlobalPara.EventManager.Subscribe(OnDeviceChooseArgs.EventID, OnDeviceChoose);
            GlobalPara.EventManager.Subscribe(OnUserInputArgss.EventID, OnUserInput);

            //--------------- 
            wpGroups.Children.Clear();
            int index = 0;
            foreach (var item in viewModel.Groups)
            { 
                MixerControl mc = new MixerControl(new GroupViewModel(item));
                mc.BorderThickness = new Thickness(5);
               if(index == 0)  mc.IsChoose = true;
                wpGroups.Children.Add(mc);
                index++;
            }
        }

     

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double titlesize = ((ActualWidth / 12) / 3 * 2) / 3;
            System.Windows.Application.Current.Resources.Remove("TitleFontSize");
            System.Windows.Application.Current.Resources.Add("TitleFontSize", titlesize);
            double tabsize = ((SystemParameters.PrimaryScreenWidth / 12) / 3 * 2) / 5 * 0.9;
            System.Windows.Application.Current.Resources.Remove("TabFontSize");
            System.Windows.Application.Current.Resources.Add("TabFontSize", tabsize);
            double gridsize = ((ActualWidth / 12) / 3 * 2) / 5 * 0.8;
            System.Windows.Application.Current.Resources.Remove("GridFontSize");
            System.Windows.Application.Current.Resources.Add("GridFontSize", gridsize);
             
            //计算额外的偏移
            Thickness mg = new Thickness(10, titlesize + 25, 10, 10);
            Application.Current.Resources.Remove("TitleTickness");
            Application.Current.Resources.Add("TitleTickness", mg);
             
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
           
        }

        private void Btn_Click_Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var item in wpDevices.Children)
            {
                if(item is MixerControl mc)
                {
                    mc.SetColor(0);
                }
            }
        }

        private void Btn_Click_OpenSysConfig(object sender, RoutedEventArgs e)
        {
            
        }

        //---------------------- 注册事件
        private void OnGroupChoose(object sender, BaseEventArgs e)
        {
            if (e is OnGroupChooseArgs args)
            {
                CurrentGroup = args.Group;
                GroupMixer = args.GroupMixer;

                int index = 0;
                wpDevices.Children.Clear();
                foreach (var item in CurrentGroup.DeviceConfigs)
                {
                    MixerControl mc = new MixerControl(new DeviceViewModel(item));
                    mc.BorderThickness = new Thickness(5);
                    if (index == 0) mc.IsChoose = true;
                    wpDevices.Children.Add(mc);
                    index++;
                }
            }
        }
        private void OnDeviceChoose(object sender, BaseEventArgs e)
        {
            if (e is OnDeviceChooseArgs args)
            {
                CurrenDevice = args.Device;
                DeviceMixer = args.DeviceMixer ;
            } 
        }

        private void OnUserInput(object sender, BaseEventArgs e)
        {
          if( e is OnUserInputArgss args)
            {

            }
        }
    }
}
