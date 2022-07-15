using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modle.DeviceCfg
{
    /// <summary>
    /// 串口通信配置
    /// </summary>
    public class SerialPortCfg: IDeviceConfig
    {
        /// <summary>
        /// 串口的名称
        /// </summary>
        public string PortName { get; set; }
        /// <summary>
        /// COM口的名称
        /// </summary>
        public string COMNmae { get; set; }

        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate { get; set; }

        public DeviceType DevType =>  DeviceType.Serial;
    }
}
