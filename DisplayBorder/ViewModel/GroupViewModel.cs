using CommonApi;
using DeviceConfig;
using DeviceConfig.Core;
using DisplayBorder.Model;
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
    public class GroupViewModel : ViewModelBase
    {
        public GroupViewModel(Group group)
        {
            this.currentGroup = group;
            //if (group.DeviceConfigs == null)
            //{
            //    group.DeviceConfigs = new List<Device>();
            //}
            //devices =  new ObservableCollection<Device>(group.DeviceConfigs);
            //addDevice = new RelayCommand(AddDevice);
        }

        private Group currentGroup;
        //private ObservableCollection<Device> devices;

        //public RelayCommand addDevice { get; set; }

        public Group CurrentGroup
        {
            get => currentGroup; set
            {
                currentGroup = value;
                RaisePropertyChanged();
            }
        }
        //#region device model

        //private int deviceId;
        //private string deviceName;
        //private double stayTime =5d;
        //private ConnectionConfigBase defaultConn =null; 

        //#endregion
        //public ObservableCollection<Device> Devices
        //{
        //    get => devices; set
        //    {
        //        devices = value;
        //        RaisePropertyChanged();
             
        //    }
        //}



        //public int DeviceId
        //{
        //    get => deviceId; set
        //    {
        //        deviceId = value;
        //        RaisePropertyChanged();
        //    }
        //}
        //public string DeviceName
        //{
        //    get => deviceName; set
        //    {
        //        deviceName = value;
        //        RaisePropertyChanged();
        //    }
        //}
        //public double StayTime
        //{
        //    get => stayTime; set
        //    {
        //        stayTime = value;
        //        RaisePropertyChanged();
        //    }
        //}
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
        //    get => DefaultConn== null ? "点击创建": DefaultConn.GetType().Name; 
        //}


        //public void AddDevice()
        //{
        //    //if (defaultConn == null)
        //    //{
        //    //    Growl.Error("添加失败,请创建默认连接方式");
        //    //    return;
        //    //}
        //    if( CurrentGroup.DeviceConfigs.Where(a => a.DeviceId == DeviceId).Count() >0)
        //    {
        //        Growl.Error($"'{DeviceId}'设备编号已经存在!");
        //        return;
        //    }

        //    Device device = new Device()
        //    {
        //        DeviceId = DeviceId,
        //        DeviceName = DeviceName,
        //        StayTime = StayTime,
        //        //DefaultConn = DefaultConn,
        //        DeviceInfos = new List<DeviceInfo>()
        //    };
        //    Devices.Add(device); 
        //    if (currentGroup != null)
        //    {
        //        currentGroup.DeviceConfigs = Devices.ToList(); 
        //    }
        //    Growl.Success($"添加'{device.DeviceId}','{device.DeviceName}'设备成功!");
        //}
         
    }
}
