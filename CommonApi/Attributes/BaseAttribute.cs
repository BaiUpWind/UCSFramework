using System;

namespace CommonApi.Attributes
{
    public class BaseAttribute: Attribute
    { 
        public BaseAttribute(string name)
        {
            this.name = name;
        }

        readonly string name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name => name;
    }
}
