using CommonApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayBorder.Events
{
    public class OnValueChangedArgs : BaseEventArgs
    {
        public static int EventID = typeof(OnValueChangedArgs).GetHashCode();

        public override int Id => EventID;
        public object Value { get; private set; }
        public static OnValueChangedArgs Create(object value)
        {
            OnValueChangedArgs args = ReferencePool.Acquire<OnValueChangedArgs>();
            args.Value = value;
            return args;
        }
        public override void Clear()
        {
            Value = null;
        }
    }
}
