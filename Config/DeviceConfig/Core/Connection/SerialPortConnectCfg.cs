namespace DeviceConfig.Core
{
    public sealed class SerialPortConnectCfg : ConnectionConfigBase
    {
        /// <summary>
        /// 串口的名称
        /// </summary> 
        [Control("PortName", "串口名称", ControlType.TextBox)]
        public string PortName { get; set; }
        /// <summary>
        /// COM口的名称
        /// </summary> 
        [Control("COMNmae","COM", ControlType.TextBox)]
        public string COMNmae { get; set; }

        /// <summary>
        /// 波特率
        /// </summary> 
        [Control("BaudRate","波特率", ControlType.TextBox)]
        public int BaudRate { get; set; }
    }
}
