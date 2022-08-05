using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  DeviceConfig.Core 
{
    public abstract class ResultBase
    {
        public IList<object> Datas;

        /// <summary>
        /// 数据库表
        /// </summary>
        public IList<Dictionary<string, object>> Tables;

        public object Data { get; set; }
    }
}
