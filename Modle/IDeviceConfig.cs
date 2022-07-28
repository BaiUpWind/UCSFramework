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
        /// 设备连接的类型
        /// <para>默认的连接方式</para>
        /// </summary>
        DeviceConnectedType DevType { get;   }
         
      
    }

 
}
