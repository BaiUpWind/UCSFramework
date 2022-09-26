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
        public NickNameAttribute(string NickName)
        {
            nickName = NickName;
        }

        private string nickName;
        public string NickName => nickName;
    }
}
