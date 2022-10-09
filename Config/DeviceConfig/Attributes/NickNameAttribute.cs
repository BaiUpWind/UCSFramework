using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  DeviceConfig 
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field  , Inherited = false,AllowMultiple =false)]
    public class NickNameAttribute:Attribute
    { 
        public NickNameAttribute(string NickName, string ToolTip =null)
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
