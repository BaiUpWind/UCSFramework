using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modle 
{

    //[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    //public class SiemensCfgAttribute : ConfigBaseAttribute
    //{

    //    public SiemensCfgAttribute(string name) : base(name)
    //    {

    //    } 
    //}

    /// <summary>
    /// PLC配置类属性
    /// </summary>
    public class PLCConfigAttribute : ConfigBaseAttribute
    {
        public PLCConfigAttribute(string name) : base(name)
        {

        }
    }
}
