using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceConfig.Core;

namespace DeviceConfig
{
    /// <summary>
    /// 标记这个类操作[<see cref="OperationBase"/>]的连接和读取指令依赖与哪个实现 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public  sealed class DependOnAttribute : Attribute
    {
      
        /// <summary>
        /// 标记这个操作依赖与哪个 连接 和 指令
        /// </summary>
        /// <param name="ConnCfg"></param>
        /// <param name="Command"></param>
        public DependOnAttribute(Type ConnCfg,Type Command )
        {

            conn = ConnCfg;
            command = Command;
        }

        private readonly Type conn;
        private readonly Type command;
  

        public Type Conn => conn;

        public Type Command => command;
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class RequireAttribute : Attribute
    {
        private readonly Type requireType;

        public RequireAttribute(Type RequireType)
        {
            requireType = RequireType;
        }

        public Type RequireType => requireType;
    }
}
