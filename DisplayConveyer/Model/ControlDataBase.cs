using ControlHelper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayConveyer.Model
{
    public class ControlDataBase
    {
        [ReadOnly]
        public int ID { get; set; }
        [Hide]
        public double Width { get; set; }
        [Hide]
        public double Height { get; set; }
        [Hide]
        public double PosX { get; set; }
        [Hide]
        public double PosY { get; set; }
        [Hide]
        public bool EnableThumbHorizontal { get; set; } = true;
        [Hide]
        public bool EnableThumbVertical { get; set; } = true;
    }
}
