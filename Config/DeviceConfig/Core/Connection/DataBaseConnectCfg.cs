namespace DeviceConfig.Core
{
    public sealed class DataBaseConnectCfg : ConnectionConfigBase
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
}
