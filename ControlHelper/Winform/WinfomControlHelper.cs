using ControlHelper.Attributes;
using ControlHelper.Interfaces;
using ControlHelper.Model;
using System;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Reflection; 
using System.Windows.Forms;
using MessageBox = System.Windows.Forms.MessageBox;
using Point = System.Drawing.Point; 

namespace ControlHelper.Winform
{
    public static class WinfomControlHelper
    {
        #region 创建控件 
        /// <summary>
        /// 在容器控件上创建泛型类含有<see cref="LabelAttribute"/>标签属性的控件
        /// </summary> 
        public static void CreateLabel<T>(this Control container, T target) where T : class, IPropertyChanged
        {
            if (target == null) return;
            container.Controls.Clear();
            Point ledLocation = new Point(0, 0);
            int index = 0;
            int x = 20;

            var props = target.GetType().GetProperties();
            foreach (var prop in props)
            {
                Console.WriteLine(prop.Name);
                var attrs = prop.GetCustomAttributes(true);
                if (attrs.Where(a => a is ColumonFeedAttribute).Count() > 0)
                {
                    //如果是换列属性
                    x += 130;
                    index = 0;
                }
                var la = attrs.Where(a => a is LabelAttribute).Select(a => a as LabelAttribute).FirstOrDefault();
                if (la != null)
                {
                    ledLocation = new Point(x, 25 * index + 10);

                    Label lbl = new Label();
                    lbl.Text = la.Info;
                    lbl.BorderStyle = BorderStyle.FixedSingle;
                    lbl.ForeColor = Color.Black;
                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                    lbl.BackColor = Color.Transparent;
                    lbl.Location = ledLocation;
                    target.OnPropertyChanged += (s, v) =>
                    {
                        if (s == prop.Name)
                        {
                            try
                            {
                                var value = (bool)v;
                                if (value)
                                {
                                    lbl.BackColor = Color.Green;
                                }
                                else
                                {
                                    lbl.BackColor = Color.Transparent;
                                }
                            }
                            catch
                            {
                                lbl.BackColor = Color.Red;
                            }
                        }
                    };
                    container.Controls.Add(lbl);
                    lbl.Width = lbl.GetWidth() + 16;
                    index++;

                }
            }
        }
        /// <summary>
        /// 在容器控件上根据泛型类中的属性创建对应的控件
        /// </summary> 
        public static void CreateControl<T>(this Control container, T target = null) where T : class
         => CreateControl(target, typeof(T), container);
        /// <summary>
        /// 建议使用泛型那个
        /// </summary> 
        public static void CreateControl(this Control container, Type type, object target = null) => CreateControl(target, type, container);
        private static void CreateControl(object target, Type type, Control container)
        {
            if (type == null || container == null) return;
            if (target == null)
            {
                target = Activator.CreateInstance(type);
            }
            container.Controls.Clear();
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            int y = 15;
            for (int i = 0; i < props.Length; i++)
            {
                var prop = props[i];
                TypeCode typeCode = Type.GetTypeCode(prop.PropertyType);
                if (prop.GetCustomAttribute<HideAttribute>(true) != null)
                {
                    continue;
                }
                //额外标签
                var hs = prop.GetCustomAttribute<HeadAttribute>(true);
                if (hs != null)
                {
                    Label discard = new Label();
                    Label title = new Label
                    {
                        Text = $"[{hs.Info}]",
                        Tag = "i have one",
                        TextAlign = ContentAlignment.MiddleLeft

                    };

                    title.Font = new System.Drawing.Font("宋体", container.Font.Size + 2, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
                    discard.Visible = false;
                    container.Controls.Add(discard);
                    container.Controls.Add(title); 
                    title.Location = new Point(15, y);
                    y += title.Height + 5;
                }
                //别名
                var nicekAttr = prop.GetCustomAttribute<NickNameAttribute>(true);
                string nickName;
                if (nicekAttr != null)
                {
                    nickName = nicekAttr.NickName;
                    if (string.IsNullOrEmpty(nickName))
                    {
                        nickName = prop.Name;
                    }
                }
                else
                {
                    nickName = prop.Name;
                }
                //只读 
                bool readOnly = prop.GetCustomAttribute<ReadOnlyAttribute>() == null;
                var superClass = prop.GetCustomAttribute<ComboTypeAttribute>();//是否包号额外的组合框类型
                var splitAttr = prop.GetCustomAttribute<SplitAttribute>();
                var setValueAttr = prop.GetCustomAttribute<SetValueAttribute>();

                bool isSpecial = false;//是否为特殊的
                Control control = null;
                PropertyInfo setPropInfo = null;
                if (setValueAttr != null)
                {
                    setPropInfo = type.GetProperty(setValueAttr.PropName, BindingFlags.Instance | BindingFlags.Public);

                }

                if (superClass != null)
                {
                    control = new ComboBox();
                    var cb = (ComboBox)control;
                    string[] names;
                    if (superClass.ShortOfFull)
                    {
                        names = Reflection.GetClassDataName(superClass.SuperClass).ToArray();
                    }
                    else
                    {
                        names = Reflection.GetClassDataFullName(superClass.SuperClass).ToArray();
                    }
                    CreateComboBox(target, prop, cb, names, superClass, otherInfo: setPropInfo);
                }
                else if (splitAttr != null && typeCode == TypeCode.String)
                {
                    control = new ComboBox();
                    var cb = (ComboBox)control;
                    string[] names;
                    var value = prop.GetValue(target);
                    if (value != null)
                    {
                        names = value?.ToString().Split(splitAttr.SplitChart);
                    }
                    else
                    {
                        names = new string[1] { $"请对'{prop.Name}'以'{splitAttr.SplitChart}'进行配置!" };
                    }
                    CreateComboBox(target, prop, cb, names, otherInfo: setPropInfo);
                }
                else if (prop.PropertyType.IsEnum)
                {
                    control = new ComboBox();
                    var cb = (ComboBox)control;
                    CreateComboBox(target, prop, cb, Enum.GetNames(prop.PropertyType), otherInfo: setPropInfo);
                }
                else if (typeCode == TypeCode.Boolean)
                {
                    control = new CheckBox();
                    var cb = (CheckBox)control;
                    cb.Checked = (bool)prop.GetValue(target);
                    cb.CheckedChanged += (s, e) =>
                    {
                        if (setPropInfo != null && setPropInfo != prop)
                        {
                            prop = setPropInfo;
                        }
                        prop.SetValue(target, cb.Checked);
                    };
                }
                else if (typeCode == TypeCode.Object)
                {
                    if (prop.PropertyType == typeof(RangedFloat))
                    {
                        control = new Panel();
                        Label lbl = new Label();
                        TextBox t1 = new TextBox();
                        TextBox t2 = new TextBox();
                        RangedFloat rf = (RangedFloat)prop.GetValue(target);
                        if (rf == null)
                        {
                            rf = Activator.CreateInstance<RangedFloat>();
                        }
                        else
                        {
                            t1.Text = rf.minValue.ToString();
                            t2.Text = rf.maxValue.ToString();
                        }

                        lbl.Text = "-";
                        lbl.TextAlign = ContentAlignment.MiddleLeft;
                        lbl.Width = 8;
                        t1.Tag = "min";
                        t1.Dock = DockStyle.Left;
                        t2.Tag = "max";
                        t2.Dock = DockStyle.Right;
                        EventHandler action = (s, e) =>
                        {
                            TextBox tb = (TextBox)s;
                            if (tb != null)
                            {
                                if (setPropInfo != null && setPropInfo != prop)
                                {
                                    prop = setPropInfo;
                                }
                                try
                                {
                                    var value = float.Parse(tb.Text);
                                    if (tb.Tag.ToString() == "min")
                                    {
                                        rf.minValue = value;
                                    }
                                    else if (tb.Tag.ToString() == "max")
                                    {
                                        rf.maxValue = value;
                                    }
                                    prop.SetValue(target, rf);
                                }
                                catch
                                {
                                    tb.Text = string.Empty;
                                    if (tb.Tag.ToString() == "min")
                                    {
                                        rf.minValue = 0;
                                    }
                                    if (tb.Tag.ToString() == "max")
                                    {
                                        rf.maxValue = 0;
                                    }
                                    prop.SetValue(target, rf);
                                }
                            }
                        };
                        t1.TextChanged += action;
                        t2.TextChanged += action;

                        control.Controls.Add(t1);
                        control.Controls.Add(t2);
                        control.Controls.Add(lbl);
                        control.Height = t1.Height + 7;
                        control.Width = t1.Width + t2.Width + lbl.Width;
                        t1.Width = control.Width / 2 - lbl.Width;
                        t2.Width = control.Width / 2 - lbl.Width;
                        lbl.Location = new Point(t1.Width, 0);
                        control.SizeChanged += (s, e) =>
                        {
                            t1.Width = control.Width / 2 - lbl.Width;
                            t2.Width = control.Width / 2 - lbl.Width;
                            lbl.Location = new Point(t1.Width, 0);
                        };
                    }
                    else
                    {
                        control = new Panel();
                        control.Width = container.Width;
                        control.BackColor = Color.AliceBlue;
                        control.CreateControl(prop.PropertyType, prop.GetValue(target));
                        int height = 0;
                        foreach (Control item in control.Controls)
                        {
                            height += item.Height;
                        }
                        control.Height = height - (int)(height * 0.22727);
                        isSpecial = true;
                    }

                }
                else
                {
                    if (prop.GetCustomAttributes(typeof(SerialAttribute), true).Length > 0)
                    {
                        control = new ComboBox();
                        var cb = (ComboBox)control;
                        CreateComboBox(target, prop, cb, SerialPort.GetPortNames(), otherInfo: setPropInfo);
                    }
                    else
                    {
                        control = new TextBox();
                        var txt = (TextBox)control;
                        txt.Text = prop.GetValue(target)?.ToString();
                        txt.TextChanged += (s, e) =>
                        {
                            if (setPropInfo != null && setPropInfo != prop)
                            {
                                prop = setPropInfo;
                            }
                            try
                            {
                                prop.SetValue(target, Convert.ChangeType(txt.Text, prop.PropertyType));
                            }
                            catch
                            {
                                txt.Text = string.Empty;
                                prop.SetValue(target, null);

                            }
                        };

                    }
                }

                if (control != null)
                {
                    int x = 0;

                    Label label = new Label
                    {
                        Text = nickName,
                        Location = new Point(5, y),
                        TextAlign = ContentAlignment.MiddleLeft,
                    };
                    label.Tag = null;
                    control.Enabled = readOnly;
                    container.Controls.Add(label);
                    container.Controls.Add(control);
                    x = label.Width = label.GetWidth() + 20;

                    if (isSpecial)
                    {
                        y += label.Height;
                        control.Tag = "special";
                        control.Location = new Point(20, y);
                    }
                    else
                    {
                        control.Tag = "normal";
                        control.Location = new Point(x + 15, y);
                    }

                    y += control.Height + 10;
                }

            }

            container.SizeChanged += (s, e) =>
            {
                Control lastTag = null;
                foreach (Control item in container.Controls)
                {
                    if (item.Tag == null)
                    {
                        lastTag = item;
                        continue;
                    }
                    if (item.Tag.ToString() == "special")
                    {
                        item.Width = container.Width - 56;
                    }
                    else
                    {
                        item.Width = container.Width - (lastTag == null ? 100 : lastTag.Width) - 56;
                    }
                    lastTag = null;
                }
            };
        } 
        private static void CreateComboBox(object target, PropertyInfo prop, ComboBox cb, string[] names, ComboTypeAttribute ctAttr = null, PropertyInfo otherInfo = null)
        {
            cb.Items.AddRange(names);
            cb.DropDownStyle = ComboBoxStyle.DropDownList;
            var result = prop.GetValue(target, null)?.ToString();
            if (result != null)
            {
                int index = 0;
                for (int j = 0; j < cb.Items.Count; j++)
                {
                    var item = cb.Items[j];
                    if (item.ToString() == result)
                    {
                        index = j;
                        break;
                    }
                }
                cb.SelectedIndex = index;
            }
            else if (cb.Items.Count > 0)
            {
                cb.SelectedIndex = 0;
            }

            cb.SelectedIndexChanged += (s, e) =>
            {
                var c = s as ComboBox;
                if (c == null) return;
                if (c.SelectedIndex <= -1) return;
                if (otherInfo != null && otherInfo != prop)
                {
                    prop = otherInfo;
                }
                if (prop.PropertyType.IsEnum)
                {
                    prop.SetValue(target, Enum.Parse(prop.PropertyType, cb.SelectedItem.ToString()));
                }
                else
                {
                    if (Type.GetTypeCode(prop.PropertyType) == TypeCode.Object)
                    {
                        try
                        {
                            var path = cb.SelectedItem.ToString();
                            object cClass;
                            if (ctAttr.ShortOfFull)
                            {
                                cClass = Reflection.CreateObjectShortName(ctAttr.SuperClass, path);
                            }
                            else
                            {
                                cClass = Reflection.CreateObject(ctAttr.SuperClass, path);
                            }
                            prop.SetValue(target, cClass);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"创建对应实现类时，发生错误,'{ex.Message}'");
                            prop.SetValue(target, null);
                        }

                    }
                    else
                    {
                        prop.SetValue(target, Convert.ChangeType(cb.SelectedItem.ToString(), prop.PropertyType));
                    }
                }

            };
        }
        #endregion
        #region 更新控件
        public static void UpdateRemoveAdd(this Control control, bool type, Control other)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new Action<Control, bool, Control>(UpdateRemoveAdd), control, type, other);
            }
            else
            {
                if (type)
                {
                    control.Controls.Add(other);
                }
                else
                {
                    control.Controls.Remove(other);
                }
            }
        }
        public static void UpdateText(this Control txt, string info)
        {
            if (txt.InvokeRequired)
            {
                txt.Invoke(new Action<Control, string>(UpdateText), txt, info);
            }
            else
            {
                txt.Text = info;
            }
        }

        public static void UpdateEnable(this Control control, bool enable)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new Action<Control, bool>(UpdateEnable), control, enable);
            }
            else
            {
                control.Enabled = enable;
            }
        }
        public static void UpdateVisiable(this Control control, bool visiable)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new Action<Control, bool>(UpdateVisiable), control, visiable);
            }
            else
            {
                control.Visible = visiable;
            }
        }
        /// <summary>
        /// 更改该控件下的所有子控件启用/禁用
        /// </summary> 
        public static void UpdateChildrenEnable(this Control control, bool enable)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new Action<Control, bool>(UpdateChildrenEnable), control, enable);
            }
            else
            {
                foreach (Control children in control.Controls)
                {
                    children.Enabled = enable;
                }
            }
        }

        public static void UpdateBackColor(this Control control, Color color)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new Action<Control, Color>(UpdateBackColor), control, color);
            }
            else
            {
                control.BackColor = color;
            }
        }

        #endregion

        public static int GetWidth(this Label lbl) => TextRenderer.MeasureText(lbl.Text, lbl.Font).Width + 15;
        public static int TxtWidth(this TextBox txt) => TextRenderer.MeasureText(txt.Text, txt.Font).Width;
    }
}
