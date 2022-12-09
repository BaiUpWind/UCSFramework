using ControlHelper.Attributes;
using DisplayConveyer.Model;
using System.Collections.Generic;
 

namespace DisplayConveyer.Config
{
    public class MainConfig
    {
        [NickName("缩放比例")]
        [Hide]
        public double SacleRatio { get; set; } = .5d;
        [NickName("滑动速度")]
        public float SlideSpeed { get; set; } = 20f;

         
        #region 弃用
         
        [Hide]
        [NickName("启用超时", "当多少秒没有操作后就返回到主界面")]
        public bool EnableTimeOut { get; set; }
        [Hide] 
        [NickName("超时时间（毫秒）")]
        public int TimeOut { get; set; } 
        [Hide]
        public List<BeltConfig> Belts { get; set; } = new List<BeltConfig>();
        #endregion
    }

    public class BeltConfig
    {
        [NickName("物流名称")]
        public string BeltName { get; set; }
        [NickName("扫描周期（毫秒）","默认5000毫秒")]
        public int ScanIntervalTime { get; set; } = 5000;
        public DBConfig DBConfig { get; set; } = new DBConfig();
        public List<BeltEditorConfig> Editors { get; set; } = new List<BeltEditorConfig>();

    }

    public class BeltEditorConfig
    {
        /// <summary>
        /// 配置表名
        /// </summary>
        [NickName("配置表名")]
        public string TableName { get; set; }

        [NickName("读取信号表名")]
        public string ReadTableName { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [NickName("标题")]
        public string Title { get; set; }
        /// <summary>
        /// X轴偏移
        /// </summary>
        [NickName("X轴偏移")] 
        public double XOffSet { get; set; }
        /// <summary>
        /// Y轴偏移量
        /// </summary>
        [NickName("Y轴偏移量")] 
        public double YOffSet { get; set; }
    }
}
