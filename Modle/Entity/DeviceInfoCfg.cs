using CommonApi.PLC;
using Modle.DeviceCfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using CommonApi.DBHelper;

namespace Modle
{
    /// <summary>
    /// 设备需要读取的值所需要的信息
    /// <para>数据库：配置SQL语句</para>
    /// <para>PLC:配置对应的DB块</para>
    /// <para>TcpClient:配置对应的协议</para>
    /// <para>SerialPort:配置对应的 读写信息</para>
    /// </summary>
    public class DeviceInfoCfg
    {

        public DeviceInfoCfg(IDeviceConfig defaultConn)
        {
            if (defaultConn == null)
            {
                throw new Exception("初始化设备信息配置，未能获取到默认的连接方式！");
            }
            this.defaultConn = defaultConn;
        }

        private readonly IDeviceConfig defaultConn;
        private InfoCommandBase commandBase;
        private bool start;
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

        /// <summary>
        /// 刷新数据间隔  单位:毫秒 
        /// </summary>
        public int RefreshInterval { get; set; }

        /// <summary>
        /// 读取设备数据的指令
        /// <para>这里应该是需要解析的指令</para>
        /// </summary>
        public object Command { get; set; }

        public object Data { get; set; }

        public async void Start()
        {
            Init();
            start = true;
            while (start)
            {
                Data = commandBase.Read(Command);
                await Task.Delay(RefreshInterval);
            }
        }

        public void Stop()
        {
            start = false;
            if(commandBase != null && ConnConfig != null)
            {
                commandBase.Disconnected();
                commandBase = null;
            }
        }
        private void Init()
        {
            if (commandBase == null && ConnConfig !=null)
            {
                switch (ConnConfig.DevType)
                {
                    case DeviceConnectedType.None:
                        break;
                    case DeviceConnectedType.Serial:
                        break;
                    case DeviceConnectedType.PLC:
                        PLCCfg plc = (PLCCfg)ConnConfig; 
                        if (plc == null) return;

                        if(plc.PLCType == PLCKind.Siemens)
                        {
                            commandBase = new SeimensOperation(ConnConfig);
                        }　
                        break;
                    case DeviceConnectedType.TcpClient:
                        break;
                    case DeviceConnectedType.DataBase:
                        commandBase = new DatabaseOperation(ConnConfig);
                        break; 
                } 
                if(commandBase == null)
                {
                    throw new Exception($"设备编号'{DeviceID}' 设备信息编号'{DeviceInfoID}' 设备信息名称'{DeviceInfoName}' 初始化失败！");

                }
                if (!commandBase.Connection())
                {
                    throw new Exception($"设备编号'{DeviceID}' 设备信息编号'{DeviceInfoID}' 设备信息名称'{DeviceInfoName}' 连接设备失败！");
                }
            }
        }

    }

    /// <summary>
    /// 读取设备信息的指令基类
    /// </summary>
    public abstract class InfoCommandBase
    {
        public InfoCommandBase(IDeviceConfig config)
        {
            DeviceConfig = config;
        }
        protected  IDeviceConfig DeviceConfig { get; }
        public abstract object Read(object command);
        public abstract bool Connection();
        public abstract void Disconnected();

        /// <summary>
        /// 测试连接是否正常
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckConn() => true;

    }

    public sealed class SeimensOperation : InfoCommandBase
    {
        public SeimensOperation(IDeviceConfig config) : base(config)
        {
            if(config is SiemensCfg siemens)
            {
                splc = new SiemensPlc(siemens.SiemensSelected, siemens.IP, siemens.Port, siemens.Rack, siemens.Slot);
            }
            throw new Exception("错误的PLC配置类型");
        }
        private SiemensPlc splc;


        public override bool Connection()
        {
            return splc != null && splc.Connection();
        }

        public override void Disconnected()
        {
            splc?.DisConnected();
        }

        public override object Read(object command)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class DatabaseOperation : InfoCommandBase
    {
        DBUnitiyBase db;
        public DatabaseOperation(IDeviceConfig config) : base(config)
        {
            if(config is DataBaseCfg dataBase)
            {
                switch (dataBase.GetDBEnum())
                {
                    case DBEnum.MySql:
                        db = new MySqlHelp(dataBase.GetConnStr());
                        break;
                    case DBEnum.Oracle:
                        db = new OracleHelp(dataBase.GetConnStr());
                        break;
                    case DBEnum.SqlServer:
                        db = new SQLServerHelp(dataBase.GetConnStr());
                        break;
                }
            }
        }

        public override bool Connection()
        {
            try
            {
                db.CurrentConnection.Open();
                return true;
            }
            catch  
            {
                return  false; 
            }
            finally
            {
                db?.CurrentConnection.Close();
            } 
        }

        public override void Disconnected()
        {
            db.CurrentConnection.Close();
        }

        public override object Read(object command)
        {
            throw new NotImplementedException();
        }
    } 
     
}
