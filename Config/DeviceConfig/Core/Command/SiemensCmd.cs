using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  DeviceConfig.Core 
{
    public sealed class SiemensCmd : PLCCmd
    {
        [Control("CommandStr","数据解析类型",ControlType.Genericity,GenerictyType: typeof(PLCDBCmd),FieldName:nameof(CommandStr))]
        [Control("编辑DB块",null,ControlType.Data,GenerictyType: typeof(PLCDBCmd), FieldName:nameof(CommandStr))]
        [ConvertType(typeof(string))]
        public override object CommandStr { get  ; set  ; }

        [Control("Result", "创建返回结果类型", ControlType.Data, GenerictyType: typeof(PLCDBResult), FieldName: nameof(Result))]
        [Instance]
        public override ResultBase Result { get; set; } = new PLCDBResult();
    }

    public class PLCDBResult : ResultBase
    {

    }

    public class PLCDBCmd {

        [Control("DB","DB块",ControlType.TextBox)]
        public string DB { get; set; }
    }
}
