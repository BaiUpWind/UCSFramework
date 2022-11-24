using System; 

namespace ControlHelper.Attributes
{
    /// <summary>
    /// 标记实体类的属性上,在datagirdview上，列头名称和列头的宽度
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DgvColumnsHeadAttribute : Attribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="headName">列名称</param>
        /// <param name="width">列头宽度</param>
        /// <param name="order">出现的顺序</param>
        public DgvColumnsHeadAttribute(string headName, int width = 90, int order = 0)
        {
            HeadName = headName;
            Width = width;
            Order = order;
        }

        public string HeadName { get; }
        public int Width { get; }
        public int Order { get; }
    }
}
