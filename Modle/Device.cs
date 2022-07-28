using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modle
{

    public class Device
    {
        /// <summary>
        /// 设备编号
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 信息在这个设备停留的时间
        /// </summary>
        public double StayTime { get; set; }
         
        /// <summary>
        /// 设备默认的连接方式
        /// </summary>
        public IDeviceConfig DefaultConn { get; set; }


        /// <summary>
        /// 这个设备需要显示的信息数据
        /// </summary>
        public List<DeviceInfoCfg>  DeviceInfos { get; set; }
    }
}
