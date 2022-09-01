using DeviceConfig.Core;
using System.Collections.Generic;
using System.Linq;
using System;
using CommonApi;
using Newtonsoft.Json;

namespace DeviceConfig
{
    [Obsolete("不再使用 20220901 找个机会删除掉")]
    public class Device
    {
        /// <summary>
        /// 设备编号
        /// </summary> 
        [Control("DeviceId", "设备编号", ControlType.TextBox)]
        public int DeviceId { get; set; }

        /// <summary>
        /// 当前这个这边正在运行信息采集中
        /// </summary>
        public  bool IsRunning { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary> 
        [Control("DeviceName", "设备名称", ControlType.TextBox)]
        public string DeviceName { get; set; }

        /// <summary>
        /// 信息在这个设备停留的时间
        /// </summary> 
        [Control("StayTime", "停留的时间", ControlType.TextBox)]
        public double StayTime { get; set; } = 5;

        /// <summary>
        /// 设备默认的连接方式
        /// </summary> 
        //[JsonConverter(typeof(PolyConverter))]
        [JsonIgnore]
        public ConnectionConfigBase DefaultConn { get; set; }
         

        public int DeviceInfoCount
        {
            get
            {
                if(DeviceInfos == null)
                {
                    return 0;
                }
                return DeviceInfos.Count;
            }
        }

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
