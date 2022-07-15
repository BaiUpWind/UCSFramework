using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonApi;
using DAL.OCV;
using Entity.OCV;

namespace BLL.OCV
{
    /// <summary>
    ///  测试框架可行性
    /// </summary>
    public class BLL_CfgOp
    {
        public OperateResult<List<Test_Now_Table>> GetReslut()
        {
            OperateResult<List<Test_Now_Table>> or = new OperateResult<List<Test_Now_Table>>();
            try
            {
                using(DAL_GetCfg dal = new DAL_GetCfg())
                {

                    var result = dal.GetNowTable();
                    if(result.Content.Any())
                    { 
                        return result;
                    } 
                    return result;
                }

            }
            catch (Exception ex )
            { 
               
                throw ex;
            }
        }
    }
}
