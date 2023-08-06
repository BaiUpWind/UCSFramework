using System; 

namespace ControlHelper.Attributes
{
    /// <summary>
    /// 禁止控件的生成
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class HideAttribute : Attribute { }


    /// <summary>
    /// 根据属性中布尔量的值进行隐藏，如果值为真隐藏,反之
    /// 如果同时在一个属性中添加了<see cref="ShowIFAttribute"/>，则优先<see cref="HideIFAttribute"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class HideIFAttribute : Attribute {

        public HideIFAttribute(string targetName)
        {
            TargetName = targetName;
        }

        public string TargetName { get; }
    }
    /// <summary>
    ///  根据属性中布尔量的值进行显示，如果值为真显示,反之
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ShowIFAttribute : Attribute
    {

        public ShowIFAttribute(string targetName)
        {
            TargetName = targetName;
        }

        public string TargetName { get; }
    }

}
