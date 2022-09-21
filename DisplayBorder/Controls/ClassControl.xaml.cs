﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommonApi;
using DeviceConfig;
using DisplayBorder.View;
using HandyControl.Controls;
using HandyControl.Tools;
using ComboBox = System.Windows.Controls.ComboBox;
using MessageBox = HandyControl.Controls.MessageBox;
using TextBox = System.Windows.Controls.TextBox;

namespace DisplayBorder.Controls
{
    /// <summary>
    /// ClassControl.xaml 的交互逻辑
    /// </summary>
    public partial class ClassControl : UserControl
    {
        const double cutWidth = 85;
        public ClassControl()
        {
            InitializeComponent(); 

        }
        public ClassControl(Type type, bool isPorperty = true, object target = null, bool isChildren = false)
        {
            InitializeComponent(); 
            CheckProperty = isPorperty;
            orginType = type;
            this.IsArrayChild = isChildren;
            this.target = target;
            if (target == null)
            {
                if (type.IsArray)
                    data = Activator.CreateInstance(type, 0);
                else
                {
                    try
                    {
                        data = Activator.CreateInstance(type);
                    }
                    catch
                    {
                        data = null;
                    }
                }
            }
            else
            {
                data = target;
            }
            Title = type.Name;
            gTitle.DataContext = this;
            isList = Utility.Reflection.IsList(type); 
            lblSelectedInfo.Text = string.Empty;

        }
        private object target;
        private string title;
        private object data;
        private object genericData;
        private object selectedItemTemp;
        private UIElement selectedControl;
        private Type genericArgument;// 集合的泛型类型  
        private readonly Type orginType;
        /// <summary>
        /// 是为集合 包括数组 List<T>,对字典无效
        /// </summary>
        private readonly bool isList;
        /// <summary>
        /// 是 检查属性 否 检查字段
        /// </summary>
        private readonly bool CheckProperty;
        /// <summary>
        /// 是否为数组或者集合的子类
        /// </summary>
        public readonly bool IsArrayChild;
        /// <summary>
        /// 存放 需要重新计算宽的元素
        /// </summary>
        private List<UIElement> childen = new List<UIElement>();

        /// <summary>
        /// 存放集合或者数组的子类 
        /// <para>只有在是泛型添加的时才有用</para>
        /// </summary>
        private List<UIElement> generics  ;
        public string Title
        {
            get => title; set
            {
                title = value;
                lblTitle.Text = value;
            }
        }

        public object Data { get => data; }

        public object GenericData => genericData;

        private object GetValue(string name)
          => CheckProperty
         ? orginType.GetProperty(name, BindingFlags.Instance | BindingFlags.Public)?.GetValue(Data)
         : orginType.GetField(name, BindingFlags.Instance | BindingFlags.Public)?.GetValue(Data);

        private void SetValue(string name, object value)
        {
            if (CheckProperty)
            {
                orginType.GetProperty(name, BindingFlags.Instance | BindingFlags.Public)?.SetValue(Data, value);
            }
            else
            {
                orginType.GetField(name, BindingFlags.Instance | BindingFlags.Public)?.SetValue(Data, value);
            }
        }

        private void CreateControls(Panel panel, TypeData[] typeDatas)
        {
            panel.Children.Clear();
            for (int i = 0; i < typeDatas.Length; i++)
            {
                var td = typeDatas[i];
                if (td == null) continue;
                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Horizontal;
                FrameworkElement control = null;
                switch (td.ControlType)
                {
                    case ClassControlType.TextBox:
                        control = new TextBox(); 
                        if (control is TextBox txt)
                        { 
                            txt.Text = GetValue(td.Name)?.ToString();
                            txt.BorderThickness =  new Thickness(0.2);
                            txt.BorderBrush = new SolidColorBrush(Colors.Black);
                            txt.PreviewTextInput += (ts, te) =>
                            {
                                //todo:这里类型会有不同的适配
                                if (ts is TextBox t && !string.IsNullOrEmpty(t.Text))
                                {
                                    SetValue(td.Name, Convert.ChangeType((t.Text), td.ObjectType));
                                }
                            };
                        }
                        break;
                    case ClassControlType.CheckBox:
                        control = new CheckBox();
                        if (control is CheckBox cb)
                        {
                            cb.IsChecked = (bool)GetValue(td.Name);
                            cb.Checked += (s, e) =>
                            {
                                SetValue(td.Name, cb.IsChecked);
                            };
                        }
                        break;
                    case ClassControlType.List:
                    case ClassControlType.Class:
                        control = GetGenericControl(i, GetValue(td.Name), td.ObjectType, CheckProperty); 
                        if (control is ClassControl dc)
                        { 
                            dc.Background = new SolidColorBrush( Colors.Tan);
                            dc.Title = td.Name + (td.IsList ? "_{0}" : ""); 
                        }
                        break;
                    case ClassControlType.ComboBox:
                        control = new ComboBox();
                        if (control is ComboBox combo)
                        {
                            combo.IsReadOnly = true;
                            foreach (var item in Enum.GetNames(td.ObjectType))
                            {
                                combo.Items.Add(item);
                            } 
                            combo.SelectionChanged += (s, e) =>
                            {
                                SetValue(td.Name, Enum.Parse(td.ObjectType, combo.SelectedItem.ToString()));
                            };
                            combo.SelectedIndex = (int)GetValue(td.Name);
                            if (combo.Items.Count > 0) combo.SelectedIndex = 0;
                        }
                        break;
                    case ClassControlType.ComboBoxImplement:
                        control = new StackPanel();
                        ComboBox comb = new ComboBox();
                        if (control is StackPanel sp)
                        {
                            sp.Orientation = Orientation.Vertical;
                            sp.Children.Add(comb); 
                            if (comb is ComboBox ci)
                            {
                                ci.IsReadOnly = true;
                                foreach (var item in Utility.Reflection.GetClassDataFullName(td.ObjectType).ToArray())
                                {
                                    ci.Items.Add(item);
                                }
                                var result = GetValue(td.Name);
                                if (result != null)
                                {
                                    int index = 0;
                                    for (int j = 0; j < ci.Items.Count; j++)
                                    {
                                        var item = ci.Items[j];
                                        if (item.ToString() == result.ToString())
                                        {
                                            index = j;
                                            break;
                                        }
                                    }
                                    ci.SelectedIndex = index; 
                                    CreateSuperClass(sp, result);  
                                }
                                ci.SelectionChanged += (s, e) =>
                                {
                                    if (s is ComboBox comboI)
                                    {
                                        try
                                        {
                                            var obj = Utility.Reflection.CreateObject(td.ObjectType, comboI.SelectedItem.ToString());
                                            if (obj != null)
                                            {
                                                SetValue(td.Name, result);
                                                td.ObjectType = obj.GetType();
                                                #region 创建按钮
                                                //foreach (var item in stackPanel.Children)
                                                //{
                                                //    if (item is Button)
                                                //    {
                                                //        return;
                                                //    }
                                                //}
                                                //Button btn = new Button();
                                                //btn.Content = "打开";
                                                //btn.Margin = new Thickness(5, 0, 0, 0);
                                                //btn.Click += (bs, be) =>
                                                //{
                                                //    WindowHelper.CreateWindow<DataWindow>(obj.GetType(), (o) =>
                                                //    {
                                                //        if (o.GetType() == obj.GetType())
                                                //        {
                                                //            obj = o;
                                                //        }
                                                //    }, obj, "", null);
                                                //};
                                                //stackPanel.Children.Add(btn);
                                                #endregion 
                                                CreateSuperClass(sp, obj); 
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show($"创建对象失败!目前只支持无参构造.\n\r错误信息:'{ex.Message}'");
                                        }
                                    }
                                };
                            }
                        }
                        break;
                    case ClassControlType.ColorPicker:
                        control = new Button();
                        if (control is Button btnColor)
                        {
                            btnColor.Content = "选择";
                            btnColor.Background = new SolidColorBrush((Color)GetValue(td.Name));
                            btnColor.Click += (s, e) =>
                            {
                                var cp = SingleOpenHelper.CreateControl<ColorPicker>();
                                var window = new PopupWindow
                                {
                                    PopupElement = cp,
                                    AllowsTransparency = true,
                                    WindowStyle = WindowStyle.None,
                                    ResizeMode = ResizeMode.NoResize,
                                    Owner = null,
                                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                                    MinWidth = 0,
                                    MinHeight = 0,
                                };
                                //当选择颜色时
                                cp.Confirmed += (cs, ce) =>
                                {
                                     SetValue(td.Name,ce.Info);
                                    btnColor.Background = new SolidColorBrush((Color)GetValue(td.Name)); 
                                    window.Close();
                                };
                                //当取消时
                                cp.Canceled += (cs, ce) =>
                                {
                                    window.Close();
                                };
                                window.ShowDialog(null, false);
                            }; 
                        }
                        break;
                    case ClassControlType.Button:
                        control = new Button();
                        if(control is Button btnClick && td.CustomAttr != null && td.CustomAttr is ButtonAttribute ba)
                        { 
                            MethodInfo mif = null;
                            foreach (MethodInfo method in orginType.GetMethods())
                            {
                                if (method.Name == ba.MethodName)
                                {
                                    mif = method;
                                }
                            }
                            btnClick.Content = ba.Name;
                            btnClick.Click += (s, e) =>
                            {
                                if (mif == null)
                                {
                                    MessageBox.Show($"未能找到对应的方法调用'{orginType}'.'{ba.MethodName}'");
                                    return;
                                }
                                Task.Run(() =>
                                {
                                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                    {
                                        btnClick.IsEnabled = false;
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
                                        btnClick.IsEnabled = true;
                                    })); 
                                });
                            };

                        } 
                        break;
                }
                if (control != null)
                {
                    int x = 0;
                    if (td.ControlType != ClassControlType.List && td.ControlType != ClassControlType.Class)
                    {
                        TextBlock label = new TextBlock();
                        label.Text = td.Name; 
                        label.TextAlignment = TextAlignment.Left;
                        label.VerticalAlignment = VerticalAlignment.Center;
                        label.Margin = new Thickness(5,0,0,0);  
                        x = (int)label.ActualWidth +10;
                        stackPanel.Children.Add(label);
                    }
                    else
                    {
                        x += 5;
                    }
                    control.HorizontalAlignment = HorizontalAlignment.Left;
                    control.VerticalAlignment = VerticalAlignment.Stretch;
                    control.Margin = new Thickness(15, 0, 0, 0);
                    if(td.ControlType != ClassControlType.Class && td.ControlType != ClassControlType.List
                        && td.ControlType  != ClassControlType.ComboBoxImplement) 
                        control.Width = 220;
                    else
                    {
                       
                        childen.Add(control);
                    }
                    stackPanel.Background = new SolidColorBrush(Colors.LightCyan);
                    stackPanel.Margin = new Thickness(2,5,2,5);
                    stackPanel.Children.Add(control);
                    panel.SizeChanged += (se, ee) =>
                    {
                        foreach (FrameworkElement item in childen)
                        {
                            if (!double.IsNaN(item.Width) && panel.ActualWidth - 105 > 0)
                                item.Width = panel.ActualWidth - 105;
                            else
                                item.Width = 10; 
                        } 
                    };
                    //stackPanel.Height = control.Height;
                    panel.Children.Add(stackPanel);
                    //childen.Add(stackPanel);
                }
            }
        }

        private void CreateSuperClass(StackPanel sp, object obj)
        {
            for (int k = 0; k < sp.Children.Count; k++)
            {
                var child = sp.Children[k];
                if (child is ClassControl clc)
                {
                    sp.Children.Remove(child);
                }
            }
            var newDc = new ClassControl(obj.GetType(), CheckProperty, obj);
            sp.Children.Add(newDc);
            sp.SizeChanged += (sps, spe) =>
            {
                if (!double.IsNaN(newDc.Width) && sp.ActualWidth - cutWidth > 0)
                    newDc.Width = sp.ActualWidth - cutWidth;
            };
        }

        private ClassControl GetGenericControl(int index, object item, Type type, bool ischildren = false)
        {
            ClassControl dc = new ClassControl(type, CheckProperty, item, ischildren);
            dc.Name = "dc_" + index.ToString();
            dc.Title = type.Name + $"'{index}'";
            if (ischildren)
            {
                dc.gTitle.MouseEnter += (ms, me) =>
                {
                    Cursor = Cursors.Hand;
                };
                dc.gTitle.MouseLeave += (ms, me) =>
                {
                    Cursor = Cursors.Arrow;
                };
                dc.gTitle.MouseDown += (ms, me) =>
                {
                    if (ms is Grid c && c.DataContext is ClassControl drop  )
                    {
                        selectedItemTemp = drop.Data;
                        selectedControl = drop;
                        Console.WriteLine(selectedItemTemp);

                        lblSelectedInfo.Text = $"当前选中'{drop.Title}'";
                    }
                };
            } 
            return dc;
        } 
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (orginType == null) return; 
            gOper.Visibility = isList ? Visibility.Visible : Visibility.Collapsed;
            gInfos.Visibility = Visibility.Collapsed; 
            if (!isList) CreateControls(gData, WindowHelper.GetTypeDatas(orginType, CheckProperty, target));
            else
            {
                if (orginType.IsGenericType)
                {
                    //对泛型 
                    if (orginType.GenericTypeArguments.Length != 1)
                    {
                        throw new Exception("目前只对单个泛型集合进行处理");
                    }
                    genericArgument = orginType.GenericTypeArguments[0];
                }
                else
                {
                    //数组
                    genericArgument = orginType.GetArrayElementType();
                }
                TypeCode code = Type.GetTypeCode(genericArgument);
                var list = (IList)data;
                if (list.Count == 0) return;
                if (generics == null) generics = new List<UIElement>();
                if (code == TypeCode.Object)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        var item = list[i];
                        var dc = GetGenericControl(i, item, genericArgument, true);
                        gData.Children.Add(dc);
                        generics.Add(dc);
                    }
                    gData.Background = new SolidColorBrush(Colors.GreenYellow);
                }
                else
                {
                    if (genericArgument.IsEnum)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            var cb = GetComboBox(list, i);
                            gData.Children.Add(cb);
                            generics.Add(cb);
                        }
                    }
                    else if (code == TypeCode.Boolean)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            var checb = AddCheckBox(list, i);
                            gData.Children.Add(checb);
                            generics.Add(checb);
                        }
                    }
                    else
                    {
                        //todo :对每种类型做不同的正则表达式
                        for (int i = 0; i < list.Count; i++)
                        {
                            var txt = AddTextBox(list, i);
                            gData.Children.Add(txt);
                            generics.Add(txt);
                        }
                    }
                }
                Title = string.Format(Title, $"[{((IList)data).Count}]");
            }
        }
        //展开 收起
        private void Btn_Show(object sender, RoutedEventArgs e)
        {
            if(sender is Button btn && btn.Content is TextBlock tb)
            {
                if(tb.Text  == "↓")
                {
                    gInfos.Visibility = Visibility.Visible;
                    tb.Text = "↑";
                }
                else if(tb.Text == "↑")
                {
                    gInfos.Visibility = Visibility.Collapsed;
                    tb.Text = "↓";
                }
            }
        }
        //添加
        private void Btn_Add(object sender, RoutedEventArgs e)
        {
            if (!isList) return;

            try
            {
                genericData = Activator.CreateInstance(genericArgument);
            }
            catch
            {
                //当没有对应的构造函数时 使用空对象
                genericData = null;
            }
            var list = (IList)data;
            if (orginType.IsGenericType)
            {
                //泛型  
                list.Add(genericData);

            }
            else if (orginType.IsArray)
            {
                //数组 
                var array = (Array)data;
                //创建新的数组
                var tempArr = (Array)Activator.CreateInstance(orginType, array.Length + 1);
                array.CopyTo(tempArr, 0);
                tempArr.SetValue(genericData, tempArr.Length - 1);
                list = tempArr;
            }
            data = list;
            int startIndex = list.Count;
            if (list.Count > 0) startIndex = list.Count - 1;
            var code = Type.GetTypeCode(genericArgument);
            if (code == TypeCode.Object)
            {
                var dc = GetGenericControl(startIndex, genericData, genericArgument, true);
                gData.Children.Add(dc);
                if (generics == null) generics = new List<UIElement>();
                generics.Add(dc);
            }
            else if (genericArgument.IsEnum)
            {
                //枚举类型
                generics.Add(GetComboBox(list, startIndex));
            }
            else
            {
                if (code == TypeCode.Boolean)
                {
                    generics.Add(AddCheckBox(list, startIndex));
                }
                else
                {
                    generics.Add(AddTextBox(list, startIndex));
                }
            }

            var temp = Title.Split('_');
            if (temp.Length > 1)
            {
                temp[1] = $"_[{((IList)data).Count}]";
                Title = string.Empty;
                foreach (var item in temp)
                {
                    Title += item;
                }
            }
        }
        //移除
        private void Btn_Sub(object sender, RoutedEventArgs e)
        {
            if (!isList) return;
            if (generics.Count <= 0) return;

            //当没有选择时 默认选择第一个
            if (selectedItemTemp == null)
            {
                selectedControl = generics[0];
                if (selectedControl is ClassControl dc)
                {
                    selectedItemTemp = dc.Data;
                }
                else
                {
                    selectedItemTemp = ((IList)data)[0];
                }
            }
            if (orginType.IsGenericType)
            {
                //泛型集合 
                ((IList)data).Remove(selectedItemTemp);
            }
            else
            {
                //数组
                var orginArr = (Array)data;
                ArrayList arrayList = new ArrayList();
                foreach (var item in orginArr)
                {
                    arrayList.Add(item);
                }
                arrayList.Remove(selectedItemTemp);
                data = arrayList.ToArray();
            }
            generics.Remove(selectedControl);
            gData.Children.Remove(selectedControl);

            //名字更新数量
            var temp = Title.Split('_');
            if(temp.Length > 1)
            {
                temp[1] = $"_[{((IList)data).Count}]";
                Title = string.Empty;
                foreach (var item in temp)
                {
                    Title += item;
                }
            }
          
            selectedItemTemp = null;
            selectedControl = null;
            lblSelectedInfo.Text = string.Empty;
        }

        private UIElement GetComboBox(IList list, int i)
        {
            StackPanel panel = new StackPanel();
            ComboBox combo = new ComboBox();
            combo.IsReadOnly = true;
            combo.Name = "combo_" + i.ToString();
            foreach (var item in Enum.GetNames(genericArgument))
            {
                combo.Items.Add(item);
            } 
            combo.SelectedIndex = (int)list[i];
            combo.SelectionChanged += (cs, ce) =>
            {
                if (cs is ComboBox cb)
                    list[int.Parse(cb.Name.Split('_')[1])] = Enum.Parse(genericArgument, cb.SelectedItem.ToString());
            };
          
            panel.MouseDown += (s, e) =>
            {
                if (s is StackPanel p && p.Children[0] is ComboBox c)//&& e is MouseEventArgs me  )
                {
                    selectedItemTemp = c.SelectedIndex;
                    selectedControl = p;
                    Console.WriteLine(selectedItemTemp);
                    lblSelectedInfo.Text = $"当前选中'{c.Name}'";
                }
            };
            panel.MouseEnter += (s, e) =>
            {
                Cursor = Cursors.Hand;
            };
            panel.MouseLeave += (ms, me) =>
            {
                Cursor = Cursors.Arrow;
            }; 
            panel.Children.Add(combo);  
            return panel;
        }
        private UIElement AddTextBox(IList list, int i)
        {
            StackPanel panel = new StackPanel();
            TextBox textBox = new TextBox();
            textBox.Name = "txt_" + i.ToString();
            textBox.Text = list[i]?.ToString();
            textBox.PreviewTextInput += (ts, te) =>
            {
                //todo:这里类型会有不同的适配
                if (ts is TextBox t && !string.IsNullOrEmpty(t.Text))
                    list[int.Parse(t.Name.Split('_')[1])] = Convert.ChangeType(t.Text, genericArgument);
            };
            
            panel.MouseDown += (s, e) =>
            {
                if (s is StackPanel p && p.Children[0] is TextBox t)//&& e is MouseEventArgs me  )
                {
                    selectedItemTemp = t.Text;
                    selectedControl = p;
                    Console.WriteLine(selectedItemTemp);
                    lblSelectedInfo.Text = $"当前选中'{t.Name}'";
                }
            };
            panel.MouseEnter += (s, e) =>
            {
                Cursor = Cursors.Hand;
            };
            panel.MouseLeave += (ms, me) =>
            {
                Cursor = Cursors.Arrow;
            };
            panel.Children.Add(textBox); 
            return panel;
        }
        private UIElement AddCheckBox(IList list, int i)
        {
            StackPanel panel = new StackPanel();
            CheckBox checkBox = new CheckBox();
            checkBox.IsChecked = (bool)list[i];
            checkBox.Name = "checkb_" + i.ToString();
            checkBox.Checked += (cs, ce) =>
            {
                if (cs is CheckBox cb)
                    list[int.Parse(cb.Name.Split('_')[1])] = (checkBox.IsChecked);
            };
           
            panel.MouseDown += (s, e) =>
            {
                if (s is StackPanel p && p.Children[0] is CheckBox c)//&& e is MouseEventArgs me  )
                {
                    selectedItemTemp = c.IsChecked;
                    selectedControl = p;
                    Console.WriteLine(selectedItemTemp);
                    lblSelectedInfo.Text = $"当前选中'{c.Name}'";
                }
            };
            panel.MouseEnter += (s, e) =>
            {
                Cursor = Cursors.Hand;
            };
            panel.MouseLeave += (ms, me) =>
            {
                Cursor = Cursors.Arrow;
            };
            panel.Children.Add(checkBox); 
            return panel;
        }

       
      
    }
}