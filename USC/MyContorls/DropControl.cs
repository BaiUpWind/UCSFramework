using CommonApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USC.MyContorls
{
    public partial class DropControl : UserControl
    {
        private string title;
        private object data;
        private object genericData;
        private object selectedItemTemp;
        private  Control selectedControl;
        private  Type genericArgument;// 集合的泛型类型 
        //private readonly Type collType;// 获取到集合类型
        private   Type orginType;
        private readonly bool isList  ; 
        private readonly bool isProperty  ;
        /// <summary>
        /// 是否为数组或者集合的子类
        /// </summary>
        public readonly bool IsArrayChild;

        /// <summary>
        /// 存放子类控件
        /// </summary>
        private List<Control> childen  =new List<Control>() ; 
        /// <summary>
        /// 存放 子集合控件/包括泛型
        /// </summary>
        private List<Control> drops = new List<Control>();
        public DropControl()
        {
            InitializeComponent(); 
        }
        public DropControl(Type type,bool isPorperty = true,object target = null,bool isChildren = false)
        {
            InitializeComponent();
            Height = 25;
            isProperty = isPorperty;
            orginType = type;
            this.IsArrayChild = isChildren;
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
            isList = CreateHelper.IsList(type); 
            lblSelectedInfo.Text = string.Empty;
       
        }
         
        /// <summary>
        /// 当展开时 传出当前控件,宽和高
        /// </summary>
        public event Action<object,int, int> OnUnfold;
        /// <summary>
        /// 当折叠时 传出当前控件,宽和高
        /// </summary>
        public event Action<object, int,int> OnFold;
 
     
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

        private Color GetTitleColor(bool islist) => islist ? Color.SkyBlue : Color.Teal;
        private int GetPDataHeight()
        {
            int height = 0;
            foreach (Control item in pData.Controls)
            {
                height += item.Location.Y;
            }
            if (pData.Controls.Count == 0) return 35;
            height = pData.Controls[pData.Controls.Count - 1].Location.Y + pData.Controls[pData.Controls.Count - 1].Height + 5;
            return height;
        }
        #region 计算方法
        private void Flod()
        {
            btnShow.Text = "↑";
            btnShow_Click(null, null);
        }
        private void UnFlod()
        {
            btnShow.Text = "↓";
            btnShow_Click(null, null);
        }
        private object GetValue(string name)
         => isProperty
        ? orginType.GetProperty(name, BindingFlags.Instance | BindingFlags.Public).GetValue(Data)
        : orginType.GetField(name, BindingFlags.Instance | BindingFlags.Public).GetValue(Data);

        private void SetValue(string name, object value)
        {
            if (isProperty)
            {
                orginType.GetProperty(name, BindingFlags.Instance | BindingFlags.Public).SetValue(Data, value);
            }
            else
            {
                orginType.GetField(name, BindingFlags.Instance | BindingFlags.Public).SetValue(Data, value);
            } 
        }

        private void CreateControls(Panel panel, TypeData[] typeDatas)
        {
            panel.Controls.Clear();
            for (int i = 0; i < typeDatas.Length; i++)
            {
                var td = typeDatas[i];
                if (td == null) continue;
                Panel fixedPanel = new Panel();
                Control control = null;
                switch (td.ControlType)
                {
                    case ControlType.TextBox:
                        control = new TextBox();
                        control.Text = GetValue(td.Name)?.ToString();
                        if(control is TextBox txt)
                        {
                            txt.KeyPress += (ts, te) =>
                             {
                                 //todo:这里类型会有不同的适配
                                 if (ts is TextBox t)
                                 {
                                     SetValue(td.Name, Convert.ChangeType((t.Text), td.ObjectType));
                                 }
                             };
                        }
                        break;
                    case ControlType.CheckBox:
                        control = new CheckBox();
                        if (control is CheckBox cb)
                        {
                            cb.Checked = (bool)GetValue(td.Name);
                            cb.CheckedChanged += (s, e) =>
                            {
                                SetValue(td.Name,cb.Checked);
                            };
                        }
                        break;
                    case ControlType.List: 
                    case ControlType.Class: 
                        control = new DropControl(td.ObjectType,isProperty,GetValue(td.Name));
                        if (control is DropControl c)
                        {  
                            c.OnFold += (s, w, h) =>
                            {
                                Height -= h;
                                if (s is Button btn   )
                                {
                                    var dc = GetTop(btn);
                                    if(dc != null)
                                    {
                                        dc.Parent.Height -= h;
                                    } 
                                }
                                ReCalcuY(childen, childen[0].Location.Y);
                            };
                            c.OnUnfold += (s, w, h) =>
                            { 
                                Height += h;
                                if (s is Button btn)
                                {
                                    var dc = GetTop(btn);
                                    if (dc != null)
                                    {
                                        dc.Parent.Height += h;
                                    }
                                }
                                ReCalcuY(childen, childen[0].Location.Y);
                              
                            };
                            c.pData.BackColor = Color.Tan;
                            c.Title = td.Name + (td.IsList ?  "_{0}" :"");
                        }
                      
                        break;
                    case ControlType.ComboBox: 
                        control = new ComboBox();
                        if(control is ComboBox combo)
                        {
                            combo.DropDownStyle = ComboBoxStyle.DropDownList;
                            combo.Items.AddRange( Enum.GetNames(td.ObjectType)); 
                            combo.SelectedIndexChanged += (s, e) =>
                            {
                                SetValue(td.Name,Enum.Parse(td.ObjectType,combo.SelectedItem.ToString()));
                            };
                            combo.SelectedIndex = (int)GetValue(td.Name);
                            if (combo.Items.Count > 0) combo.SelectedIndex = 0;
                        } 
                        break;
                    case ControlType.ComboBoxImplement:
                        control = new ComboBox();
                        if (control is ComboBox ci)
                        {
                            ci.DropDownStyle = ComboBoxStyle.DropDownList;  
                            ci.Items.AddRange( Utility.Reflection.GetClassDataFullName(td.ObjectType).ToArray());
                            var result = GetValue(td.Name);
                            if(result != null)
                            {
                                int index = 0;
                                for (int j = 0; j  < ci.Items.Count; j++)
                                {
                                    var item = ci.Items[j];
                                    if(item.ToString() == result.ToString())
                                    {
                                        index = j;
                                        break;
                                    } 
                                }
                                ci.SelectedIndex = index;
                            } 
                            ci.SelectedIndexChanged += (s, e) =>
                            {
                                if (s is ComboBox comboI)
                                {
                                    try
                                    {
                                        SetValue(td.Name, Utility.Reflection.CreateObject(td.ObjectType, comboI.SelectedItem.ToString()));
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show($"创建对象失败!目前只支持无参构造.\n\r错误信息:'{ex.Message}'"); 
                                    }
                                }
                            }; 
                        } 
                        break;
                }  
                if(control != null)
                {
                    int x = 0;
                    if(td.ControlType != ControlType.List && td.ControlType != ControlType.Class)
                    {
                        Label label = new Label();
                        label.Text = td.Name;
                        label.Location = new Point(0,   0);
                        label.TextAlign = ContentAlignment.MiddleLeft;
                        x = label.Width + 10;
                        fixedPanel.Controls.Add(label); 
                    }
                    else
                    {
                        x += 5;
                    }
                    control.Location = new Point(x,  0); 
                    fixedPanel.BackColor = Color.Crimson;
                    fixedPanel.Location = new Point(5, 10 + i * 25);
                    fixedPanel.Controls.Add(control);
                    fixedPanel.Height = control.Height; 
                    panel.Controls.Add(fixedPanel);
                    childen.Add(fixedPanel); 
                }
            }
        }

        DropControl GetTop(Control control)
        {
            if (control == null) return null;
            DropControl result = null;
            if (control.Parent is DropControl  dc) return   dc;
            if(control.Parent != null)
            {
                result = GetTop(control.Parent);
            }
            return result;
        }

        #endregion
        #region 事件


        private void btnShow_Click(object sender, EventArgs e)
        {
            if (btnShow.Text == "↓")
            {
                //展开
                btnShow.Text = "↑";
                pData.Visible = true;
                Height = pTitle.Height + GetPDataHeight() ; 
                if (isList)
                {
                    Height += pOper.Height;
                    pOper.Visible = true;
                }

                OnUnfold?.Invoke(sender, Width, Height); 
            }
            else if (btnShow.Text == "↑")
            {
                int tempHeight = Height;
                //收起
                btnShow.Text = "↓"; 
                pData.Visible = false;
                pOper.Visible = false;
                Height = pTitle.Height;
                if (isList) pOper.Visible = false;
                OnFold?.Invoke(sender, Width, tempHeight);
             
                if (childen == null) return;
                foreach (var item in childen)
                {
                    if(item is DropControl dc)
                    {
                        dc.Flod();
                    } 
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
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
                DropControl dc = GetGenericControl(drops,startIndex, genericData, true);
                pData.Controls.Add(dc);
                drops.Add(dc);
            }
            else if (genericArgument.IsEnum)
            {
                //枚举类型
                AddComboBox(list, startIndex);
            }
            else
            {
                if (code == TypeCode.Boolean)
                {
                    AddCheckBox(list, startIndex);
                }
                else
                {
                    AddTextBox(list, startIndex);
                }
            }
             
            //重新计算Y
            ReCalcuY(drops);
            Height = pTitle.Height + GetPDataHeight() + pOper.Height;
            //名字重新计算
            var temp = Title.Split('_');
            temp[1] = $"_[{((IList)data).Count}]";
            Title = string.Empty;
            foreach (var item in temp)
            {
                Title += item;
            }
            OnUnfold?.Invoke(this, Width, Height);

        }
         
        private void btnSub_Click(object sender, EventArgs e)
        {
            if (!isList ) return;
            if (drops.Count <= 0) return;

            //当没有选择时 默认选择第一个
            if (selectedItemTemp == null)
            { 
                selectedControl = drops[0];
                if(selectedControl is DropControl dc)
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
            drops.Remove( selectedControl   );
            pData.Controls.Remove(  selectedControl );
            ReCalcuY(drops);
            Height = pTitle.Height + GetPDataHeight() + pOper.Height;
            //名字更新数量
            var temp = Title.Split('_');
            temp[1] = $"_[{((IList)data).Count}]";
            Title = string.Empty;
            foreach (var item in temp)
            {
                Title += item;
            }
            selectedItemTemp = null;
            selectedControl = null;
            OnUnfold?.Invoke(this, Width, Height);
        }

        private void DropControl_Load(object sender, EventArgs e)
        {
            if (orginType == null) return;

            pTitle.Height = 25;
            pTitle.BackColor = GetTitleColor(isList);

            pOper.Height = 25;
            pOper.Visible = false;
            pOper.BackColor = Color.Transparent;



            Height = pTitle.Height;
            ReCalcuY(drops);

            pData.Click += (ms, me) =>
            {
                selectedItemTemp = null;
                selectedControl = null;
                lblSelectedInfo.Text = string.Empty;
            };
            if (!isList) CreateControls(pData, CreateHelper.Auto(orginType, false));
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
                if (code == TypeCode.Object)
                { 
                    for (int i = 0; i < list.Count; i++)
                    {
                        var item = list[i];
                        DropControl dc = GetGenericControl(drops, i, item, true);
                        pData.Controls.Add(dc);
                        drops.Add(dc);
                    }
                    pData.Click += (ms, me) =>
                    {
                        selectedItemTemp = null;
                        selectedControl = null;
                        lblSelectedInfo.Text = string.Empty;
                    };
                  
                    pData.BackColor = Color.GreenYellow;
                }
                else
                { 
                    if (genericArgument.IsEnum)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            AddComboBox(list, i);
                        }
                    }
                    else if (code == TypeCode.Boolean)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            AddCheckBox(list, i);
                        }
                    }
                    else
                    {
                        //todo :对每种类型做不同的正则表达式
                        for (int i = 0; i < list.Count; i++)
                        {
                            AddTextBox(list, i);
                        }
                    }
                }
                Title = string.Format(Title, $"[{((IList)data).Count}]");
            }  
        }
          
        private void AddComboBox(IList list, int i)
        {
            Panel panel = new Panel();
            ComboBox combo = new ComboBox();
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.Name = "combo_" + i.ToString();
            combo.Items.AddRange(Enum.GetNames(genericArgument));
            combo.SelectedIndex = (int)list[i];
            combo.SelectedIndexChanged += (cs, ce) =>
            {
                if (cs is ComboBox cb)
                    list[int.Parse(cb.Name.Split('_')[1])] = Enum.Parse(genericArgument, cb.SelectedItem.ToString());
            };
            combo.Location = new Point(10, 0);
            panel.Click += (s, e) =>
            {
                if (s is Panel p && p.Controls[0] is ComboBox c)//&& e is MouseEventArgs me  )
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
            panel.Location = new Point(0, i * 25 + 5);
            panel.Controls.Add(combo);
            panel.Height = combo.Height;
            panel.BackColor = Color.Pink;
            pData.Controls.Add(panel);
            drops.Add(panel);
        } 
        private void AddTextBox(IList list, int i)
        {
            Panel panel = new Panel();
            TextBox textBox = new TextBox();
            textBox.Name = "txt_" + i.ToString();
            textBox.Text = list[i]?.ToString();
            textBox.KeyPress += (ts, te) =>
            {
                //todo:这里类型会有不同的适配
                if (ts is TextBox t)
                    list[int.Parse(t.Name.Split('_')[1])]=Convert.ChangeType( (t.Text  ),genericArgument);
            };
            textBox.Location = new Point(10, 0);
            panel.Click += (s, e) =>
            {
                if (s is Panel p && p.Controls[0] is TextBox t)//&& e is MouseEventArgs me  )
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
            panel.Location = new Point(0, i * 25 + 5);
            panel.Controls.Add(textBox);
            panel.Height = textBox.Height;
            panel.BackColor = Color.Pink;
            pData.Controls.Add(panel);
            drops.Add(panel);
        } 
        private void AddCheckBox(IList list, int i)
        {
            Panel panel = new Panel();
            CheckBox checkBox = new CheckBox();
            checkBox.Checked = (bool)list[i];
            checkBox.Name = "checkb_" + i.ToString();
            checkBox.CheckedChanged += (cs, ce) =>
            {
                if (cs is CheckBox cb)
                    list[int.Parse(cb.Name.Split('_')[1])] =(checkBox.Checked );
            };
            checkBox.Location = new Point(10, 0);
            panel.Click += (s, e) =>
            {
                if (s is Panel p && p.Controls[0] is CheckBox c)//&& e is MouseEventArgs me  )
                {
                    selectedItemTemp = c.Checked;
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
            panel.Location = new Point(0, i * 25 + 5);
            panel.Controls.Add(checkBox);
            panel.Height = checkBox.Height;
            panel.BackColor = Color.Pink;
            pData.Controls.Add(panel);
            drops.Add(panel);
        }
        private DropControl GetGenericControl(List<Control> drops, int i, object item,bool ischildren =false)
        {
            DropControl dc = new DropControl(genericArgument, isProperty, item, ischildren);
            dc.Name = "dc_" + i.ToString();
            dc.Title = genericArgument.Name + $"'{i}'"; 
            dc.pTitle.MouseEnter += (ms, me) =>
            {
                Cursor = Cursors.Hand;
            };
            dc.pTitle.MouseLeave += (ms, me) =>
            {
                Cursor = Cursors.Arrow;
            };
            dc.pTitle.Click += (ms, me) =>
            {
                if (ms is Control c && c.Parent is DropControl drop)
                {
                    selectedItemTemp = drop.Data;
                    selectedControl = drop;
                    Console.WriteLine(selectedItemTemp);

                    lblSelectedInfo.Text = $"当前选中'{drop.Title}'";
                }
            };
            dc.OnFold += (s, w, h) =>
            {
                Height -= h;
                if (s is Button btn && btn.Parent != null && btn.Parent is Panel p)
                {
                    p.Height -= h;
                }
                ReCalcuY(drops);
            };
            dc.OnUnfold += (s, w, h) =>
            {
                Height += h;
                if(s is Button btn && btn.Parent !=null && btn.Parent is Panel p)
                {
                    p.Height += h;
                }
                ReCalcuY(drops);
            };
            dc.pData.BackColor = Color.Tan;
            return dc;
        }

       
        /// <summary>
        /// 重新计算y轴
        /// </summary>
        /// <param name="h"></param>
        /// <param name="drops"></param>
        private void ReCalcuY(  List<Control> drops,int orginY = 5)
        { 
            for (int j = 0; j < drops.Count; j++)
            { 
                if (drops[j] is DropControl drop)
                { 
                    if (drop.IsArrayChild) drop.Title = drop.orginType?.Name + $"_'{j}'"; 
                }
                int y;
                if (j == 0)
                {
                    y = orginY;
                }
                else
                {
                    y = drops[j - 1].Location.Y + drops[j - 1].Height + 5;
                }
                drops[j].Location = new Point(5, y);
            }
     
        }
        #endregion

    }
}
public static class TypeExMothod
{
    public static Type GetArrayElementType(this Type t)
    {
        if (!t.IsArray) return null;

        string tName = t.FullName.Replace("[]", string.Empty);

        Type elType = t.Assembly.GetType(tName);

        return elType;
    }
}

public class TypeData
{
    /// <summary>
    /// 这个字段的名称属性或者名称
    /// </summary>
    public string Name;
    public TypeCode TypeCode = TypeCode.Empty;
    public ControlType ControlType;
    public bool IsObject;
    /// <summary>
    /// 是否为集合
    /// </summary>
    public bool IsList;
    public bool IsGeneric;
    public Type ObjectType;
    public Type GenericType;
  
    public Regex InputRegex;

}
public enum ControlType
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
    ///  超类所有的实现
    /// </summary>
    ComboBoxImplement,
}
public   static  class CreateHelper
{
   
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
    public static TypeData[] Auto(Type type, bool checkProp = true, params object[] para)
    { 
        TypeData[] td;
        if (checkProp)
        {
            //获取公共的属性
            var props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            td = new TypeData[props.Length];
            for (int i = 0; i < props.Length; i++)
            {

            }
        }
        else
        {
            //获取公共的字段
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
            td = new TypeData[fields.Length];
            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                TypeCode typeCode = Type.GetTypeCode(field.FieldType);
               

                Console.WriteLine($"当前字段名称'{field.Name}',类型'{typeCode}'");
                td[i] = new TypeData()
                {
                    Name = field.Name,
                    TypeCode = typeCode,
                    ObjectType = field.FieldType
                };
                if(field.FieldType.IsAbstract)
                {
                    td[i].ControlType = ControlType.ComboBoxImplement; 
                    continue;
                }
                if (field.FieldType .IsEnum)
                { 
                    td[i].ControlType = ControlType.ComboBox; 
                    continue;
                }
                if (typeCode == TypeCode.Object)
                {
                    bool islist = IsList(field.FieldType); 
                    td[i].IsList = islist;
                    td[i].IsObject = true; 
                    //这里只用两种可能,因为只对未知的类型和集合类型进行拆解
                    td[i].ControlType = islist ? ControlType.List : ControlType.Class;
                     
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
                    //            td[i].IsGeneric = true;
                    //            td[i].GenericType = field.FieldType.GenericTypeArguments[0];
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
                    td[i].ControlType = ControlType.CheckBox;
                }
                else
                {
                    td[i].ControlType = ControlType.TextBox;
                } 
            }
        }
        return td;
    }
}
