using ControlHelper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBackUpDB.Model
{
    public class SysConfig
    {

        [NickName("备份路径")]
        public string BackUpPath { get; set; } = "D:/data/db_BackUp";
        [NickName("MysqlBin目录")]
        public string MySqlPath { get; set; } = "D:\\Program Files\\MySQL\\MySQL Server 5.6\\bin";
        [NickName("开机启动")]
        public bool EnableStratUp { get; set; } = true;

        [NickName("备份间隔(分钟)")]
        public double BackUpInterval { get; set; } = 60;

        [NickName("删除超时(分钟)")]
        public double DeleteExpired { get; set; } = 420;

        [NickName("检查间隔(毫秒)")]
        public int CheckInterval { get; set; } = 1000;

        [Hide]
        public List<DBConnectConfig> DBConnectConfigs { get; set; } = new List<DBConnectConfig>();
    }

    public class BackUpInfo
    {
        [NickName("备份路径")]
        public string Path { get; set; }
        [NickName("备份时间")]
        public string Time { get; set; }
        [NickName("数据库配置")]
        public DBConnectConfig DBConnect { get; set; }
    }
}
