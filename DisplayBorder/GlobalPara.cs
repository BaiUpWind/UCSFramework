using CommonApi;
using DeviceConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using CommonApi.Event;

namespace DisplayBorder
{
    /// <summary>
    /// 全局参数
    /// </summary>
    internal class GlobalPara
    {
        /// <summary>
        /// 当前系统的工作路径
        /// </summary>
        public readonly static string SysPath = Directory.GetCurrentDirectory().ToString();
        /// <summary>
        /// 系统文件夹路径
        /// <para>包含基础的配置文件</para>
        /// </summary>
        public readonly static string SysFilePath = SysPath + "\\SystemFile";

        /// <summary>
        /// 配置文件的路径
        /// </summary>
        public readonly static string ConfigPath = SysPath + "\\Configs";

        /// <summary>
        /// 组文件路径
        /// </summary>
        public readonly static string GroupsFilePath = ConfigPath + "\\Groups.json";
        private static IList<Group> groups; 
        private static EventManager eventManager; 

        public static IList<Group> Groups
        {
            get
            {
                if (groups == null)
                {
                    groups = JsonHelper.ReadJson<Group>(GroupsFilePath);
                }
                return groups;
            }
            set
            {
                if (value != null && value.Count > 0)
                {
                    groups = value;
                    JsonHelper.WriteJson(groups, GroupsFilePath);
                } 
            }
        }


        public static EventManager EventManager
        {
            get
            {
                if (eventManager == null)
                {
                    eventManager = new EventManager();
                }  
                return eventManager;
            }
        }


        public static void CheckPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
