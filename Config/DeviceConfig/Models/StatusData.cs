using ControlHelper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Config.DeviceConfig.Models
{
    public class StatusData
    {
        [NickName("设备编号","与区域中的设备编号对应")]
        public string WorkId { get; set; }
        [NickName("设备状态_DB地址")]
        public string MachineAddress { get; set; }
        [NickName("盘状态_DB地址")]
        public string LoadDBAddress { get;set; }
        /// <summary>
        /// 设备状态
        /// 0 未定义 无显示
        /// 1-99 报警
        /// 100 自动
        /// 101 手动
        /// </summary>
        [Hide]
        public int MachineState { get; set; }
        /// <summary>
        /// 盘状态
        /// 0 无盘
        /// 1 有盘
        /// </summary>
        [Hide]
        public int LoadState { get; set; }
        
    }
}
