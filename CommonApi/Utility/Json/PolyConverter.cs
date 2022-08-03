using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace CommonApi
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
            var jObject = JObject.Load(reader);
            foreach (var item in jObject.Properties())
            {

                Type type = Type.GetType(item.Name);

                var value = item.Value.ToObject(type); 
                return value;

            }
            return null;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)

        {
            JObject jObject = new JObject();

            jObject.Add(value.GetType().FullName, JToken.FromObject(value));

            serializer.Serialize(writer, jObject);
        } 
    }

    /// <summary>
    /// 用于多态列表的转化
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PolyListConverter<T> : JsonConverter
    {
#pragma warning disable CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
#pragma warning disable CS8765 // 参数类型的为 Null 性与重写成员不匹配(可能是由于为 Null 性特性)。 
#pragma warning disable CS8604 // 引用类型参数可能为 null。
#pragma warning disable CS8603 // 可能返回 null 引用。
        public override bool CanConvert(Type objectType)
        {
            return true;
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            List<T> values = new List<T>();
            foreach (var item in jObject.Properties())
            {
                Type type = Type.GetType(item.Name);
                var value = item.Value.ToObject(type);
                values.Add((T)value);
            }
            return values;
        }
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var values = (List<T>)value;
            JObject jObject = new JObject();
            foreach (var item in values)
            {
                jObject.Add(item?.GetType().FullName, JToken.FromObject(item));
            }
            var p = jObject.Properties();
            //foreach (var item in p)
            //{
            //    Debug.Log(item.Name);
            //}
            serializer.Serialize(writer, jObject);
        }
#pragma warning restore CS8603 // 可能返回 null 引用。
#pragma warning restore CS8604 // 引用类型参数可能为 null。
#pragma warning restore CS8765 // 参数类型的为 Null 性与重写成员不匹配(可能是由于为 Null 性特性)。
#pragma warning restore CS8600 // 将 null 字面量或可能为 null 的值转换为非 null 类型。
    }
}
