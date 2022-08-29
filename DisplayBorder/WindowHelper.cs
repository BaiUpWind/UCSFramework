using DeviceConfig;
using DisplayBorder.Controls;
using HandyControl.Controls;
using HandyControl.Tools;
using Microsoft.Win32;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ComboBox = System.Windows.Controls.ComboBox;
using TextBox = System.Windows.Controls.TextBox;
using Window = System.Windows.Window;

namespace DisplayBorder
{
    public static class WindowHelper
    {
        /// <summary>
        /// 生成一个组合框 创建对象实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="OnScusses"></param>
        public static void GetObject<T>(Action<T> OnScusses,Window owner= null,params object[] para) where T : class
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

        /// <summary>
        /// 创建一个类型,自动根据类型中的字段特性[<see cref="ControlAttribute"/>]创建对应的控件
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <typeparam name="Control">编辑的控件</typeparam>
        /// <param name="OnScussec">当创建成功时返回对应的类型</param>
        /// <param name="orginTarget">自己创建的类</param>
        /// <param name="ownerWindow">控件出现在的父类窗口</param>
        /// <param name="para">类创建的所需参数</param>
        /// <exception cref="Exception">初始化失败,创建失败</exception>
        public static void Create<T,Control>( Action<T> OnScussec,T orginTarget =null, Window ownerWindow = null ,params object[] para) where T:class,new() where Control : FrameworkElement, ISingleOpen, IControlHelper, new()
        {
            var control = SingleOpenHelper.CreateControl<Control>();
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
            control.CreateType(orginTarget); 
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

        /// <summary>
        /// 创建一个显示datagrid的窗体
        /// <para>支持的数据格式[DataTable][List<T>]</para>
        /// </summary>
        /// <typeparam name="Control"></typeparam>
        /// <param name="data">显示的数据</param>
        /// <param name="OnSet">当赋予数据时发生的事件</param>
        /// <param name="OnClose">但窗体关闭时发生</param>
        /// <param name="ownerWindow">属于的父窗体</param>
        public static void Create<Control>(object data, Window ownerWindow = null,
            Action<object> OnSet = null,
            Action OnClose = null ) where Control : FrameworkElement, IControlData,ISingleOpen, new()
        {
            var control = SingleOpenHelper.CreateControl<Control>();
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

 
        /// <summary>
        /// 自动根据类型中的字段特性[<see cref="ControlAttribute"/>]创建对应的控件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <param name="container"></param>
        public static void CreateContrl<T>(T target , Panel container,Window ownerWindow = null) where T : class
        {
            if (target == null) return;
            Type objType = target.GetType();

            var properties = objType.GetProperties();
            foreach (var propInfo in properties)
            {
                //当前这个类
                if (!propInfo.DeclaringType.IsPublic) continue;

                object[] objAttrs = propInfo.GetCustomAttributes(typeof(ControlAttribute), true);

                if (objAttrs.Length > 0)
                {
                    StackPanel sp = new StackPanel();
                    foreach (var att in objAttrs)
                    {
                        if (att is ControlAttribute conattr)
                        {
                            if (!string.IsNullOrEmpty(conattr.LabelName ))
                            {
                                sp.Margin = new Thickness(18);
                                TextBlock textBlock = new TextBlock();
                                textBlock.Text = conattr.LabelName;
                                textBlock.FontSize = 21;
                                sp.Children.Add(textBlock);
                            } 
                            switch (conattr.ControlType)
                            { 
                                case ControlType.TextBox: 
                                    TextBox txtBox = new TextBox();
                                    txtBox.Tag = conattr.Name;
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
                                    cmb.Tag = conattr.Name;
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

                                    foreach (var cbItem in conattr.Items)
                                    {
                                        cmb.Items.Add(cbItem);
                                    } 
                                    sp.Children.Add(cmb);
                                    break;
                                case ControlType.FilePathSelector:  
                                    foreach (var item in sp.Children)
                                    {
                                        if(item is TextBox txt)
                                        {
                                            txt.IsReadOnly = true;
                                            if ( string.IsNullOrEmpty(txt.Text))
                                            {
                                                txt.Text = "双击选择文件";
                                            } 
                                            //找到需要填充的属性/字段名称
                                            PropertyInfo fileTarget = null; 
                                            foreach (PropertyInfo pif in properties)
                                            {
                                                if (pif.Name == conattr.FieldName)
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

                                                dialog.Filter = "文件(*." + conattr.FileType + ")|*." + conattr.FileType + "|所有文件(*.*)|*.*";
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
                                default:
                                    break;
                            }

                         
                        }
                    }
                    container.Children.Add(sp);
                }

            }
        }

    }

}
