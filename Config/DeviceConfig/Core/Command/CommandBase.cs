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
    /// 读取指令基类
    /// </summary>
    public abstract   class CommandBase
    { 

        [JsonIgnore]
        public virtual ResultBase Result { get; set; }  
    } 
}
