using System;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;

namespace DBHelper
{
    public class SqliteHelp : DBUnitiyBase
    {
        /// <summary>
        /// connStr = 文件绝对路径
        /// </summary>
        /// <param name="connStr"></param>
        public SqliteHelp(string connStr) : base("data source = " + connStr)
        { 
            if (!File.Exists(connStr))
            {
                throw new Exception($"丢失系统文件{connStr}");
            }
        }
   
        SQLiteConnection _DBConnectionObj;
        SQLiteCommand _DbCommandObj;
        SQLiteDataAdapter _DbDataAdapterObj;

        protected override DbConnection DBConnectionObj
        {
            get
            {
                if (_DBConnectionObj == null)
                {
                    _DBConnectionObj = new SQLiteConnection(ConnStr);
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
                    _DbCommandObj = new SQLiteCommand();
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
                    _DbDataAdapterObj = new SQLiteDataAdapter();
                }
                return _DbDataAdapterObj;
            }
        }

        public override void MultiInsertData(DataSet ds)
        {
           
        }
    }
}
