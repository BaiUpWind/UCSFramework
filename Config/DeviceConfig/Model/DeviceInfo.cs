using CommonApi;
using DeviceConfig.Core;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;


namespace DeviceConfig
{
    /// <summary>
    /// 设备需要读取的信息所需要的配置 
    /// </summary>
    public class DeviceInfo
    { 
        public DeviceInfo(ConnectionConfigBase defaultConn)
        {
            this.defaultConn = defaultConn ?? throw new Exception("初始化设备信息配置，未能获取到默认的连接方式！");
        }
        private readonly ConnectionConfigBase defaultConn;// 默认的连接方式 来源于设备
        private OperationBase operation;//对设备的操作 读取之类的 
        private bool start;//是否开始获取信息
 

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
        /// 获取信息的连接方式
        /// </summary> 
        [JsonConverter(typeof(PolyConverter))]
        public ConnectionConfigBase ConnConfig
        {
            get
            {  
                return Operation?.ConnectConfig;
            }
            //set => connConfig = value;
        }
        /// <summary>
        /// 对设备的操作类型
        /// </summary>
        [JsonConverter(typeof(PolyConverter))]
        public OperationBase Operation { get => operation; set => operation = value; }
        /// <summary>
        /// 刷新数据间隔  单位:毫秒 
        /// </summary>
        public int RefreshInterval { get; set; }

        #region 操作的创建
         
        /// <summary>
        /// 获取所有操作类型
        /// <para></para>
        /// <see cref=" Utility.Reflection.GetInheritors"/>
        /// </summary>
        /// <returns></returns>
        public ClassData GetOperations() => Utility.Reflection.GetClassData<OperationBase>();
        
        /// <summary>
        /// 创建一个操作类型
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="Exception"></exception>
        public void CreateOpertaion(string name)
        { 
            Operation = Utility.Reflection.CreateObjectShortName<OperationBase>(name, defaultConn);
            if (Operation == null)
            {
                throw new Exception($"{info} 创建失败！");
            } 
        }

        #endregion
         

        public async void Start()
        {
            if (Operation == null)
            {
                throw new NullReferenceException($"{info} 操作'{nameof(Operation)}'未建立! 开启失败 ");
            }
            if (!Operation.Connect())
            {
                throw new Exception($" {info} 连接失败！");
            }
            start = true;
            while (start)
            {
                Operation.Read(Operation.Command);
                await Task.Delay(RefreshInterval);
            }
        }

        public void Stop()
        {
            start = false; 
            if(Operation != null && ConnConfig != null)
            {
                Operation.Disconnected();
                Operation = null;
            }
        }

        [JsonIgnore]
        private  string info => $"设备编号'{DeviceID}' 设备信息编号'{DeviceInfoID}' 设备信息名称'{DeviceInfoName}';";


    } 
}
