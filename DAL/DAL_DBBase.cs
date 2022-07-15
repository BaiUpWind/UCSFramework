using CommonApi.DBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
namespace DAL
{
    /// <summary>
    /// 需要访问数据的都继承这个类
    /// </summary>
    public abstract class DAL_DBBase : GetDB
    { 
        public DAL_DBBase(): base(Modle.SysParaBase.BEnum, Modle.SysParaBase.DataBaseConnectionStr) 
        {
            
        }
    }
}