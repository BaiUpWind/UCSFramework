using CommonApi.DBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static DBEnum BEnum { get; set; } = DBEnum.MySql;
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string DataBaseConnectionStr { get; set; } = "";

        public static DataBasePara DBpara { get; set; }
        #endregion
    }
}
