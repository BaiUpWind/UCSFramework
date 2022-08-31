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
        [Control("Sql","SQL", ControlType.TextBox,Height:400,Width:300 )]
        public string Sql { get; set; } = "";

        #region 测试代码

        [Control("test1", "test1", ControlType.TextBox )] 
        public string test1 { get; set; }

        [Control("test2", "test2", ControlType.TextBox)] 
        public string test2 { get; set; } 

        [Control("test3", "test3", ControlType.TextBox)] 
        public string test3 { get; set; }

        [Control("test4", "test4", ControlType.TextBox)] 
        public string test4 { get; set; }

        [Control("test5", "test5", ControlType.TextBox)] 
        public string test5{ get; set; }

        [Control("test6", "test6", ControlType.TextBox)] 
        public string test6 { get; set; }

        [Control("test7", "test7", ControlType.TextBox)] 
        public string test7 { get; set; }

        #endregion
    }
}
