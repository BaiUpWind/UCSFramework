using CommonApi;
using DisplayBorder.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayBorder.Events
{
    /// <summary>
    /// 当设备被选中时
    /// </summary>
    internal class OnDeviceChooseArgs : BaseEventArgs
    {
        public static int EventID = typeof(OnDeviceChooseArgs).GetHashCode();

        public override int Id => EventID;

        public DeviceConfig.Device Device { get; private set; }
        public MixerControl DeviceMixer { get; private set; }
        public static OnDeviceChooseArgs Create(MixerControl deviceMixer )
        {
            var args = ReferencePool.Acquire<OnDeviceChooseArgs>();
            args.Device = deviceMixer.Dvm.CurrentDevice;
            args.DeviceMixer = deviceMixer;
            return args;
        }

        public override void Clear()
        {
            Device = null;
        }
    }
}
