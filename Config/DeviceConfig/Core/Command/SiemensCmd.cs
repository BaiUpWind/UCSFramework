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
        private List<DBData> cmdStr  ;
         [DataGrid]
        [ConvertType(typeof(List<DBData>))]
        public override object CommandStr
        {
            get
            {
                if(cmdStr==null)
                {
                    cmdStr = new List<DBData>();
                }
                return cmdStr;
            }
            set
            {
                cmdStr = value as List<DBData>;
            }
        }
         
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
     
}
