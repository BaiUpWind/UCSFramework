using Oracle.ManagedDataAccess.Client;
using System.Data.Common;

namespace CommonApi.DBHelper
{
    /// <summary>
    /// oracle 数据库访问
    /// </summary>
    public class OracleHelp : DBUnitiyBase
    {
        public OracleHelp(string constr) : base(constr)
        {

        }
        OracleConnection _DBConnectionObj;
        OracleCommand _DbCommandObj;
        OracleDataAdapter _DbDataAdapterObj;

        protected override DbConnection DBConnectionObj
        {
            get
            { 
                if (_DBConnectionObj == null)
                {
                    _DBConnectionObj = new OracleConnection(ConnStr);
                }
                return _DBConnectionObj;
            }
        }


        protected override DbCommand DbCommandObj
        {
            get
            {
                if (_DbCommandObj == null)
                {
                    _DbCommandObj = new OracleCommand();
                }
                return _DbCommandObj;
            }
        }

        protected override DbDataAdapter DbDataAdapterObj
        {
            get
            {
                if (_DbDataAdapterObj == null)
                {
                    _DbDataAdapterObj = new OracleDataAdapter();
                }
                return _DbDataAdapterObj;
            }
        }
        public override void MultiInsertData(System.Data.DataSet ds)
        { 
            string sql = string.Format("select * from {0}", ds.Tables[0].TableName); //必须与ds中的一致
            System.Data.DataTable dt = ds.Tables[0];
            OracleConnection conn = new OracleConnection(ConnStr);
            OracleCommand cmd = new OracleCommand(sql, conn);
            conn.Open();
            OracleDataAdapter da = new OracleDataAdapter(cmd);
            OracleCommandBuilder cb = new OracleCommandBuilder(da);
            da.Update(dt);
            conn.Close();
            conn.Dispose();

        }
    }
}

