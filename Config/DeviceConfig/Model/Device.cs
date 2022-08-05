using DeviceConfig.Core;
using System.Collections.Generic;
using System.Linq;
using System;
using CommonApi;
using Newtonsoft.Json;

namespace DeviceConfig
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
        public double StayTime { get; set; } = 5;

        /// <summary>
        /// 设备默认的连接方式
        /// </summary> 
        //[JsonConverter(typeof(PolyConverter))]
        [JsonIgnore]
        public ConnectionConfigBase DefaultConn { get; set; }
         
        /// <summary>
        /// 这个设备需要显示的信息数据
        /// </summary>
        public IList<DeviceInfo>  DeviceInfos { get; set; }


        //public void AddDeviceInfo(DeviceInfo data)
        //{
        //    DeviceInfos.Add(data);
        //} 
        //public void RemoveDeviceInfo(int deviceID)
        //{
        //    var result = DeviceInfos.Where(a => a.DeviceID == deviceID).FirstOrDefault();
        //    if (result == null)
        //    {
        //        throw new ArgumentNullException($"未找到对应的设备编号'{deviceID}'");
        //    }
        //    DeviceInfos.Remove(result);
        //}
    }
}
