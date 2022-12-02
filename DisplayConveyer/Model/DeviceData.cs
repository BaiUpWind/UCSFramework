using ControlHelper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace DisplayConveyer.Model
{
    public class DeviceData
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public Color FontColor { get; set; }
        public double FontSize { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }

        public Action<int> StatusChanged;
    }
}
