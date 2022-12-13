using ControlHelper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayConveyer.Model
{
    public class RectData: ControlDataBase
    {
        [NickName("线条宽度")]
        public double StrokeThickness { get;set; }
 
    }
}
