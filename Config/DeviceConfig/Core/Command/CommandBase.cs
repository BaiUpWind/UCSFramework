using CommonApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceConfig.Core
{ 
    /// <summary>
    /// 指令基类
    /// </summary>
    public abstract  class CommandBase
    {
        
        /// <summary>
        /// 操作指令
        /// </summary>
        public abstract object CommandStr { get; set; }

        /// <summary>
        /// 指令的返回结果
        /// </summary>
        [JsonConverter(typeof(PolyConverter))]
        public abstract ResultBase Result { get; set; }  
    } 
}
