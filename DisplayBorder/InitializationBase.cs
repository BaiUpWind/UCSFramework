using CommonApi;
using DeviceConfig;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayBorder
{
    internal class InitializationBase
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

        public readonly static string ConfigPath = SysPath + "\\Configs";

        public readonly static string JsonFilePath = ConfigPath + "\\Groups.json";
        private static IList<Group> groups;

        public static void CheckPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static IList<Group> Groups
        {
            get
            {
                if (groups == null)
                {
                    groups = JsonHelper.ReadJson<Group>(JsonFilePath);
                }
                return groups;
            }
            set
            {
                if (value != null && value.Count > 0)
                {
                    groups = value;
                    JsonHelper.WriteJson(groups, JsonFilePath);
                } 
            }
        }

    }
}
