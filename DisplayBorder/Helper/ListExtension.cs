using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;

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
    public static class TypeExMothod
    {
        public static Type GetArrayElementType(this Type t)
        {
            if (!t.IsArray) return null;

            string tName = t.FullName.Replace("[]", string.Empty);

            Type elType = t.Assembly.GetType(tName);

            return elType;
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

        /// <summary>
        /// 返回第一个匹配的结果
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static T GetControl<T>(this UIElementCollection collection) where T : FrameworkElement
        {
            if (collection ==null ||collection.Count == 0) return null;
            foreach (var item in collection)
            {
                if(item is T result)
                {
                    return result;
                }
            }
            return null;

        }

        public static void SetVisibility(this UIElementCollection collection, Visibility visibility = Visibility.Collapsed)
        {
            if (collection == null || collection.Count == 0) return;
            foreach (var item in collection)
            {
                if (item is FrameworkElement element)
                {
                   if (element.Visibility != visibility)  element.Visibility = visibility;
                }
            }
        }

        public static List<ChartBasicInfo> GetChartBasicInfos(this DataTable data)
        {
            List<ChartBasicInfo> result = null;
            if (data.Columns.Count > 0 && data.Rows.Count > 0 && data.Rows.Count == 1)
            {
                result = new List<ChartBasicInfo>();
                for (int c = 0; c < data.Columns.Count; c++)
                {
                    result.Add(new ChartBasicInfo()
                    {
                        Name = data.Columns[c].ColumnName,
                        Value = double.Parse((data.Rows[0].ItemArray[c]).ToString())
                    });
                }
            }
            return result;
        }
    }
}
