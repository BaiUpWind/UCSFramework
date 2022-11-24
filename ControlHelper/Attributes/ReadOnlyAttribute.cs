using System; 

namespace ControlHelper.Attributes
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class ReadOnlyAttribute : Attribute { }
}
