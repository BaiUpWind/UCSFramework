using System; 

namespace ControlHelper.Attributes
{
    /// <summary>
    /// 根据字符进行分割 创建组合框
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class SplitAttribute : Attribute
    {
        public SplitAttribute(char splitChart)
        {
            SplitChart = splitChart;
        }

        public char SplitChart { get; }
    }
}
