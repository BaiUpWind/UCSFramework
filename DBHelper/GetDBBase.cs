using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBHelper
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DBType
    {
        MySql,
        Oracle,
        SqlServer,
        Sqllite
    }
    /// <summary>
    /// 获取数据库访问实例，需要访问则继承这个类
    /// </summary>
    public abstract class GetDBBase 
    {
        public GetDBBase(DBType type, string connStr)
        {
            switch (type)
            {
                case DBType.MySql:
                    db = new MySqlHelp(connStr);
                    break;
                case DBType.Oracle:
                    db = new OracleHelp(connStr);
                    break;
                case DBType.SqlServer:
                    db = new SQLServerHelp(connStr);
                    break;
                case DBType.Sqllite:
                    db = new SqliteHelp(connStr);
                    break;
            }
        }

        protected DBUnitiyBase db;

        public void SetConnStr(string connStr) => db.ConnStr = connStr;
        public bool TestConnected()
        {
            if (db == null) return false;
            try
            {
                db.CurrentConnection.Open();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                db.CurrentConnection.Close();
                db.CurrentConnection.Dispose();
            }
        }
    }
 
}
