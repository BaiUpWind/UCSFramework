using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeviceConfig
{
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
