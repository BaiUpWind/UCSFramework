using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CommonApi
{
    public static class ListExtension
    {
        /// <summary>
        /// 深克隆一个泛型集合
        /// <para>需要在实体上加上[Serializable]</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> CloneSer<T>(this List<T> list) where T : new()
        {
            using (Stream objectStream = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(objectStream, list);
                objectStream.Seek(0, SeekOrigin.Begin);
                return (List<T>)formatter.Deserialize(objectStream);
            }
        }
        /// <summary>
        /// 深克隆一个泛型集合
        /// <para>使用反射方式</para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<T> CloneRef<T>(this List<T> list) where T : new()
        {
            List<T> items = new List<T>();
            foreach (var m in list)
            {
                var model = new T();
                var ps = model.GetType().GetProperties();
                var properties = m.GetType().GetProperties();
                foreach (var p in properties)
                {
                    foreach (var pm in ps)
                    {
                        if (pm.Name == p.Name)
                        {
                            pm.SetValue(model, p.GetValue(m));
                        }
                    }
                }
                items.Add(model);
            }
            return items;
        }

    }


}
