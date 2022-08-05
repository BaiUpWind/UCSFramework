namespace DeviceConfig.Core
{
    public sealed class DataBaseConnectCfg : ConnectionConfigBase
    {
        /// <summary>
        /// 数据库类型
        /// </summary>
        [Control("DbType", "数据库类型"  , ControlType.ComboBox, Items: new object[] { "Oracle", "SqlServer", "MySQL" })]
        public int DbType { get; set; } = 2;
        /// <summary>
        /// 数据库的ip
        /// </summary>
        [Control("DbIp", "IP地址", ControlType.TextBox, ReadOnly: true)]
        public string DbIp { get; set; } = "192.168.1.1";
        /// <summary>
        /// 数据库实例名
        /// </summary>
        [Control("DbName", "实例名", ControlType.TextBox)]
        public string DbName { get; set; } = "capanew";
        /// <summary>
        /// 数据库端口
        /// </summary>
        [Control("DbPort", "端口", ControlType.TextBox)]
        public string DbPort { get; set; } = "3306";
        /// <summary>
        /// 数据库登入名
        /// </summary>
        [Control("DbUserName","登入名", ControlType.TextBox)]
        public string DbUserName { get; set; } = "root";
        /// <summary>
        /// 数据库登入密码
        /// </summary>
        [Control("DbUserName","登入密码", ControlType.TextBox)]
        public string DbPassWord { get; set; } = "root";

    }
}
