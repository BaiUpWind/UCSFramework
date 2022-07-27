namespace Modle
{
    /// <summary>
    /// 目前支持的PLC连接类型
    /// </summary>
    public enum PLCKind
    {
        /// <summary>
        /// 西门子
        /// </summary>
        Siemens,
    }

    /// <summary>
    /// 设备连接类型
    /// </summary>
    public enum DeviceConnectedType
    {
        /// <summary>
        /// 当连接类型为空时 不做任何操作
        /// </summary>
        None,
        /// <summary>
        /// 使用串口
        /// </summary>
        Serial,
        /// <summary>
        /// 使用hsl库进行连接对应的PLC
        /// </summary>
        PLC,
        /// <summary>
        /// 使用tcpClient方式连接
        /// </summary>
        TcpClient,
        /// <summary>
        /// 数据库方式
        /// </summary>
        DataBase
    }

}
