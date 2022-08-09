using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  DeviceConfig.Core 
{
    public class SQLCmd : CommandBase
    {
        public SQLCmd()
        {
            Result = new SQLResult();
        }
        [Control("Sql","SQL", ControlType.TextBox)]
        public string Sql { get; set; } = "";
    }
}
