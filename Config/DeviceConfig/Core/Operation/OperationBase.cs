using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DeviceConfig.Core
{
    /// <summary>
    /// 读取设备信息的指令基类 
    /// </summary>
    public abstract class OperationBase
    {
        public OperationBase()
        { 
            CreateConn();
        } 
        private ConnectionConfigBase connectConfig;
        private List<object> results = new List<object>();

        /// <summary>
        /// 读取信息
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        protected abstract ResultBase Read(object cmd);
        /// <summary>
        /// 连接配置的基类
        /// </summary>
        [JsonConverter(typeof(PolyConverter))]
        [Control("ConnectConfig", "编辑连接内容", ControlType.Data, GenerictyType: typeof(ConnectionConfigBase), FieldName: nameof(ConnectConfig))]
        [Control("测试连接", null, ControlType.Button, MethodName: nameof(Connect))]
        [Instance]
        [Button("测试连接", nameof(Connect))]
        public virtual ConnectionConfigBase ConnectConfig
        {
            get
            {
                return connectConfig;
            }
            set => connectConfig = value;
        }
      
        /// <summary>
        /// 这玩意是个集合[List],存放所有的指令操作和返回结果
        /// <para>基类型为<see cref="CommandBase"/></para>
        /// </summary>
        [JsonConverter(typeof(PolyConverter))]
        [Instance]
        [Control("Commands", "编辑指令集合", ControlType.Collection, GenerictyType: typeof(CommandBase), FieldName: nameof(Commands))]
        public virtual object Commands { get; set; } 

        /// <summary>
        /// 获取所有的返回结果
        /// <para>object的基础类型为<see cref="ResultBase"/></para>
        /// </summary>
        /// <returns></returns>
        public  List<object> GetResults()
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
         

    }
}
