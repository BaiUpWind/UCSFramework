using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DeviceConfig
{
    /// <summary>
    /// 组/车间 的配置 最顶级的单位
    /// </summary>  
    [Serializable]
    public class Group 
    { 
        /// <summary>
        /// 组编号
        /// </summary>  
        //[Control("GroupID","组编号", ControlType.TextBox)]
        [Hide]
        public int GroupID { get; set; }
        /// <summary>
        /// 组名称
        /// </summary>
        [Control("GroupName", "工艺名称", ControlType.TextBox)]
        [NickName("工艺名称")]
        public string GroupName { get; set; }
 

        #region 控件位置属性 
 
         
        [Control("LinePathColor", "线条颜色", ControlType.ColorPicker, FieldName: nameof(LinePathColor))]
        [NickName("线条颜色")]
        public Color LinePathColor { get; set; } = Colors.Blue;
       
        [Control("FontColor", "字体颜色", ControlType.ColorPicker, FieldName: nameof(FontColor))]
        [NickName("字体颜色")]
        public Color FontColor { get; set; } = Colors.White;

        //[NickName("字体大小")]
        //public int FontSize { get; set; } = 12;
        [Hide]
        [JsonIgnore]
        public SolidColorBrush PathBrush => new SolidColorBrush(LinePathColor);
        [Hide]
        [JsonIgnore]
        public SolidColorBrush FontBrush => new SolidColorBrush(FontColor);
        /// <summary>
        /// 位置x 对应在图像像素的x位置
        /// </summary>
        [Hide]
        //[Control("PosX", "位置X", ControlType.TextBox)]
        public double PosX { get; set; } = -1;
        /// <summary>
        /// 位置y 对应在图像像素的y位置
        /// </summary>
        [Hide]
        //[Control("PosY", "位置Y", ControlType.TextBox)]
        public double PosY { get; set; } = -1;

        /// <summary>
        /// 控件的宽度
        /// </summary>
        [Hide]
        public double CWidth { get; set; } = 100;

        /// <summary>
        /// 控件的高度
        /// </summary>
        [Hide]
        public double CHeight { get; set; } = 100;

        /// <summary>
        /// 方向
        /// <para>上下左右 分别是0,1,2,3</para>
        /// </summary>
        [Hide]
        public int Direction { get; set; } = 0;
        #endregion

        [JsonIgnore]
        public int DeviceCount
        {
            get
            {
                if (DeviceInfos != null)
                {
                    return DeviceInfos.Count;
                }
                return 0;
            }
        }
     

        [Control("DeviceInfos", "设备集合", ControlType.Collection, GenerictyType: typeof(DeviceInfo), FieldName: nameof(DeviceInfos))]
        [NickName("设备集合")]
        public List<DeviceInfo> DeviceInfos { get; set; }

       
    }
}
