using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonApi 
{
    public class JsonHelper
    { 

        //public static void WriteJson<T>(T target ,string path  )
        //{
        //    if (!File.Exists(path))
        //    {
        //        File.Create(path);
        //    }
        //    var json = JsonConvert.SerializeObject(target, Formatting.Indented);
        //    File.AppendAllText(path, json);
        //}

        public static void WriteJson<T>(IList<T> targets,string path  )
        {
            if (!File.Exists(path))
            {
                File.Create(path);
            }
            StringBuilder sb = new StringBuilder();
            //foreach (var target in targets)
            //{
                sb.Append(JsonConvert.SerializeObject(targets, Formatting.Indented));
            //}
            File.WriteAllText(path,sb.ToString());
        }

        //public static T ReadJson<T>(string path  )
        //{ 
        //    if (!File.Exists(path))
        //    {
        //        throw new Exception($"未找到对应路径文件,path'{path}'");
        //    }

        //    var txt=    File.ReadAllText(path); 
        //    var json = JsonConvert.DeserializeObject<T>(txt);
        //    return json;
        //}

        public static IList<T> ReadJson<T>(string path)
        {
            if (!File.Exists(path))
            {
                throw new Exception($"未找到对应路径文件,path'{path}'");
            }
            var txt = File.ReadAllText(path);
            var json = JsonConvert.DeserializeObject<IList<T>>(txt);
            return json;
        }
    }
}
