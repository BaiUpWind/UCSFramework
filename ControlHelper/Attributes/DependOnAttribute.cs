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
        /// 标记这个类 哪个配置类型 和数据库类型
        /// </summary>
        /// <param name="configType"></param>
        /// <param name="dbType"></param>
        public DependOnAttribute(Type configType, Type dbType)
        {
            ConfigType = configType;
            DBType = dbType;
        }

        public Type ConfigType { get; }
        public Type DBType { get; }
    }
}
