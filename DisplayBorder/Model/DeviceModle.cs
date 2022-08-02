using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayBorder.Model
{
    public class DeviceModle  
    {
        private int deviceID;
        private string deviceName;

        public int DeviceID { get => deviceID; set { deviceID = value;  } }
        public string DeviceName
        {
            get => deviceName; set {    deviceName = value;    } 
        }
    }
}
