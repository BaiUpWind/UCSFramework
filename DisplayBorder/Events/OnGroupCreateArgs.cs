using CommonApi;
using DisplayBorder.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayBorder.Events
{
    public class OnGroupCreateArgs : BaseEventArgs
    {
        public static int EventID = typeof(OnGroupCreateArgs).GetHashCode();

        public override int Id => EventID;

        public Group Group { get; private set; }

        public static OnGroupCreateArgs Create(Group group)
        {
            var args = ReferencePool.Acquire<OnGroupCreateArgs>();

            args.Group = group;
            return args;
        }


        public override void Clear()
        {
            Group = null;
        }
    }
}
