using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modle.DeviceCfg
{
    /// <summary>
    /// 基于TCP/IP访问的配置基类
    /// </summary>
    [DeviceConnectedType("TcpClient")]
    public class TCPCfgBase : IDeviceConfig
    {
        /// <summary>
        /// 访问地址
        /// </summary>
        [Control("IP",ControlType.TextBox)]
        public string IP { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        [Control("Port", ControlType.TextBox)]
        public int Port { get; set; }

        public virtual DeviceConnectedType DevType =>  DeviceConnectedType.TcpClient;
    }
}
