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
        public string IP { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public string Prot { get; set; }

        public virtual DeviceConnectedType DevType =>  DeviceConnectedType.TcpClient;
    }
}
