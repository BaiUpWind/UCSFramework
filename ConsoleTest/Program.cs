using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleTest
{
    internal class Program
    {

         
        public readonly static string SysPath = Directory.GetCurrentDirectory().ToString();
        public readonly static string SavePath = SysPath  +@"\img\";
        public readonly static string DbPath = SysPath + @"\sesedb";
        public readonly static Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            var dirName = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            if (!Directory.Exists(SavePath + dirName))
            {
                Directory.CreateDirectory(SavePath + dirName);
            }
            SQLiteConnection connection = null;
            try
            {
                connection = new SQLiteConnection("data source = " + DbPath);
                connection.Open();
            }
            catch (Exception)
            {
                Console.WriteLine("数据库打开失败!");
                return;
            }
            Console.WriteLine("数据库打开成功!");

            var timestampStr = config.AppSettings.Settings["timestamp"].Value;
            var maxcount = config.AppSettings.Settings["maxcount"].Value;
            if (string.IsNullOrWhiteSpace(timestampStr))
            {
                timestampStr = "0";
            }

            if (!long.TryParse(timestampStr, out long timestamp)) return;

            string sql = @"select a.name,a.size,a.url,a.type,b.timestamp from attachments a join messages b  on a.message_id = b.message_id
                           where b.timestamp > "+ timestamp + " and a.type like '%image%'  order by b.timestamp  limit " + maxcount + "";
            SQLiteCommand cmd = new SQLiteCommand();
            SQLiteDataAdapter adapter = new SQLiteDataAdapter();
            cmd.CommandText = sql;
            cmd.Connection = connection;
            adapter.SelectCommand = cmd;
            DataSet ds = new DataSet();
            adapter.Fill(ds);
            var list = DataSetToEntityList<ImageInfo>(ds, 0);
            int failCount = 0;
            foreach (var item in list)
            { 
                using (WebClient webClient = new WebClient())
                {
                    try
                    {
                        webClient.DownloadFile(item.URL, SavePath + dirName + "\\" + item.NAME);
                        Console.WriteLine($"获取成功'{item.NAME}'");
                        failCount = 0;
                    }
                    catch (Exception)
                    {
                        if (failCount > 3)
                        {
                            Console.WriteLine($"尝试重新获取次数大于3次，跳到下一个目标进行获取");
                            continue;
                        }
                        failCount++;
                        Console.WriteLine($"错误，尝试重新获取！count'{failCount}'");
                    }
                }
            } 
            config.AppSettings.Settings["timestamp"].Value = list[list.Count - 1].TIMESTAMP.ToString();
            config.Save();
            Console.WriteLine("Finishied,按任意键关闭!"); 
            Console.ReadKey();
        }
        /// <summary>
        /// DataSet转换为实体列表
        /// </summary>
        /// <typeparam name="T">实体类</typeparam>
        /// <param name="p_DataSet">DataSet</param>
        /// <param name="p_TableIndex">待转换数据表索引</param>
        /// <returns>实体类列表</returns>
        public static IList<T> DataSetToEntityList<T>(DataSet p_DataSet, int p_TableIndex)
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
        public class ImageInfo
        {
            public string NAME { get; set; }
            public long SIZE { get; set; }
            public string URL { get; set; }
            public long TIMESTAMP { get; set; }
        }
    }
}
