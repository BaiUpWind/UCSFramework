using HslCommunication.Profinet.Siemens;

namespace DeviceConfig.Core
{
    public sealed class SiemensConnectCfg : PLCConnectionCfg
    {

        // <summary>
        /// 西门子型号
        /// <para>区间[1-6]</para>
        /// <para> 1 : S1200  </para>
        /// <para> 2 : S300 </para>
        /// <para> 3 : S400,</para>
        /// <para> 4 : S1500 </para>
        /// <para> 5 : S200Smart</para>
        /// <para> 6 : S200</para>
        /// </summary>    
        [Control("SiemensSelected","西门子型号", ControlType.ComboBox, EnumType: typeof(SiemensPLCS))]
        public SiemensPLCS SiemensSelected { get; set; }  

        /// <summary>
        /// 机架号
        /// </summary>
        [Control("Rack","机架号",ControlType.TextBox)]
        public byte Rack { get; set; }
        /// <summary>
        /// 槽号
        /// </summary> 
        [Control("Slot","槽号", ControlType.TextBox)] 
        public byte Slot { get; set; }
    }
}
