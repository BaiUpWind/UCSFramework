using ControlHelper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackUpDB.Model
{
    public class DBConnectConfig
    {

        [ReadOnly]
        public int ID { get; set; } = 1;
        [NickName("别名")]
        public string Name { get; set; } = "数据库";
        /// <summary>
        /// 数据库的ip
        /// </summary>
        [NickName("数据库的地址")]
        public string DBIp { get; set; } = "127.0.0.1";
        /// <summary>
        /// 数据库实例名
        /// </summary>
        [NickName("数据库名")]
        public string DBName { get; set; } = "capanew";
        /// <summary>
        /// 数据库端口
        /// </summary>
        [NickName("数据库端口")]
        public string DbPort { get; set; } = "3306";
        /// <summary>
        /// 数据库登入名
        /// </summary>
        [NickName("数据库登入名")]
        public string DBUserName { get; set; } = "root";
        /// <summary>
        /// 数据库登入密码
        /// </summary>
        [NickName("数据库登入密码")]
        public string DBPassWord { get; set; } = "root";
    }

    internal static class DBConfigExtension
    {
        public static string GetConnectionStr(this DBConnectConfig tag)
        => $"server={tag?.DBIp}; uid={tag?.DBUserName}; pwd={tag?.DBPassWord};database={tag?.DBName}";

    }
}
