using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modle.DeviceCfg
{
    /// <summary>
    /// 串口通信配置
    /// </summary>
    [DeviceConnectedType("SerialPort")]
    public class SerialPortCfg: IDeviceConfig
    {
        /// <summary>
        /// 串口的名称
        /// </summary>
        [Control("PortName",ControlType.TextBox)]
        public string PortName { get; set; }
        /// <summary>
        /// COM口的名称
        /// </summary>
        [Control("COMNmae", ControlType.ComboBoxSerialPort)]
        public string COMNmae { get; set; }

        /// <summary>
        /// 波特率
        /// </summary>
        [Control("BaudRate",ControlType.TextBox)]
        public int BaudRate { get; set; }

        public DeviceConnectedType DevType =>  DeviceConnectedType.Serial;
    }
}
