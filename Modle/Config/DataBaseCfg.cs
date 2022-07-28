using CommonApi.DBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modle
{
    [DeviceConnectedType("DataBase")]
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
        /// <summary>
        /// sql
        /// </summary>
        public string Sql1 { get; set; } = "select * from";
        public string Sql2 { get; set; } = "select * from";

        public string Sql3 { get; set; } = "select * from";

        public string Sql4 { get; set; } = "select * from";


    }

    public static class ConnStrHelper
    {
        public static string GetConnStr(this DataBaseCfg tag)
        {
            if (tag == null) return null;
            switch (tag.DbType)
            {
                case "0":
                    return string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3};Password={4}", tag.DbIp, tag.DbPort, tag.DbName, tag.DbUserName, tag.DbPassWord);
                case "1":
                    return string.Format("server={0}; uid={1}; pwd={2};database={3}", tag.DbIp, tag.DbUserName, tag.DbPassWord, tag.DbName);
                case "2":
                    return string.Format("server={0};database={1}; uid={2};pwd ={3}", tag.DbIp, tag.DbName, tag.DbUserName, tag.DbPassWord); 
            }
            throw new Exception($"错误的数据库类型{tag.DbType}");
        }

        public static DBEnum GetDBEnum(this DataBaseCfg tag)
        {
            if (tag == null) throw new Exception("数据库配置数据空值异常！");
            switch (tag.DbType)
            {
                case "0":
                    return DBEnum.Oracle;
                case "1":
                    return DBEnum.SqlServer;
                case "2":
                    return DBEnum.MySql;
            }
            throw new Exception($"错误的数据库类型{tag.DbType}");
        }
    }

}
