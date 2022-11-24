using DBHelper;
using System;

namespace DeviceConfig.Core
{
    /// <summary>
    /// 连接配置基类
    /// </summary>
    public abstract class ConnectionConfigBase
    {
        [Hide]
        public int ConnectID { get; set; }
        [Hide] 
        public string ConnectName { get; set; } 
    } 

    public static class ConnStrHelper
    {
        //public static string GetConnStr(this DataBaseCfg tag)
        //{
        //    if (tag == null) return null;
        //    switch (tag.DbType)
        //    {
        //        case "0":
        //            return string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3};Password={4}", tag.DbIp, tag.DbPort, tag.DbName, tag.DbUserName, tag.DbPassWord);
        //        case "1":
        //            return string.Format("server={0}; uid={1}; pwd={2};database={3}", tag.DbIp, tag.DbUserName, tag.DbPassWord, tag.DbName);
        //        case "2":
        //            return string.Format("server={0};database={1}; uid={2};pwd ={3}", tag.DbIp, tag.DbName, tag.DbUserName, tag.DbPassWord);
        //    }
        //    throw new Exception($"错误的数据库类型{tag.DbType}");
        //}
        internal static string GetConnStr(this DataBaseConnectCfg tag)
        {
            if (tag == null) return null;
            switch (tag.DbType)
            {
                case   DBType.Oracle :
                    return string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3};Password={4}", tag.DbIp, tag.DbPort, tag.DbName, tag.DbUserName, tag.DbPassWord);
                case   DBType.SqlServer :
                    return string.Format("server={0}; uid={1}; pwd={2};database={3}", tag.DbIp, tag.DbUserName, tag.DbPassWord, tag.DbName);
                case    DBType.MySql :
                    return string.Format("server={0};database={1}; uid={2};pwd ={3}", tag.DbIp, tag.DbName, tag.DbUserName, tag.DbPassWord);
            }
            throw new Exception($"错误的数据库类型{tag.DbType}");
        } 
        /// <summary>
        /// 获取所有连接
        /// <para></para>
        /// <see cref=" Utility.Reflection.GetInheritors"/>
        /// </summary>
        /// <returns></returns>
        //public static ClassData GetConnections(this ConnectionConfigBase target)
        //{ 
        //    return Utility.Reflection.GetClassData<ConnectionConfigBase>();
        //} 

        /// <summary>
        ///  创建一个连接
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="ArgumentNullException"></exception>
        //public static ConnectionConfigBase CreataConnection(this ConnectionConfigBase target,string name)
        //{ 
        //     return Utility.Reflection.CreateObjectShortName<ConnectionConfigBase>(name);
        //} 
    }
}
