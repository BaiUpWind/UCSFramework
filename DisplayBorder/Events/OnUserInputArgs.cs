using CommonApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayBorder.Events
{
    /// <summary>
    /// 当用户进行有输入时发生
    /// </summary>
    internal class OnUserInputArgss : BaseEventArgs
    {
        public static int EventID = typeof(OnUserInputArgss).GetHashCode();

        public override int Id => EventID;

        public static OnUserInputArgss Create()
        {
            var args = ReferencePool.Acquire<OnUserInputArgss>();

            return args;
        }

        public override void Clear()
        {
        
        }
    }
}
