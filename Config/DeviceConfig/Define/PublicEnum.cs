namespace DeviceConfig
{
    /// <summary>
    /// 目前支持的PLC连接类型
    /// </summary>
    public enum PLCKind
    {
        None,
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

    public enum ControlType { 
    
        //Label,
        TextBox,
        /// <summary>
        /// 组合框
        /// </summary>
        ComboBox,
        /// <summary>
        /// 文件选择器
        /// </summary>
        FilePathSelector,
      
    }

    /// <summary>
    /// 获取数据使用对应的控件显示
    /// <para>不对样式操作</para>
    /// </summary>
    public enum ResultControl
    {
        /// <summary>
        /// 网格数据
        /// </summary>
        GridView,
        /// <summary>
        /// 标签text显示
        /// </summary>
        Label,
    }

}
