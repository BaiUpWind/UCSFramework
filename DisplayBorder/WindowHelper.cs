using DeviceConfig;
using DisplayBorder.Controls;
using DisplayBorder.View;
using HandyControl.Controls;
using HandyControl.Tools;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

            control.OnCancel += () =>
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

            control.OnCancel += () =>
            {

                window.Close();
            };

            window.ShowDialog(window, false);
        }
       
        /// <summary>
        /// 自动根据类型中的字段特性[<see cref="ControlAttribute"/>]创建对应的控件
        /// </summary> 
        public static void CreateContrl(Type targetType, object target, Panel container, Window ownerWindow = null)
        {
            if (target == null) return;
            Type objType = targetType;

            var properties = targetType.GetProperties().ToList(); 
            foreach (var propInfo in properties)
            {
                Console.WriteLine(propInfo.Name);
                //当前这个类
                if (!propInfo.DeclaringType.IsPublic) continue;

                object[] objAttrs = propInfo.GetCustomAttributes(typeof(ControlAttribute), true);

                if (objAttrs.Length > 0)
                {
                    StackPanel sp = new StackPanel();
                    foreach (var att in objAttrs)
                    {
                        if (att is ControlAttribute attr)
                        {
                            if (!string.IsNullOrEmpty(attr.LabelName))
                            {
                                sp.Margin = new Thickness(18);
                                TextBlock textBlock = new TextBlock();
                                textBlock.Text = attr.LabelName;
                                textBlock.FontSize = 21;
                                sp.Children.Add(textBlock);
                            }
                            switch (attr.ControlType)
                            {
                                case ControlType.TextBox:
                                    TextBox txtBox = new TextBox();
                                    txtBox.Margin = new Thickness(5);
                                    txtBox.HorizontalAlignment = HorizontalAlignment.Left;
                                    txtBox.Tag = attr.Name;
                                    txtBox.Width = 240;
                                    txtBox.Text = objType.GetProperty(propInfo.Name).GetValue(target, null)?.ToString();
                                    //根据值发生变化时 自动将值附上去
                                    txtBox.LostFocus += (sender, e) =>
                                    {
                                        if (sender is TextBox t)
                                        {
                                            if (string.IsNullOrEmpty(t.Text)) return;
                                            if (t.Tag.ToString() == propInfo.Name)
                                            {
                                                propInfo.SetValue(target, Convert.ChangeType(t.Text, propInfo.PropertyType));
                                                //var value = objType.GetProperty(propInfo.Name).GetValue(target, null)?.ToString();

                                            }
                                        }
                                    };

                                    sp.Children.Add(txtBox);
                                    break;
                                case ControlType.ComboBox:
                                    ComboBox cmb = new ComboBox();
                                    cmb.Margin = new Thickness(5);
                                    cmb.HorizontalAlignment = HorizontalAlignment.Left;
                                    cmb.Tag = attr.Name;
                                    cmb.SelectedIndex = int.Parse(objType.GetProperty(propInfo.Name).GetValue(target, null)?.ToString());
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

                                    foreach (var cbItem in attr.Items)
                                    {
                                        cmb.Items.Add(cbItem);
                                    }
                                    sp.Children.Add(cmb);
                                    break;
                                case ControlType.Button: 
                                    //找到需要对应的方法
                                    MethodInfo mif = null; 
                                    foreach (MethodInfo method in targetType.GetMethods())
                                    {
                                        if(method.Name == attr.MethodName)
                                        {
                                            mif = method;
                                        }
                                    } 
                                    Button btnMethod = new Button();
                                    btnMethod.HorizontalAlignment = HorizontalAlignment.Left;
                                    btnMethod.Margin = new Thickness(5);
                                    btnMethod.Content = "调用"; 
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
                                          
                                            } ); 
                                        }
                                        else
                                        {
                                            MessageBox.Show($"未能找到对应的方法调用'{targetType}'.'{attr.MethodName}'");
                                        }
                                    };  
                                    sp.Children.Add(btnMethod); ;
                                    break;
                                case ControlType.FilePathSelector:
                                    foreach (var item in sp.Children)
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

                                            txt.MouseDoubleClick += (s, e) =>
                                            {
                                                OpenFileDialog dialog = new OpenFileDialog();
                                                dialog.InitialDirectory = GlobalPara.SysPath;
                                                dialog.Multiselect = true;//该值确定是否可以选择多个文件
                                                dialog.Title = "请选择文件夹";

                                                dialog.Filter = "文件(*." + attr.FileType + ")|*." + attr.FileType + "|所有文件(*.*)|*.*";
                                                if (dialog.ShowDialog(ownerWindow) == true)
                                                {
                                                    if (fileTarget != null)
                                                    {
                                                        fileTarget.SetValue(target, dialog.FileName);
                                                        txt.ToolTip = txt.Text = objType.GetProperty(fileTarget.Name).GetValue(target, null)?.ToString();

                                                    }
                                                }
                                            };
                                        }
                                    }
                                    break;
                                case ControlType.ColorPicker:

                                    //找到需要填充的属性/字段名称
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
                                    break;
                                case ControlType.Genericity:
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

                                    var g = objType.GetProperty(gTaget.Name).GetValue(target, null)?.ToString();
                                    btnCreateType.Content = g == null ? "选择" : g.ToString();

                                    btnCreateType.Click += (s, e) =>
                                    {
                                        CreateComboBox(attr.GenerictyType, (o) =>
                                        {
                                            gTaget.SetValue(target, o);
                                            btnCreateType.Content = o.ToString();
                                        }, ownerWindow);
                                    };

                                    sp.Children.Add(btnCreateType);
                                    break;
                                case ControlType.Data:
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
                                    btnData.Content = data == null ? "错误!" : data.ToString();
                                    btnData.Click += (s, e) =>
                                    {
                                        CreateWindow<DataWindow>(attr.GenerictyType, (o) =>
                                        {
                                            dataTaget.SetValue(target, o);
                                            btnData.Content = o.ToString();
                                        }, data);
                                    };
                                    sp.Children.Add(btnData);
                                    break;
                            }
                        }
                    }
                    container.Children.Add(sp);
                }

            }

            
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
        public static void CreateWindow<W>(Type target, Action<object> OnScussec, object orginTarget = null,string titleName =null,   params object[] para) where W :  Window, IControlHelper ,new()
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

            window.OnCancel += () =>
            {

                window.Close();
            };
            window.ShowDialog();
        }

        public static void CreateWindow<T,W>(Action<T> OnScussec, T orginTarget = null,   string titleName = null, params object[] para) where T : class, new() where W : Window, IControlHelper, new()
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

            window.OnCancel += () =>
            {

                window.Close();
            };

            window.ShowDialog();

        }
       

        public static void CreateDataGirdWindow<W>(object data,Action<object> OnSet =null, string titleName = null, Action OnClose = null) where W : Window, IControlData, new()
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


        private static W WindowProperty<W>( string titleName =null) where W : Window,new()
        {
            var window = new W
            {
                MinHeight = 300,
                MinWidth = 400,
                Width = 580,
                Height = 700,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
            };
            window.Title = (titleName ?? window.Title);
            return window;
        }

        private static void Cp_Canceled(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }

}
