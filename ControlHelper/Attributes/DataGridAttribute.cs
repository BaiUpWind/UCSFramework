using System;

namespace ControlHelper.Attributes
{
    /// <summary>
    /// 让集合使用dataGrid的方式进行展示
    /// 当类型不是集合时，改属性无效
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public class DataGridAttribute : Attribute
    {
     
 
    }
}
