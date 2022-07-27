using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Modle
{
   
    /// <summary>
    /// 设备配置接口
    /// </summary>
    public interface IDeviceConfig
    {

        /// <summary>
        /// 设备类型
        /// </summary>
        DeviceConnectedType DevType { get;   }
    }

 
}
