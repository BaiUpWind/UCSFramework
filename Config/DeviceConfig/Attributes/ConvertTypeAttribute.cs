using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceConfig
{
    /// <summary>
    /// 告诉解析器做对应的解析,一般用在object对象上
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false,AllowMultiple =false)]
    public class ConvertTypeAttribute:Attribute
    {
        /// <summary>
        /// 告诉转换的类型
        /// </summary>
        /// <param name="targetType"></param>
        public ConvertTypeAttribute(Type targetType)
        {
            this.type = targetType;
        }
        private readonly Type type;
        public Type TargetType => type;
    }

    
}
