using Config.DeviceConfig.Models;
using ControlHelper.Attributes;
using Newtonsoft.Json;
using System;
using System.Windows.Media;
using ReadOnlyAttribute = ControlHelper.Attributes.ReadOnlyAttribute;

namespace DisplayConveyer.Model
{
    public class DeviceData : ControlDataBase
    {
        /// <summary>
        /// 区域编号 <see cref="AreaData.ID"/>
        /// 当没有指定区域时为 0
        /// </summary>
        [ReadOnly]
        public uint AreaID { get; set; } = 0;

        [NickName("设备编号", "与WorkId对应")]
        public string WorkId { get; set; }
        [NickName("设备名称")]
        public string Name { get; set; } 
        [NickName("字体大小")]
        public double FontSize { get; set; }

        [NickName("物流方向描述")]
        public string Direction { get; set; } = "•";

        [JsonIgnore]
        public Action<StatusData> StatusChanged;


    }
}
