using System; 

namespace ControlHelper.Attributes
{
    /// <summary>
    /// 禁止控件的生成
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class HideAttribute : Attribute { }
}
