using CommonApi.DBHelper;

namespace Modle
{
    /// <summary>
    /// 系统基础的参数
    /// </summary>
    public  class SysParaBase
    {
        
     
        #region 数据库基本参数
        /// <summary>
        /// 数据库类型
        /// </summary>
        public static DBType BEnum { get; set; } = DBType.MySql;
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string DataBaseConnectionStr { get; set; } = "";
         
        #endregion

      
    }
}
