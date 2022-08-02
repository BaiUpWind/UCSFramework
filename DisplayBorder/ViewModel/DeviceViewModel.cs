using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayBorder.ViewModel
{
    public class DeviceViewModel : ViewModelBase
    {
        public DeviceViewModel( )
        {
            Modifiy = new RelayCommand(null);
        }
        private int deviceID;
        private string deviceName;


        public RelayCommand Modifiy { get; set; }
        public int DeviceID
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


    }
}
