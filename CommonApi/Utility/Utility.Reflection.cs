using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommonApi.Attributes;

namespace CommonApi
{
    public static partial class Utility
    {
        public static class Reflection
        {
            /// <summary>
            /// 获取超类下所有不包含抽象类的子类名称
            /// </summary>
            /// <typeparam name="T">超类</typeparam>
            /// <returns></returns>
            public static IEnumerable<string> GetChildrenNames<T>() where T : class
            {
                return typeof(T).Assembly.GetTypes()
                    .Where(a => !a.IsAbstract && typeof(T).IsAssignableFrom(a))
                    .Select(a => a.Name);
            }
            /// <summary>
            /// 获取超类下所有不包含抽象类的子类全部名称，包括命名空间，不包括程序集名称
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <returns></returns>
            public static IEnumerable<string> GetChildrenFullNames<T>() where T : class
            {
                return typeof(T).Assembly.GetTypes()
                    .Where(a => !a.IsAbstract && typeof(T).IsAssignableFrom(a))
                    .Select(a => a.FullName);
            }

            /// <summary>
            /// 创建实例(FullName) 包括命名空间,但不包括程序集名称.
            /// </summary>
            /// <typeparam name="T">实例类型</typeparam>
            /// <param name="fullName">实例的全名称</param>
            /// <param name="para">对象实例参数</param>
            /// <returns></returns>
            public static T CreateObject<T>(string fullName, params object[] para) where T : class
            {
                try
                {
                    var te = typeof(T).Assembly.GetTypes().Where(a => a.FullName == fullName).FirstOrDefault(); //反射入口
                    if (te == null)
                    {
                        throw new ArgumentNullException($" '{fullName}' 未找到实例！");
                    }
                    return Activator.CreateInstance(te, para) as T;//创建实例
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            public static object CreateObject(Type type, string fullName, params object[] para)
            {
                try
                {
                    var te = type.Assembly.GetTypes().Where(a => a.FullName == fullName).FirstOrDefault();
                    if (te == null)
                    {
                        throw new ArgumentNullException($" '{fullName}' 未找到实例！"); 
                    }
                    return Activator.CreateInstance(te, para);
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }

            

            /// <summary>
            /// 创建实例, 类名(shortName),但是是T类型的子类,不包括抽象类
            /// <para>建议使用这个<see cref="CreateObject"/></para>
            /// </summary>
            /// <typeparam name="T">超类</typeparam>
            /// <param name="shortName">子类的类名(不包括抽象类)</param>
            /// <param name="objects">构造参数</param>
            /// <returns></returns>
            public static T CreateObjectShortName<T>(string shortName, params object[] objects) where T : class
            {
                try
                {
                    var result = typeof(T).Assembly.GetTypes()
                      .Where(a => !a.IsAbstract && a.Name == shortName).FirstOrDefault();
                    if (result == null)
                    {
                        throw new ArgumentNullException($"'{typeof(T).FullName}'未找到对应的名称'{shortName}'实现,实现是不包括抽象!");
                    }
                    return CreateObject<T>(result.FullName, objects); 
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            /// <summary>
            /// 判断该类型是否继承了<see cref="System.Collections.IList "/>
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException"></exception>
            public static bool IsList(Type type)
            {
                if (null == type)
                    throw new ArgumentNullException("type");

                if (typeof(System.Collections.IList).IsAssignableFrom(type))
                    return true;
                foreach (var it in type.GetInterfaces())
                    if (it.IsGenericType && typeof(IList<>) == it.GetGenericTypeDefinition())
                        return true;
                return false;
            }

            public static object CreateObjectShortName(Type type ,string shortName,params object[] objects)
            {
                try
                {
                    var result=  type.Assembly.GetTypes().Where(a => !a.IsAbstract && a.Name == shortName).FirstOrDefault();
                    if (result == null)
                    {
                        throw new ArgumentNullException($"'{type.FullName}'未找到对应的名称'{shortName}'实现,实现是不包括抽象!");
                    }
                    return CreateObject(type, result.FullName, objects);
                }   
                catch (Exception ex)
                {
                    throw ex;
                    throw;
                }
            }

        
            /// <summary>
            /// 创建指定类型实例,并且规定类型继承于 T1,且有添加对应的特性T2.
            /// </summary>
            /// <typeparam name="T"> 创建的类型</typeparam>
            /// <typeparam name="T1">超类(父类,接口,抽象类)</typeparam>
            /// <typeparam name="T2">继承于<see cref="ConfigBaseAttribute"/>的特性</typeparam>
            /// <param name="className">名称与属性'<see cref="BaseAttribute.Name"/>'一致</param>
            /// <returns></returns>
            /// <exception cref="ArgumentNullException"> </exception>
            public static T CreateInstance<T, T1, T2>(string className) where T : class, new() where T2 : BaseAttribute
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
                if (result == null)
                {
                    throw new Exception($"未找到对应的类型  'T:{typeof(T).Name}' 'T1:{typeof(T1).Name}' 'T2:{typeof(T2).Name}' ");
                }
                return Activator.CreateInstance(result) as T;
            }
            /// <summary>
            /// 获取对应超类继承的对象和添加的对应特性的所有类型名称 
            /// </summary>
            /// <typeparam name="T1">超类(父类,接口,抽象类)</typeparam>
            /// <typeparam name="T2">继承于<see cref="BaseAttribute"/>的特性</typeparam>
            /// <returns>返回所有子类的标签的名称'<see cref="BaseAttribute.Name"/>'</returns>
            internal static IEnumerable<string> GetCfgNames<T1, T2>() where T2 : BaseAttribute
            {
                return typeof(T1).Assembly.GetTypes()
                    .Where(a => typeof(T1).IsAssignableFrom(a)).
                    Where(a => a.GetCustomAttribute<T2>() != null)
                    .Select(a => a.GetCustomAttribute<T2>().Name);
            }



            /// <summary>
            ///  获取指定类型所有继承者
            ///  <para>这里用了递归，按照父类再进行递归创建对应的控件</para>
            /// </summary>
            /// <param name="target">对象类型</param>
            /// <param name="inList">存入的集合</param>
            /// <param name="father"></param>
            //public static void GetInheritors(Type target, ref List<ClassData> inList, ClassData father = null)
            //{
            //    if (inList == null)
            //    {
            //        inList = new List<ClassData>();
            //    }

            //    var orgin = Type.GetType(target.FullName);
            //    //获取所有target的继承者,且只获取直接继承的类型
            //    var types = orgin.Assembly.GetTypes()
            //        .Where(a => orgin.IsAssignableFrom(a)
            //                    && a != target
            //                    && a.BaseType == orgin).ToList();

            //    ClassData ac = new ClassData();
            //    ac.ClassType = orgin;
            //    ac.Father = father;
            //    ac.ChildrenTypes = types.Where(a => !a.IsAbstract).ToList(); //获取所有非继承抽象  
            //    inList.Add(ac);
            //    //获取所有抽象的继承类型
            //    var childrenType = types.Where(a => a.IsAbstract && a != orgin).ToList();
            //    if (childrenType.Count == 0)
            //    {
            //        return;
            //    }
            //    else
            //    {
            //        foreach (var type in childrenType)
            //        {
            //            ClassData children = new ClassData()
            //            {
            //                Father = ac,
            //                ClassType = type,
            //            };
            //            GetInheritors(type, ref inList, ac);
            //        }
            //    }
            //}


            /// <summary>
            /// 获取类型所有的直接继承的类型，对[抽象类]再递归获取
            /// <para>使用的话:用递归，遍历<see cref="ClassData.ChildrenTypes"/> 判断<see cref="Type.IsAbstract"/>进行递归</para>
            /// </summary>
            /// <param name="target">目标对象</param>
            /// <param name="inData"></param>
            public static ClassData GetClassData<T>() where T : class
            { 
                ClassData cd = new ClassData();
                GetInheritors(typeof(T), ref cd);
                return cd;
            }
            /// <summary>
            /// 获取类型所有的直接继承的类型，对[抽象类]再递归获取
            /// <para>使用的话:用递归，遍历<see cref="ClassData.ChildrenTypes"/> 判断<see cref="Type.IsAbstract"/>进行递归</para>
            /// </summary>
            /// <param name="target">目标对象</param>
            /// <param name="inData"></param>
            public static ClassData GetClassData(Type t)
            {
                if (t == null) return null; 
                ClassData cd = new ClassData();
                GetInheritors(t, ref cd);
                return cd;
            }

            /// <summary>
            /// 获取该类所有实现类的全名称
            /// </summary>
            /// <param name="t">对应类的类型</param>
            /// <returns></returns>
            public static List<string> GetClassDataFullName(Type t)
            {
                if(t == null) return null;
                ClassData cd = new ClassData();
                GetInheritors(t, ref cd);
                return GetAllImplement(cd);
            }

            private static List<string> GetAllImplement(ClassData cd, List<string> classFullName = null)
            {
                if (cd == null) return null;
                if (classFullName == null) classFullName = new List<string>(); 
                if (cd.ClassType.IsAbstract)
                {
                    foreach (var item in cd.ChildrenTypes)
                    {
                        if (item.IsAbstract)
                        {
                            var result = GetAllImplement(cd.Children.Where(a => a.ClassType == item).FirstOrDefault(), classFullName);
                            if (result == null)
                            {
                                classFullName.AddRange(result);
                            }
                        }
                        else
                        {
                            classFullName.Add(item.FullName);
                        }
                    }
                }
                else
                {
                    classFullName.Add(cd.ClassType.FullName);
                }
                return classFullName;
            }
            /// <summary>
            /// 获取类型所有的直接继承的类型，对[抽象类]再递归获取
            /// <para>使用的话:用递归，遍历<see cref="ClassData.ChildrenTypes"/> 判断<see cref="Type.IsAbstract"/>进行递归</para>
            /// </summary>
            /// <param name="target">目标对象</param>
            /// <param name="inData"></param>
            public static void GetInheritors(Type target, ref  ClassData inData  ,int layerindex =0)
            {
                if (inData == null) return;
              
                //获取所有target的继承者,且只获取直接继承的类型
                var types = target.Assembly.GetTypes()
                    .Where(a => target.IsAssignableFrom(a)
                                && a != target
                                && a.BaseType == target).ToList();
                inData.LayerIndex += layerindex == 0 ? 1: layerindex;
                inData.ClassType = target;
                inData.Father = inData;
                //获取直接继承的类
                inData.ChildrenTypes = types.ToList(); 
           
                //获取所有抽象的继承类型
                var childrenType = types.Where(a => a.IsAbstract && a != target).ToList();
                if (childrenType.Count == 0)
                {
                    return;
                }
                else
                {
                    foreach (var type in childrenType)
                    {
                        ClassData children = new ClassData()
                        {
                            Father = inData,
                            ClassType = type,
                        }; 
                        GetInheritors(type, ref children, inData.LayerIndex+1);
                        inData.Children.Add(children);
                    }
                }
            }
        }

       
    }
    /// <summary>
    /// 类的数据
    /// <para>存入的是这个超类的下所有子类的类型数据</para>
    /// <para>以直接继承为条件,抽象类再分一次</para>
    /// </summary>
    public sealed  class ClassData
    {
        /// <summary>
        /// 层级索引
        /// </summary>
        public int LayerIndex { get; set; }
        /// <summary>
        /// 当前类的类型
        /// </summary>
        public Type ClassType { get; set; } 
        /// <summary>
        /// 当前类的父类
        /// </summary>
        public ClassData Father { get; set; }

        /// <summary>
        /// 所有再次抽象的类
        /// </summary>
        public List<ClassData> Children { get; set; } = new List<ClassData>();
        /// <summary>
        /// 当前类被直接继承的子类集合
        /// </summary>
        public List<Type> ChildrenTypes { get; set; }
    }
}
