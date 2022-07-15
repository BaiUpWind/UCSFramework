using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Linq;

namespace CommonApi.FileOperate
{
    /// <summary>
    /// 读写Ini文件
    /// </summary>
    public class RWIniFile
    {
         private string path;
        public RWIniFile( )
        {
        
        }
        public RWIniFile(string Path)
        {
            path = Path;
        }
        ////声明读写INI文件的API函数 
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);


        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 设置新的路径
        /// </summary>
        /// <param name="path"></param>
        public void SetPath(string path) => this.path = path;

        /// <summary>
        /// 写入ini文件
        /// </summary>
        /// <param name="Section">主节点</param>
        /// <param name="Key">键</param>
        /// <param name="Value">值</param>
        public void IniWriteValue(string Section, string Key, string Value)
        {
            try
            {
                WritePrivateProfileString(Section, Key, Value, path);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 读取ini文件
        /// </summary>
        /// <param name="Section">主节点</param>
        /// <param name="Key">键</param>
        /// <returns>值</returns>
        //读取INI文件指定
        public string IniReadValue(string Section, string Key)
        {
            try
            {
                StringBuilder temp = new StringBuilder(2000);
                int i = GetPrivateProfileString(Section, Key, "", temp, 2000, path);
                return temp.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 写入一个泛型类的ini文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Section"></param>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        public void WriteIni<T>(string Section, T tag)
        {
            try
            {
                Type t = typeof(T);
                PropertyInfo[] properties = t.GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    PropertyInfo proInfo = t.GetProperty(property.Name);
                    try
                    {
                        IniWriteValue(Section, proInfo.Name, proInfo.GetValue(tag) == null ? "" : proInfo.GetValue(tag).ToString());
                    }
                    catch (Exception)
                    {
                        IniWriteValue(Section, proInfo.Name, "none");
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// 泛型读取一个int文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Section"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public T ReadIni<T>(string Section, T tag)
        {
            try
            {
                tag = Activator.CreateInstance<T>();//  实例化 
                Type t = typeof(T);
                PropertyInfo[] properties = t.GetProperties();
                for (int i = 0; i < properties.Length; i++)
                {

                    var result = IniReadValue(Section, properties[i].Name);
                    var type = properties[i].PropertyType;//获取改行的数据类型
                    if (type.Name == "DateTime" || type.Name == "TimeSpan")
                    {
                        continue;
                    }
                    var data = Convert.ChangeType(result, type);
                    properties[i].SetValue(tag, data, null);
                }

                return tag;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 写入一个ini泛型集合文件
        /// <para>写入之前会进行删除操作</para>
        /// </summary>
        /// <typeparam name="T">泛型类(传入无参数的实体)</typeparam> 
        /// <param name="list">实体集合</param>
        /// <returns></returns>
        public OperateResult WriteIniList<T>(List<T> list)
        {
            OperateResult or = new OperateResult();
            T tar = Activator.CreateInstance<T>();//  实例化  

            try
            {
                FileInfo fileInfo = new FileInfo(path);
                //RWIniFile rw = new RWIniFile(path);
                if (!fileInfo.Exists)
                {
                    //rw.
                        IniWriteValue(tar.ToString(), "ListCount", "0");
                    if (list.Any())
                    {
                        WriteIniList(list);
                    }
                    or.IsSuccess = true;
                    or.Message = $"初始化{tar}";
                    return or;
                }
                fileInfo.Delete();//删除文件
                //rw.
                    IniWriteValue(tar.ToString(), "ListCount", list.Count.ToString());
                for (int i = 0; i < list.Count; i++)
                {
                    //rw.
                        WriteIni((i + 1).ToString(), list[i]);
                }

                or.IsSuccess = true;
                or.Message = $"写入{tar}成功";
                return or;
            }
            catch (Exception ex)
            {

                or.Message = ExceptionOperater.GetSaveStringFromException($"写入{tar}文件错误", ex);
                or.IsSuccess = false;
                return or;
            }
        }

        /// <summary>
        /// 读取一个泛型类的集合ini文件
        /// </summary>
        /// <typeparam name="T">泛型（无参数的构造函数）</typeparam>
        /// <param name="tar">实体</param>
        /// <returns></returns>
        public OperateResult<List<T>> ReadIniList<T>()
        {
            OperateResult<List<T>> or = new OperateResult<List<T>>();
            T tar = Activator.CreateInstance<T>();//  实例化   
            try
            {
                FileInfo fileInfo = new FileInfo(path);
                //RWIniFile rw = new RWIniFile(path);
                if (!fileInfo.Exists)
                {
                    //rw.
                        IniWriteValue(tar.ToString(), "ListCount", "0");
                    or.IsSuccess = true;
                    or.Message = $"未找到该文件，{tar}.ini，但是进行了初始化";
                    return or;
                }
                List<T> list = new List<T>();
                int count = Convert.ToInt32(
                    //rw.
                    IniReadValue(tar.ToString(), "ListCount"));
                for (int i = 0; i < count; i++)
                {
                    list.Add(
                        //rw.
                        ReadIni((i + 1).ToString(), tar));
                }
                or.Content = list;
                or.IsSuccess = true;
                or.Message = $"读取{tar}.ini,文件成功！ ";
                return or;
            }
            catch (Exception ex)
            {
                or.Message = ExceptionOperater.GetSaveStringFromException($"读取{tar }文件错误", ex);
                or.IsSuccess = false;
                or.Content = new List<T>();
                return or;
            }
        }
    }
}
