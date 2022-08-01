namespace DeviceConfig.Core
{
    public sealed class TcpConnectCfg : ConnectionConfigBase
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 端口
        /// </summary> 
        public int Port { get; set; }
    }
}
