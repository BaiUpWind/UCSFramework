using CommonApi.DBHelper;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DeviceConfig.Core
{


    [DependOn(typeof(DataBaseConnectCfg) ,typeof(SQLCmd))]
    public sealed class DataBaseOperation : OperationBase 
    {
      
        public DataBaseOperation( )  
        {
            if (ConnectConfig is DataBaseConnectCfg dbcc)
            {
                CreateType(dbcc);
                return;
            }
           throw new ArgumentException("创建数据库操作实例失败!");
        }
        private DBUnitiyBase db;


        private List<SQLCmd> cmds = new List<SQLCmd>();
        [Instance]
        public override object Commands { get => cmds; set => cmds = value as List<SQLCmd>; }  

        public override bool Connect()
        { 
            try
            {
                if(ConnectConfig is DataBaseConnectCfg dbcc)
                {
                  db.ConnStr =  dbcc.GetConnStr().Replace("\r\n", "");
                }
                db.CurrentConnection.Open();
                return true;
            }
            catch
            {
                return false;
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
   
        protected override ResultBase Read(object cmd)
        {
            if (cmd is SQLCmd sqlcmd)
            {
                if (ConnectConfig is DataBaseConnectCfg dbcc)
                {
                    db.ConnStr = dbcc.GetConnStr().Replace("\r\n", "");
                }
                //因为测试先注释掉
                try
                {
                    if (string.IsNullOrEmpty((sqlcmd.CommandStr?.ToString()))) return null;
                    var data = db.GetDataTable(sqlcmd.CommandStr.ToString().Replace("\r\n", ""), System.Data.CommandType.Text);
                    sqlcmd.Result.Data = data;
                    return sqlcmd.Result;
                }
                catch (Exception)
                {
                    return null;
                } 
            }
            return null; 
        }


        public override void SetConn(ConnectionConfigBase conn)
        {
            base.SetConn(conn);
             
            if (conn is DataBaseConnectCfg dbconn)
            {
                CreateType(dbconn); 
            }
        }

        private void CreateType(DataBaseConnectCfg type)
        {  
            if (type is DataBaseConnectCfg dataBase)
            {
                switch (dataBase.DbType)
                {
                    case  DBType.Oracle:
                        db = new OracleHelp(dataBase.GetConnStr());
                        break;
                    case  DBType.SqlServer:
                        db = new SQLServerHelp(dataBase.GetConnStr());
                        break;
                    case  DBType.MySql:
                        db = new MySqlHelp(dataBase.GetConnStr());
                        break;
                }
                ConnectConfig = dataBase;
                return;
            }
            throw new ArgumentException("创建数据库操作实例失败!"); 
        }

      
    }
}
