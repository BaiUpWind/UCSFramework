using ControlHelper.Attributes;

namespace DisplayConveyer.Model
{
    public class DBConfig
    {
        [NickName("编号","不要重复")]
        public int ID { get; set; }
        /// <summary>
        /// 数据库地址
        /// </summary>
        [NickName("DB-IP")]
        public string DBIp { get; set; } = "192.168.22.22";
        /// <summary>
        /// 数据库实例名
        /// </summary> 
        [NickName("数据库名称")]
        public string DBName { get; set; } = "capanew";
        /// <summary>
        /// 数据库端口
        /// </summary> 
        [NickName("数据库端口")]
        public string DBPort { get; set; } = "3306";
        /// <summary>
        /// 数据库登入名
        /// </summary> 
        [NickName("用户名")]
        public string DBUserName { get; set; } = "root";
        /// <summary>
        /// 数据库登入密码
        /// </summary> 
        [NickName("密码")]
        public string DBPassWord { get; set; } = "root";
    }

    internal static class DBConfigExtension
    {
        public static string GetConnectionStr(this DBConfig tag)
        => $"server={tag?.DBIp}; uid={tag?.DBUserName}; pwd={tag?.DBPassWord};database={tag?.DBName}";

    }
}
