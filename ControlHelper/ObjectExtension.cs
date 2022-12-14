using System; 
using Newtonsoft.Json;

namespace ControlHelper
{
    public static class ObjectExtension
    {
        public static T Clone<T>(this T obj) where T : new()
        {
            var str = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(str);
        }

        public static T Clone<T>(this T obj, Type targetType) where T : new()
        {
            var str = JsonConvert.SerializeObject(obj);
            return (T)JsonConvert.DeserializeObject(str, targetType);
        }

        public static T CastTo<T>(this object value, T defaultValue)
        {
            Type typeFromHandle = typeof(T);
            object obj;
            try
            {
                obj = (typeFromHandle.IsEnum ? Enum.Parse(typeFromHandle, value.ToString()) : Convert.ChangeType(value, typeFromHandle));
            }
            catch
            {
                obj = defaultValue;
            }

            return (T)obj;
        }
    }
}
