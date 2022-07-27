using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Modle
{
    /// <summary>
    /// 设备连接类型
    /// </summary>
    public enum DeviceConnectedType
    {
        /// <summary>
        /// 当连接类型为空时 不做任何操作
        /// </summary>
        None,
        /// <summary>
        /// 使用串口
        /// </summary>
        Serial,
        /// <summary>
        /// 使用hsl库进行连接对应的PLC
        /// </summary>
        PLC,
        /// <summary>
        /// 使用tcpClient方式连接
        /// </summary>
        TcpClient,
        /// <summary>
        /// 数据库方式
        /// </summary>
        DataBase
    }

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

    public　class test
    {
        public IEnumerable<string> GetCfgSelect()
        {
            //var types = typeof(EffectData).Assembly.GetTypes()
            //   .Where(x => !x.IsAbstract)
            //   .Where(x => typeof(EffectData).IsAssignableFrom(x))
            //   .Where(x => x.GetCustomAttribute<EffectAttribute>() != null)
            //   .OrderBy(x => x.GetCustomAttribute<EffectAttribute>().Order)
            //   .Select(x => x.GetCustomAttribute<EffectAttribute>().EffectType);
            //var results = types.ToList();
            //results.Insert(0, "(添加效果)");
            //return results;

            var types = typeof(IDeviceConfig).Assembly.GetTypes()
                .Where(a => typeof(IDeviceConfig).IsAssignableFrom(a))
                .Where(a => a.GetCustomAttribute<DeviceConnectedTypeAttribute>() != null)
                .Select(a => a.GetCustomAttribute<DeviceConnectedTypeAttribute>().Name);
            return types.ToList();
        }
    }
}
