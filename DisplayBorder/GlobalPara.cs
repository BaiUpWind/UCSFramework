using CommonApi;
using CommonApi.Event;
using DisplayBorder.Events;
using DisplayBorder.Model;
using GalaSoft.MvvmLight;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.IO;

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
        public readonly static string SysFliePath = SysPath + "\\SystemFile";

        /// <summary>
        /// 配置文件夹路径
        /// </summary>
        public readonly static string ConfigsPath = SysPath + "\\Configs";


        /// <summary>
        /// 系统配置文件的路径(默认路径)
        /// </summary>
        public readonly static string ConfigPath = SysFliePath + "\\Configs.syscfg";

        /// <summary>
        /// 组文件路径(默认路径)
        /// </summary>
        public readonly static string GroupsFilePath = ConfigsPath + "\\Groups.cfg";
        /// <summary>
        /// 人员配置路径(默认路径)
        /// </summary>
        public readonly static string ClassesFilePath = ConfigsPath + "\\Classes.cfg";
        public readonly static string BackImageFilePath = ConfigsPath + "\\背景图片.jpg";



        private static IList<Group> groups;
        private static EventManager eventManager;
        private static SysConfigPara sysConfig;
        private static IList<ClassInfo> classInfo;
        public static void Init()
        {
            CheckPath(SysPath);
        }

        /// <summary>
        /// 系统的所有组的信息
        /// </summary>
        public static IList<Group> Groups
        {
            get
            {
              
                if (groups == null)
                {
                    try
                    {
                        groups = JsonHelper.ReadJson<IList<Group>>(SysConfig.GroupsFilePath, true);
                    }
                    catch (Exception ex)
                    {
                        groups = null;
                        MessageBox.Error($"组文件配置损坏,\n\r具体信息'{ex.Message}'");
                        return null;
                    }
                }
                return groups;
            }
            set
            {
                if (value != null   )
                {
                    groups = value;
                    JsonHelper.WriteJson(groups, SysConfig.GroupsFilePath);
                    EventManager.Fire(null, OnValueChangedArgs.Create(groups));
                }
            }
        }

        /// <summary>
        /// 系统配置的参数
        /// </summary>
        public static SysConfigPara SysConfig
        {
            get
            {
                if (ViewModelBase.IsInDesignModeStatic)
                {
                    return null;
                }
                if (sysConfig == null)
                {
                    try
                    {
                        CheckPath(SysFliePath);
                        sysConfig = JsonHelper.ReadJson<SysConfigPara>(ConfigPath, true);
                    }
                    catch (Exception ex)
                    {
                        sysConfig = null;
                        MessageBox.Error($"配置文件损坏,\n\r具体信息'{ex.Message}'");
                        return null;
                    }
                }
                return sysConfig;
            }
            set
            {
                if (value != null)
                {
                    sysConfig = value;
                    JsonHelper.WriteJson(sysConfig, ConfigPath);
                    EventManager.Fire(null, OnValueChangedArgs.Create(sysConfig));
                }
            }
        }

        public static IList<ClassInfo> ClassInfos
        {
            get
            {

                if (classInfo == null)
                {
                    try
                    {
                        classInfo = JsonHelper.ReadJson<IList<ClassInfo>>(ClassesFilePath, true);
                    }
                    catch (Exception ex)
                    {
                        classInfo = null;
                        MessageBox.Error($"班次信息文件配置损坏,\n\r具体信息'{ex.Message}'");
                        return null;
                    }
                }
                return classInfo;
            }
            set
            {
                if (value != null)
                {
                    classInfo = value;
                    JsonHelper.WriteJson(classInfo, ClassesFilePath);
                    EventManager.Fire(null, OnValueChangedArgs.Create(classInfo));
                }
            }
        }

        /// <summary>
        /// 事件系统
        /// </summary>
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
        public static double TitleFontSize { get; set; }
        public static double TabFontSize { get;  set;  }
        public static double GridFontSize { get; set; } = 9;
 


        public static void CheckPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }


}
