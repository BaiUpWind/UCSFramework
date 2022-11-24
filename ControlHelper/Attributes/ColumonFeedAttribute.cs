using System; 

namespace ControlHelper.Attributes
{
    /// <summary>
    /// 在同一容器控件中换列
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class ColumonFeedAttribute : Attribute
    {

    }
}
