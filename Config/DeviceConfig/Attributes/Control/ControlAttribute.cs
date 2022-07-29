using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceConfig 
{
    /// <summary>
    /// 使用哪种控件
    /// <para>Name 不能重复</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)] 
    public class ControlAttribute : ConfigBaseAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="ControlType"></param>
        /// <param name="Enable"></param>
        /// <param name="ReadOnly"></param>
        /// <param name="Visable"></param>
        /// <param name="Items">当选择类型'<see cref="ControlType.ComboBox"/>'时，传入对应的项集合</param>
        public ControlAttribute(string Name, ControlType ControlType, bool Enable =true, bool ReadOnly =false, bool Visable = true,object[] Items = null,Type EnumType = null) : base(Name)
        {
            this.controlType = ControlType;
            this.visable = Visable;
            visable = Visable;
            enable = Enable;
            readOnly = ReadOnly;
            items = Items;
            enumType = EnumType;
        }

        private readonly ControlType controlType;
        private readonly bool visable;
        private readonly bool enable;
        private readonly bool readOnly;
        private readonly object[] items;
        private readonly Type enumType;
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
    }
}
