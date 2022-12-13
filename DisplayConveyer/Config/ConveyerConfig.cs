using ControlHelper.Attributes;
using DisplayConveyer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayConveyer.Config
{
    public class ConveyerConfig
    {
        [NickName("画布宽")]
        public double CanvasWidth { get; set; }
        [NickName("画布高")]
        public double CanvasHeight { get; set; }
        [NickName("配置信息")] 
        public List<AreaData> Areas { get; set; }= new List<AreaData>();

        [Hide]
        [NickName("区域标记")]
        public List<RectData> RectDatas { get; set; } = new List<RectData>();

        [Hide]
        [NickName("标签显示")]
        public List<LabelData> Labels { get; set; } = new List<LabelData>();
    }
}
