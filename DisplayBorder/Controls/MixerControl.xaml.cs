using DisplayBorder.Events;
using DisplayBorder.ViewModel;
using GalaSoft.MvvmLight;
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

namespace DisplayBorder.Controls
{
    /// <summary>
    /// MixerControl.xaml 的交互逻辑
    /// </summary>
    public partial class MixerControl : UserControl
    {
        public MixerControl()
        {
            InitializeComponent();
        }
          
        public MixerControl(ViewModelBase modelBase)
        {
            InitializeComponent();
            if (modelBase is GroupViewModel groupViewModel)
            {
                Gvm = groupViewModel;
                DataContext = mvm = new MixerViewModel(gvm.CurrentGroup.GroupID,gvm.CurrentGroup.GroupName,"正常");
            }
            else if( modelBase is DeviceViewModel deviceViewModel)
            {
                Dvm = deviceViewModel;
                DataContext = mvm = new MixerViewModel(dvm.CurrentDevice.DeviceId,dvm.CurrentDevice.DeviceName,"正常");
            }
            else
            {
                throw new Exception("瞎几把传对象");
            }
       
        }

        private GroupViewModel gvm;
        private DeviceViewModel dvm;
        private MixerViewModel mvm;

        private bool isChoose;

        public bool IsChoose
        {
            get => isChoose; set
            {
                isChoose = value;
                choose.BorderBrush = value ? new SolidColorBrush(Color.FromRgb(255, 139, 97)) : new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                if (value)
                { 
                    if (Gvm != null)
                    {
                        GlobalPara.EventManager.Fire(this, OnGroupChooseArgs.Create(this));
                    }
                    if (Dvm != null)
                    {
                        GlobalPara.EventManager.Fire(this, OnDeviceChooseArgs.Create(this));
                    }
                }
            }
        }

        public GroupViewModel Gvm { get => gvm; set => gvm = value; }
        public DeviceViewModel Dvm { get => dvm; set => dvm = value; }

        /// <summary>
        /// 设置背景色
        /// <para>参数[1] 是组的颜色 其他是设备的颜色</para>
        /// </summary>
        /// <param name="type"></param>
        public void SetColor(int type)
        {
            //92,92,255 #5c5cff 
            Color color = new Color(); 
            if (type == 1)
            {
                //组的颜色 
                color = Color.FromRgb(92, 92 ,255);
            }
            else
            {
                //设备的颜色
                //#7a7aff
                //rgb(122, 122, 255) 
                color= Color.FromRgb(122, 122, 255);
            } 
            bgc.Background = new SolidColorBrush(color);
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(!isChoose)
            IsChoose = true;
        }
    }
}
