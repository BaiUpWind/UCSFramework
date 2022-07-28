using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modle 
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ClassNameAttribute:ConfigBaseAttribute
    {
        public ClassNameAttribute(string name) : base(name)
        {

        }
    }
}
