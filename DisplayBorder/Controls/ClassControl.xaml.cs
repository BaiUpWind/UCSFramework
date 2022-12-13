using System;
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
using ControlHelper.Attributes;
using DeviceConfig;
using DisplayBorder.Events;
using DisplayBorder.View;
using HandyControl.Controls;
using HandyControl.Tools;
using Microsoft.Win32;
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
             
            btnCopy.Visibility = Visibility.Hidden;

            #region load

            if (orginType == null) return;
            gOper.Visibility = isList ? Visibility.Visible : Visibility.Collapsed;
            gInfos.Visibility = Visibility.Collapsed;
            if (!isList)
            {
                CreateControls(gData, WindowHelper.GetTypeDatas(orginType, CheckProperty, target));
            }  
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
                        for (int i = 0; i < list.Count; i++)
                        {
                            var txt = AddTextBox(list, i);
                            gData.Children.Add(txt);
                            generics.Add(txt);
                        }
                    }
                } 
            }
            #endregion
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

        public Action<Type,object> OnSetData;

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
                OnSetData?.Invoke(orginType.GetProperty(name, BindingFlags.Instance | BindingFlags.Public)?.PropertyType, value);
            }
            else
            {
                orginType.GetField(name, BindingFlags.Instance | BindingFlags.Public)?.SetValue(Data, value);
                OnSetData?.Invoke(orginType.GetField(name, BindingFlags.Instance | BindingFlags.Public)?.FieldType, value); 
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
                        control = new RichTextBox();

                        if (control is RichTextBox txt)
                        {
                            txt.VerticalContentAlignment = VerticalAlignment.Top;
                            txt.HorizontalContentAlignment = HorizontalAlignment.Left;
                            txt.Document = new FlowDocument();
                            txt.Document.LineHeight = 2;
                            var txtValue = GetValue(td.Name)?.ToString();
                            if (!string.IsNullOrWhiteSpace(txtValue))
                            {
                                Paragraph paragraph = new Paragraph();
                                paragraph.Inlines.Add(txtValue);
                                txt.Document.Blocks.Add(paragraph);
                            }
                            txt.BorderThickness = new Thickness(0.2);
                            txt.BorderBrush = new SolidColorBrush(Colors.Black);
                            txt.TextChanged += (ts, te) =>
                            {
                                try
                                {
                                    //todo:这里类型会有不同的适配
                                    if (ts is RichTextBox txtBox)  // && !string.IsNullOrEmpty(te.Changes))
                                    {
                                        TextRange textRange = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);
                                        //这里在存入时 会把 \r\n 符号也存入进去 ,所以在使用的时候注意repalce掉
                                        SetValue(td.Name, Convert.ChangeType(textRange.Text.Replace("\r\n", ""), td.ObjectType));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Info($"请输入正确的类型'{td.ObjectType}',\n\r 错误信息:'{ex.Message}' \r\n 内部信息:'{ex.InnerException.Message}'", "输入提示");
                                    txt.Document.Blocks.Clear();
                                }
                            };
                            if(td.FileSelectorAttr != null && td.FileSelectorAttr is FileSelectorAttribute attr)
                            { 
                                txt.IsReadOnly = true; 
                                txt.MouseDoubleClick += (ds, de) =>
                                {
                                    if (ds is RichTextBox txtBox)
                                    { 
                                        txtBox.Document.Blocks.Clear();
                                        OpenFileDialog dialog = new OpenFileDialog
                                        {
                                            InitialDirectory = GlobalPara.SysPath,
                                            Multiselect = true,//该值确定是否可以选择多个文件
                                            Title = "请选择文件",
                                            Filter = "文件(*." + attr.FileType + ")|*." + attr.FileType + "|所有文件(*.*)|*.*"
                                        };
                                        if (dialog.ShowDialog() == true)
                                        {
                                            Paragraph paragraph = new Paragraph();
                                            paragraph.Inlines.Add(dialog.FileName);
                                            txt.Document.Blocks.Add(paragraph);
                                            SetValue(td.Name, dialog.FileName);
                                            //TextRange textRange = new TextRange(txtBox.Document.ContentStart, txtBox.Document.ContentEnd);
                                            txt.ToolTip =  GetValue(td.Name)?.ToString();// objType.GetProperty(fileTarget.Name).GetValue(target, null)?.ToString();
                                        }
                                    }
                                };
                                txt.AddHandler(MouseDownEvent,
                                new MouseButtonEventHandler((s, e) =>
                                {
                                    if (e.ChangedButton == MouseButton.Right)
                                    { 
                                        txt.Document.Blocks.Clear();
                                        txt.ToolTip = "双击选择文件路径";
                                    }
                                }), true);
                            }
                     
                        }
                        break;
                    case ClassControlType.CheckBox:
                        control = new CheckBox();
                        if (control is CheckBox cb)
                        {
                            cb.IsChecked = (GetValue(td.Name)  !=null && (bool)GetValue(td.Name));
                            cb.Checked += (s, e) =>
                            {
                                SetValue(td.Name, cb.IsChecked);
                            };
                            cb.Unchecked += (s, e) =>
                            {
                                SetValue(td.Name, cb.IsChecked);
                            };
                        }
                        break;
                    case ClassControlType.List:
                    case ClassControlType.Class:
                        control = GetGenericControl(i, GetValue(td.Name), td.ObjectType, CheckProperty,td.NickName); 
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
                            if (combo.Items.Count > 0)
                            {
                                combo.SelectedIndex =  GetValue(td.Name) ==null ? 0 :(int)GetValue(td.Name);
                            }
                            combo.SelectionChanged += (s, e) =>
                            {
                                SetValue(td.Name, Enum.Parse(td.ObjectType, combo.SelectedItem.ToString()));
                            };
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
                                    //获取已经选择值的索引
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
                                    //创建目前实现的超类控件
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
                                                SetValue(td.Name, obj); 
                                                CreateSuperClass(sp, obj); 
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            MessageBox.Show($"创建对象失败!目前只支持无参构造.\n\r错误信息:'{ex.Message}'");
                                        }
                                    }
                                };
                                //创建超类的额外控件
                                void CreateSuperClass(StackPanel internalSp, object obj)
                                {
                                    for (int k = 0; k < internalSp.Children.Count; k++)
                                    {
                                        var child = internalSp.Children[k];
                                        if (child is ClassControl clc)
                                        {
                                            internalSp.Children.Remove(child);
                                        }
                                    }
                                    var newDc = new ClassControl(obj.GetType(), CheckProperty, obj);
                                    newDc.OnSetData += (o, v) =>
                                    {
                                        this.OnSetData?.Invoke(o, v);
                                    };
                                    internalSp.Children.Add(newDc);
                                    internalSp.SizeChanged += (sps, spe) =>
                                    {
                                        if (!double.IsNaN(newDc.Width) && internalSp.ActualWidth - cutWidth > 0)
                                            newDc.Width = internalSp.ActualWidth - cutWidth;
                                    };
                                }
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
                        if(control is Button btnClick && td.ButtonAttr != null && td.ButtonAttr is ButtonAttribute ba)
                        { 
                            MethodInfo mif = null;
                            
                            if(string.IsNullOrWhiteSpace(ba.MethodName))
                            {
                                break;
                            }
                            foreach (MethodInfo method in orginType.GetMethods())
                            {
                                if (method.Name == ba.MethodName)
                                {
                                    mif = method;
                                }
                            }
                            dynamic member =null;
                            if(!string.IsNullOrWhiteSpace(ba.MemberName))
                            {
                                //获取属性
                                foreach (var prop in orginType.GetProperties())
                                {
                                    if(prop.Name == ba.MemberName)
                                    {
                                        member = prop;
                                    }
                                }
                                //获取字段
                                foreach (var field in orginType.GetFields())
                                {
                                    if(field.Name == ba.MemberName)
                                    {
                                        member = field;
                                    }
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
                                        if (member == null)
                                        {
                                            MessageBox.Show($"不支持的结果:'{result}'");
                                        }
                                        else
                                        {
                                            if(result.GetType() == typeof(string))
                                            {
                                                if (member is PropertyInfo pi)
                                                {
                                                    pi.SetValue(Data, result);
                                                }
                                                else if (member is FieldInfo fi)
                                                {
                                                    fi.SetValue(Data, result);
                                                    
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show($"目前只对字符串类型进行赋值,目标类型'{result}'");
                                            }
                                           
                                        }
                                    
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
                        if (string.IsNullOrEmpty(td.NickName))
                        {
                            label.Text = td.Name;
                        }
                        else
                        {
                            label.Text = td.NickName;
                        }
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
                    if (!string.IsNullOrWhiteSpace(td.ToolTip))
                    {
                        control.ToolTip = td.ToolTip;
                    }
                    control.HorizontalAlignment = HorizontalAlignment.Left;
                    control.VerticalAlignment = VerticalAlignment.Stretch;
                    control.Margin = new Thickness(15, 0, 0, 0); 
                    if (td.ControlType != ClassControlType.Class 
                        && td.ControlType != ClassControlType.List
                        && td.ControlType != ClassControlType.ComboBoxImplement 
                        && td.ControlType != ClassControlType.TextBox
                        ) 
                    { 
                        if(!double.IsNaN(td.Width))
                        {
                            control.Width = td.Width;
                        }
                        else
                        {
                            control.Width = 220;
                        }

                        if(!double.IsNaN(td.Height))
                        {
                            control.Height = td.Height;
                        } 
                    } 
                    else
                    {
                       //自动缩放
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
                    panel.Children.Add(stackPanel); 
                }
            }
        } 
        private ClassControl GetGenericControl(int index, object item, Type type, bool ischildren = false,string otherName=null)
        {
            ClassControl dc = new ClassControl(type, CheckProperty, item, ischildren);
            dc.Name = "dc_" + index.ToString();
            dc.Title =  $"{(otherName ==null? "":$"{otherName}")}{type.Name}_'{index}'" ;
            dc.OnSetData += (o, v) =>
            {
                this.OnSetData?.Invoke(o, v);
            };
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
                    if (!isList) return;
                    if (ms is Grid c && c.DataContext is ClassControl drop  )
                    {
                        selectedItemTemp = drop.Data;
                        selectedControl = drop;
                        btnCopy.Visibility = Visibility.Visible;
                        Console.WriteLine(selectedItemTemp);

                        lblSelectedInfo.Text = $"当前选中'{drop.Title}'";
                    }
                }; 
            } 
            return dc;
        } 
        private UIElement GetComboBox(IList list, int i)
        {
            StackPanel panel = new StackPanel();
            panel.Margin = new Thickness(10);
            TextBlock block = new TextBlock();
            ComboBox combo = new ComboBox();
            combo.IsReadOnly = true;
            block.Text = combo.Name = "combo_" + i.ToString();
            foreach (var item in Enum.GetNames(genericArgument))
            {
                combo.Items.Add(item);
            } 
            if(combo.Items.Count > 0)
            {
                combo.SelectedIndex = (int)list[i]; 
            }
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
            panel.Children.Add(block); 
            panel.Children.Add(combo);  
            return panel;
        }
        private UIElement AddTextBox(IList list, int i)
        {
            StackPanel panel = new StackPanel();
            TextBlock block = new TextBlock();
            panel.Margin = new Thickness(10);
            TextBox textBox = new TextBox();
            block.Text  =textBox.Name = "txt_" + i.ToString();
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
            panel.Children.Add(block); 
            panel.Children.Add(textBox); 
            return panel;
        }
        private UIElement AddCheckBox(IList list, int i)
        {
            StackPanel panel = new StackPanel();
            TextBlock block = new TextBlock();
          
            panel.Margin = new Thickness(10);
            CheckBox checkBox = new CheckBox();
            checkBox.IsChecked = (bool)list[i];
            block.Text = checkBox.Name = "checkb_" + i.ToString();
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
            panel.Children.Add(block);
            panel.Children.Add(checkBox); 
            return panel;
        }
        private void AddChildren(object copyData)
        {
            if (!isList) return;

            if (copyData != null)
            {
                genericData = copyData.Clone(genericArgument);
            }
            else
            {
                try
                {
                    genericData = Activator.CreateInstance(genericArgument);
                }
                catch
                {
                    //当没有对应的构造函数时 使用空对象
                    genericData = null;
                }
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
            if (generics == null) generics = new List<UIElement>();
            if (code == TypeCode.Object)
            {
                var dc = GetGenericControl(startIndex, genericData, genericArgument, true);
                gData.Children.Add(dc);
                generics.Add(dc);
            }
            else if (genericArgument.IsEnum)
            {
                //枚举类型
                var cb = GetComboBox(list, startIndex);
                gData.Children.Add(cb);
                generics.Add(cb);
            }
            else
            {
                if (code == TypeCode.Boolean)
                {
                    var cb = AddCheckBox(list, startIndex);
                    gData.Children.Add(cb);
                    generics.Add(cb);
                }
                else
                {
                    var cb = AddTextBox(list, startIndex);
                    gData.Children.Add(cb);
                    generics.Add(cb);
                }
            }

            OnSetData?.Invoke(orginType, data);
            ReClaculateName();
            ClaerSelectItem();
        }
        private void ClaerSelectItem()
        {
            selectedItemTemp = null;
            selectedControl=null;
            lblSelectedInfo.Text = string.Empty;
        }

        private void ReClaculateName()
        {
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
            if (generics == null) return;
            for (int i = 0; i < generics.Count; i++)
            {
                var control = generics[i]; 
                if(control is ClassControl dc)
                {
                    dc.Title = $" {dc.orginType.Name}_'{i}'";
                }
            }
        }
        #region 事件
        //当拷贝时
        private void Btn_Copy(object sender, RoutedEventArgs e)
        {
            btnCopy.Visibility = Visibility.Hidden;
            if (selectedItemTemp == null) return; 
            AddChildren(selectedItemTemp);
            ClaerSelectItem();
        } 
        //控件加载时
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
           if(isList) Title = string.Format(Title, $"[{((IList)data).Count}]");
        }
        //展开 收起
        private void Btn_Show(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Content is TextBlock tb)
            {
                if (tb.Text == "↓")
                {
                    gInfos.Visibility = Visibility.Visible;
                    tb.Text = "↑";
                }
                else if (tb.Text == "↑")
                {
                    gInfos.Visibility = Visibility.Collapsed;
                    tb.Text = "↓";
                }
            }
        }
        //添加
        private void Btn_Add(object sender, RoutedEventArgs e)
        {
            AddChildren(null);
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
            ReClaculateName();
            OnSetData?.Invoke(orginType, data);
            ClaerSelectItem();
        }
        #endregion

     
    }
}
