using System;

namespace CommonApi.Attributes
{
    public class BaseAttribute: Attribute
    {
        readonly string name;
        /// <summary>
        /// 类名
        /// </summary>
        public string Name => name;

        public BaseAttribute(string name)
        {
            this.name = name;
        }
    }
}
