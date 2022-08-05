using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;

namespace CommonApi.DBHelper
{
    /// <summary>
    /// mysql数据库访问
    /// </summary>
    public class MySqlHelp : DBUnitiyBase
    {
        public MySqlHelp(string connStr) : base(connStr)
        {

        }
        MySqlConnection _dbConnection;
        MySqlCommand _dbCommand;
        MySqlDataAdapter _dbDataAdapter;
        protected override DbConnection DBConnectionObj {
            get
            {
                if(_dbConnection == null)
                {
                    _dbConnection = new MySqlConnection(ConnStr);
                }
                return _dbConnection;
            }
        }

        protected override DbCommand DbCommandObj
        {
            get
            {
                if(_dbCommand == null)
                {
                    _dbCommand = new MySqlCommand();
                }
                return _dbCommand;
            }
        }

        protected override DbDataAdapter DbDataAdapterObj
        {
            get
            {
                if(_dbDataAdapter == null)
                {
                    _dbDataAdapter = new MySqlDataAdapter();
                }
                return _dbDataAdapter;
            }
        }

        public override void MultiInsertData(DataSet ds)
        {
            //多行数据插入 
      
            string sql = string.Format("select * from {0}", ds.Tables[0].TableName); //必须与ds中的一致
            System.Data.DataTable dt = ds.Tables[0];
            MySqlConnection conn = new MySqlConnection(ConnStr);
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            conn.Open();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd); 
            MySqlCommandBuilder cb = new MySqlCommandBuilder(da);
            da.Update(dt);
            conn.Close();
            conn.Dispose(); 
        }
    }
}
