using System;

namespace ControlHelper.Attributes
{
    /// <summary>
    /// 当值发生变化时，将值赋予其他属性,注意类型转换问题
    /// ,只有在SetValue的时候，才使用找到的PropName
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class SetValueAttribute : Attribute
    {
        public SetValueAttribute(string propName)
        {
            PropName = propName;
        }

        public string PropName { get; }
    }
}
