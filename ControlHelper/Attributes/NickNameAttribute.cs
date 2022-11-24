using System; 

namespace ControlHelper.Attributes
{
    /// <summary>
    /// 对属性的名称重新命名显示
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class NickNameAttribute : Attribute
    {
        public NickNameAttribute(string NickName, string ToolTip = null)
        {
            nickName = NickName;
            this.toolTip = ToolTip;
        }

        private string nickName;
        private readonly string toolTip;

        public string NickName => nickName;

        public string ToolTip => toolTip;
    }
}
