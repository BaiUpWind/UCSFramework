using ControlHelper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayConveyer.Model
{
    public class MapPartData : ControlDataBase
    {
        [NickName("工艺名称")]
        public string Title { get; set; }
        [NickName("区域编号集合")]
        public List<uint> AreaIDs { get; set; } = new List<uint>();
    }
}
