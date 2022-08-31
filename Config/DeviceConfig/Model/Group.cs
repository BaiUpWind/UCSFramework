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
    public class Group
    {
        /// <summary>
        /// 组编号
        /// </summary>  
        //[Control("GroupID","组编号", ControlType.TextBox)]
        public int GroupID { get; set; }
        /// <summary>
        /// 组名称
        /// </summary>
        [Control("GroupName", "组名称", ControlType.TextBox)] 
        public string GroupName { get; set; }

        #region 控件位置属性 

        [Control("LinePathColor","线条颜色",ControlType.ColorPicker,FieldName:nameof(LinePathColor))]
        public Color LinePathColor { get; set; } = Colors.Blue;

        [Control("FontColor", "字体颜色", ControlType.ColorPicker, FieldName: nameof(FontColor))] 
        public Color FontColor { get; set; } = Colors.White;

        [JsonIgnore]
        public SolidColorBrush PathBrush => new SolidColorBrush(LinePathColor);
        [JsonIgnore]
        public SolidColorBrush FontBrush => new SolidColorBrush(FontColor);
        /// <summary>
        /// 位置x 对应在图像像素的x位置
        /// </summary>
        //[Control("PosX", "位置X", ControlType.TextBox)]
        public double PosX { get; set; } =-1;
        /// <summary>
        /// 位置y 对应在图像像素的y位置
        /// </summary>
        //[Control("PosY", "位置Y", ControlType.TextBox)]
        public double PosY { get; set; }=-1;

        /// <summary>
        /// 控件的宽度
        /// </summary>
        public double CWidth { get; set; } = 100;

        /// <summary>
        /// 控件的高度
        /// </summary>
        public double CHeight { get; set; } = 100;

        /// <summary>
        /// 方向
        /// <para>上下左右 分别是0,1,2,3</para>
        /// </summary>
        public int Direction { get; set; } = 0;
        #endregion

        [JsonIgnore]
        public int DeviceCount
        {
            get
            {
                if(DeviceInfos != null)
                {
                    return DeviceInfos.Count;
                }
                return 0;
            }
        }
        [JsonIgnore]
        [Obsolete("暂时弃用 20220831")]
        /// <summary>
        /// 当前正在扫描的设备名称
        /// </summary>
        public string IsRunningDeviceName
        {
            get
            {
                if (DeviceConfigs == null)
                {
                    return "没有设备";
                }
                var result = DeviceConfigs.Where(a => a.IsRunning).FirstOrDefault();
                if(result == null)
                {
                    return "无";
                }
                return $"[{result.DeviceId}]{result.DeviceName}";
            }

        }


        [JsonIgnore]
        [Obsolete("暂时弃用 20220831")]
        public IList<Device> DeviceConfigs { get; set; }

        [Control("DeviceInfos", "设备集合", ControlType.Collection, GenerictyType: typeof(DeviceInfo), FieldName: nameof(DeviceInfos))]
        public IList<DeviceInfo> DeviceInfos { get; set; }  
        
    }
}
