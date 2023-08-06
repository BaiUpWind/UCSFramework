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
        [NickName("开机启动")]
        public bool EnableStratUp { get; set; } = true;

        [NickName("备份间隔")]
        public int BackUpInterval { get; set; } = 60;

        [NickName("删除超时")]
        public int DeleteExpired { get; set; } = 420;



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
