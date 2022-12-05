using System;

namespace ControlHelper.Model
{
    internal class TypeData
    {
        /// <summary>
        /// 这个字段的名称属性或者名称
        /// </summary>
        public string Name;
        /// <summary>
        /// 标识这个类
        /// </summary>
        public TypeCode TypeCode = TypeCode.Empty;
        /// <summary>
        /// 识别需要生成的对应控件类型
        /// </summary>
        public ClassControlType ControlType; 
        /// <summary>
        /// 是否为集合
        /// </summary>
        public bool IsList;
        /// <summary>
        /// 使用数据网格显示集合
        /// </summary>
        public bool UseDataGrid;
        /// <summary>
        /// 类型
        /// </summary>
        public Type ObjectType; 
        /// <summary>
        /// 是否包号的按钮属性
        /// </summary>
        public Attribute ButtonAttr;
        /// <summary>
        /// 是否包含了文件选择器属性
        /// </summary>
        public Attribute FileSelectorAttr;
        /// <summary>
        /// 是否只读
        /// </summary>
        public bool IsReadOnly;
        /// <summary>
        /// 别名
        /// </summary>
        public string NickName = string.Empty;
        /// <summary>
        /// 提示
        /// </summary>
        public string ToolTip = string.Empty;
        /// <summary>
        /// 自定义宽度
        /// </summary>
        public double Width = double.NaN;
        /// <summary>
        /// 自定义高度
        /// </summary>
        public double Height = double.NaN;
    }
    public enum ClassControlType
    {
        Empty,
        /// <summary>
        /// 文本框输入
        /// </summary>
        TextBox,
        /// <summary>
        /// 单选框输入
        /// </summary>
        CheckBox,
        /// <summary>
        /// 集合/数组 创建或者修改
        /// </summary>
        List,
        /// <summary>
        /// 类
        /// </summary>
        Class,
        /// <summary>
        /// 组合框(枚举)
        /// </summary>
        ComboBox,
        /// <summary>
        ///  超类的所有的实现,不包括超类
        /// </summary>
        ComboBoxImplement,
        /// <summary>
        /// 颜色选择器（这个弃用，因为是HandyControl.Controls这个包的，或者自己再写一个）
        /// </summary>
        //ColorPicker,
        /// <summary>
        /// 创建一个对指定方法名的调用按钮
        /// </summary>
        Button,
    }
}
