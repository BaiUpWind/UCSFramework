using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modle
{
    public class DataBasePara
    {


        /// <summary>
        /// 数据库的ip
        /// </summary>
        public string DbIp { get; set; } = "192.168.0.0";
        /// <summary>
        /// 数据库类型 0 oracle 1 sqlserver 2 mysql
        /// </summary>
        public string DbType { get; set; } = "0";
        /// <summary>
        /// 数据库实例名
        /// </summary>
        public string DbName { get; set; } = "ORCL";

        /// <summary>
        /// 数据库端口
        /// </summary>
        public string DbPort { get; set; } = "1521";
        /// <summary>
        /// 数据库登入名
        /// </summary>
        public string DbUserName { get; set; } = "admin";
        /// <summary>
        /// 数据库登入密码
        /// </summary>
        public string DbPassWord { get; set; } = "admin";

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
}
