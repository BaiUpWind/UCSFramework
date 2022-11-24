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
    }
}
