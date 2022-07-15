 
namespace Modle.DeviceCfg
{
    public enum PLCKind
    {
        Siemens,
    }
    public class PLCCfg : TCPCfgBase 
    {
        public PLCKind PLCType { get; set; } = PLCKind.Siemens;

        public override DeviceType DevType => DeviceType.PLC;
        
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
