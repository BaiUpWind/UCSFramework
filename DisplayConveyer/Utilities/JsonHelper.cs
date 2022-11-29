using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayConveyer
{
    /// <summary>
    /// 找不到文件异常
    /// </summary>
    public class CantFindFileException : Exception
    {
        public CantFindFileException(string info) : base(info)
        {

        }
    }
    public class JsonHelper
    {
        public JsonHelper(string path)
        {
            this.path = path;
        }
        private readonly string path;
        /// <summary>
        /// 写入json
        /// </summary> 
        /// <param name="target"></param> 
        public void WriteJson(object target)
        {
            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }
            File.WriteAllText(path, JsonConvert.SerializeObject(target, Formatting.Indented));
        }

        /// <summary>
        /// 读取一个json文件
        /// </summary> 
        /// <param name="autoWrite">当路径文件不存在时,是否自动创建</param>
        /// <returns></returns>
        /// <exception cref="CantFindFileException"></exception>
        public T ReadJson<T>(bool autoWrite = false)
        {
           return (T)ReadJson(typeof(T), autoWrite);
        }
        public object ReadJson(Type type, bool autoWrite = false)
        {
            if (!File.Exists(path))
            {
                if (!autoWrite)
                {
                    throw new CantFindFileException($"未找到对应路径文件,path'{path}'");
                }
                else
                {
                    try
                    {
                        var islist =  IsList(type);// GetCollectionElementType(typeof(T));
                        if (islist)
                        {
                         
                            var obj = Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { type.GenericTypeArguments[0] }));
                            WriteJson(obj);
                        }
                        else
                        {
                            var target = Activator.CreateInstance(type);
                            WriteJson(target);
                        }
                    }
                    catch (Exception ex)
                    {
                        //可能会有有参构造的类
                        throw ex;
                    }
                }
            }
            var txt = File.ReadAllText(path);
            var json = JsonConvert.DeserializeObject(txt,type);
            return json;
        }
        /// <summary>
        /// 判断该类型是否继承了<see cref="System.Collections.IList "/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private   bool IsList(Type type)
        {
            if (null == type)
                throw new ArgumentNullException("type");

            if (typeof(System.Collections.IList).IsAssignableFrom(type))
                return true;
            foreach (var it in type.GetInterfaces())
                if (it.IsGenericType && typeof(IList<>) == it.GetGenericTypeDefinition())
                    return true;
            return false;
        }

    }
}
