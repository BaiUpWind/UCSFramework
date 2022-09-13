namespace DeviceConfig
{ 
    /// <summary>
    /// 控件类型
    /// </summary>
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
    /// 数据显示的方式
    /// </summary>
    public enum DataType
    {
        饼状图,
        线状图,
        柱状图
    }
}
