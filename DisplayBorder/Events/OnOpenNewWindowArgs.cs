using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using CommonApi;
using CommonApi.Event;
using HandyControl.Controls;

namespace DisplayBorder.Events
{
    /// <summary>
    /// 当打开新的窗体时
    /// </summary>
    public class OnOpenNewWindowArgs : BaseEventArgs
    {
        public static int EventID = typeof(OnOpenNewWindowArgs).GetHashCode();

        public override int Id => EventID;

        /// <summary>
        /// 父类窗体
        /// </summary>
        public Window ParentWindow { get; private set; }

        /// <summary>
        /// 新的窗体
        /// </summary>
        public Window NewWindow { get; private set; }


        public static OnOpenNewWindowArgs Create(Window newWindow, Window parentWindow)
        {
            var args = ReferencePool.Acquire<OnOpenNewWindowArgs>();
            args.NewWindow = newWindow;
            args.ParentWindow = parentWindow;
            return args;
        }

        public override void Clear()
        {
            ParentWindow = null;
            NewWindow = null;
        }
    }
}
