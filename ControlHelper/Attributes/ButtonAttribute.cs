using System;

namespace ControlHelper.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public class ButtonAttribute : Attribute
    {
        private readonly string name;
        private readonly string methodName;
        private readonly string memberName;
        /// <summary>
        /// 创建一个按钮
        /// </summary>
        /// <param name="Name">按钮名称</param>
        /// <param name="MethodName">方法名称</param>
        /// <param name="MemberName">将方法的返回结果赋值给指定字段</param>
        public ButtonAttribute(string Name, string MethodName, string MemberName = null)
        {
            name = Name;
            methodName = MethodName;
            this.memberName = MemberName;
        }
        public string Name => name;
        public string MethodName => methodName;

        public string MemberName => memberName;
    }
}
