 
namespace Modle.DeviceCfg
{
   /// <summary>
   /// PLC配置基类
   /// </summary>
    [DeviceConnectedType("PLC")]
    public  class PLCCfg : TCPCfgBase 
    { 
        public virtual PLCKind PLCType { get; set; } = PLCKind.Siemens;

        /// <summary>
        /// 设备连接类型
        /// </summary>
        public override DeviceConnectedType DevType => DeviceConnectedType.PLC;
        
        /// <summary>
        /// 获取对应的PLC配置类型
        /// <para>如果不是的对应的类型则返回空</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetPlcCfg<T>() where T : PLCCfg
        {
            if(this is T t)
            {
                return t;
            } 
            return null;
        }
    }
}
