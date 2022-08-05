using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace CommonApi.DBHelper
{
    public class SQLServerHelp : DBUnitiyBase
    {
        public SQLServerHelp(string connStr) : base(connStr)
        {
        }
        SqlConnection _sqlConnection;
        SqlCommand _sqlCommand;
        SqlDataAdapter _sqlDataAdapter;

        protected override DbConnection DBConnectionObj
        {
            get
            {
                if(_sqlConnection == null)
                {
                    _sqlConnection = new SqlConnection(ConnStr);
                }
                return _sqlConnection;
            }
        }

        protected override DbCommand DbCommandObj
        {
            get
            {
                if(_sqlCommand == null)
                {
                    _sqlCommand = new SqlCommand();
                }
                return _sqlCommand;
            }
        }

        protected override DbDataAdapter DbDataAdapterObj
        {
            get
            {
                if(_sqlDataAdapter == null)
                {
                    _sqlDataAdapter = new SqlDataAdapter();
                }
                return _sqlDataAdapter;
            }
        }

        public override void MultiInsertData(DataSet ds)
        { 
            string sql = string.Format("select * from {0}", ds.Tables[0].TableName); //必须与ds中的一致
            DataTable dt = ds.Tables[0];
            SqlConnection conn = new SqlConnection(ConnStr);
            SqlCommand cmd = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            SqlCommandBuilder cb = new SqlCommandBuilder(da);
            da.Update(dt);
            conn.Close();
            conn.Dispose();
        }
    }
}
