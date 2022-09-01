using System;
using DeviceConfig.Core;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeviceConfig
{
    internal static class CreateFactory
    {
 

        /// <summary>
        /// 创建读取指令类型
        /// </summary>
        /// <param name="name"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static CommandBase CreateCmb(string name, params object[] objects)
        {
            return CreateObject<CommandBase>($"DeviceConfig.Core.{name}", objects);
        }

        private static T CreateObject<T>(string name, params object[] para) where T : class
        {
            Type type = Type.GetType(name);//反射入口
            return Activator.CreateInstance(type, para) as T;//创建实例
        }
    }


    /// <summary>
    /// 用于多态序列化
    /// <para>
    /// 用法
    ///  '[JsonConverter(typeof(PolyConverter))]'
    ///  添加到对应的抽象类字段上
    ///  </para>
    /// </summary>
    public class PolyConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            try
            {
                var jObject = JObject.Load(reader);
                foreach (var item in jObject.Properties())
                {

                    Type type = Type.GetType(item.Name);

                    var value = item.Value.ToObject(type);
                    return value;

                }

                return null;

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)

        {
            JObject jObject = new JObject();

            jObject.Add(value.GetType().FullName, JToken.FromObject(value));

            serializer.Serialize(writer, jObject);
        }
    }

}
