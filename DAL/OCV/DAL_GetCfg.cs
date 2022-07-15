using CommonApi.DBHelper;
using CommonApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entity.OCV;

namespace DAL.OCV
{
    /// <summary>
    /// 测试框架可行性
    /// </summary>
    public class DAL_GetCfg : DAL_DBBase
    {
     
        public  OperateResult<List<Test_Now_Table>> GetNowTable()
        {
            OperateResult<List<Test_Now_Table>> or = new OperateResult<List<Test_Now_Table>>();
            try
            {
                string sql = "SELECT * from test_now_table ORDER BY position ASC ,id ASC";
               var result = db.GetDataList<Test_Now_Table>(sql, System.Data.CommandType.Text);
                if (result.Any())
                { 
                    or.IsSuccess = true;
                    or.Content = result;
                    or.Message = "获取数据成功！";
                    return or;
                }

                or.Message = "未获取任何的数据";
                or.IsSuccess = false;
                or.Content = null;
                return or;
            }
            catch (Exception ex)
            {
                or.IsSuccess = false;
                or.Message = ExceptionOperater.GetSaveStringFromException($"获取基础数据表错误", ex);
                or.Content = null;
                return or; 
            }
        }
    }
}
