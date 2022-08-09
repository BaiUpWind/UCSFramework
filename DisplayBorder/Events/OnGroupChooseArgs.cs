using CommonApi;
using DeviceConfig;
using DisplayBorder.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayBorder.Events
{
    /// <summary>
    /// 当组被选中时触发事件
    /// </summary>
    internal class OnGroupChooseArgs : BaseEventArgs
    {
        public static int EventID = typeof(OnGroupChooseArgs).GetHashCode();

        public override int Id => EventID;

        /// <summary>
        /// 当前被选中的组
        /// </summary>
        public Group Group { get; private set; }

        /// <summary>
        /// 当前选中
        /// </summary>
        public MixerControl GroupMixer { get; private set; }

        public static OnGroupChooseArgs Create(MixerControl groupMixer)
        {
            var args = ReferencePool.Acquire<OnGroupChooseArgs>();
            args.GroupMixer = groupMixer;
            args.Group = groupMixer.Gvm.CurrentGroup;
            return args;
        }


        public override void Clear()
        {
            Group = null;
            GroupMixer = null;
        }
    }
}
