using CommonApi;
using DeviceConfig;
using DisplayBorder.Events;
using DisplayBorder.Model;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayBorder.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        private SysConfigPara sysConfig;
        private Group runGroup;
        private string currentRunDeviceName;
        private ClassInfo man1;
        private ClassInfo man2;

        public MainViewModel()
        {
            GlobalPara.EventManager.Subscribe(OnValueChangedArgs.EventID, OnSysConfigChanged);
            SysConfig = GlobalPara.SysConfig;

        }


        /// <summary>
        /// 系统配置参数
        /// </summary>
        public SysConfigPara SysConfig
        {
            get => sysConfig; set
            {
                sysConfig = value;
                RaisePropertyChanged();
            }
        }

        public Group RunGroup
        {
            get => runGroup; set
            {
                runGroup = value;
                RaisePropertyChanged();
            }
        }
        /// <summary>
        /// 当前正在获取的设备名称
        /// </summary>
        public string CurrentRunDeviceName
        {
            get => currentRunDeviceName; set
            {
                currentRunDeviceName = value;
                RaisePropertyChanged();
            }
        }

        public ClassInfo Man1
        {
            get => man1; set
            {
                man1 = value;
                RaisePropertyChanged();
            }

        }
        public ClassInfo Man2
        {
            get => man2; set
            {
                man2 = value;
                RaisePropertyChanged();
            }
        }



        //----------------重写


        public override void Cleanup()
        {
            base.Cleanup();
            GlobalPara.EventManager.Unsubscribe(OnValueChangedArgs.EventID, OnSysConfigChanged);
        }

        //---------------事件

        private void OnSysConfigChanged(object sender, BaseEventArgs e)
        {
            if (e is OnValueChangedArgs args && args.Value is SysConfigPara configPara)
            {
                SysConfig = configPara;
            }
        }
    }
}
