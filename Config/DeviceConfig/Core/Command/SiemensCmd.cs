using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  DeviceConfig.Core 
{
    public sealed class SiemensCmd : PLCCmd
    {
        [Control("CommandStr","数据解析类型",ControlType.Genericity,GenerictyType: typeof(SimensCmd),FieldName:nameof(CommandStr))]
        [Control("编辑DB块",null,ControlType.Data,GenerictyType: typeof(SimensCmd), FieldName:nameof(CommandStr))]
        public override object CommandStr { get  ; set  ; }
    }

    public class SimensCmd {

        [Control("DB","DB块",ControlType.TextBox)]
        public string DB { get; set; }
    }
}
