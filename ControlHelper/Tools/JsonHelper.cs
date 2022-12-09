using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlHelper.Tools
{
    public class JsonHelper
    {
        /// <summary>
        /// 写入json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="path"></param>
        public static void WriteJson(object target, string path)
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
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <param name="autoWrite">当路径文件不存在时,是否自动创建</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static T ReadJson<T>(string path, bool autoWrite = false)
        {
            if (!File.Exists(path))
            {
                if (!autoWrite)
                {
                    throw new Exception($"未找到对应路径文件,path'{path}'");
                }
                else
                {
                    try
                    {
                        var type = GetCollectionElementType(typeof(T));
                        if (type != null)
                        {
                            var obj = new List<T>();
                            WriteJson(obj, path);
                        }
                        else
                        {
                            var target = Activator.CreateInstance<T>();
                            WriteJson(target, path);
                        }
                    }
                    catch (Exception ex)
                    {
                        //可能会有无参构造的类
                        throw ex;
                    }
                }
            }
            var txt = File.ReadAllText(path);
            var json = JsonConvert.DeserializeObject<T>(txt);
            return json;
        }
        /// <summary>
        /// 判断该类型是否继承了<see cref="System.Collections.IList "/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private static bool IsList(Type type)
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
        /// <summary>
        /// Retrieves the collection element type from this type
        /// </summary>
        /// <param name="type">The type to query</param>
        /// <returns>The element type of the collection or null if the type was not a collection
        /// </returns>
        public static Type GetCollectionElementType(Type type)
        {
            if (null == type)
                throw new ArgumentNullException("type");

            // first try the generic way
            // this is easy, just query the IEnumerable<T> interface for its generic parameter
            var etype = typeof(IEnumerable<>);
            foreach (var bt in type.GetInterfaces())
                if (bt.IsGenericType && bt.GetGenericTypeDefinition() == etype)
                    return bt.GetGenericArguments()[0];

            // now try the non-generic way

            // if it's a dictionary we always return DictionaryEntry
            if (typeof(System.Collections.IDictionary).IsAssignableFrom(type))
                return typeof(System.Collections.DictionaryEntry);

            // if it's a list we look for an Item property with an int index parameter
            // where the property type is anything but object
            if (typeof(System.Collections.IList).IsAssignableFrom(type))
            {
                foreach (var prop in type.GetProperties())
                {
                    if ("Item" == prop.Name && typeof(object) != prop.PropertyType)
                    {
                        var ipa = prop.GetIndexParameters();
                        if (1 == ipa.Length && typeof(int) == ipa[0].ParameterType)
                        {
                            return prop.PropertyType;
                        }
                    }
                }
            }

            // if it's a collection, we look for an Add() method whose parameter is 
            // anything but object
            if (typeof(System.Collections.ICollection).IsAssignableFrom(type))
            {
                foreach (var meth in type.GetMethods())
                {
                    if ("Add" == meth.Name)
                    {
                        var pa = meth.GetParameters();
                        if (1 == pa.Length && typeof(object) != pa[0].ParameterType)
                            return pa[0].ParameterType;
                    }
                }
            }
            if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type))
                return typeof(object);
            return null;
        }
    }
}
