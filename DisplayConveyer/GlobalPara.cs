
using DisplayConveyer.Config;
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
        public static readonly string ConfigPath = Directory.GetCurrentDirectory() + @"\Config.cfg";


        static readonly JsonHelper configFile = new JsonHelper(ConfigPath);
        private static MainConfig config;

        /// <summary>
        /// 系统配置文件
        /// </summary>
        public static JsonHelper ConfigFile => configFile;


        public static void PathCheck()
        {
            if (!File.Exists(ConfigPath))
            {
                throw new CantFindFileException("未能找到设备的配置文件!");
            }
        }

        public static MainConfig Config
        {
            get
            {
                if (config == null)
                {
                    try
                    {
                        config = configFile.ReadJson<MainConfig>(true);
                    }
                    catch (Exception ex )
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
                    configFile.WriteJson(value);
                    config = value;
                } 
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
