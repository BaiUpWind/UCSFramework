using CommonApi.DBHelper;
using System;
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
        private    DBUnitiyBase db;
         
         
        public override bool Connect()
        {
        
            try
            {
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
                var data = db.GetDataTable(sqlcmd.SQL, System.Data.CommandType.Text);
                sqlcmd.Result.Data = data;
                return sqlcmd.Result;
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
                    case 0:
                        db = new OracleHelp(dataBase.GetConnStr());
                        break;
                    case 1:
                        db = new SQLServerHelp(dataBase.GetConnStr());
                        break;
                    case 2:
                        db = new MySqlHelp(dataBase.GetConnStr());
                        break;
                }
                return;
            }
            throw new ArgumentException("创建数据库操作实例失败!"); 
        }

      
    }
}
