using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

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
            CreateConn();
        } 
        private ConnectionConfigBase connectConfig;
        private List<ResultBase> results = new List<ResultBase>(); 
        /// <summary>
        /// 连接配置的基类
        /// </summary>
        [JsonConverter(typeof(PolyConverter))]
        [Control("ConnectConfig", "编辑连接内容", ControlType.Data, GenerictyType: typeof(ConnectionConfigBase), FieldName: nameof(ConnectConfig))]
        [Control("测试连接", null, ControlType.Button, MethodName: nameof(Connect))]
        public ConnectionConfigBase ConnectConfig
        {
            get
            {
                return connectConfig;
            }
            set => connectConfig = value;
        }
      
        /// <summary>
        /// 这玩意是个集合[List]
        /// </summary>
        [JsonConverter(typeof(PolyConverter))]
        [Control("Commands", "编辑指令集合", ControlType.Collection, GenerictyType: typeof(CommandBase), FieldName: nameof(Commands))]
        public object Commands { get; set; } 

        public  List<ResultBase> GetResults()
        {
            results.Clear();
            if (Commands is IList cmds)
            {
                foreach (var cmd in cmds)
                {
                    results.Add(Read(cmd)); 
                }
            }
            return results;
        }
        /// <summary>
        /// 读取信息
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected abstract ResultBase Read(object cmd) ; 
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
                        //var  commandType = Activator.CreateInstance(depend.Command) as CommandBase;
                        if (Commands == null)
                        {
                            //创建指令集合
                            var reslut = Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { depend.Command }));
                            Commands = reslut;
                            //获取当前类的字段
                            //var property = GetType().GetProperty(nameof(Commands)); 
                            //property.SetValue(this, reslut);
                        }
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
