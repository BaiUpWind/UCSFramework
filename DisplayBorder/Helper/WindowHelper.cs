using CommonApi.Utilitys.Encryption;
using ControlHelper.Attributes;
using DeviceConfig;
using DisplayBorder.Controls;
using DisplayBorder.View;
using HandyControl.Controls;
using HandyControl.Tools;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using ComboBox = System.Windows.Controls.ComboBox;
using MessageBox = HandyControl.Controls.MessageBox;
using TextBox = System.Windows.Controls.TextBox;
using Window = System.Windows.Window;

namespace DisplayBorder
{
    /// <summary>
    /// 线程操作控件委托
    /// </summary>
    public  delegate void ThreadControlHandle();
    public static class WindowHelper
    {
        /// <summary>
        /// 生成一个组合框 创建对象实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="OnScusses"></param>
        public static void CreateComboBox<T>(Action<T> OnScusses,Window owner= null,params object[] para) where T : class
        {

            var cbw = SingleOpenHelper.CreateControl<ComboBoxLinked>();
            var window = new PopupWindow
            {
                PopupElement = cbw,
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                Owner = owner,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                MinWidth = 0,
                MinHeight = 0,
            };
            cbw.SetType<T>();
            cbw.OnEnter += () =>
            {
                try
                {
                    var result = cbw.GetType<T>(para);
                    if (result != null)
                    {
                        Growl.Info ($"创建对象实例成功'{result.GetType().Name}'");
                
                        OnScusses?.Invoke(result);
                    }
                    else
                    {
                        Growl.Error ("创建对象实例 失败 未知原因?");
                    }
                    window.Close();
                }
                catch (Exception ex)
                {
                    Growl.Error ( $"创建对象实例 失败 未知原因'{ex.InnerException}'" );
                }
            };
            cbw.OnCancel += () =>
            {
                window.Close();
            };
            window.ShowDialog(window, false);
        }
        public static void CreateComboBox(Type type,Action<object> OnScusses,Window owner = null,params object[] para)
        {
            var cbw = SingleOpenHelper.CreateControl<ComboBoxLinked>();
            var window = new PopupWindow
            {
                PopupElement = cbw,
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                Owner = owner,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                MinWidth = 0,
                MinHeight = 0,
            };
            cbw.SetType (type);
            cbw.OnEnter += () =>
            {
                try
                {
                    var result = cbw.GetType (type,para);
                    if (result != null)
                    {
                        Growl.Info($"创建对象实例成功'{result.GetType().Name}'");

                        OnScusses?.Invoke(result);
                    }
                    else
                    {
                        Growl.Error("创建对象实例 失败 未知原因?");
                    }
                    window.Close();
                }
                catch (Exception ex)
                {
                    Growl.Error($"创建对象实例 失败 未知原因'{ex.InnerException}'");
                }
            };
            cbw.OnCancel += () =>
            {
                window.Close();
            };
            window.ShowDialog(window, false);
        }
        #region 创建用户控件
         
        /// <summary>
        /// 创建一个类型,自动根据类型中的字段特性[<see cref="ControlAttribute"/>]创建对应的控件
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <typeparam name="C">编辑的控件</typeparam>
        /// <param name="OnScussec">当创建成功时返回对应的类型</param>
        /// <param name="orginTarget">自己创建的类</param>
        /// <param name="ownerWindow">控件出现在的父类窗口</param>
        /// <param name="para">类创建的所需参数</param>
        /// <exception cref="Exception">初始化失败,创建失败</exception>
        public static void CreateControl<T,C>( Action<T> OnScussec,T orginTarget =null, Window ownerWindow = null ,params object[] para) where T:class,new() where C : FrameworkElement, ISingleOpen, IControlHelper, new()
        {
            var control = SingleOpenHelper.CreateControl<C>();
            var window = new PopupWindow
            {
                PopupElement = control,
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                Owner = ownerWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                MinWidth = 0,
                MinHeight = 0,
             
            };

            if (orginTarget == null)
            {
                orginTarget = (T)Activator.CreateInstance(typeof(T), para);
                if (orginTarget == null)
                {
                    throw new Exception($"初始化'{typeof(T)}'失败!");
                }
            } 
            control.CreateType( orginTarget.GetType(), orginTarget); 
            control.OnEnter += (obj) =>
            { 
                if(obj is T t)
                {
                    OnScussec?.Invoke(t);
                }
                else
                {
                    throw new Exception($"类型'{typeof(T)}'创建失败!");
                }

                window.Close();
            };

            control.OnCancel += (obj) =>
            {

                window.Close();
            };

            window.ShowDialog(window, false);
        }
        public static void CreateControl<C>(Type target,Action<object> OnScussec,object orginTarget =null, Window ownerWindow = null, params object[] para) where C : FrameworkElement, ISingleOpen, IControlHelper, new()
        {
            var control = SingleOpenHelper.CreateControl<C>();
            var window = new PopupWindow
            {
                PopupElement = control,
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                Owner = ownerWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                MinWidth = 0,
                MinHeight = 0,

            };

            if (orginTarget == null)
            {
                orginTarget =  Activator.CreateInstance(target, para);
                if (orginTarget == null)
                {
                    throw new Exception($"初始化'{target}'失败!");
                }
            }
            control.CreateType(orginTarget.GetType(), orginTarget);

            control.OnEnter += (obj) =>
            {
                if (obj.GetType() == target)
                {
                    OnScussec?.Invoke(obj);
                }
                else
                {
                    throw new Exception($"类型'{target}'创建失败!");
                }

                window.Close();
            };

            control.OnCancel += (obj) =>
            {

                window.Close();
            };

            window.ShowDialog(window, false);
        }

        /// <summary>
        /// 创建一个显示datagrid的窗体
        /// <para>支持的数据格式[DataTable][List<T>]</para>
        /// </summary>
        /// <typeparam name="C"></typeparam>
        /// <param name="data">显示的数据</param>
        /// <param name="OnSet">当赋予数据时发生的事件</param>
        /// <param name="OnClose">但窗体关闭时发生</param>
        /// <param name="ownerWindow">属于的父窗体</param>
        public static void CreateDataGridControl<C>(object data, Window ownerWindow = null,
            Action<object> OnSet = null,
            Action OnClose = null) where C : FrameworkElement, IControlData, ISingleOpen, new()
        {
            var control = SingleOpenHelper.CreateControl<C>();
            var window = new PopupWindow
            {
                PopupElement = control,
                AllowsTransparency = true,
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                Owner = ownerWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                MinWidth = 0,
                MinHeight = 0,
            };
            control.OnClose += () =>
            {
                window.Close();
                OnClose?.Invoke();
            };
            control.OnSet += (b) =>
            {
                OnSet?.Invoke(b);
            };
            control.SetData(data);
            window.ShowDialog(window, false);
        }

        #endregion
        #region 创建window 
        /// <summary>
        /// 创建一个窗体,自动根据类型中的字段特性[<see cref="ControlAttribute"/>]创建对应的控件
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <typeparam name="W">窗体</typeparam>
        /// <param name="OnScussec"></param>
        /// <param name="orginTarget"></param>
        /// <param name="titleName"></param>
        /// <param name="para"></param>
        /// <exception cref="Exception"></exception>
        public static void CreateWindow<T, W>(Action<T> OnScussec, T orginTarget = null, string titleName = null, params object[] para) where T : class, new() where W : Window, IControlHelper, new()
        {
            W window = WindowProperty<W>(titleName);

            if (orginTarget == null)
            {
                orginTarget = (T)Activator.CreateInstance(typeof(T), para);
                if (orginTarget == null)
                {
                    throw new Exception($"初始化'{typeof(T)}'失败!");
                }
            }
            window.CreateType(orginTarget.GetType(), orginTarget);
            window.OnEnter += (obj) =>
            {
                if (obj is T t)
                {
                    OnScussec?.Invoke(t);
                }
                else
                {
                    throw new Exception($"类型'{typeof(T)}'创建失败!");
                }

                window.Close();
            };

            window.OnCancel += (obj) =>
            {

                window.Close();
            };

            window.ShowDialog();

        }
        public static void CreateWindow<W>(Type target, Action<object> OnScussec , object orginTarget = null,string titleName =null,Window oldWindow =null,   params object[] para) where W :  Window, IControlHelper ,new()
        {
            W window = WindowProperty<W>(titleName);
            if (orginTarget == null)
            {
                orginTarget = Activator.CreateInstance(target, para);
                if (orginTarget == null)
                {
                    throw new Exception($"初始化'{target}'失败!");
                }
            }

            window.CreateType(orginTarget.GetType(), orginTarget);
            window.OnEnter += (obj) =>
            {
                if (obj.GetType() == orginTarget.GetType())
                {
                    OnScussec?.Invoke(obj);
                }
                else
                {
                    throw new Exception($"类型'{target}'创建失败!");
                }

                window.Close();
            };

            window.OnCancel += (obj) =>
            {
              
                window.Close();
            };
            window.Closed += (s, e) =>
            {
                if (oldWindow != null)
                {

                    //if ((bool)oldWindow.DialogResult)
                    //{
                        oldWindow.ShowDialog();
                    //}
                    //else
                    //{
                    //    oldWindow.Show();
                    //}
                }
            };
            oldWindow?.Hide();
            window.ShowDialog(); 
        }

        public static void CreateDataGirdWindow<W>(object data,Action<object> OnSet, Action OnClose = null, string titleName = null) where W : Window, IControlData, new()
        {
            W window = WindowProperty<W>( titleName) ;

            window.OnClose += () =>
            {
                window.Close();
                OnClose?.Invoke();
            };
            window.OnSet += (b) =>
            {
                OnSet?.Invoke(b);
            };
            window.SetData(data);
            window.ShowDialog( );
        }

        /// <summary>
        /// 创建一个DataGrid窗口,带操作的
        /// </summary> 
        public static void CreateDataGirdWindow<W>(object data,string titleName =null,Window oldWindow =null,Action onClose =null) where W : Window, IControlData, new()
        {
            W window = WindowProperty<W>(titleName);
            window.OnClose += () =>
            {
                window.Close(); 
            };
            window.Closed += (s, e) =>
            {
                onClose?.Invoke();
                if (oldWindow != null)
                {

                    //if ((bool)oldWindow.DialogResult)
                    //{
                        oldWindow.ShowDialog();
                    //}
                    //else
                    //{
                    //    oldWindow.Show();
                    //}
                }
            };
            oldWindow?.Hide();
            window.OnlyWacth = false;
            window.SetData(data);
            window.ShowDialog();

        }

        private static W WindowProperty<W>(string titleName = null) where W : Window, new()
        {
            var window = new W
            {
                MinHeight = 400,
                MinWidth = 300,
                Width = 1200,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };
            window.Title = (titleName ?? window.Title);
            return window;
        }
        #endregion 
        #region 根据特性生成对应的控件 
        /// <summary>
        /// 自动根据类型中的字段特性[<see cref="ControlAttribute"/>]创建对应的控件
        /// </summary> 
        public static void CreateControl(Type targetType, object target, Panel container, Window ownerWindow = null)
        {
            if (target == null) return;
            Type objType = targetType;

            var properties = targetType.GetProperties().ToList();
             //var resul  =  properties
             //       .Where(a => a.GetCustomAttributes(typeof(ControlAttribute)).Count() != 0)
             //       .Select(a => a.GetCustomAttributes(typeof(ControlAttribute)).ToList()) 
             //       .OrderBy(a=> ((ControlAttribute)a[0]).Order ).ToList();

             //todo:根据order再排序

            foreach (PropertyInfo propInfo in properties)
            {
                //Console.WriteLine(propInfo.Name);
                //当前这个类
                if (!propInfo.DeclaringType.IsPublic) continue;

                var objAttrs = propInfo.GetCustomAttributes(typeof(ControlAttribute), true);
                    //.Select(a=> (ControlAttribute)a)
                    //.OrderBy(a=> a.Order).ToList();  
                if (objAttrs.Length > 0)
                {
                    StackPanel sp = new StackPanel();
                    Border bod = new Border();
                    bod.BorderBrush = new SolidColorBrush(Colors.White);
                    bod.BorderThickness = new Thickness(1);
                    bod.CornerRadius =  new CornerRadius(10);
                    bod.Margin = new Thickness(5);
                    bod.Child = sp;
                    //存放当前生成的元素
                    List<UIElement> listElements = new List<UIElement>();
                    foreach (var att in objAttrs)
                    {
                        if (att is ControlAttribute attr)
                        {
                            if (!string.IsNullOrEmpty(attr.LabelName))
                            {
                                sp.Margin = new Thickness(18);
                                TextBlock textBlock = new TextBlock();
                                textBlock.Text = attr.LabelName+$"({propInfo.PropertyType.Name})";
                                textBlock.FontSize = 21;
                                sp.Children.Add(textBlock);
                            }
                            switch (attr.ControlType)
                            {
                                case ControlType.TextBox:
                                case ControlType.PassWord:
                                    listElements.Add( TextBox(target, objType, propInfo, sp, attr));
                                    break;
                                case ControlType.ComboBox:
                                    listElements.Add(ComboBox(target, objType, propInfo, sp, attr));
                                    break;
                                case ControlType.Button:
                                    listElements.Add(Button(targetType, target, sp, attr));
                                    break;
                                case ControlType.FilePathSelector:
                                    FileSelector(target, ownerWindow, objType, properties,  attr, listElements);
                                    break;
                                case ControlType.ColorPicker: 
                                    ColorPicker(target, ownerWindow, objType, properties, propInfo, sp, attr);
                                    break;
                                case ControlType.Genericity:
                                    Genericity(target, ownerWindow, objType, properties, sp, attr);
                                    break;
                                case ControlType.Data:
                                    Data(target, objType, properties, sp, attr,ownerWindow);
                                    break;
                                case ControlType.Collection:
                                    Type collTaget = null;
                                    foreach (var item in propInfo.PropertyType.GetInterfaces())
                                    {
                                        if (item.Name == typeof(ICollection<>).Name && propInfo.Name == attr.FieldName)
                                        {
                                            collTaget = item;
                                            break;
                                        }
                                    }
                                    Button btnColl = new Button();
                                    btnColl.HorizontalAlignment = HorizontalAlignment.Left;
                                    btnColl.Margin = new Thickness(5);
                                 
                                    btnColl.ToolTip = "点击编辑集合内容";
                                    var modelList = propInfo.GetValue(target);
                                    if (modelList == null)
                                    {
                                        //获取这个集合的泛型类型
                                        Type GenericType =null;
                                        if (collTaget == null) 
                                        {
                                            GenericType = attr.GenerictyType;
                                        }
                                        else
                                        {
                                            GenericType = collTaget.GenericTypeArguments[0];
                                        }
                                        modelList = Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { GenericType }));
                                        propInfo.SetValue(target, modelList);
                                    }
                                    btnColl.Content = $"集合'{propInfo.Name}'[{((ICollection)modelList).Count}]";
                                    btnColl.Click += (s, e) =>
                                    {
                                        CreateDataGirdWindow<DataGirdWindow>(modelList, $"编辑集合'{propInfo.Name}'",ownerWindow);
                                    };
                                    sp.Children.Add(btnColl);
                                    #region 老的


                                    //Type collTaget = null;
                                    //foreach (var item in propInfo.PropertyType.GetInterfaces())
                                    //{
                                    //    if(item.Name == typeof(ICollection<>).Name && propInfo.Name ==attr.FieldName)
                                    //    {
                                    //        collTaget = item;
                                    //    }
                                    //}
                                    //var arg =   propInfo.PropertyType.GetGenericArguments()[0];
                                    //StackPanel sp2 = new StackPanel();
                                    //sp2.Orientation = Orientation.Horizontal;

                                    //Button btnAddEle = new Button();
                                    //btnAddEle.HorizontalAlignment = HorizontalAlignment.Left;
                                    //btnAddEle.Margin = new Thickness(5); 
                                    //var modelList = propInfo.GetValue(target);
                                    //if (modelList == null)
                                    //{
                                    //    modelList = Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { collTaget.GenericTypeArguments[0] }));
                                    //    propInfo.SetValue(target,modelList);
                                    //}
                                    //btnAddEle.Content = $"添加元素[{((ICollection)modelList).Count}]";
                                    //if (arg != null)
                                    //{
                                    //    btnAddEle.Click += (s, e) =>
                                    //    {

                                    //        CreateWindow<DataWindow>(arg, 
                                    //        (o) =>
                                    //        {
                                    //            if (collTaget.GenericTypeArguments[0] == o.GetType())
                                    //            {
                                    //                collTaget.GetMethod("Add").Invoke(modelList, new object[] { o });
                                    //                btnAddEle.Content = $"添加元素[{((ICollection)modelList).Count}]";
                                    //            } 
                                    //        },  
                                    //        null,"添加元素");

                                    //    };
                                    //} 

                                    //Button btnRemoveEle = new Button();
                                    //btnRemoveEle.HorizontalAlignment = HorizontalAlignment.Left;
                                    //btnRemoveEle.Margin = new Thickness(5);

                                    //sp2.Children.Add(btnAddEle);
                                    //sp2.Children.Add(btnRemoveEle);

                                    //sp.Children.Add(sp2);
                                    #endregion
                                    break;
                                case ControlType.CheckBox:
                                    CheckBox check = new CheckBox();
                                    check.Margin = new Thickness(5);
                                    check.HorizontalAlignment = HorizontalAlignment.Left;
                                    check.Tag = attr.Name; 

                                    check.VerticalContentAlignment = VerticalAlignment.Top;
                                    check.HorizontalContentAlignment = HorizontalAlignment.Left;
                                    if (attr.Width != 0) check.Width = attr.Width;
                                    if (attr.Height != 0) check.Height = attr.Height; 
                                    
                                    check.IsChecked = (bool)objType.GetProperty(propInfo.Name).GetValue(target, null) ;

                                    //根据值发生变化时 自动将值附上去
                                    check.Checked += (sender, e) =>
                                    { 
                                        if (sender is CheckBox c)
                                        { 
                                            if (c.Tag.ToString() == propInfo.Name)
                                            {  
                                                propInfo.SetValue(target, Convert.ChangeType(c.IsChecked, propInfo.PropertyType));
                                            }
                                        } 
                                    };
                                    check.Unchecked += (sender, e) =>
                                    {
                                        if (sender is CheckBox c)
                                        {
                                            if (c.Tag.ToString() == propInfo.Name)
                                            {
                                                propInfo.SetValue(target, Convert.ChangeType(c.IsChecked, propInfo.PropertyType));
                                            }
                                        }
                                    };
                                    sp.Children.Add(check);
                                    break;
                            }
                        }
                    }
                    listElements.Clear();
                    container.Children.Add(bod);
                }

            }


        }
        //private Regex GetCommonType(PropertyInfo info)
        //{
        //    TypeCode typeCode = Type.GetTypeCode(info.PropertyType);
        //}
      
        /// <summary>
        /// 生成一个数据窗体
        /// </summary> 
        private static void Data(object target, Type objType, List<PropertyInfo> properties, StackPanel sp, ControlAttribute attr,Window oldWindow)
        {
            PropertyInfo dataTaget = null;
            foreach (PropertyInfo pif in properties)
            {
                if (pif.Name == attr.FieldName)
                {
                    dataTaget = pif;
                }
            }
            Button btnData = new Button();
            btnData.Margin = new Thickness(5);
            btnData.HorizontalAlignment = HorizontalAlignment.Left;
            var data = objType.GetProperty(dataTaget.Name).GetValue(target, null);
          
            if(string.IsNullOrEmpty(attr.LabelName))
            {
                btnData.Content = attr.Name;
            }
            else
            {
                btnData.Content = data ?? "错误!";
            }
         
            btnData.IsEnabled = !objType.IsAbstract || data != null;
            if (btnData.IsEnabled)
            {
                btnData.Content =  attr.LabelName  ?? attr.Name;
                btnData.Click += (s, e) =>
                {

                    //重新获取一边
                    data = objType.GetProperty(dataTaget.Name).GetValue(target, null);
                    if (attr.GenerictyType == null) return;
                    CreateWindow<DataWindow>(attr.GenerictyType, (o) =>
                    {
                        dataTaget.SetValue(target, o);
                        btnData.Content = o.GetType().Name; 
                    }, data,$"数据类型'{dataTaget.Name}'",oldWindow: oldWindow);
                };
            }
            else
            {

                //获取当前字段的类型
                PropertyInfo fieldPi = null;
                foreach (PropertyInfo pi in properties)
                {
                    if (pi.Name == attr.FieldName)
                    {
                        fieldPi = pi;
                        break;
                    }
                }
                if (fieldPi != null  )
                {
                    btnData.IsEnabled = false;
                    btnData.Content = "等待创建";
                    Task.Run(async () =>
                    { 

                        while (true)
                        {
                            var value = fieldPi.GetValue(target);
                            if (value != null)
                            {
                                btnData.Click += (s, e) =>
                                {
                                    CreateWindow<DataWindow>(fieldPi.PropertyType, (o) =>
                                    {
                                        fieldPi.SetValue(target, o);
                                        btnData.Content = o.GetType().Name;
                                    }, value,$"数据类型'{fieldPi.Name}'",oldWindow: oldWindow);
                                };

                              await Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                               {
                                   btnData.Content = attr.Name ;
                                   btnData.IsEnabled = true;
                               }));

                                break;
                            }
                            await Task.Delay(500);
                        }
                    });
                }
            } 
            sp.Children.Add(btnData);
        }

        /// <summary>
        /// 生成一个泛型选择器
        /// </summary> 
        private static void Genericity(object target, Window ownerWindow, Type objType, List<PropertyInfo> properties, StackPanel sp, ControlAttribute attr)
        {
            PropertyInfo gTaget = null;
            foreach (PropertyInfo pif in properties)
            {
                if (pif.Name == attr.FieldName)
                {
                    gTaget = pif;
                }
            }
            Button btnCreateType = new Button();
            btnCreateType.Margin = new Thickness(5);
            btnCreateType.HorizontalAlignment = HorizontalAlignment.Left;
            var value = gTaget.GetValue(target);
            btnCreateType.Content = value == null ? $"选择'{gTaget.Name}'" : gTaget.GetValue(target); 
            btnCreateType.Click += (s, e) =>
            {
                CreateComboBox(attr.GenerictyType, (o) =>
                {
                    gTaget.SetValue(target, o);
                    btnCreateType.Content = o.GetType().Name ;
                }, ownerWindow);
            };

            sp.Children.Add(btnCreateType);
        }

        /// <summary>
        /// 生成一个颜色选择
        /// <para>本质还是一个按钮</para>
        /// </summary> 
        private static void ColorPicker(object target, Window ownerWindow, Type objType, List<PropertyInfo> properties, PropertyInfo propInfo, StackPanel sp, ControlAttribute attr)
        {
            PropertyInfo colorTaget = null;
            foreach (PropertyInfo pif in properties)
            {
                if (pif.Name == attr.FieldName)
                {
                    colorTaget = pif;
                }
            } 
            Button btnColor = new Button();
            btnColor.Margin = new Thickness(5);
            btnColor.HorizontalAlignment = HorizontalAlignment.Left;
            btnColor.Content = "选择";
            btnColor.Background = new SolidColorBrush((Color)objType.GetProperty(propInfo.Name).GetValue(target, null));
            btnColor.Click += (s, e) =>
            {
                var cp = SingleOpenHelper.CreateControl<ColorPicker>();
                var window = new PopupWindow
                {
                    PopupElement = cp,
                    AllowsTransparency = true,
                    WindowStyle = WindowStyle.None,
                    ResizeMode = ResizeMode.NoResize,
                    Owner = ownerWindow,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    MinWidth = 0,
                    MinHeight = 0,
                };
                //当选择颜色时
                cp.Confirmed += (cs, ce) =>
                {
                    if (colorTaget != null)
                    {
                        colorTaget.SetValue(target, ce.Info);
                        btnColor.Background = new SolidColorBrush((Color)objType.GetProperty(colorTaget.Name).GetValue(target, null));
                    }
                    window.Close();
                };
                //当取消时
                cp.Canceled += (cs, ce) =>
                {
                    window.Close();
                };
                window.ShowDialog(ownerWindow, false);
            };
            sp.Children.Add(btnColor);
        }

        /// <summary>
        /// 生成一个组合框
        /// </summary> 
        private static ComboBox ComboBox(object target, Type objType, PropertyInfo propInfo, StackPanel sp, ControlAttribute attr)
        {
            ComboBox cmb = new ComboBox();
            cmb.Margin = new Thickness(5);
            cmb.HorizontalAlignment = HorizontalAlignment.Left;
            cmb.Tag = attr.Name;
            cmb.Width = 258;
         
          
            //if (attr.Items != null)
            //{
            //    foreach (var cbItem in attr.Items)
            //    {
            //        cmb.Items.Add(cbItem);
            //    }
            //}
            //else 
            if(attr.EnumType != null)
            {
                foreach (var item in Enum.GetNames(attr.EnumType))
                {
                    cmb.Items.Add(item);
                } 
            }

            var result =  objType.GetProperty(propInfo.Name).GetValue(target, null)?.ToString();
            if ( result != null)
            {
                int index = 0;
                for (int j = 0; j < cmb.Items.Count; j++)
                {
                    var item = cmb.Items[j];
                    if (item.ToString() == result.ToString())
                    {
                        index = j;
                        break;
                    }
                }
                cmb.SelectedIndex = index;
            }
            else
            {
                cmb.SelectedIndex = 0;
            }

            cmb.SelectionChanged += (sender, e) =>
            {
                if (sender is ComboBox c)
                {
                    if (c.SelectedIndex <= -1) return;
                    if (c.Tag.ToString() == propInfo.Name)
                    {
                        propInfo.SetValue(target, c.SelectedIndex);
                        //var value = objType.GetProperty(propInfo.Name).GetValue(target, null)?.ToString(); 
                    }
                }
            };
            sp.Children.Add(cmb);

            return cmb;
        }
        /// <summary>
        /// 生成文本框
        /// </summary> 
        private static TextBox TextBox(object target, Type objType, PropertyInfo propInfo, StackPanel sp, ControlAttribute attr)
        {
            TextBox txtBox = new TextBox();
            txtBox.Margin = new Thickness(5);
            txtBox.HorizontalAlignment = HorizontalAlignment.Left;
            txtBox.Tag = attr.Name;
            txtBox.Width = 240;
            
            txtBox.VerticalContentAlignment=  VerticalAlignment.Top;
            txtBox.HorizontalContentAlignment = HorizontalAlignment.Left;
            if (attr.Width != 0) txtBox.Width = attr.Width;
            if (attr.Height != 0)  txtBox.Height = attr.Height ;
            bool isPD =false;
            var str = objType.GetProperty(propInfo.Name).GetValue(target, null)?.ToString();
            if (attr.ControlType == ControlType.PassWord)
            {
                isPD = true;
                str = Cryptography.DecryptString(attr.EncryptType, str);
                txtBox.MaxLength = attr.MaxLenght;
                
            } 
            txtBox.Text = str;
             
            //根据值发生变化时 自动将值附上去
            txtBox.TextChanged += (sender, e) =>
            { 
                try
                {
                    if (sender is TextBox t)
                    {
                        if (string.IsNullOrEmpty(t.Text)) return;
                        if (t.Tag.ToString() == propInfo.Name)
                        {
                            var inputxt = t.Text;
                            if (isPD)
                            {
                                inputxt = Cryptography.EncryptString(attr.EncryptType, inputxt);
                            }
                            //todo:这里需要根据类型获取对应的输入限制
                            propInfo.SetValue(target, Convert.ChangeType(inputxt, propInfo.PropertyType));
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Info($"请输入正确的类型'{objType}',\n\r 错误信息:'{ex.Message}' \r\n 内部信息:'{ex.InnerException.Message}'", "输入提示"); 
                    txtBox.Text = String.Empty;// $"正确的类型:'{propInfo.PropertyType.Name}'";
               
                }
           
            };

            sp.Children.Add(txtBox);
            return txtBox;
        }
        /// <summary>
        /// 在现有的TextBox上生成一个双击的文件选择器
        /// </summary> 
        private static void FileSelector(object target, Window ownerWindow, Type objType, List<PropertyInfo> properties,   ControlAttribute attr,List<UIElement> txtBoxs)
        {
            foreach (var item in txtBoxs)
            {
                if (item is TextBox txt)
                {
                    txt.IsReadOnly = true;
                    if (string.IsNullOrEmpty(txt.Text))
                    {
                        txt.Text = "双击选择文件";
                    }
                    //找到需要填充的属性/字段名称
                    PropertyInfo fileTarget = null;
                    foreach (PropertyInfo pif in properties)
                    {
                        if (pif.Name == attr.FieldName)
                        {
                            fileTarget = pif;
                        }
                    }

                    //双击事件
                    txt.MouseDoubleClick += (s, e) =>
                    {
                        OpenFileDialog dialog = new OpenFileDialog
                        {
                            InitialDirectory = GlobalPara.SysPath,
                            Multiselect = true,//该值确定是否可以选择多个文件
                            Title = "请选择文件",
                            Filter = "文件(*." + attr.FileType + ")|*." + attr.FileType + "|所有文件(*.*)|*.*"
                        };
                        if (dialog.ShowDialog(ownerWindow) == true)
                        {
                            if (fileTarget != null)
                            {
                                fileTarget.SetValue(target, dialog.FileName);
                                txt.ToolTip = txt.Text = objType.GetProperty(fileTarget.Name).GetValue(target, null)?.ToString();

                            }
                        }
                    }; 
                    txt.AddHandler(UIElement.MouseDownEvent, 
                    new System.Windows.Input.MouseButtonEventHandler((s,e) =>
                    {
                        if (e.ChangedButton == System.Windows.Input.MouseButton.Right)
                        { 
                            txt.Text = string.Empty;
                            txt.ToolTip = "双击选择文件路径！";
                        }
                    }), true); 
                }
            }
        }
    
        /// <summary>
        /// 生成一个按钮,带调用事件的按钮
        /// </summary> 
        private static Button Button(Type targetType, object target, StackPanel sp, ControlAttribute attr)
        {
            MethodInfo mif = null;
            foreach (MethodInfo method in targetType.GetMethods())
            {
                if (method.Name == attr.MethodName)
                {
                    mif = method;
                }
            }
            Button btnMethod = new Button();
            btnMethod.HorizontalAlignment = HorizontalAlignment.Left;
            btnMethod.Margin = new Thickness(5);
            btnMethod.Content = attr.Name;
            btnMethod.Click += (s, e) =>
            {
                if (mif != null)
                {
                    Task.Run(() =>
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            btnMethod.IsEnabled = false;
                        }));
                        var result = mif.Invoke(target, null);
                        if (result.GetType() == typeof(bool))
                        {
                            MessageBox.Show($"调用结果:{((bool)result ? "成功" : "失败")}");
                        }
                        else
                        {
                            MessageBox.Show($"不支持的结果:'{result}'");
                        }
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            btnMethod.IsEnabled = true;
                        }));

                    });
                }
                else
                {
                    MessageBox.Show($"未能找到对应的方法调用'{targetType}'.'{attr.MethodName}'");
                }
            };
            sp.Children.Add(btnMethod); ;
            return btnMethod;
        }

        public static TypeData[] GetTypeDatas(Type type, bool checkProp = true, object target = null)
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
                        if (attrs.Where(a => a is HideAttribute).Count() > 0)
                        {
                            continue;
                        }
                    }
                    TypeCode typeCode = Type.GetTypeCode(pi.PropertyType);
                    var convert = attrs.Where(a=> a is ConvertTypeAttribute).Select(a => a as ConvertTypeAttribute).ToList();
                    if (convert.Any())
                    {
                        typeCode = Type.GetTypeCode(convert[0].TargetType);
                    } 
                    Console.WriteLine($"当前属性名称'{pi.Name}',类型'{typeCode}'");
                    TypeData typeData = new TypeData()
                    {
                        Name = pi.Name,
                        TypeCode = typeCode,
                        ObjectType = pi.PropertyType
                    };
                    if(attrs.Count() > 0)
                    {
                        var size = attrs.Where(a => a is SizeAttribute).FirstOrDefault();
                        if (size != null && size is SizeAttribute wh)
                        {
                            typeData.Width = wh.Width;
                            typeData.Height = wh.Height;
                        }
                        var nickName = attrs.Where(a => a is NickNameAttribute).FirstOrDefault();
                        if(nickName != null && nickName is NickNameAttribute nna)
                        {
                            typeData.NickName = nna.NickName;
                            typeData.ToolTip = nna.ToolTip;
                        }
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
                    if(fileSelector.Count() > 0)
                    {
                        typeData.FileSelectorAttr = fileSelector.FirstOrDefault();
                    }

                    if (pi.PropertyType.IsAbstract)
                    {
                        if(attrs.Where(a=> a is InstanceAttribute).Count() > 0)
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
                        typeData.ControlType = ClassControlType.ComboBox; 
                    }
                    else if (typeCode == TypeCode.Object)
                    {  
                        if (attrs.Where(a => a is InstanceAttribute).Count() > 0)
                        {
                            typeData.ObjectType = pi.GetValue(target).GetType();
                        }

                        if (pi.PropertyType.Name == "Color")
                        {
                            typeData.ControlType = ClassControlType.ColorPicker;
                            continue;
                        }
                        typeData.IsList = CommonApi.Utility.Reflection.IsList(typeData.ObjectType);
                        typeData.IsObject = true;
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
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
              
                List<TypeData> typedatas = new List<TypeData>();
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldInfo field = fields[i];
                    var attrs = field.GetCustomAttributes();
                    if (attrs.Count() > 0)
                    {
                        if (attrs.Where(a => a is HideAttribute).Count() > 0)
                        {
                            continue;
                        }
                    }

                    TypeCode typeCode = Type.GetTypeCode(field.FieldType);
                    var convert = attrs.Where(a => a is ConvertTypeAttribute).Select(a => a as ConvertTypeAttribute).ToList();
                    if (convert.Any())
                    {
                        typeCode = Type.GetTypeCode(convert[0].TargetType);
                    }
                    Console.WriteLine($"当前字段名称'{field.Name}',类型'{typeCode}'");
                    var typeData = new TypeData()
                    {
                        Name = field.Name,
                        TypeCode = typeCode,
                        ObjectType = field.FieldType
                    };

                    var size = attrs.Where(a => a is SizeAttribute).First( );
                    if (size != null && size is SizeAttribute wh)
                    {
                        typeData.Width = wh.Width;
                        typeData.Height = wh.Height;
                    }
                    typedatas.Add(typeData);    
                    var but = attrs.Where(a => a is ButtonAttribute);
                    if (but.Count() > 0)
                    {
                        typedatas.Add(new TypeData()
                        {
                            ControlType = ClassControlType.Button,
                            ObjectType = field.FieldType,
                            Name = field.Name,
                            TypeCode = typeCode,
                            ButtonAttr = but.First()
                        });
                    }

                    if (field.FieldType.IsAbstract)
                    {
                        typeData.ControlType = ClassControlType.ComboBoxImplement; 
                    }
                    else if (field.FieldType.IsEnum)
                    {
                        typeData.ControlType = ClassControlType.ComboBox; 
                    }
                    else if (typeCode == TypeCode.Object)
                    {
                        if (attrs.Where(a => a is InstanceAttribute).Count() > 0)
                        {
                            typeData.ObjectType = field.GetValue(target).GetType();
                        }

                        if (field.FieldType.Name == "Color")
                        {
                            typeData.ControlType = ClassControlType.ColorPicker;
                            continue;
                        } 

                        { 
                            typeData.IsList = CommonApi.Utility.Reflection.IsList(field.FieldType);
                            typeData.IsObject = true;
                            //这里只用两种可能,因为只对未知的类型和集合类型进行拆解
                            typeData.ControlType = typeData.IsList ? ClassControlType.List : ClassControlType.Class;
                        } 
                        #region 创建对应的实例 好像没啥用....
                        ////创建实例
                        //object target = Activator.CreateInstance(type, para);
                        //var value = field.GetValue(target);
                        //if (islist)
                        //{
                        //    //对集合创建实例
                        //    if (value == null)
                        //    {
                        //        if (field.FieldType.IsGenericType)
                        //        {
                        //            if (field.FieldType.GenericTypeArguments.Length != 1)
                        //            {
                        //                throw new Exception("目前只对单个泛型集合进行处理");
                        //            }
                        //            typeData.IsGeneric = true;
                        //            typeData.GenericType = field.FieldType.GenericTypeArguments[0];
                        //            //泛型集合 只对单个泛型进行操作
                        //            var generics = typeof(List<>).MakeGenericType(field.FieldType.GenericTypeArguments);
                        //            value = Activator.CreateInstance(generics); 
                        //        }
                        //        else
                        //        {
                        //            //数组
                        //            value = Activator.CreateInstance(field.FieldType, 0);  
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    //对类型进行处理
                        //    if (value == null)
                        //    {
                        //        value = Activator.CreateInstance(field.FieldType);
                        //    }
                        //}
                        #endregion 
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
                td = typedatas.ToArray();//
            }
            return td;
        }
        #endregion

        #region 常用方法

        public static void SetWindowGrowl<T>(T window) where T :System.Windows.Window
        {
            if (window == null) return;
            window.SetValue(Growl.GrowlParentProperty, true);
            window. Activated += (s, e) =>
            {
                Growl.SetGrowlParent(window, true);
            };
            window. Deactivated += (s, e) => {

                Growl.SetGrowlParent(window, false);
            };
        }

        private static Dictionary<string, Window> dicCacheWindow = new Dictionary<string, Window>();
        public static Window GetSingleWindow<T>(/* bool init = false*/) where T : Window ,new()
        {
            string fullName = typeof(T).FullName;
            if (string.IsNullOrEmpty(fullName))
            {
                return null;
            }
            if(!dicCacheWindow.TryGetValue(fullName,out Window window) )//&& !init)
            {
                window = new T();
                dicCacheWindow.Add(fullName, window);
            }
            //if (init && window != null)
            //{
            //    window.Close();
            //    window = new T();
            //    dicCacheWindow.Remove(fullName);
            //    dicCacheWindow.Add(fullName, window); 
            //}
            window.Closing += (s, e) =>
            {
                window.Hide();
                e.Cancel = true;
            };

            return window;
        }


        public static void WindowShake(Window window = null)
        {
            if (window == null)
                if (Application.Current.Windows.Count > 0)
                    window = Application.Current.Windows.OfType<Window>().FirstOrDefault(o => o.IsActive);

            var doubleAnimation = new DoubleAnimation
            {
                From = window.Left,
                To = window.Left + 15,
                Duration = TimeSpan.FromMilliseconds(50),
                AutoReverse = true,
                RepeatBehavior = new RepeatBehavior(3),
                FillBehavior = FillBehavior.Stop
            };
            window.BeginAnimation(Window.LeftProperty, doubleAnimation);
            //var wavUri = new Uri(@"pack://application:,,,/WPFDevelopers;component/Resources/Audio/shake.wav");
            //var streamResource = Application.GetResourceStream(wavUri);
            //var player1 = new SoundPlayer(streamResource.Stream);
            //player1.Play();
        }

        #endregion
    }




    public class TypeData
    {
        /// <summary>
        /// 这个字段的名称属性或者名称
        /// </summary>
        public string Name;
        public TypeCode TypeCode = TypeCode.Empty;
        public ClassControlType ControlType;
        public bool IsObject;
        /// <summary>
        /// 是否为集合
        /// </summary>
        public bool IsList;
        public bool IsGeneric;
        public Type ObjectType;
        public Type GenericType; 
        public Attribute ButtonAttr;
        public Attribute FileSelectorAttr;
        public string NickName = string.Empty;
        public string ToolTip = string.Empty;
        public double Width = double.NaN;
        public double Height = double.NaN;
    }
    public enum ClassControlType
    {
        /// <summary>
        /// 文本框输入
        /// </summary>
        TextBox,
        /// <summary>
        /// 单选框输入
        /// </summary>
        CheckBox,
        /// <summary>
        /// 集合/数组 创建或者修改
        /// </summary>
        List,
        /// <summary>
        /// 类
        /// </summary>
        Class,
        /// <summary>
        /// 组合框(枚举)
        /// </summary>
        ComboBox,
        /// <summary>
        ///  超类的所有的实现,不包括超类
        /// </summary>
        ComboBoxImplement,
        /// <summary>
        /// 颜色选择器
        /// </summary>
        ColorPicker,
        /// <summary>
        /// 创建一个对指定方法名的调用按钮
        /// </summary>
        Button,
    }

}
