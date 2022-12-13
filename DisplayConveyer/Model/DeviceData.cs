using Config.DeviceConfig.Models;
using ControlHelper.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ReadOnlyAttribute = ControlHelper.Attributes.ReadOnlyAttribute;

namespace DisplayConveyer.Model
{
    public class DeviceData
    {
        /// <summary>
        /// 区域编号 <see cref="AreaData.ID"/>
        /// 当没有指定区域时为 0
        /// </summary>
        [ReadOnly]
        public uint AreaID { get; set; } = 0;

        [NickName("设备编号", "与WorkId对应")]
        public string ID { get; set; }
        [NickName("设备名称")]
        public string Name { get; set; }
        public Color FontColor { get; set; } = Colors.Black;
        public SolidColorBrush FontColorBursh => new SolidColorBrush(FontColor);
        public TranslateTransform Translate => new TranslateTransform(PosX, PosY);
        [NickName("字体大小")]
        public double FontSize { get; set; }
        [NickName("宽度")]
        public double Width { get; set; }
        [NickName("高度")]
        public double Height { get; set; }
        [NickName("物流方向描述")]
        public string Direction { get; set; } = "•";
        [Hide]
        public double PosX { get; set; }
        [Hide]
        public double PosY { get; set; }


        [JsonIgnore]
        public Action<StatusData> StatusChanged;

      
    }
}
