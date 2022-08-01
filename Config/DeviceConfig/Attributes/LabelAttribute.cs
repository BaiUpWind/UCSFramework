using CommonApi.Attributes;
using System;

namespace DeviceConfig
{

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public sealed class LabelAttribute : ControlBaseAttribute
    {
        public LabelAttribute(string name,string text) : base(name)
        {
            this.text = text;
        }

        public LabelAttribute(string name,  string text, bool enable = true, bool readOnly = false) : base(name, enable, readOnly)
        {
            this.text = text;
        }

        private readonly string text;
        /// <summary>
        /// 文本信息
        /// </summary>
        public string Text => text;
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)] 
    public sealed class TextBoxAttribute : ControlBaseAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name">控件名称 唯一</param>
        /// <param name="text">默认输入内容</param>
        /// <param name="labelText">输入框前面的提升提示标签/param>
        public TextBoxAttribute(string name,string text,string labelText) : base(name)
        {
            this.text=text;
            this.labelText = labelText;
        }

        public TextBoxAttribute(string name,  string text, string labelText, bool enable = true, bool readOnly = false) : base(name, enable, readOnly)
        {
            this.text = text;
            this.labelText = labelText;
        }

        private readonly string text;
        private readonly string labelText;
        

        public string Text => text;

        public string LabelText => labelText;
    }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)] 
    public sealed class ComboBoxAttribute : ControlBaseAttribute
    {
        /// <summary>
        /// 自动获取项进行绑定
        /// </summary>
        /// <param name="name"></param>
        /// <param name="labelText"></param>
        public ComboBoxAttribute(string name, string labelText ) : base(name)
        {
            this.items = Items;
            this.labelText = labelText;
            items = null;
        }
        public ComboBoxAttribute(string name,  string labelText, object[] Items = null) : base(name)
        {
            this.items = Items; 
            this.labelText = labelText;
        }

        public ComboBoxAttribute(string name, string labelText, object[] Items = null, bool enable = true, bool readOnly = false) : base(name, enable, readOnly)
        {
            this.items = Items;
            this.labelText = labelText;
        }
        private readonly object[] items;
        private readonly string labelText;

        public object[] Items => items; 
        public string LabelText => labelText;
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = false, AllowMultiple = false)] 
    public abstract class ControlBaseAttribute : BaseAttribute
    {
        public ControlBaseAttribute(string name) : base(name)
        {
            this.enable = true;
            this.readOnly = false;
        }
        public ControlBaseAttribute(string name, bool enable = true, bool readOnly = false) : base(name)
        {
            this.enable = enable;
            this.readOnly = readOnly;
        }

        public readonly bool enable;
        public readonly bool readOnly;
    }

}
