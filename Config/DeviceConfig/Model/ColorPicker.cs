using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace  DeviceConfig 
{
    public class ColorChoose
    {
        public Color Color { get; set; }

        [Hide]
        public SolidColorBrush SolidColor => new SolidColorBrush(Color);
    }
}
