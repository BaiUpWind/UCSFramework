
using ControlHelper.Tools;
using DisplayConveyer.Config;
using DisplayConveyer.Model;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace DisplayConveyer
{
    public static class GlobalPara
    {
        public static readonly string ConfigPath = Directory.GetCurrentDirectory() + @"\SysConfig.cfg";
        public static readonly string ConveyerConfigPath = Directory.GetCurrentDirectory() + @"\ConveyerConfig.cfg";

        private static MainConfig config;
        private static ConveyerConfig conveyerConfig;
        private static IList<AreaData> areaDatas;

      
        /// <summary>
        /// 系统配置文件
        /// </summary>
        public static MainConfig Config
        {
            get
            {
                if (config == null)
                {
                    try
                    {
                        config = JsonHelper.ReadJson<MainConfig>(ConfigPath,true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Error($"组文件配置损坏,\n\r具体信息'{ex.Message}'");
                    }

                }
                return config;
            }

            set
            {
                if (value != null)
                {
                    JsonHelper.WriteJson( value, ConfigPath);
                    config = value;
                }
            }
        }
        public static ConveyerConfig ConveyerConfig
        {
            get
            {
                if (conveyerConfig == null)
                {
                    try
                    {
                        conveyerConfig = JsonHelper.ReadJson<ConveyerConfig>(ConveyerConfigPath, true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Error($"物流线配置文件损坏,\n\r具体信息'{ex.Message}'");
                    }
                }
                return conveyerConfig;
            }
            set
            {
                if (value != null)
                {
                    JsonHelper.WriteJson(value, ConveyerConfigPath);
                    conveyerConfig = value;
                }
                conveyerConfig = value;
            }
        }
   
        /// <summary>
        /// 记录上一次操作时间
        /// </summary>
        public static  DateTime   LastMotionTime { get; set; }
        /// <summary>
        /// 锁定当前界面
        /// </summary>
        public static bool Locked { get; set; }


        /// <summary>
        /// 记录当前操作时间
        /// </summary>
        public static void RecordTime() => LastMotionTime = DateTime.Now;

       
    }
}
