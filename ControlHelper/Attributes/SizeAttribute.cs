using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlHelper.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class SizeAttribute : Attribute
    {
        public SizeAttribute(double Width, double Height)
        {
            width = Width;
            height = Height;
        }
        private double width;
        private double height;
        public double Width => width;
        public double Height => height;
    }
}
