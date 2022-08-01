using CommonApi.DBHelper;
using DeviceConfig.Core;
using System;

namespace DeviceConfig
{

    public sealed class DataBaseOperation : OperationBase
    {
      
        public DataBaseOperation(ConnectionConfigBase defaultConn) : base(defaultConn)
        {
            if (defaultConn is DataBaseConnectCfg dataBase)
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
            }
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
            throw new NotImplementedException();
        }
    }
}
