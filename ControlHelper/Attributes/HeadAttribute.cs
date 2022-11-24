using System; 

namespace ControlHelper.Attributes
{
    /// <summary>
    /// 创建一个居中的lable
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class HeadAttribute : Attribute
    {
        public HeadAttribute(string info)
        {
            Info = info;
        }

        public string Info { get; }
    }
}
