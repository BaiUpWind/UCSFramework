using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  DeviceConfig 
{
    /// <summary>
    ///  对字段或者属性 允许直接实例化操作
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false,AllowMultiple =false)]
    public class InstanceAttribute:Attribute  {   }
}
