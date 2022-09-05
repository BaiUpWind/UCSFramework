using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  DeviceConfig.Core 
{
    public abstract class ResultBase
    {
        /// <summary>
        /// 数据,原始数据
        /// </summary>
        public object Data { get; set; } = null;

        /// <summary>
        /// 解析完成后的数据
        /// </summary>
        public object FinalData { get; set; } = null;



    }
}
