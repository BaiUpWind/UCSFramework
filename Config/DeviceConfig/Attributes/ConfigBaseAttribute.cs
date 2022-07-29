using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceConfig
{

    public abstract class ConfigBaseAttribute : Attribute
    {
        readonly string name;
        /// <summary>
        /// 类名
        /// </summary>
        public string Name => name;

        public ConfigBaseAttribute(string name)
        {
            this.name = name;
        }
    }
}
