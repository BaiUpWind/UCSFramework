using CommonApi.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceConfig 
{
    /// <summary>
    /// 使用哪种控件 
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method, Inherited = false, AllowMultiple = true)] 
    public class ControlAttribute : BaseAttribute
    {
      
        public ControlAttribute(string Name, string LabelName, ControlType ControlType, bool Enable = true, bool ReadOnly = false, bool Visable = true
            , object[] Items = null, Type EnumType = null, string FieldName = null, string FileType = null
            , Type GenerictyType = null, string MethodName = null, double Height = 0, double Width = 0, int Order = 0) : base(Name)
        {
            this.controlType = ControlType;
            this.visable = Visable;
            visable = Visable;
            enable = Enable;
            readOnly = ReadOnly;
            items = Items;
            enumType = EnumType;
            labelName = LabelName;
            fieldName = FieldName;
            fileType = FileType;
            generictyType = GenerictyType;
            methodName = MethodName;
            height = Height;
            width = Width;
            order = Order;
        }

        private readonly ControlType controlType;
        private readonly bool visable;
        private readonly bool enable;
        private readonly bool readOnly;
        private readonly object[] items;
        private readonly Type enumType;
        private readonly string labelName;
        private readonly string fieldName;
        private readonly string fileType;
        private readonly Type generictyType;
        private readonly string methodName;
        private readonly double width;
        private readonly double height;
        private readonly int order;

        public int Order => order;

        public ControlType ControlType { get => controlType; } 
        /// <summary>
        /// 是否可见的 
        /// </summary>
        public bool Visable { get => visable; } 
        /// <summary>
        /// 是否启用控件
        /// </summary>
        public bool Enable => enable;
        /// <summary>
        /// 是否只读
        /// </summary>
        public bool ReadOnly => readOnly;

        public object[] Items => items;

        public Type EnumType => enumType;
        /// <summary>
        /// 生成的类型
        /// </summary>
        public Type GenerictyType => generictyType;

        public string LabelName => labelName;

        /// <summary>
        /// 如果类型是按钮文件,返回值所对应的属性名称
        /// </summary>
        public string FieldName => fieldName;
        /// <summary>
        /// 如果类型是按钮文件,选择对应的文件类型
        /// <para>只支持一种</para>
        /// </summary>
        public string FileType => fileType;

        public string MethodName => methodName;
        public double Width => width;
        public double Height => height;

    }
}
