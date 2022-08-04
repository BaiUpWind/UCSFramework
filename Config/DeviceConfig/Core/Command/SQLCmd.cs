using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  DeviceConfig.Core 
{
    public class SQLCmd : CommandBase
    {

        public string Sql { get; set; } = "select * from";
    }
}
