using CommonApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayBorder.Events
{
    /// <summary>
    /// 当画布下的子控件被击中时发生
    /// <para> sender是<see cref="CanvasHelper.CanvasParent_MouseLeftButtonDown"/></para>
    /// </summary>
    internal class OnCanvasChildrenClickArgs : BaseEventArgs
    {
        public static int EventID = typeof(OnCanvasChildrenClickArgs).GetHashCode();

        public override int Id => EventID;

        /// <summary>
        /// 引发事件的对象的引用
        /// </summary>
        public object Source { get; private set; }
        /// <summary>
        /// 当前击中的元素
        /// <para>textblock</para>
        /// </summary>
        public object OrginalSource { get; private set; }

        public static OnCanvasChildrenClickArgs Create(object source,object originalSource)
        {
            var args = ReferencePool.Acquire<OnCanvasChildrenClickArgs>();
            args.Source = source;
            args.OrginalSource = originalSource;
            return args;
        }

        public override void Clear()
        {
            Source = null;
            OrginalSource= null;
        }
    }
}
