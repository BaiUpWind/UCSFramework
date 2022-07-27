using CommonApi;
using CommonApi.DBHelper;
using CommonApi.FileOperate;
using Modle.DeviceCfg;
using Modle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USC
{
    /// <summary>
    /// 初始化基类
    /// </summary>
    public class InitializationBase
    {
        /// <summary>
        /// 当前系统的工作路径
        /// </summary>
        public readonly static string SysPath = Directory.GetCurrentDirectory().ToString();
        /// <summary>
        /// 系统文件夹路径
        /// <para>包含基础的配置文件</para>
        /// </summary>
        public readonly static string SysFilePath = SysPath + "\\SystemFile"; 
        #region  配置文件路径
        /// <summary>
        /// 数据库配置文件
        /// </summary>
        public readonly static string DbCfgPath = SysFilePath + "\\DBCfg.ini";
         
        /// <summary>
        /// 串口通信的配置路径
        /// </summary>
        public readonly static string SerialPortCfgPath = SysFilePath + "\\SerialPortCfg.ini";
     
        /// <summary>
        /// PLC 通信配置路径
        /// </summary>
        public readonly static string PLCCfgPath = SysFilePath + "\\PLCCfg.ini";

        /// <summary>
        /// Tcp客户端配置路径
        /// </summary>
        public readonly static string TCPCfgPath = SysFilePath + "\\TCPClientCfg.ini";
        #endregion

        private readonly RWIniFile rw = new RWIniFile();
 
        /// <summary>
        /// 创建系统文件
        /// </summary>
        public void CreateSysFile()
        {
            if (!Directory.Exists(SysFilePath))
            {
                Directory.CreateDirectory(SysFilePath);
            } 
        }


        /// <summary>
        /// 写入数据库连接方式
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public   OperateResult WriteDbIni(DataBaseCfg tag)
        {
            OperateResult or = new OperateResult();
            try
            {
                rw.SetPath(DbCfgPath); 
                rw.WriteIni("DataBasePara", tag);
                or.Message = "写入成功！";
                or.IsSuccess = true;
                return or;
            }
            catch (Exception ex)
            {

                or.Message = ExceptionOperater.GetSaveStringFromException("写入数据库配置文件错误", ex);
                or.IsSuccess = false;
                return or;
            }
        }

        /// <summary>
        /// 读取数据库连接方式 
        /// <para>并且缓存到程序中<see cref="SysParaBase"/></para>
        /// </summary>
        /// <returns>数据库连接参数</returns>
        public OperateResult<DataBaseCfg> ReadDbIni()
        {
            OperateResult<DataBaseCfg> or = new OperateResult<DataBaseCfg>();
            try
            {
                
                FileInfo fileInfo = new FileInfo(DbCfgPath);
                rw.SetPath(DbCfgPath);
                if (!fileInfo.Exists)
                {
                    WriteDbIni(new DataBaseCfg());
                }
                DataBaseCfg tag = new DataBaseCfg();
                tag = rw.ReadIni("DataBasePara", tag);
                switch (tag.DbType)
                {
                    case "0":
                        SysParaBase.BEnum = DBEnum.Oracle;
                        SysParaBase.DataBaseConnectionStr = string.Format("Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))(CONNECT_DATA=(SERVICE_NAME={2})));User Id={3};Password={4}", tag.DbIp, tag.DbPort, tag.DbName, tag.DbUserName, tag.DbPassWord);
                        SysParaBase.DBpara = tag;
                        break;
                    case "1":
                        SysParaBase.BEnum =  DBEnum.SqlServer;
                        SysParaBase.DataBaseConnectionStr = string.Format("server={0}; uid={1}; pwd={2};database={3}", tag.DbIp, tag.DbUserName, tag.DbPassWord, tag.DbName);
                        SysParaBase.DBpara = tag;

                        break;
                    case "2":
                        SysParaBase.BEnum =  DBEnum.MySql;
                        SysParaBase.DataBaseConnectionStr = string.Format("server={0};database={1}; uid={2};pwd ={3}", tag.DbIp, tag.DbName, tag.DbUserName, tag.DbPassWord);
                        SysParaBase.DBpara = tag;
                        break;
                    default:
                        or.Message = ExceptionOperater.GetSaveStringFromException("读取数据库配置文件错误", new Exception("数据库类型错误:" + tag.DbType));
                        or.IsSuccess = false;
                        or.Content = new DataBaseCfg();
                        return or;

                }
                or.IsSuccess = true;
                or.Content = tag;
                return or;
            }
            catch (Exception ex)
            {

                or.Message = ExceptionOperater.GetSaveStringFromException("读取数据库配置文件错误", ex);
                or.IsSuccess = false;
                or.Content = new DataBaseCfg();
                return or;
            }
        }

        /// <summary>
        /// 读取对应设备类型的配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public OperateResult<List<T>> ReadDeviceCfg<T>(DeviceConnectedType deviceType) 
        {
            OperateResult<List<T>> or = new OperateResult<List<T>>();
            try
            {
        
                switch (deviceType)
                {
                    case DeviceConnectedType.None:
                        rw.SetPath(SysPath);
                        break;
                    case DeviceConnectedType.Serial:
                        rw.SetPath( SerialPortCfgPath);
                        break;
                    case DeviceConnectedType.PLC:
                        rw.SetPath(PLCCfgPath);
                        break;
                    case DeviceConnectedType.TcpClient:
                        rw.SetPath( TCPCfgPath);
                        break;
                    default:
                        or.Content = null;
                        or.IsSuccess = false;
                        or.Message = $"读取失败，未找到对应的设备[{deviceType}]配置文件读取!";
                        return or;
                }
                return rw.ReadIniList<T>();
            }
            catch (Exception ex)
            {
                or.Content = null;
                or.IsSuccess = false;
                or.Message = ExceptionOperater.GetSaveStringFromException($"读取{typeof(T).GetType().Name}配置文件出错!", ex);
                return or;
            }
        }
        
        /// <summary>
        /// 写入对应设备类型的配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tar"></param>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        public OperateResult WriteDeviceCfg<T>(List<T> tar, DeviceConnectedType deviceType)
        {
            OperateResult or = new OperateResult();
            try
            { 
                switch (deviceType)
                {
                    case DeviceConnectedType.None:
                        rw.SetPath(SysPath);
                        break;
                    case DeviceConnectedType.Serial:
                        rw.SetPath(SerialPortCfgPath);
                        break;
                    case DeviceConnectedType.PLC:
                        rw.SetPath(PLCCfgPath);
                        break;
                    case DeviceConnectedType.TcpClient:
                        rw.SetPath(TCPCfgPath);
                        break;
                    default:
                        or.IsSuccess = false;
                        or.Message = $"写入失败，未找到对应的设备[{deviceType}]配置文件写入!";
                        return or;
                } 
                return rw.WriteIniList(tar);
            }
            catch (Exception ex)
            {
                or.IsSuccess = false;
                or.Message = ExceptionOperater.GetSaveStringFromException($"读取{typeof(T).GetType().Name}配置文件出错!", ex);
                return or;
            }
        }
         
    }
}
