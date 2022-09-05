using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  DeviceConfig.Core 
{

    [Require(typeof(SQLResultAnalysis))]  
    
    public class SQLCmd : CommandBase
    {
        public SQLCmd()
        {
            Result = new SQLResult(); 
        }

        [Control("SQL", "查询语句", ControlType.TextBox, Height: 200, Width: 300,Order:1)]
        public override object CommandStr { get  ; set ; }
         
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

    public class SQLResultAnalysis : ResulteAnalysisBase
    {
        public override object GetData(ResultBase rb, string cmdStr, DataType dataType )
        {
            if(rb is SQLResult result)
            {
                if(result.Data != null)
                {
                    //这里的Data一定是DataTable
                    if(result.Data is DataTable dt)
                    {

                    }

                    switch (dataType)
                    {
                        case DataType.饼状图:
                             
                            break;
                        case DataType.网格:
                            break;
                        case DataType.柱状图:
                            break; 
                    }
                }
            }
            return null;
        }
    }

    /// <summary>
    /// 指令解析基类
    /// <para>用于解析对应</para>
    /// </summary>
    public abstract class ResulteAnalysisBase
    {
        public abstract object GetData(ResultBase rb,string cmdStr, DataType dataType);
    }
}
