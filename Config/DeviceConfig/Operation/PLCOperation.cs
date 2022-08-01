using DeviceConfig.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceConfig 
{
    //这里的作用是再次分配
    public abstract class PLCOperation : OperationBase
    {
        protected PLCOperation(ConnectionConfigBase defaultConn) : base(defaultConn)
        {
        }
    }
}
