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
        /// 数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 显示数据使用的类型
        /// </summary>
        public DataType ShowDataType { get; set; }
    }
}
