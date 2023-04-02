namespace DeviceConfig.Core
{
    //这里作用是再次分配
    public abstract class PLCConnectionCfg : ConnectionConfigBase
    {
        [Control("IP","IP", ControlType.TextBox)]
        public string IP { get; set; } = "192.168.0.1";
        [Control("Port", "端口", ControlType.TextBox)]
        public int Port { get; set; } = 102;
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
