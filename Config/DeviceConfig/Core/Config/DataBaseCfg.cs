using System;

namespace DeviceConfig.Core
{
    /// <summary>
    /// 数据库访问配置
    /// </summary>
    [ConnectedType("DataBase")]
    public class DataBaseCfg : IDeviceConfig
    {
        public DeviceConnectedType DevType => DeviceConnectedType.DataBase;
        /// <summary>
        /// 数据库类型 0 oracle 1 sqlserver 2 mysql
        /// </summary>
        [Control("DbType", ControlType.ComboBox, Items: new object[] { "Oracle", "SqlServer", "MySQL" })]
        public string DbType { get; set; } = "2";
        /// <summary>
        /// 数据库的ip
        /// </summary>
        [Control("Ip", ControlType.TextBox, ReadOnly: true)]
        public string DbIp { get; set; } = "127.0.0.1"; 
        /// <summary>
        /// 数据库实例名
        /// </summary>
        [Control("DbName", ControlType.TextBox)]
        public string DbName { get; set; } = "rgvline";
        /// <summary>
        /// 数据库端口
        /// </summary>
        [Control("DbPort",ControlType.TextBox)]
        public string DbPort { get; set; } = "3306";
        /// <summary>
        /// 数据库登入名
        /// </summary>
        [Control("DbUserName",ControlType.TextBox)]
        public string DbUserName { get; set; } = "root";
        /// <summary>
        /// 数据库登入密码
        /// </summary>
        [Control("DbPassWord", ControlType.TextBox)]
        public string DbPassWord { get; set; } = "root";

        /// <summary>
        /// 数据库连接超时时间
        /// </summary>
        public string ConnectTimeOut { get; set; } = "3"; 

    } 


}
