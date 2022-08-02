using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceConfig.Core
{
    /// <summary>
    /// 组/车间 的配置 最顶级的单位
    /// </summary>
    public class Group
    {
        /// <summary>
        /// 组编号
        /// </summary>  
        //[TextBox("GroupNo","组编号","组编号:")]
        public int GroupID { get; set; }
        /// <summary>
        /// 组名称
        /// </summary>
        //[TextBox("GroupName", "组名称", "组名称:")]
        public string GroupName { get; set; }

         
        /// <summary>
        /// 组内包含的设备类型
        /// </summary>
        public List<Device> DeviceConfigs { get; set; }
        
    }
}
