using DeviceConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayBorder.Model
{
    /// <summary>
    /// 班次信息，人员管理配置
    /// </summary>
    public class ClassInfo
    {
        public int GroupID { get; set; }

        [Control("IsHidden", "隐藏显示", ControlType.CheckBox)]
        public bool IsHidden { get; set; } = false;

        [NickName("职称")]
        [Control("Position", "部门", ControlType.TextBox, Width: 380)]
        public string Position { get; set; } = "ME";
        [NickName("姓名")]
        [Control("Name", "姓名", ControlType.TextBox, Width: 380)]
        public string Name { get; set; }
        [NickName("电话")]
        [Control("TelelPhone", "电话", ControlType.TextBox, Width: 380)]
        public string TelelPhone { get; set; }
    }
}
