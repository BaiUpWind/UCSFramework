using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DisplayBorder
{
    public static class ListExtension
    {
        public static List<T> Clone<T>(this List<T> list) where T : new()
        {
            var str = JsonConvert.SerializeObject(list);
            return JsonConvert.DeserializeObject<List<T>>(str);
        }
    }

    public static class ObjectExtension
    {
        public static T Clone<T>(this T obj) where T : new()  
        {
            var str = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(str);
        }

        public static T Clone<T>(this T obj,Type targetType) where T : new()
        {
            var str = JsonConvert.SerializeObject(obj);
            return (T)JsonConvert.DeserializeObject(str, targetType);
        }
    }
}
