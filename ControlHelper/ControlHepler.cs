using ControlHelper.Attributes;
using ControlHelper.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ControlHelper
{
    public static class ControlHepler
    {
        internal static TypeData[] GetTypeDatas(Type type, bool checkProp = true, object target = null)
        {
            TypeData[] td;
            if (target != null)
            {
                type = target.GetType();
            }

            if (checkProp)
            {
                //获取公共的属性
                var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

                List<TypeData> typedatas = new List<TypeData>();

                for (int i = 0; i < props.Length; i++)
                {
                    PropertyInfo pi = props[i];
                    if (!pi.CanWrite)
                    {
                        continue;
                    }
                    var attrs = pi.GetCustomAttributes();
                    if (attrs.Count() > 0)
                    {
                        if (attrs.Where(a => a is HideAttribute).Any())
                        {
                            continue;
                        }
                    }
                    TypeCode typeCode = Type.GetTypeCode(pi.PropertyType); 
                    Console.WriteLine($"当前属性名称'{pi.Name}',类型'{typeCode}'");
                    TypeData typeData = new TypeData()
                    {
                        Name = pi.Name,
                        TypeCode = typeCode,
                        ObjectType = pi.PropertyType
                    };
                    if (attrs.Count() > 0)
                    {
                        var size = attrs.Where(a => a is SizeAttribute).FirstOrDefault();
                        if (size != null && size is SizeAttribute wh)
                        {
                            typeData.Width = wh.Width;
                            typeData.Height = wh.Height;
                        }
                        var nickName = attrs.Where(a => a is NickNameAttribute).FirstOrDefault();
                        if (nickName != null && nickName is NickNameAttribute nna)
                        {
                            typeData.NickName = nna.NickName;
                            typeData.ToolTip = nna.ToolTip;
                        }

                        var convert = attrs.Where(a => a is ConvertTypeAttribute).FirstOrDefault();
                        if (convert != null && convert is ConvertTypeAttribute cta)
                        {
                            typeCode = typeData.TypeCode  = Type.GetTypeCode(cta.TargetType);
                            typeData.ObjectType = cta.TargetType;
                        }
                        typeData.IsReadOnly = attrs.Where(a => a is ReadOnlyAttribute).Any();
                        
                    }

                    typedatas.Add(typeData);
                    //找到按钮属性 额外添加一个按钮
                    var but = attrs.Where(a => a is ButtonAttribute);
                    if (but.Count() > 0)
                    {
                        typedatas.Add(new TypeData()
                        {
                            ControlType = ClassControlType.Button,
                            ObjectType = pi.PropertyType,
                            Name = pi.Name,
                            TypeCode = typeCode,
                            ButtonAttr = but.First()
                        });
                    }
                    //获取文件选择器 有更优的方式
                    var fileSelector = attrs.Where(a => a is FileSelectorAttribute);
                    if (fileSelector.Count() > 0)
                    {
                        typeData.FileSelectorAttr = fileSelector.FirstOrDefault();
                    }
                    //如果是 抽象的
                    if (pi.PropertyType.IsAbstract)
                    {
                        if (attrs.Where(a => a is InstanceAttribute).Any())
                        {
                            typeData.ObjectType = pi.GetValue(target).GetType();
                            typeData.ControlType = ClassControlType.Class;
                        }
                        else
                        {
                            typeData.ControlType = ClassControlType.ComboBoxImplement;
                        }
                    }
                    else if (pi.PropertyType.IsEnum)
                    {
                        //枚举
                        typeData.ControlType = ClassControlType.ComboBox;
                    }
                    else if (typeCode == TypeCode.Object)
                    {
                        //基类
                        if (attrs.Where(a => a is InstanceAttribute).Any())
                        {
                            typeData.ObjectType = pi.GetValue(target).GetType();
                        }
                        if (pi.PropertyType.Name == "Color")
                        {
                            typeData.ControlType = ClassControlType.Empty;
                            continue;
                        }
                        typeData.IsList = Reflection.IsList(typeData.ObjectType);
                        typeData.UseDataGrid = typeData.IsList && attrs.Where(a => a is DataGridAttribute).Any();

                        //这里只用两种可能,因为只对未知的类型和集合类型进行拆解
                        typeData.ControlType = typeData.IsList ? ClassControlType.List : ClassControlType.Class;
                    }
                    else if (typeCode == TypeCode.Boolean)
                    {
                        typeData.ControlType = ClassControlType.CheckBox;
                    }
                    else
                    {
                        typeData.ControlType = ClassControlType.TextBox;
                    }
                }
                td = typedatas.ToArray();
            }
            else
            {
                throw new Exception("先别用字段的,获取字段跟属性一样一样,暂时把属性的搞完先,再搞字段的");
                //获取公共的字段
                #region 先不用

            
                //var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);

                //List<TypeData> typedatas = new List<TypeData>();
                //for (int i = 0; i < fields.Length; i++)
                //{
                //    FieldInfo field = fields[i];
                //    var attrs = field.GetCustomAttributes();
                //    if (attrs.Count() > 0)
                //    {
                //        if (attrs.Where(a => a is HideAttribute).Count() > 0)
                //        {
                //            continue;
                //        }
                //    }

                //    TypeCode typeCode = Type.GetTypeCode(field.FieldType);
                //    var convert = attrs.Where(a => a is ConvertTypeAttribute).Select(a => a as ConvertTypeAttribute).ToList();
                //    if (convert.Any())
                //    {
                //        typeCode = Type.GetTypeCode(convert[0].TargetType);
                //    }
                //    Console.WriteLine($"当前字段名称'{field.Name}',类型'{typeCode}'");
                //    var typeData = new TypeData()
                //    {
                //        Name = field.Name,
                //        TypeCode = typeCode,
                //        ObjectType = field.FieldType
                //    };

                //    var size = attrs.Where(a => a is SizeAttribute).First();
                //    if (size != null && size is SizeAttribute wh)
                //    {
                //        typeData.Width = wh.Width;
                //        typeData.Height = wh.Height;
                //    }
                //    typedatas.Add(typeData);
                //    var but = attrs.Where(a => a is ButtonAttribute);
                //    if (but.Count() > 0)
                //    {
                //        typedatas.Add(new TypeData()
                //        {
                //            ControlType = ClassControlType.Button,
                //            ObjectType = field.FieldType,
                //            Name = field.Name,
                //            TypeCode = typeCode,
                //            ButtonAttr = but.First()
                //        });
                //    }

                //    if (field.FieldType.IsAbstract)
                //    {
                //        typeData.ControlType = ClassControlType.ComboBoxImplement;
                //    }
                //    else if (field.FieldType.IsEnum)
                //    {
                //        typeData.ControlType = ClassControlType.ComboBox;
                //    }
                //    else if (typeCode == TypeCode.Object)
                //    {
                //        if (attrs.Where(a => a is InstanceAttribute).Count() > 0)
                //        {
                //            typeData.ObjectType = field.GetValue(target).GetType();
                //        }

                //        if (field.FieldType.Name == "Color")
                //        {
                //            typeData.ControlType = ClassControlType.Empty;
                //            continue;
                //        }

                //        {
                //            typeData.IsList = Reflection.IsList(field.FieldType);
                //            typeData.IsObject = true;
                //            //这里只用两种可能,因为只对未知的类型和集合类型进行拆解
                //            typeData.ControlType = typeData.IsList ? ClassControlType.List : ClassControlType.Class;
                //        }
                //        #region 创建对应的实例 好像没啥用....
                //        ////创建实例
                //        //object target = Activator.CreateInstance(type, para);
                //        //var value = field.GetValue(target);
                //        //if (islist)
                //        //{
                //        //    //对集合创建实例
                //        //    if (value == null)
                //        //    {
                //        //        if (field.FieldType.IsGenericType)
                //        //        {
                //        //            if (field.FieldType.GenericTypeArguments.Length != 1)
                //        //            {
                //        //                throw new Exception("目前只对单个泛型集合进行处理");
                //        //            }
                //        //            typeData.IsGeneric = true;
                //        //            typeData.GenericType = field.FieldType.GenericTypeArguments[0];
                //        //            //泛型集合 只对单个泛型进行操作
                //        //            var generics = typeof(List<>).MakeGenericType(field.FieldType.GenericTypeArguments);
                //        //            value = Activator.CreateInstance(generics); 
                //        //        }
                //        //        else
                //        //        {
                //        //            //数组
                //        //            value = Activator.CreateInstance(field.FieldType, 0);  
                //        //        }
                //        //    }
                //        //}
                //        //else
                //        //{
                //        //    //对类型进行处理
                //        //    if (value == null)
                //        //    {
                //        //        value = Activator.CreateInstance(field.FieldType);
                //        //    }
                //        //}
                //        #endregion
                //    }
                //    else if (typeCode == TypeCode.Boolean)
                //    {
                //        typeData.ControlType = ClassControlType.CheckBox;
                //    }
                //    else
                //    {
                //        typeData.ControlType = ClassControlType.TextBox;
                //    }
                //}
                //td = typedatas.ToArray();//
                #endregion
            }
            return td;
        }
    }
}
