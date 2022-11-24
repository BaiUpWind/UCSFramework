using ControlHelper.Attributes;
using ControlHelper.Interfaces;
using ControlHelper.Model;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ControlHelper.WPF
{
    public static class WPFControlHelper
    {
        /// <summary>
        /// 在容器控件上创建泛型类含有<see cref="LabelAttribute"/>标签属性的控件
        /// </summary> 
        //public static void CreateLabel<T>(this Panel container, T target) where T : class, IPropertyChanged
        //{
        //    if (target == null) return;
        //    container.Children.Clear();

        //    Point ledLocation = new Point(0, 0);
        //    int index = 0;
        //    int x = 20;

        //    var props = target.GetType().GetProperties();
        //    foreach (var prop in props)
        //    {
        //        Console.WriteLine(prop.Name);
        //        var attrs = prop.GetCustomAttributes(true);
        //        if (attrs.Where(a => a is ColumonFeedAttribute).Count() > 0)
        //        {
        //            //如果是换列属性
        //            x += 130;
        //            index = 0;
        //        }
        //        var la = attrs.Where(a => a is LabelAttribute).Select(a => a as LabelAttribute).FirstOrDefault();
        //        if (la != null)
        //        {
        //            ledLocation = new Point(x, 25 * index + 10);

        //            Label lbl = new Label();
        //            lbl.Text = la.Info;
        //            lbl.BorderStyle = BorderStyle.FixedSingle;
        //            lbl.ForeColor = Color.Black;
        //            lbl.TextAlign = ContentAlignment.MiddleCenter;
        //            lbl.BackColor = Color.Transparent;
        //            lbl.Location = ledLocation;
        //            target.OnPropertyChanged += (s, v) =>
        //            {
        //                if (s == prop.Name)
        //                {
        //                    try
        //                    {
        //                        var value = (bool)v;
        //                        if (value)
        //                        {
        //                            lbl.BackColor = Color.Green;
        //                        }
        //                        else
        //                        {
        //                            lbl.BackColor = Color.Transparent;
        //                        }
        //                    }
        //                    catch
        //                    {
        //                        lbl.BackColor = Color.Red;
        //                    }
        //                }
        //            };
        //            container.Controls.Add(lbl);
        //            lbl.Width = lbl.GetWidth() + 16;
        //            index++;

        //        }
        //    }
        //}


        //private static void CreateControl(object target, Type type, Panel container)
        //{
        //    if (type == null || container == null) return;
        //    if (target == null)
        //    {
        //        target = Activator.CreateInstance(type);
        //    }
        //    container.Children.Clear();
        //    var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        //    int y = 15;
        //    for (int i = 0; i < props.Length; i++)
        //    {
        //        var prop = props[i];
        //        TypeCode typeCode = Type.GetTypeCode(prop.PropertyType);
        //        if (prop.GetCustomAttribute<HideAttribute>(true) != null)
        //        {
        //            continue;
        //        }
        //        //额外标签
        //        var hs = prop.GetCustomAttribute<HeadAttribute>(true);
        //        if (hs != null)
        //        {
        //            Label discard = new Label();
                   
        //            TextBlock title = new TextBlock()
        //            {
        //                Text = $"[{hs.Info}]",
        //            };
                   
                  
        //            container.Children.Add(title);
        //            title.Location = new Point(15, y);
        //            y += title.Height + 5;
        //        }
        //        //别名
        //        var nicekAttr = prop.GetCustomAttribute<NickNameAttribute>(true);
        //        string nickName;
        //        if (nicekAttr != null)
        //        {
        //            nickName = nicekAttr.NickName;
        //            if (string.IsNullOrEmpty(nickName))
        //            {
        //                nickName = prop.Name;
        //            }
        //        }
        //        else
        //        {
        //            nickName = prop.Name;
        //        }
        //        //只读 
        //        bool readOnly = prop.GetCustomAttribute<ReadOnlyAttribute>() == null;
        //        var superClass = prop.GetCustomAttribute<ComboTypeAttribute>();//是否包号额外的组合框类型
        //        var splitAttr = prop.GetCustomAttribute<SplitAttribute>();
        //        var setValueAttr = prop.GetCustomAttribute<SetValueAttribute>();

        //        bool isSpecial = false;//是否为特殊的
        //        UIElement control = null;
        //        PropertyInfo setPropInfo = null;
        //        if (setValueAttr != null)
        //        {
        //            setPropInfo = type.GetProperty(setValueAttr.PropName, BindingFlags.Instance | BindingFlags.Public);

        //        }

        //        if (superClass != null)
        //        {
        //            control = new ComboBox();
        //            var cb = (ComboBox)control;
        //            string[] names;
        //            if (superClass.ShortOfFull)
        //            {
        //                names = Reflection.GetClassDataName(superClass.SuperClass).ToArray();
        //            }
        //            else
        //            {
        //                names = Reflection.GetClassDataFullName(superClass.SuperClass).ToArray();
        //            }
        //            CreateComboBox(target, prop, cb, names, superClass, otherInfo: setPropInfo);
        //        }
        //        else if (splitAttr != null && typeCode == TypeCode.String)
        //        {
        //            control = new ComboBox();
        //            var cb = (ComboBox)control;
        //            string[] names;
        //            var value = prop.GetValue(target);
        //            if (value != null)
        //            {
        //                names = value?.ToString().Split(splitAttr.SplitChart);
        //            }
        //            else
        //            {
        //                names = new string[1] { $"请对'{prop.Name}'以'{splitAttr.SplitChart}'进行配置!" };
        //            }
        //            CreateComboBox(target, prop, cb, names, otherInfo: setPropInfo);
        //        }
        //        else if (prop.PropertyType.IsEnum)
        //        {
        //            control = new ComboBox();
        //            var cb = (ComboBox)control;
        //            CreateComboBox(target, prop, cb, Enum.GetNames(prop.PropertyType), otherInfo: setPropInfo);
        //        }
        //        else if (typeCode == TypeCode.Boolean)
        //        {
        //            control = new CheckBox();
        //            var cb = (CheckBox)control;
        //            cb.Checked = (bool)prop.GetValue(target);
        //            cb.CheckedChanged += (s, e) =>
        //            {
        //                if (setPropInfo != null && setPropInfo != prop)
        //                {
        //                    prop = setPropInfo;
        //                }
        //                prop.SetValue(target, cb.Checked);
        //            };
        //        }
        //        else if (typeCode == TypeCode.Object)
        //        {
        //            if (prop.PropertyType == typeof(RangedFloat))
        //            {
        //                control = new StackPanel();
        //                Label lbl = new Label();
        //                TextBox t1 = new TextBox();
        //                TextBox t2 = new TextBox();
        //                RangedFloat rf = (RangedFloat)prop.GetValue(target);
        //                if (rf == null)
        //                {
        //                    rf = Activator.CreateInstance<RangedFloat>();
        //                }
        //                else
        //                {
        //                    t1.Text = rf.minValue.ToString();
        //                    t2.Text = rf.maxValue.ToString();
        //                }

        //                lbl.Text = "-";
        //                lbl.TextAlign = ContentAlignment.MiddleLeft;
        //                lbl.Width = 8;
        //                t1.Tag = "min";
        //                t1.Dock = DockStyle.Left;
        //                t2.Tag = "max";
        //                t2.Dock = DockStyle.Right;
        //                EventHandler action = (s, e) =>
        //                {
        //                    TextBox tb = (TextBox)s;
        //                    if (tb != null)
        //                    {
        //                        if (setPropInfo != null && setPropInfo != prop)
        //                        {
        //                            prop = setPropInfo;
        //                        }
        //                        try
        //                        {
        //                            var value = float.Parse(tb.Text);
        //                            if (tb.Tag.ToString() == "min")
        //                            {
        //                                rf.minValue = value;
        //                            }
        //                            else if (tb.Tag.ToString() == "max")
        //                            {
        //                                rf.maxValue = value;
        //                            }
        //                            prop.SetValue(target, rf);
        //                        }
        //                        catch
        //                        {
        //                            tb.Text = string.Empty;
        //                            if (tb.Tag.ToString() == "min")
        //                            {
        //                                rf.minValue = 0;
        //                            }
        //                            if (tb.Tag.ToString() == "max")
        //                            {
        //                                rf.maxValue = 0;
        //                            }
        //                            prop.SetValue(target, rf);
        //                        }
        //                    }
        //                };
        //                t1.TextChanged += action;
        //                t2.TextChanged += action;

        //                control.Controls.Add(t1);
        //                control.Controls.Add(t2);
        //                control.Controls.Add(lbl);
        //                control.Height = t1.Height + 7;
        //                control.Width = t1.Width + t2.Width + lbl.Width;
        //                t1.Width = control.Width / 2 - lbl.Width;
        //                t2.Width = control.Width / 2 - lbl.Width;
        //                lbl.Location = new Point(t1.Width, 0);
        //                control.SizeChanged += (s, e) =>
        //                {
        //                    t1.Width = control.Width / 2 - lbl.Width;
        //                    t2.Width = control.Width / 2 - lbl.Width;
        //                    lbl.Location = new Point(t1.Width, 0);
        //                };
        //            }
        //            else
        //            {
        //                control = new Panel();
        //                control.Width = container.Width;
        //                control.BackColor = Color.AliceBlue;
        //                control.CreateControl(prop.PropertyType, prop.GetValue(target));
        //                int height = 0;
        //                foreach (Control item in control.Controls)
        //                {
        //                    height += item.Height;
        //                }
        //                control.Height = height - (int)(height * 0.22727);
        //                isSpecial = true;
        //            }

        //        }
        //        else
        //        {
        //            if (prop.GetCustomAttributes(typeof(SerialAttribute), true).Length > 0)
        //            {
        //                control = new ComboBox();
        //                var cb = (ComboBox)control;
        //                CreateComboBox(target, prop, cb, SerialPort.GetPortNames(), otherInfo: setPropInfo);
        //            }
        //            else
        //            {
        //                control = new TextBox();
        //                var txt = (TextBox)control;
        //                txt.Text = prop.GetValue(target)?.ToString();
        //                txt.TextChanged += (s, e) =>
        //                {
        //                    if (setPropInfo != null && setPropInfo != prop)
        //                    {
        //                        prop = setPropInfo;
        //                    }
        //                    try
        //                    {
        //                        prop.SetValue(target, Convert.ChangeType(txt.Text, prop.PropertyType));
        //                    }
        //                    catch
        //                    {
        //                        txt.Text = string.Empty;
        //                        prop.SetValue(target, null);

        //                    }
        //                };

        //            }
        //        }

        //        if (control != null)
        //        {
        //            int x = 0;

        //            Label label = new Label
        //            {
        //                Text = nickName,
        //                Location = new Point(5, y),
        //                TextAlign = ContentAlignment.MiddleLeft,
        //            };
        //            label.Tag = null;
        //            control.Enabled = readOnly;
        //            container.Controls.Add(label);
        //            container.Controls.Add(control);
        //            x = label.Width = label.GetWidth() + 20;

        //            if (isSpecial)
        //            {
        //                y += label.Height;
        //                control.Tag = "special";
        //                control.Location = new Point(20, y);
        //            }
        //            else
        //            {
        //                control.Tag = "normal";
        //                control.Location = new Point(x + 15, y);
        //            }

        //            y += control.Height + 10;
        //        }

        //    }

        //    container.SizeChanged += (s, e) =>
        //    {
        //        Control lastTag = null;
        //        foreach (Control item in container.Controls)
        //        {
        //            if (item.Tag == null)
        //            {
        //                lastTag = item;
        //                continue;
        //            }
        //            if (item.Tag.ToString() == "special")
        //            {
        //                item.Width = container.Width - 56;
        //            }
        //            else
        //            {
        //                item.Width = container.Width - (lastTag == null ? 100 : lastTag.Width) - 56;
        //            }
        //            lastTag = null;
        //        }
        //    };
        //}
    }
}
