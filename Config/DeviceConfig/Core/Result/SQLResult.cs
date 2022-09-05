using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceConfig.Core 
{
    public class SQLResult : ResultBase
    {
        public string sql = "select count(1) as 剩余货位_绿色 , count(1) as 满盘货物_红色 ,count(1) as 超时货位_蓝色 from dual";
 
    }
}
