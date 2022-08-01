using CommonApi.PLC;
using DeviceConfig.Core;
using System;

namespace DeviceConfig
{
    /// <summary>
    /// 西门子读取指令
    /// </summary>
    public sealed class SeimensOperation : PLCOperation
    {
        public SeimensOperation(ConnectionConfigBase config) : base(config)
        {
            if (config is SiemensConnectCfg siemens)
            {
                splc = new SiemensPlc(siemens.SiemensSelected, siemens.IP, siemens.Port, siemens.Rack, siemens.Slot);
            }
            throw new Exception("错误的PLC配置类型");
        }
        private SiemensPlc splc;


        public override bool Connect()
        {
            return splc != null && splc.Connection();
        }

        public override void Disconnected()
        {
            splc?.DisConnected();
        }
         
        public override ResultBase Read(CommandBase command)
        {
            throw new NotImplementedException();
        }
    }
}
