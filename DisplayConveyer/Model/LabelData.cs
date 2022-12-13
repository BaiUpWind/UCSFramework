using ControlHelper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayConveyer.Model
{
    public class LabelData: ControlDataBase
    {
        [NickName("显示信息")]
        public string Text { get; set; }
        [NickName("字体大小")]
        public double FontSize { get; set; }
 
    }
}
