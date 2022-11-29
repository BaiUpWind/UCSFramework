using DisplayConveyer.Model;
using DBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayConveyer.DA
{
    public class DA_BeltConfig :GetDBBase
    {
        public DA_BeltConfig(string connStr) : base( DBType.MySql, connStr)
        {

        }

        /// <summary>
        /// 获取编辑 表名（sys_belt_edit_table）
        /// </summary>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public List<BeltEditor> GetBeltEditors(string TableName,out string errinfo  )
        {
            try
            {
                errinfo = "";
                string sql = $"select * from {TableName}";
                var result = db.GetDataList<BeltEditor>(sql);
                if (result != null && result.Any())
                {
                    return result;
                }
                return null;
            }
            catch (Exception  ex)
            {
                errinfo =$"未知错误：{ex.Message}" ;
                return null;
            }
          
        }
        /// <summary>
        /// 搞到这个表的数据 sys_belt_plc_read_data_table
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public List<BeltReadPlc> GetBeltReadPlcs(string tableName,out string errinfo)
        {
            try
            { 
                errinfo = null;
                string sql = $"select * from {tableName}";
                var result = db.GetDataList<BeltReadPlc>(sql);
                if (result != null && result.Any())
                {
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                errinfo = $"未知错误：{ex.Message}";
                return null;
            }
        }
    }
}
