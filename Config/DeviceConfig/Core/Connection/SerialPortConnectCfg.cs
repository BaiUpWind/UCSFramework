namespace DeviceConfig.Core
{
    public sealed class SerialPortConnectCfg : ConnectionConfigBase
    {
        /// <summary>
        /// 串口的名称
        /// </summary> 
        public string PortName { get; set; }
        /// <summary>
        /// COM口的名称
        /// </summary> 
        public string COMNmae { get; set; }

        /// <summary>
        /// 波特率
        /// </summary> 
        public int BaudRate { get; set; }
    }
}
