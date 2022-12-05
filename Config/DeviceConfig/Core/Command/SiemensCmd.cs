using ControlHelper.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  DeviceConfig.Core 
{
    public sealed class SiemensCmd : PLCCmd
    {

        //[DataGrid]
        [ConvertType(typeof(List<DBData>))]
        public override object CommandStr { get  ; set  ; }
         
        [Hide]
        public override ResultBase Result { get; set; } = new PLCDBResult();
    }

    public class PLCDBResult : ResultBase
    {
        public override object Data { get  ; set  ; }  
    }
    public class DBData
    {
        [NickName("设备编号")]
        public string WorkId { get; set; }
        [NickName("DB地址")]
        public string DBAddress { get; set; }
        [Hide]
        public short Status { get; set; }
    }

    public class PLCDBCmd {

        [Control("DB","DB块",ControlType.TextBox)]
        public string DB { get; set; }
    }
}
