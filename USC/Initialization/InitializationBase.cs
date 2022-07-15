using CommonApi;
using CommonApi.DBHelper;
using CommonApi.FileOperate;
using Modle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USC.Initialization
{
    /// <summary>
    /// 初始化基类
    /// </summary>
    public   class InitializationBase
    {
        /// <summary>
        /// 当前系统的工作路径
        /// </summary>
        public readonly static string SysPath = System.IO.Directory.GetCurrentDirectory().ToString();

        /// <summary>
        /// 创建系统文件
        /// </summary>
        public static void CreateSysFile()
        {
            if (!Directory.Exists(SysPath + "\\SystemFile"))
            {
                Directory.CreateDirectory(SysPath + "\\SystemFile");
            } 
        }


        /// <summary>
        /// 写入数据库连接方式
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public   OperateResult WriteDbIni(DataBasePara tag)
        {
            OperateResult or = new OperateResult();
            try
            {
                RWIniFile rw = new RWIniFile(SysPath + "\\SystemFile\\DB.ini");
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
        public OperateResult<DataBasePara> ReadDbIni()
        {
            OperateResult<DataBasePara> or = new OperateResult<DataBasePara>();
            try
            {
                RWIniFile rw = new RWIniFile(SysPath + "\\SystemFile\\DB.ini");
                FileInfo fileInfo = new FileInfo(SysPath + "\\SystemFile\\DB.ini"); 
                if (!fileInfo.Exists)
                {
                    WriteDbIni(new DataBasePara());
                }
                DataBasePara tag = new DataBasePara();
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
                        or.Content = new DataBasePara();
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
                or.Content = new DataBasePara();
                return or;
            }
        }
    }
}
