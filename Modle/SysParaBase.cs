using CommonApi.DBHelper;
using Modle.DeviceCfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modle
{
    /// <summary>
    /// 系统基础的参数
    /// </summary>
    public  class SysParaBase
    {
        
     
        #region 数据库基本参数
        /// <summary>
        /// 数据库类型
        /// </summary>
        public static DBEnum BEnum { get; set; } = DBEnum.MySql;
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string DataBaseConnectionStr { get; set; } = "";

        public static DataBaseCfg DBpara { get; set; }

        #endregion

        #region 设备通信配置

        /// <summary>
        /// 目前程序包含的串口通信
        /// </summary>
        public List<SerialPortCfg> SerialPortCfgs { get; set; } = new List<SerialPortCfg>();
        /// <summary>
        /// 目前程序包含的PLC通信
        /// </summary>
        public List<PLCCfg> PLCCfgs { get; set; } = new List<PLCCfg>();
        /// <summary>
        /// 目前程序包含的TCP/ip的客户端通信
        /// </summary>
        public List<TCPCfgBase> TCPClients { get; set; } = new List<TCPCfgBase>();

        #endregion
    }
}
