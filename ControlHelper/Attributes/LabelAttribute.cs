using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlHelper.Attributes
{
    /// <summary>
    /// 自动创建含有监视布尔量变化Label控件，当传入的值不是bool 背景为红色，
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class LabelAttribute : Attribute
    {
        public LabelAttribute(string Info)
        {
            info = Info;
        }
        private readonly string info;
        public string Info => info;
    }
}
