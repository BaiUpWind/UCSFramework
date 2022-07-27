using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modle 
{

    /// <summary>
    /// 设备连接类型属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class DeviceConnectedTypeAttribute:Attribute
    {
        readonly string name;
        public string Name => name;
        public DeviceConnectedTypeAttribute(string name)
        {
            this.name = name;
        }
    }
}
