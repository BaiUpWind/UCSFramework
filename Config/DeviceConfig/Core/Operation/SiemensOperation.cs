using CommonApi.PLC;
using ControlHelper.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DeviceConfig.Core
{
    /// <summary>
    /// 西门子读取指令
    /// </summary>
    [DependOn(typeof(SiemensConnectCfg),typeof(SiemensCmd))]
    public sealed class SiemensOperation : PLCOperation 
    {
        public SiemensOperation()  
        {
            if (ConnectConfig is SiemensConnectCfg siemens)
            {
                splc = new SiemensPlc( siemens.SiemensSelected, siemens.IP, siemens.Port, siemens.Rack, siemens.Slot);
                return;
            }
            throw new Exception("错误的PLC配置类型");
        }
        private SiemensPlc splc;

        SiemensCmd cmds = new SiemensCmd();

        [JsonConverter(typeof(PolyConverter))]
        [Instance]
        public override object Commands { get => cmds; set=> cmds = value as  SiemensCmd ; }

        public override bool Connect()
        {
            try
            {
                return splc != null && splc.Connection();
            }
            catch  
            {

                return false;
            } 
        }

        public override void Disconnected()
        {
            splc?.DisConnected();
        }

        protected override ResultBase Read(object cmd)
        {
            try
            {
                if (cmd is SiemensCmd sieCmd && sieCmd.CommandStr is IList list)
                {
                    if (sieCmd.Result.Data == null)
                    {
                        // 这个  CommandStr 就直接包含了结果
                        sieCmd.Result.Data = sieCmd.CommandStr;
                    }
                    foreach (var item in list)
                    {
                        if (item is DBData db)
                        {
                            // 暂时写死为读取short的 2022 12 05
                            db.Status = splc.PlcS7.ReadInt16(db.DBAddress).Content;
                        }
                    }
                    return sieCmd.Result;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
