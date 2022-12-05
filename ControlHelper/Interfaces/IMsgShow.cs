using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlHelper.Interfaces
{
    public interface IMsgShow
    {
        /// <summary>
        /// 消息通知
        /// <para>string 是消息</para>
        /// <para>int 是消息等级</para>
        /// </summary>
        event Action<string, int> ShowMsg;
    }
}
