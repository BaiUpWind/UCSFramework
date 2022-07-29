using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceConfig.Core
{
    /// <summary>
    /// 组的配置
    /// </summary>
    public class Group
    {
        /// <summary>
        /// 组编号
        /// </summary> 
        [Control("GroupNo", ControlType.TextBox)]
        public int GroupNo { get; set; }
        /// <summary>
        /// 组的名称
        /// </summary>
        [Control("GroupName",ControlType.TextBox)]
        public string GroupName { get; set; }

         
        /// <summary>
        /// 组内包含的设备类型
        /// </summary>
        public List<Device> DeviceConfigs { get; set; }
        
    }
}
