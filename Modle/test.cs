using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Modle
{
    public class test
    {
   
        /// <summary>
        /// 获取对应超类继承的对象和添加的对应特性的所有类型名称 
        /// </summary>
        /// <typeparam name="T1">超类(父类,接口,抽象类)</typeparam>
        /// <typeparam name="T2">继承于<see cref="ConfigBaseAttribute"/>的特性</typeparam>
        /// <returns></returns>
        public IEnumerable<string > GetCfgNames<T1,T2>()  where T2 : ConfigBaseAttribute
        {
            return typeof(T1).Assembly.GetTypes()
                .Where(a => typeof(T1).IsAssignableFrom(a)).
                Where(a => a.GetCustomAttribute<T2>() != null)
                .Select(a => a.GetCustomAttribute<T2>().Name);
        }

        public IEnumerable<string> GetChildenNames<T>() where T : class
        {
            return typeof(T).Assembly.GetTypes()
                .Where(a => !a.IsAbstract && typeof(T).IsAssignableFrom(a))
                .Select(a => a.Name);
        }
        public IEnumerable<string> GetChildenFullNames<T>() where T : class
        {
            return typeof(T).Assembly.GetTypes()
                .Where(a => !a.IsAbstract && typeof(T).IsAssignableFrom(a))
                .Select(a => a.FullName);
        }

        /// <summary>
        /// 创建指定类型实例,并且规定类型继承于 T1,且有添加对应的特性T2.
        /// </summary>
        /// <typeparam name="T"> 创建的类型</typeparam>
        /// <typeparam name="T1">超类(父类,接口,抽象类)</typeparam>
        /// <typeparam name="T2">继承于<see cref="ConfigBaseAttribute"/>的特性</typeparam>
        /// <param name="className"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"> </exception>
        public T CreateInstance<T,T1,T2>(string className) where T: class ,new() where T2 : ConfigBaseAttribute
        {
            if (string.IsNullOrEmpty(className))
            {
                throw new ArgumentNullException($"传入空的对应  className  '{className}'");
            }
            var result = typeof(T1).Assembly.GetTypes()
                 .Where(a => !a.IsAbstract)
                 .Where(a => typeof(T1).IsAssignableFrom(a))
                 .Where(a => a.GetCustomAttribute<T2>() != null)
                 .Where(a => a.GetCustomAttribute<T2>().Name == className)
                 .FirstOrDefault();
            if(result == null)
            {
                throw new Exception($"未找到对应的类型  'T:{typeof(T).Name}' 'T1:{typeof(T1).Name}' 'T2:{typeof(T2).Name}' ");
            } 
            return Activator.CreateInstance(result) as T;
        }

        /// <summary>
        /// 获取对应标签继承于<see cref="IDeviceConfig"/>的子类型的名称
        /// <para>这是快捷方法 方便调用而已</para>
        /// </summary>
        /// <typeparam name="T">继承于<see cref="ConfigBaseAttribute"/>的特性</typeparam>
        /// <returns></returns>
        public IEnumerable<string> GetDeviceConfigs<T>() where T : ConfigBaseAttribute
        { 
           return GetCfgNames<IDeviceConfig, T>();
        }
    }
}
