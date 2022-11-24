using System; 

namespace ControlHelper.Attributes
{
    /// <summary>
    /// 实现的类型 至少要有一个无参构造，否则会有异常 
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class ComboTypeAttribute : Attribute
    {
        public ComboTypeAttribute(Type superClass, bool shortOfFull = false)
        {
            SuperClass = superClass;
            ShortOfFull = shortOfFull;
        }

        public Type SuperClass { get; }
        public bool ShortOfFull { get; }
    }
}
