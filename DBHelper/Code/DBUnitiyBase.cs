using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace DBHelper
{
    /*******************************************************
     * 
     * 
     * 
     *      数据库访问的基类
     *      时间：2019年7月11日08:57:53
     *      更新：连接，执行，数据适配器，查询结果集，执行SQL（返回影响行数）,DataSet转实体类，Data
     * 
     * 
     * ****************************************************/
    public abstract class DBUnitiyBase
    {
        public DBUnitiyBase(string connStr)
        {
            ConnStr = connStr;
        }
        public string ConnStr { get; set; }
        protected abstract DbConnection DBConnectionObj { get; }
        protected abstract DbCommand DbCommandObj { get; }
        protected abstract DbDataAdapter DbDataAdapterObj { get; }
        /// <summary>
        /// 多行数据插入
        /// </summary>
        /// <param name="ds"></param>
        public abstract void MultiInsertData(DataSet ds);
        protected DbTransaction DbTransObj;
        /// <summary>
        /// 连接到的数据库
        /// </summary>
        public DbConnection CurrentConnection
        {
            get
            {
                return DBConnectionObj;
            }
        }
        bool _IsTrans = false;

        /// <summary>
        /// 打开连接,如果已经打开则什么都不执行了
        /// </summary>
        void OpenConnection()
        {
            if (DBConnectionObj.State != ConnectionState.Open)
            {
                DBConnectionObj.ConnectionString = ConnStr;
                DBConnectionObj.Open();
            }
        }
        /// <summary>
        /// 给当前DbCommand对象赋值,并且OpenConnection();
        /// </summary>
        void SetCommandAndOpenConnect(string sqlText,CommandType cmdType = CommandType.Text, params DbParameter[] param)
        {
            //按说赋值Connection,CommandType,是不用多次赋值的
            DbCommandObj.CommandType = cmdType;
            DbCommandObj.Connection = DBConnectionObj;
            DbCommandObj.Parameters.Clear();
            if (param != null)
            {
                DbCommandObj.Parameters.AddRange(param);
            }
            DbCommandObj.CommandText = sqlText;
            OpenConnection();
        }
        /// <summary>
        /// 执行一条指定命令类型(SQL语句或存储过程等)的SQL语句,返回所影响行数
        /// </summary>
        public int ExecNonQuery(string sqlText, CommandType cmdType = CommandType.Text, params DbParameter[] param)
        {
            try
            {
                SetCommandAndOpenConnect(sqlText, cmdType, param);
                return DbCommandObj.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnect();
            }
        }
        /// <summary>
        /// 执行一条指定命令类型(SQL语句或存储过程等)的SQL语句,返回所影响行数
        /// ，使用事务执行大批量的SQL。
        /// </summary>
        public int ExecNonQueryTrans(string sqlText, CommandType cmdType = CommandType.Text, params DbParameter[] param)
        {
            try
            {
                SetCommandAndOpenConnect(sqlText, cmdType, param);
                TransStart();
                var result = DbCommandObj.ExecuteNonQuery();
                TransCommit();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnect();
            }
        }
        /// <summary>
        /// 关闭连接,如果没有开始事务或连接打开时才关闭
        /// </summary>
        void CloseConnect()
        {
            if (!_IsTrans)
            {
                if (DBConnectionObj.State == ConnectionState.Open)
                {
                    DBConnectionObj.Close();
                }
            }
        }


        /// <summary>
        /// 获得DataReader对象
        /// </summary>
        public DbDataReader GetDataReader(string sqlText,CommandType cmdType = CommandType.Text, params DbParameter[] param)
        {
            try
            {
                SetCommandAndOpenConnect(sqlText, cmdType, param);
                CommandBehavior cmdBehavior = CommandBehavior.CloseConnection;
                if (_IsTrans)
                {
                    cmdBehavior = CommandBehavior.Default;
                }
                DbDataReader dbReader = DbCommandObj.ExecuteReader(cmdBehavior);
                return dbReader;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

                //DataReader用dbReader对象来关闭
                CloseConnect();
            }
        }

        /// <summary>
        /// 执行一条SQL语句返回DataSet对象
        /// </summary>
        public DataSet GetDataSet(string sqlText,CommandType cmdType = CommandType.Text, params DbParameter[] param)
        {
            try
            {
                SetCommandAndOpenConnect(sqlText, cmdType, param);
                DbDataAdapterObj.SelectCommand = DbCommandObj;
                DataSet ds = new DataSet();
                DbDataAdapterObj.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnect();
            }
        }

        public DataTable GetDataTable(string sqlText,CommandType cmdType = CommandType.Text, params DbParameter[] param)
        {
            try
            {
                SetCommandAndOpenConnect(sqlText, cmdType, param);
                DbDataAdapterObj.SelectCommand = DbCommandObj;
                DataTable dt = new DataTable();
                DbDataAdapterObj.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnect();
            }
        }
        /// <summary>
        /// 返回一个查询的泛型集合  
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="sqlText">查询语句</param>
        /// <param name="cmdType">执行类型</param>
        /// <param name="index">DataSet的索引</param>
        /// <param name="param">传入参数</param>
        /// <returns></returns>
        public List<T> GetDataList<T>(string sqlText,CommandType cmdType = CommandType.Text, int index = 0, params DbParameter[] param) where T :class ,new()
        {
            try
            {
                SetCommandAndOpenConnect(sqlText, cmdType, param);
                DbDataAdapterObj.SelectCommand = DbCommandObj;
                DataSet ds = new DataSet();
                DbDataAdapterObj.Fill(ds);
                var List = DataSetToEntityList<T>(ds, index);
                if (List == null)
                {
                    return new List<T>();
                }
                else
                {
                    return List.ToList();

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnect();
            }
        }
        /// <summary>
        /// 获得首行首列
        /// </summary>
        public object GetScalar(string sqlText,CommandType cmdType = CommandType.Text, params DbParameter[] param)
        {
            try
            {
                SetCommandAndOpenConnect(sqlText, cmdType, param);
                return DbCommandObj.ExecuteScalar();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                CloseConnect();
            }
        }
        #region  Trans
        /// <summary>
        /// 开始执行事务
        /// </summary>
        public void TransStart()
        {
            OpenConnection();
            DbTransObj = DBConnectionObj.BeginTransaction();
            DbCommandObj.Transaction = DbTransObj;
            _IsTrans = true;
        }
        /// <summary>
        /// 事务提交
        /// </summary>
        public void TransCommit()
        {
            _IsTrans = false;
            DbTransObj.Commit();
            CloseConnect();
        }
        /// <summary>
        /// 事务回滚
        /// </summary>
        public void TransRollback()
        {
            _IsTrans = false;
            DbTransObj.Rollback();
            CloseConnect();
        }
        #endregion

        #region DataSetToEntity&List


        /// <summary>
        /// dataTable 转list
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<Dictionary<string, object>> DatatoTable(DataTable dt)
        {

            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dt.Rows)//每一行信息，新建一个Dictionary<string,object>,将该行的每列信息加入到字典
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    result.Add(dc.ColumnName, dr[dc].ToString());
                }
                list.Add(result);
            }
            return list;
        }
        /// <summary>
        /// DataSet转换为实体类
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="p_DataSet">DataSet</param>
        /// <param name="p_TableIndex">待转换数据表索引</param>
        /// <returns>实体类</returns>
        public T DataSetToEntity<T>(DataSet p_DataSet, int p_TableIndex) where T : class, new()
        {
            if (p_DataSet == null || p_DataSet.Tables.Count < 0)
                return default(T);
            if (p_TableIndex > p_DataSet.Tables.Count - 1)
                return default(T);
            if (p_TableIndex < 0)
                p_TableIndex = 0;
            if (p_DataSet.Tables[p_TableIndex].Rows.Count <= 0)
                return default(T);

            DataRow p_Data = p_DataSet.Tables[p_TableIndex].Rows[0];
            // 返回值初始化
            T _t = (T)Activator.CreateInstance(typeof(T));
            PropertyInfo[] propertys = _t.GetType().GetProperties();
            foreach (PropertyInfo pi in propertys)
            {
                if (p_DataSet.Tables[p_TableIndex].Columns.IndexOf(pi.Name.ToUpper()) != -1 && p_Data[pi.Name.ToUpper()] != DBNull.Value)
                {
                    pi.SetValue(_t, p_Data[pi.Name.ToUpper()], null);
                }
                else
                {
                    pi.SetValue(_t, null, null);
                }
            }
            return _t;
        }
        /// <summary>
        /// DataSet转换为实体列表
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="p_DataSet">DataSet</param>
        /// <param name="p_TableIndex">待转换数据表索引</param>
        /// <returns>实体类列表</returns>
        public IList<T> DataSetToEntityList<T>(DataSet p_DataSet, int p_TableIndex) where T : class, new()
        {
            if (p_DataSet == null || p_DataSet.Tables.Count < 0)
                return default(IList<T>);
            if (p_TableIndex > p_DataSet.Tables.Count - 1)
                return default(IList<T>);
            if (p_TableIndex < 0)
                p_TableIndex = 0;
            if (p_DataSet.Tables[p_TableIndex].Rows.Count <= 0)
                return default(IList<T>);
            DataTable p_Data = p_DataSet.Tables[p_TableIndex];
            // 返回值初始化
            IList<T> result = new List<T>();
            for (int j = 0; j < p_Data.Rows.Count; j++)
            {
                T _t = (T)Activator.CreateInstance(typeof(T));
                PropertyInfo[] propertys = _t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (p_Data.Columns.IndexOf(pi.Name.ToUpper()) != -1 && p_Data.Rows[j][pi.Name.ToUpper()] != DBNull.Value)
                    {
                        pi.SetValue(_t, p_Data.Rows[j][pi.Name.ToUpper()], null);
                    }
                    else
                    {
                        pi.SetValue(_t, null, null);
                    }
                }
                result.Add(_t);
            }
            return result;
        }
        #endregion
    } 
}
