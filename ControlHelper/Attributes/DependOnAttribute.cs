using System; 

namespace ControlHelper.Attributes
{
    /// <summary>
    /// 标记这个操作依赖 哪个类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class DependOnAttribute : Attribute
    {

        /// <summary>
        /// 标记这个类 哪个配置类型 和 依赖的第二个类型
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="type2"></param>
        public DependOnAttribute(Type configType, Type type2)
        {
            ConfigType = configType;
            Type2 = type2;
        }

        public Type ConfigType { get; }
        public Type Type2 { get; }
    }
}
