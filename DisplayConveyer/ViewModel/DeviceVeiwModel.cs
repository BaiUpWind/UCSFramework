using DisplayConveyer.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DisplayConveyer.ViewModel
{
    public class DeviceViewModel : ViewModelBase
    {
        private DeviceData data;

        public DeviceViewModel(DeviceData data)
        {
            this.data = data;
        }

        public DeviceData Data
        {
            get { return data; }
            set
            {
                data = value;
                RaisePropertyChanged();
            }
        }
    }
}
