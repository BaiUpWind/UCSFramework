using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modle
{
    /// <summary>
    /// 设备所包含的设备信息
    /// <para>每个设备可以包含多个信息，每个信息的内容可以单独配置</para>
    /// </summary>
    public interface IDeviceInfo
    {
        /// <summary>
        /// 该设备读取信息连接的方式
        /// <para>没有配置则使用默认的连接方式 <see cref="IDeviceConfig.DevType"/></para>
        /// </summary>
        DeviceConnectedType ConnectionType { get; set; }

        /// <summary>
        /// 设备读取配置信息
        /// </summary>
        DeviceInfoCfg InfoCfg { get;  }

        /// <summary>
        /// 读取信息
        /// </summary>
        /// <returns></returns>
        string Read();
    }


    public interface IDeviceConnection
    {
        IDeviceConfig Config { get; }
        bool Connection();

        void DisConnection();

        object Read(object commd);
    }
}
