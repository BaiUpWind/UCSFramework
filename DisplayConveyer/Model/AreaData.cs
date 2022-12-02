using DeviceConfig.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayConveyer.Model
{
    internal class AreaData
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Order { get; set; } 
        public OperationBase Operation { get; set; } 
        public List<DeviceData> Devices { get; set; } = new List<DeviceData>();
    }

    public class StatusModel
    {
        public string ID { get; set; }
        public int Status { get; set; }
    }

    public class StatusResult : ResultBase
    {

    }
}
