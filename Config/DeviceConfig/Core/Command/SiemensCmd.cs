using Config.DeviceConfig.Models;
using ControlHelper.Attributes;
using System.Collections.Generic;

namespace DeviceConfig.Core
{
    public sealed class SiemensCmd : PLCCmd
    {
        private List<StatusData> cmdStr  ;
         [DataGrid]
        [ConvertType(typeof(List<StatusData>))]
        public override object CommandStr
        {
            get
            {
                if(cmdStr == null)
                {
                    cmdStr = new List<StatusData>();
                }
                return cmdStr;
            }
            set
            {
                cmdStr = value as List<StatusData>;
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
