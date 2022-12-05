using ControlHelper.Attributes;
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
       
    }
}
