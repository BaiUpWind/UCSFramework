using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceConfig
{

    /// <summary>
    /// 设备连接类型属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ConnectedTypeAttribute: ConfigBaseAttribute
    { 
        public ConnectedTypeAttribute(string name):base(name)
        {
            
        }
    }
}
