using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonApi;

namespace DeviceConfig.Core
{
    /// <summary>
    /// 连接基类
    /// </summary>
    public abstract class ConnectionBase
    {
        public int ConnectID { get; set; }
        public string ConnectName { get; set; }


   
    }


    public sealed class TcpConnect : ConnectionBase
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 端口
        /// </summary> 
        public int Port { get; set; }
    }



    public sealed class SerialPortConnect : ConnectionBase
    {
        /// <summary>
        /// 串口的名称
        /// </summary> 
        public string PortName { get; set; }
        /// <summary>
        /// COM口的名称
        /// </summary> 
        public string COMNmae { get; set; }

        /// <summary>
        /// 波特率
        /// </summary> 
        public int BaudRate { get; set; }
    }

    public sealed class DataBaseConnect : ConnectionBase
    {
        /// <summary>
        /// 数据库类型 0 oracle 1 sqlserver 2 mysql
        /// </summary> 
        public string DbType { get; set; } = "2";
        /// <summary>
        /// 数据库的ip
        /// </summary> 
        public string DbIp { get; set; } = "127.0.0.1";
        /// <summary>
        /// 数据库实例名
        /// </summary> 
        public string DbName { get; set; } = "rgvline";
        /// <summary>
        /// 数据库端口
        /// </summary> 
        public string DbPort { get; set; } = "3306";
        /// <summary>
        /// 数据库登入名
        /// </summary> 
        public string DbUserName { get; set; } = "root";
        /// <summary>
        /// 数据库登入密码
        /// </summary> 
        public string DbPassWord { get; set; } = "root";

        /// <summary>
        /// 数据库连接超时时间
        /// </summary>
        public string ConnectTimeOut { get; set; } = "3";
         
    }

    
    //--------------------------- PLC

    public abstract class PLCConnection : ConnectionBase
    {
        public string ServierIp { get; set; } = "192.168.0.1";
        public int Port { get; set; }
    }
    


    public sealed class SiemensConnect : PLCConnection
    {
         
        // <summary>
        /// 西门子型号
        /// <para>区间[1-6]</para>
        /// <para> 1 : S1200  </para>
        /// <para> 2 : S300 </para>
        /// <para> 3 : S400,</para>
        /// <para> 4 : S1500 </para>
        /// <para> 5 : S200Smart</para>
        /// <para> 6 : S200</para>
        /// </summary>    
        public int SiemensSelected { get; set; } = 5;
         
        /// <summary>
        /// 机架号
        /// </summary>
        public byte Rack { get; set; }
        /// <summary>
        /// 槽号
        /// </summary> 
        public byte Slot { get; set; }
    }
}
