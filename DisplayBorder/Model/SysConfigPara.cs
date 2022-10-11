using CommonApi.Utilitys.Encryption;
using DeviceConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayBorder.Model
{
    /// <summary>
    /// 系统配置文件
    /// </summary>
    public class SysConfigPara
    {
        private string groupsFilePath;
        private string backImagPath;
        private string passWord = "lpzlRMWity8=";

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
        public string BackImagPath
        {
            get
            {
                //当没有路径时,使用默认路径
                if (string.IsNullOrEmpty(backImagPath))
                {
                    backImagPath = GlobalPara.BackImageFilePath;
                }
                return backImagPath;
            }
            set => backImagPath = value;
        }

        [Control("PassWord", "密码", ControlType.PassWord,MaxLenght:6)]
        public string PassWord
        {
            get
            {
                return  passWord;
            }
            set
            {
                passWord =value;
            }
        }  
    }
}
