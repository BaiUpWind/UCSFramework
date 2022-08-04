using CommonApi.DBHelper;
using System;

namespace DeviceConfig.Core
{

    public sealed class DataBaseOperation : OperationBase
    {
      
        public DataBaseOperation( )  
        {
            if (ConnectConfig is DataBaseConnectCfg dataBase)
            {
                switch (int.Parse(dataBase.DbType))
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
            //throw new ArgumentException("创建数据库操作实例失败!");
        }
       private readonly  DBUnitiyBase db;
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
            db.GetDataTable(command.CommandStr, System.Data.CommandType.Text);

            return command.Result;
        }
    }
}
