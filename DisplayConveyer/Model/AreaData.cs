using ControlHelper.Attributes;
using DeviceConfig;
using DeviceConfig.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayConveyer.Model
{
    public class AreaData
    {
        [NickName("区域编号")]
        public uint ID { get; set; }
        [NickName("区域名称")]
        public string Name { get; set; }
        //[NickName("顺序")]
        //public int Order { get; set; } 
        [NickName("读取方式")]
        [JsonConverter(typeof(PolyConverter))]
        public OperationBase Operation { get; set; }

        [DataGrid]
        [NickName("设备集合")]
        public List<DeviceData> Devices { get; set; } = new List<DeviceData>();
    }

    
}
