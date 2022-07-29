using CommonApi.DBHelper;
using CommonApi.PLC;
using DeviceConfig.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommonApi;


namespace DeviceConfig
{
    /// <summary>
    /// 设备需要读取的值所需要的信息
    /// <para>数据库：配置SQL语句</para>
    /// <para>PLC:配置对应的DB块</para>
    /// <para>TcpClient:配置对应的协议</para>
    /// <para>SerialPort:配置对应的 读写信息</para>
    /// </summary>
    public class DeviceInfo
    {

        public DeviceInfo(IDeviceConfig defaultConn)
        {
            this.defaultConn = defaultConn ?? throw new Exception("初始化设备信息配置，未能获取到默认的连接方式！");
        }

        private readonly IDeviceConfig defaultConn;//默认的连接类型 来源于设备
        private OperationBase operation;//对设备的操作 读取之类的
        private ResultBase result;
        private bool start;//是否开始获取信息
        private IDeviceConfig connConfig;

        /// <summary>
        /// 对应的设备编号
        /// </summary> 
        public int DeviceID { get; set; }

        /// <summary>
        /// 设备信息编号
        /// </summary>
        public int DeviceInfoID { get; set; }
        /// <summary>
        /// 设备信息名称
        /// </summary>
        public int DeviceInfoName { get; set; }
        /// <summary>
        /// 使用默认的连接方式
        /// </summary>
        public bool UseDefaultConn { get; set; }
        /// <summary>
        /// 获取信息的连接方式
        /// </summary> 
        public IDeviceConfig ConnConfig
        {
            get
            {
                if (UseDefaultConn)
                {
                    return defaultConn;
                }
                return connConfig;
            }
            set => connConfig = value;
        }

        public DeviceConnectedType ConnectionType { get; set; }

        /// <summary>
        /// 刷新数据间隔  单位:毫秒 
        /// </summary>
        public int RefreshInterval { get; set; }

        /// <summary>
        /// 读取设备数据的指令
        /// <para>这里根据对应类型进行解析的指令</para>
        /// </summary>
        public CommandBase Command { get; set; }

        public ResultBase Reslut { get => Command?.Result; }

        /// <summary>
        /// 创建读取指令
        /// </summary>
        /// <param name="name"></param>
        public void CreateCommand(string name)
        {
            //根据子类实现的名称 创建对象
            var result = typeof(CommandBase).Assembly.GetTypes().Where(a => !a.IsAbstract && a.Name == name).FirstOrDefault();
            if (result == null)
            {
                throw new ArgumentNullException($"未找到对应子类的名称'{name}'实现");
            }
            Command = Utility.Reflection.CreateObject<CommandBase>(result.FullName);
        }

        /// <summary>
        /// 获取所有指令名称
        /// </summary>
        /// <returns></returns>
        public string[] GetCmds()
        {
            //获取所有子类的实现的名称
            return Utility.Reflection.GetChildrenNames<CommandBase>().ToArray();
        }

        public CommandBase GetCommand() => Command;

        public async void Start()
        {
            Init();
            start = true;
            while (start)
            {
                Command.Result = operation.Read(Command);
                await Task.Delay(RefreshInterval);
            }
        }

        public void Stop()
        {
            start = false; 
            if(operation != null && ConnConfig != null)
            {
                operation.Disconnected();
                operation = null;
            }
        }
         
        private void Init()
        {
            if (operation == null && ConnConfig != null)
            {
                string opreationName ;
                //这个比较特殊
                if (ConnConfig.DevType == DeviceConnectedType.PLC)
                {
                    PLCCfg plc = (PLCCfg)ConnConfig;
                    if (plc == null || plc.PLCType == PLCKind.None)
                    {
                        throw new Exception("PLC配置为空 或者选择类型为none");
                    }
                    opreationName = plc.PLCType.ToString();
                }
                else
                {
                    opreationName = ConnConfig.DevType.ToString();
                }

                try
                {
                    operation = Utility.Reflection.CreateObject<OperationBase>($"DeviceConfig.{opreationName}Operation",ConnConfig);// CreateFactory.CreateOperation(opreationName, ConnConfig);
                }
                catch (Exception ex)
                { 
                    throw ex;
                }

                if (operation == null)
                {
                    throw new Exception($"设备编号'{DeviceID}' 设备信息编号'{DeviceInfoID}' 设备信息名称'{DeviceInfoName}' 初始化失败！");
                }
                if (!operation.Connection())
                {
                    throw new Exception($"设备编号'{DeviceID}' 设备信息编号'{DeviceInfoID}' 设备信息名称'{DeviceInfoName}' 连接设备失败！");
                }
            }
        }

        private void CreateOpreation()
        {
            if (ConnectionType == DeviceConnectedType.None)
            {
                throw new Exception($"不能创建空的连接类型");
            }

            var type = typeof(ConnectionBase).Assembly.GetTypes()
                  .Where(a => typeof(ConnectionBase).IsAssignableFrom(a) && a.Name == ConnectionType.ToString() + "Connect").FirstOrDefault();
            if (type == null)
            {
                throw new ArgumentNullException($"未找到对应的连接类型'{ConnectionType}' ！");
            }
            //如果该实现还是抽象的
            if (type.DeclaringType.IsAbstract)
            {
                var children = type.Assembly.GetTypes();

            }

        }

    } 
}
