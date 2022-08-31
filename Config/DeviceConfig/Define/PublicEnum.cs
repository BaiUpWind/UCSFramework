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

    public enum ControlType 
    { 
        /// <summary>
        /// 输入框
        /// </summary>
        TextBox,
        /// <summary>
        /// 组合框
        /// </summary>
        ComboBox,
        /// <summary>
        /// 按钮,
        /// </summary>
        Button,
        /// <summary>
        /// 文件选择器
        /// <para>放在<see cref="TextBox"/>上,双击进行选择文件路径</para>
        /// </summary>
        FilePathSelector,
        /// <summary>
        /// 颜色选择器
        /// <para>自动生成一个按钮,将返回结果写入到对应的字段/属性,字段/属性类型要求是<see cref="System.Windows.Media.Color"/></para>
        /// </summary>
        ColorPicker,
        /// <summary>
        /// 类型生成器 
        /// <para>创建类型生成器,暂时只支持无参构造</para>
        /// </summary>
        Genericity,
        /// <summary>
        /// 数据修改生成器
        /// </summary>
        Data,
        /// <summary>
        /// 集合类型的数据生成器
        /// </summary>
        Collection,

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
