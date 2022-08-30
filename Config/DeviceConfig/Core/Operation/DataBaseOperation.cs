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
            if (ConnectConfig is DataBaseConnectCfg dataBase)
            {
                switch ( dataBase.DbType )
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
        public override ResultBase Read(CommandBase command)
        {
            if (command is SQLCmd cmd)
            { 
                //var data = db.DatatoTable(db.GetDataTable(cmd.Sql, System.Data.CommandType.Text));
                ////var  data1 = db.GetDataSet(cmd.Sql, System.Data.CommandType.Text);
                ////var data2 = db.GetDataReader(cmd.Sql, System.Data.CommandType.Text); 
                //command.Result.Tables = data;
                command.Result.Data = db.GetDataTable(cmd.Sql, System.Data.CommandType.Text);
            }

            return command.Result;
        }

        public override void SetConn(ConnectionConfigBase conn)
        {
            base.SetConn(conn);
             
            if (conn is DataBaseConnectCfg dbconn)
            {
                CreateType(conn);
                //db.ConnStr = dbconn.GetConnStr();
            }
        }

        private void CreateType(ConnectionConfigBase type)
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
