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
        [NickName("动画速度")]
        public double AnimationSpeed { get; set; } = 10d;
        [NickName("动画方向,左往右(真),反之")]
        public bool AnimationDircetion { get; set; } = true;
        [Hide]
        [NickName("画布宽")]
        public double CanvasWidth { get; set; } = 1920;
        [Hide]
        [NickName("画布高")]
        public double CanvasHeight { get; set; } = 1080;
        [Hide]
        [NickName("小地图背景图片")]
        public string MiniMapImagePath { get; set; }
        [Hide]
        [NickName("小地图区域数据")]
        public List<MapPartData> MiniMapData { get; set; } = new List<MapPartData>();
        [Hide]
        [NickName("配置信息")]
        public List<AreaData> Areas { get; set; } = new List<AreaData>();

        [Hide]
        [NickName("区域标记")]
        public List<RectData> RectDatas { get; set; } = new List<RectData>();

        [Hide]
        [NickName("标签显示")]
        public List<LabelData> Labels { get; set; } = new List<LabelData>();
    }
}
