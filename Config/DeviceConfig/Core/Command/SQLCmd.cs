using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  DeviceConfig.Core 
{
    public enum DataType
    {
        饼状图,
        网格,
        树状图
    }
  
    public class SQLCmd : CommandBase
    {
        public SQLCmd()
        {
            Result = new SQLResult(); 
        }
        [Control("SelectType", "结果显示图集", ControlType.ComboBox, EnumType: typeof(DataType))]
        public int SelectType { get; set; }
        [Control("SQL", "查询语句", ControlType.TextBox, Height: 200, Width: 300)]
        public string SQL { get; set; }

        
        //[Control("cmbResult", "结果显示图集", ControlType.ComboBox,Items:new object[] {"图标","网格","树状图"})]
        //[Control("Sql","SQL", ControlType.TextBox,Height:200,Width:300 )]
        //public string Sql { get; set; } = "";

        #region 测试代码
        //[Control("cmbResult", "结果显示图集", ControlType.ComboBox, Items: new object[] { "图标", "网格", "树状图" })]
        //[Control("test1", "test1", ControlType.TextBox )] 
        //public string test1 { get; set; }
        //[Control("cmbResult", "结果显示图集", ControlType.ComboBox, Items: new object[] { "图标", "网格", "树状图" })]
        //[Control("test2", "test2", ControlType.TextBox)] 
        //public string test2 { get; set; }
        //[Control("cmbResult", "结果显示图集", ControlType.ComboBox, Items: new object[] { "图标", "网格", "树状图" })]
        //[Control("test3", "test3", ControlType.TextBox)] 
        //public string test3 { get; set; }
        //[Control("cmbResult", "结果显示图集", ControlType.ComboBox, Items: new object[] { "图标", "网格", "树状图" })]
        //[Control("test4", "test4", ControlType.TextBox)] 
        //public string test4 { get; set; }

        //[Control("test5", "test5", ControlType.TextBox)] 
        //public string test5{ get; set; }

        //[Control("test6", "test6", ControlType.TextBox)] 
        //public string test6 { get; set; }

        //[Control("test7", "test7", ControlType.TextBox)] 
        //public string test7 { get; set; }

        #endregion
    }
}
