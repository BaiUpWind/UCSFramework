using System;
using DeviceConfig.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceConfig
{
    internal static class CreateFactory
    {
        /// <summary>
        /// 创建操作类型
        /// </summary>
        /// <param name="name"></param>
        /// <param name="para"></param>
        /// <returns></returns>
        public static OperationBase  CreateOperation(string name ,params object[] para)
        {
            //命名规则 'Name' + Operation
            //实现定义:SeimensOperation
            return CreateObject<OperationBase>($"DeviceConfig.{name}", para);
        }

        /// <summary>
        /// 创建读取指令类型
        /// </summary>
        /// <param name="name"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static CommandBase CreateCmb(string name,params object[] objects)
        {
            return CreateObject<CommandBase>($"DeviceConfig.Core.{name}", objects);
        }

        private static T CreateObject<T>(string name,params object[] para) where T : class
        {
            Type type = Type.GetType(name);//反射入口
            return Activator.CreateInstance(type, para) as T;//创建实例
        }
    }
}
