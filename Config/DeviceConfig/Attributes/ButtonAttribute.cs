using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace   DeviceConfig 
{
    
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public class ButtonAttribute : Attribute
    {
        private readonly string name;
        private readonly string methodName;

        public ButtonAttribute(string Name,string MethodName)
        {
            name = Name;
            methodName = MethodName;
        }
        public string Name => name;
        public string MethodName => methodName;
    }
}
