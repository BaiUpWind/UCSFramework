using DeviceConfig.Core;

namespace DeviceConfig
{
    /// <summary>
    /// 读取设备信息的指令基类
    /// </summary>
    internal abstract class OperationBase
    {
        public OperationBase(IDeviceConfig config)
        {
            DeviceConfig = config;
        }
        /// <summary>
        /// 这个信息操作连接类型
        /// </summary>
        protected IDeviceConfig DeviceConfig { get; }

        /// <summary>
        /// 根据连接类型的读取指令读取对应内容
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
         public abstract ResultBase Read(CommandBase command);

        public abstract Reuslt Read<Command, Reuslt>(Command cmd) where Command : CommandBase, new() where Reuslt : ResultBase; 

        /// <summary>
        /// 连接设备
        /// </summary>
        /// <returns></returns>
        public abstract bool Connection();
        /// <summary>
        /// 断开连接
        /// </summary>
        public abstract void Disconnected();

        /// <summary>
        /// 连接是否正常
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckConn() => true;

    }
}
