using CommonApi.DBHelper;
using DeviceConfig.Core;
using System;

namespace DeviceConfig
{

    internal sealed class DataBaseOperation : OperationBase
    {
        DBUnitiyBase db;
        public DataBaseOperation(IDeviceConfig config) : base(config)
        {
            if (config is DataBaseCfg dataBase)
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

        public override bool Connection()
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

        public override Reuslt Read<Command, Reuslt>(Command cmd)
        {
            throw new NotImplementedException();
        }

        public override ResultBase Read(CommandBase command)
        {
            throw new NotImplementedException();
        }
    }
}
