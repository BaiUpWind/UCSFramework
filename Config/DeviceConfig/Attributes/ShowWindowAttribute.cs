using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceConfig
{
    /// <summary>
    /// 在中窗体显示
    /// </summary>
    public class ShowWindowAttribute : Attribute
    {
        public ShowWindowAttribute()
        {
            this.defalutText = "默认文本";
            this.hide = false;
        }

        public ShowWindowAttribute(string defalutTex)
        {
            this.defalutText = defalutTex;
        }
        public ShowWindowAttribute(bool hide)
        {
            this.hide = hide;
        }
        public ShowWindowAttribute(string defalutTex, bool hide)
        {
            this.defalutText = defalutTex;
            this.hide = hide;
        }

        private bool hide = false;
        private string defalutText = "默认文本";
        /// <summary>
        /// 默认显示信息
        /// </summary>
        public string DefalutText { get => defalutText; }
        /// <summary>
        /// 是否隐藏，默认不隐藏
        /// </summary>
        public bool Hide { get => hide; }
    }
}
