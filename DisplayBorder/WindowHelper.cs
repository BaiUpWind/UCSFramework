using DeviceConfig;
using DisplayBorder.Controls;
using HandyControl.Controls;
using HandyControl.Tools;
using System;
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


        public static void Create<T,Control>( Action<T> OnScussec,T orginTarget =null, Window ownerWindow = null ,params object[] para) where T:class,new() where Control : FrameworkElement, ISingleOpen, IControHelper, new()
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

        public static void CreateContrl<T>(T target , Panel container) where T : class
        {
            if (target == null) return;
            Type objType = target.GetType();
          
            foreach (var propInfo in objType.GetProperties())
            {
                if (!propInfo.DeclaringType.IsPublic) return;
                object[] objAttrs = propInfo.GetCustomAttributes(typeof(ControlAttribute), true);
                if (objAttrs.Length > 0)
                {
                    foreach (var att in objAttrs)
                    {
                        if (att is ControlAttribute conattr)
                        {
                            StackPanel sp = new StackPanel();
                            sp.Margin = new Thickness(18);
                            TextBlock textBlock = new TextBlock();
                            textBlock.Text = conattr.LabelName;
                            textBlock.FontSize = 21;
                            sp.Children.Add(textBlock);
                            switch (conattr.ControlType)
                            {
                                case ControlType.Label:
                                    break;
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
                                                var value = objType.GetProperty(propInfo.Name).GetValue(target, null)?.ToString();

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
                                                var value = objType.GetProperty(propInfo.Name).GetValue(target, null)?.ToString();

                                            }
                                        }
                                    };

                                    foreach (var cbItem in conattr.Items)
                                    {
                                        cmb.Items.Add(cbItem);
                                    } 
                                    sp.Children.Add(cmb);
                                    break;
                                case ControlType.ComboBoxSerialPort:
                                    break;
                                case ControlType.ComboBoxEnum:

                                    break;
                                default:
                                    break;
                            }

                            container.Children.Add(sp);
                        }
                    }
                }

            }
        }

    }

}
