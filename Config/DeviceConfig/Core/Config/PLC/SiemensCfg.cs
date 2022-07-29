
namespace DeviceConfig.Core
{

    /// <summary>
    /// 西门子PLC访问数据配置
    /// </summary> 
    [PLCType("西门子PLC")]
    public class SiemensCfg : PLCCfg
    {
        /// <summary>
        /// 西门子型号
        /// <para>区间[1-6]</para>
        /// <para> 1 : S1200  </para>
        /// <para> 2 : S300 </para>
        /// <para> 3 : S400,</para>
        /// <para> 4 : S1500 </para>
        /// <para> 5 : S200Smart</para>
        /// <para> 6 : S200</para>
        /// </summary>    
        [Control("SiemensSelected",ControlType.ComboBox, Items: new object[] {"S1200","S300","S400 ","S1500","S200Smart","S200"})]
        public int SiemensSelected { get; set; } = 5;
        /// <summary>
        /// 机架号
        /// </summary>
        [Control("Rack",ControlType.TextBox)]
        public byte Rack { get; set; }
        /// <summary>
        /// 槽号
        /// </summary>
        [Control("Slot",ControlType.TextBox)]
        public byte Slot { get; set; }  
    }
  
}
