using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modle 
{
    /// <summary>
    /// 显示这个属性到界面  
    /// </summary>
    [Serializable]
    [AttributeUsage( AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ShowIfAttribute : Attribute
    {

        public ShowIfAttribute(string MeberName)
        {
            meberName = MeberName;
        }

        private readonly string meberName;

        public string MeberName { get => meberName;  }
    }

    [Serializable]
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class HideIfAttribute : Attribute
    {
        public HideIfAttribute(string MeberName)
        {
            meberName = MeberName;
        }

        private readonly string meberName;

        public string MeberName { get => meberName; }
    }
}
