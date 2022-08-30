using CommonApi;
using Newtonsoft.Json;
using System;

namespace DeviceConfig.Core
{
    /// <summary>
    /// 读取设备信息的指令基类
    /// <para>数据库：配置SQL语句</para>
    /// <para>PLC:配置对应的DB块</para>
    /// <para>TcpClient:配置对应的协议</para>
    /// <para>SerialPort:配置对应的 读写信息</para>
    /// </summary>
    public abstract class OperationBase
    {
        public OperationBase()
        {
            //defaultConn = null;
            CreateConn();
        }
        //public OperationBase(ConnectionConfigBase defaultConn)
        //{
        //    this.defaultConn = defaultConn ?? throw new Exception("初始化操作配置失败，未能获取到默认的连接方式！");
        //}
        //private readonly ConnectionConfigBase defaultConn;// 默认的连接方式 来源于设备 
        private ConnectionConfigBase connectConfig; 
        private CommandBase command;

        /// <summary>
        /// 连接配置的基类
        /// </summary>
        [JsonConverter(typeof(PolyConverter))]
        [Control("ConnectConfig", "连接类型", ControlType.Data, GenerictyType: typeof(ConnectionConfigBase), FieldName: nameof(ConnectConfig))]
        public ConnectionConfigBase ConnectConfig
        {
            get
            { 
                return connectConfig;
            }
           set => connectConfig = value;
        }

        [Control("testconn", "测试连接", ControlType.Button, MethodName: nameof(Connect))]
        public string TestConn { get; set; }

        /// <summary>
        /// 读取设备数据的指令
        /// <para>这里根据对应类型进行解析的指令</para>
        /// </summary>
        [JsonConverter(typeof(PolyConverter))]
        public CommandBase Command { get => command; set => command = value; }

        /// <summary>
        ///  当前指令读取后返回的结果(缓存)
        /// </summary>
        [JsonIgnore]
        public ResultBase Result { get => Command?.Result; }
        /// <summary>
        /// 使用默认的连接方式
        /// </summary>
        //public bool UseDefaultConn { get; set; }
        /// <summary>
        /// 使用跟随配置
        /// </summary>
        //public bool UseFllowConfiguration { get; set; } = true;

        /// <summary>
        /// 根据连接类型的读取指令读取对应内容
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public abstract ResultBase Read(CommandBase command);
        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        public abstract bool Connect();
        /// <summary>
        /// 断开连接
        /// </summary>
        public abstract void Disconnected();

        /// <summary>
        /// 连接是否正常
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckConn() => true;

        public virtual void SetConn(ConnectionConfigBase conn) => connectConfig =conn;
        private void CreateConn()
        {
            try
            { 
                object[] objAttrs = GetType().GetCustomAttributes(typeof(DependOnAttribute), true);
                foreach (var att in objAttrs)
                {
                    if (att is DependOnAttribute depend)
                    {
                        //创建连接
                        connectConfig = Activator.CreateInstance(depend.Conn) as ConnectionConfigBase;
                        //创建指令
                        command = Activator.CreateInstance(depend.Command) as CommandBase; ;
                    }
                }
                if (objAttrs.Length == 0)
                {
                    throw new Exception($"'{GetType().FullName}'未找到依赖于标签,创建连接配置和读取配置失败!");
                }
            }
            catch (Exception ex)
            { 
                throw  ex;
            }
        }

     
        #region 暂时弃用 2022年8月5日 07:49:50


        //#region 创建连接
        ///// <summary>
        ///// 获取所有连接
        ///// <para></para>
        ///// <see cref=" Utility.Reflection.GetInheritors"/>
        ///// </summary>
        ///// <returns></returns>
        //public ClassData GetConnections() => Utility.Reflection.GetClassData<ConnectionConfigBase>();
        ///// <summary>
        /////  创建一个连接
        ///// </summary>
        ///// <param name="name"></param> 
        //public void CreataConnection(string name) => connectConfig = Utility.Reflection.CreateObjectShortName<ConnectionConfigBase>(name);

        //#endregion

        //#region 指令创建 
        ///// <summary>
        ///// 获取所有指令
        ///// <para></para>
        ///// <see cref=" Utility.Reflection.GetInheritors"/>
        ///// </summary>
        ///// <returns></returns>
        //public ClassData GetCommands() => Utility.Reflection.GetClassData<CommandBase>( ); 

        ///// <summary>
        ///// 创建读取指令
        ///// </summary>
        ///// <param name="name"></param>
        //public void CreateCommand(string name) =>  Command = Utility.Reflection.CreateObjectShortName<CommandBase>(name); 
        //#endregion


        #endregion

    }
}
