namespace DeviceConfig.Core
{
    public sealed class TcpConnectCfg : ConnectionConfigBase
    {
        /// <summary>
        /// IP地址
        /// </summary>
        [Control("IP","IP",ControlType.TextBox)]
        public string IP { get; set; }
        /// <summary>
        /// 端口
        /// </summary> 
        [Control("Port","端口", ControlType.TextBox)] 
        public int Port { get; set; }
    }
}
