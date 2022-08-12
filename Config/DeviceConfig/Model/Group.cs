using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceConfig
{
    /// <summary>
    /// 组/车间 的配置 最顶级的单位
    /// </summary>
    public class Group
    {
        /// <summary>
        /// 组编号
        /// </summary>  
        [Control("GroupID","组编号", ControlType.TextBox)]
        public int GroupID { get; set; }
        /// <summary>
        /// 组名称
        /// </summary>
        [Control("GroupName", "组名称", ControlType.TextBox)] 
        public string GroupName { get; set; }

         
        /// <summary>
        /// 组内包含的设备类型
        /// </summary>
        //public List<Device> DeviceConfigs { get; set; }

        public int DeviceCount
        {
            get
            {
                if(DeviceConfigs != null)
                {
                    return DeviceConfigs.Count;
                }
                return 0;
            }
        }
        /// <summary>
        /// 当前正在扫描的设备名称
        /// </summary>
        public string IsRunningDeviceName
        {
            get
            {
                if (DeviceConfigs == null)
                {
                    return "没有设备";
                }
                var result = DeviceConfigs.Where(a => a.IsRunning).FirstOrDefault();
                if(result == null)
                {
                    return "无";
                }
                return $"[{result.DeviceId}]{result.DeviceName}";
            }

        }



        public IList<Device> DeviceConfigs { get; set; }  
        
    }
}
