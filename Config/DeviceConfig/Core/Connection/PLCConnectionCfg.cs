namespace DeviceConfig.Core
{
    //这里作用是再次分配
    public abstract class PLCConnectionCfg : ConnectionConfigBase
    {
        public string IP { get; set; } = "192.168.0.1";
        public int Port { get; set; }
    }

    //public abstract class AAAA : PLCConnectionCfg
    //{

    //}

    //public sealed class BBBB : AAAA
    //{

    //}

    //public sealed class CCCC : AAAA
    //{

    //}
}
