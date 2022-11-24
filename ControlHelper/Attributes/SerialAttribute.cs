using System; 

namespace ControlHelper.Attributes
{
    /// <summary>
    /// 自动搜寻COM
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class SerialAttribute : Attribute { }
}
