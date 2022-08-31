﻿using CommonApi;
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

        private static IList<Group> groups; 
        private static EventManager eventManager;
        private static SysConfigPara sysConfig;
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
                   
                        groups = JsonHelper.ReadJson<IList<Group>>(SysConfig.GroupsFilePath,true);
                    }
                    catch (Exception ex)
                    {
                        groups = null;
                        throw ex; 
                    } 
                }
                return groups;
            }
            set
            {
                if (value != null && value.Count > 0)
                {
                    groups = value;
                    JsonHelper.WriteJson(groups, SysConfig.GroupsFilePath);
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
                if (sysConfig == null)
                {
                    sysConfig = JsonHelper.ReadJson<SysConfigPara>(ConfigPath,true);
                }
                return sysConfig;
            }
            set
            {
                if (value != null )
                {
                    sysConfig = value;
                    JsonHelper.WriteJson(sysConfig, ConfigPath);     
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


        public static void CheckPath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }

    /// <summary>
    /// 系统配置文件
    /// </summary>
    public class SysConfigPara
    {
        private string groupsFilePath;

        /// <summary>
        /// 标题
        /// </summary>
        [Control("Title", "标题", ControlType.TextBox)]
        public string Title { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Control("Description", "描述", ControlType.TextBox)]
        public string Description { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        [Control("Version", "版本", ControlType.TextBox)]
        public string Version { get; set; }
 
        /// <summary>
        /// 组的文件路径
        /// </summary>
        [Control("GroupsFilePath", "组配置路径", ControlType.TextBox)]
        [Control("FileChoose", null, ControlType.FilePathSelector, FieldName: nameof(GroupsFilePath), FileType: "cfg")]
        public string GroupsFilePath
        {
            get
            {
                //当没有路径时,使用默认路径
                if (string.IsNullOrEmpty(groupsFilePath))
                {
                    groupsFilePath = GlobalPara.GroupsFilePath;
                } 
                return groupsFilePath;
            }
            set => groupsFilePath = value;
        }

        /// <summary>
        /// 显示的图片信息
        /// </summary>
        [Control("BackImagPath", "图片配置路径", ControlType.TextBox)]
        [Control("FileChosee2", null, ControlType.FilePathSelector, FieldName: nameof(BackImagPath), FileType: "jpg")]
        public string BackImagPath { get; set; }

    }
}
