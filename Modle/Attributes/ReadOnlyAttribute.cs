using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modle 
{

    [Serializable]
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ReadOnlyAttribute : Attribute
    {
        public ReadOnlyAttribute()
        {

        }
    }
}
