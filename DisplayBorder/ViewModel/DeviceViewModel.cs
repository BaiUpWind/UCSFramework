using DeviceConfig;
using DeviceConfig.Core;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayBorder.ViewModel
{
    public class DeviceViewModel : ViewModelBase
    {
        public DeviceViewModel(Device device)
        {
            addDeviceInfos = new RelayCommand(AddDeviceInfo);
            CurrentDevice = device;
            //defaultConn = device.DefaultConn;
            Infos = new ObservableCollection<DeviceInfo>(device.DeviceInfos);
        }

      

        private int deviceID;
        private string deviceName;
        private double stayTime = 5d;
        //private ConnectionConfigBase defaultConn = null; 
        private Device currentDevice;
        private DeviceInfo deviceInfo;
        private DeviceInfo currenDeviceInfo;

        #region 设备信息

        private int deviceInfoId;
        private string deviceInfoName;
        private int refreshInterval;
        private OperationBase operation;
         
        public int DeviceInfoId
        {
            get => deviceInfoId; set
            {

                RaisePropertyChanged();
                deviceInfoId = value;
            }
        }
        public string DeviceInfoName
        {
            get => deviceInfoName; set
            {
                RaisePropertyChanged();
                deviceInfoName = value;
            }
        }
        public int RefreshInterval
        {
            get => refreshInterval; set
            {
                RaisePropertyChanged();

                refreshInterval = value;
            }
        }
        public OperationBase Operation
        {
            get => operation; set
            {
                operation = value;
                RaisePropertyChanged(nameof(OperationName));
            }

        }
        public string OperationName
        {
            get
            {
                if (CurrentDevice == null)
                {
                    return null;
                } 
                return operation == null ? "创建操作" : Operation.GetType().Name;
            }
        }
        #endregion 
        public Device CurrentDevice
        {
            get => currentDevice; set
            {
                currentDevice = value;
                RaisePropertyChanged();
            }

        }
        private ObservableCollection<DeviceInfo> infos;
        public ObservableCollection<DeviceInfo> Infos
        {
            get => infos;
            set
            {
                infos = value; 
                RaisePropertyChanged();
            }
        }

        public RelayCommand addDeviceInfos { get; set; }
        public int DeviceId
        {
            get => deviceID; set
            {
                deviceID = value;
                RaisePropertyChanged();
            }
        }
        public string DeviceName
        {
            get => deviceName; set
            {
                deviceName = value;
                RaisePropertyChanged();
            }
        }
        public double StayTime
        {
            get => stayTime; set
            {
                stayTime = value;
                RaisePropertyChanged();
            }
        }

        public DeviceInfo CurrenDeviceInfo
        {
            get => currenDeviceInfo;
            set { currenDeviceInfo = value;
                RaisePropertyChanged();
            } 
        }

        //public ConnectionConfigBase DefaultConn
        //{
        //    get => defaultConn; set
        //    {
        //        defaultConn = value;
        //        RaisePropertyChanged(nameof(ConnName));
        //    }
        //}
        //public string ConnName
        //{
        //    get
        //    {
        //        if (CurrentDevice == null)
        //        {
        //            return null;
        //        }
        //        return DefaultConn == null ? "点击创建" : DefaultConn.GetType().Name;
        //    }
        //}

        /// <summary>
        /// 添加设备需要显示的一个信息
        /// </summary>
        private void AddDeviceInfo()
        {
            if (Operation == null)
            {
                Growl.Error("添加失败,请创建读取指令");
                return;
            }
            if (CurrentDevice == null)
            {
                Growl.Error($"未找到当前设备数据");
                return;
            }
            if (CurrentDevice.DeviceInfos.Where(a => a.DeviceInfoID == DeviceInfoId).Count() > 0)
            {
                Growl.Error($"'{DeviceInfoId}'设备信息编号已经存在!");
                return;
            } 
            deviceInfo = new DeviceInfo( )
            {
                DeviceID = DeviceId,
                Operation = operation,
                DeviceInfoID = DeviceInfoId,
                DeviceInfoName = DeviceInfoName,
                RefreshInterval = RefreshInterval, 
            };
            Infos.Add(deviceInfo); 
            if (CurrentDevice != null)
            {
                CurrentDevice.DeviceInfos = Infos.ToList();
            }
            Growl.Success($"添加'{deviceInfo.DeviceInfoID}','{deviceInfo.DeviceInfoName}'设备信息成功!");
        }
    }
}
