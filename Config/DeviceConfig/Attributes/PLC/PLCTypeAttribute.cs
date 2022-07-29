using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceConfig 
{ 
    /// <summary>
    /// PLC配置类属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class PLCTypeAttribute : ConfigBaseAttribute
    {
        public PLCTypeAttribute(string name) : base(name)
        {

        }
    }
}
