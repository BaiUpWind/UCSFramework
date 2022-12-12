using Config.DeviceConfig.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DisplayConveyer 
{
    internal static class Extend
    { 
        /// <summary>
        /// DataTable转换为实体类
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="dt">DataSet</param> 
        /// <returns>实体类</returns>
        public static IList<T> ToEntityList<T>(this DataTable dt ) where T : class, new()
        {
            if (dt == null || dt.Rows.Count < 0)
                return default(IList<T>); 
            // 返回值初始化
            IList<T> result = new List<T>();
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                T _t = (T)Activator.CreateInstance(typeof(T));
                PropertyInfo[] propertys = _t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    if (dt.Columns.IndexOf(pi.Name.ToUpper()) != -1 && dt.Rows[j][pi.Name.ToUpper()] != DBNull.Value)
                    {
                        pi.SetValue(_t, dt.Rows[j][pi.Name.ToUpper()], null);
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
    }
}
