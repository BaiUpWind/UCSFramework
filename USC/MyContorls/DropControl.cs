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
        private object selectItemTemp;
        private  Type genericArgument;// 集合的泛型类型 
        //private readonly Type collType;// 获取到集合类型
        private   Type orginType;
        private bool isList =true;
        private bool isProperty  ;

        /// <summary>
        /// 存放下级的集合
        /// </summary>
        private List<DropControl> childen  ; 
        /// <summary>
        /// 存放泛型的子集合
        /// </summary>
        private List<DropControl> drops = new List<DropControl>();
        public DropControl()
        {
            InitializeComponent(); 
        }
        public DropControl(Type type,bool isPorperty = true,object target = null)
        {
            InitializeComponent();
            Height = 25;
            isProperty = isPorperty;
            orginType = type;
            if (target == null)
            {
                data = Activator.CreateInstance(type);
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
       
            return pData.Controls[pData.Controls.Count -1].Location.Y + pData.Controls[pData.Controls.Count - 1].Height +5;
        }
        #region 计算方法
        private void Flod()
        {
            btnShow.Text = "↑";
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
                Control control = null;
                switch (td.ControlType)
                {
                    case ControlType.TextBox:
                        control = new TextBox();
                        control.Text = GetValue(td.Name)?.ToString();
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
                        if (childen == null) childen = new List<DropControl>();
                        control = new DropControl(td.ObjectType,isProperty,GetValue(td.Name));
                        if (control is DropControl c)
                        {  
                            c.OnFold += (s, w, h) =>
                            {
                                Height -= h;
                                ReCalcuY(childen, childen[0].Location.Y);
                            };
                            c.OnUnfold += (s, w, h) =>
                            { 
                                Height += h;
                                ReCalcuY(childen, childen[0].Location.Y);
                            };

                            c.pData.BackColor = Color.Tan;
                            c.Title = td.Name + (td.IsList ?  "_{0}" :"");
                            childen.Add(c);
                            
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
                        label.Location = new Point(10, 10 + i * 25);
                        label.TextAlign = ContentAlignment.MiddleLeft;
                        x = label.Width + 10;
                        panel.Controls.Add(label);
                    }
                    else
                    {
                        x += 10;
                    }
                    control.Location = new Point(x, 10 + i * 25); 
                    panel.Controls.Add(control);
             
                }
            }
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

                OnUnfold?.Invoke(this,Width, Height);
              
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
                OnFold?.Invoke(this, Width, tempHeight);
             
                if (childen == null) return;
                foreach (var item in childen)
                {
                    item.Flod();
                }
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!isList) return;

            if (genericArgument == null)
            {
                 //数组类型

                 
            }
            else
            {
                //泛型 
                genericData = Activator.CreateInstance(genericArgument); 
                ((IList)data).Add(genericData); 
                var dc=  GetGenericControl(drops, pData.Controls.Count  , genericData) ;
                drops.Add(dc);
                pData.Controls.Add(dc); 
                Height = pTitle.Height + GetPDataHeight()  +  pOper.Height; ;

               
           
                var temp = Title.Split('_');
                temp[1] = $"_[{((IList)data).Count}]";
                Title = string.Empty;
                foreach (var item in temp)
                {
                    Title += item;
                }
                ReCalcuY(drops);
            }
        }

        private void btnSub_Click(object sender, EventArgs e)
        {
            if (!isList ||selectItemTemp ==null) return;
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

            if (!isList) CreateControls(pData, CreateHelper.Auto(orginType, false)); 
            else 
            {
                //对泛型类型做处理
                if (orginType.IsGenericType)
                {
                    if (orginType.GenericTypeArguments.Length != 1)
                    {
                        throw new Exception("目前只对单个泛型集合进行处理");
                    }
                    genericArgument = orginType.GenericTypeArguments[0];
                    if (Type.GetTypeCode(genericArgument) == TypeCode.Object)
                    { 
                        var list = (IList)data;
                   
                        for (int i = 0; i < list.Count; i++)
                        {
                            var item = list[i];
                            //pData.AutoScroll = true;
                           
                            if (item.GetType() == genericArgument)
                            {
                                DropControl dc = GetGenericControl(drops, i, item); 
                                pData.Controls.Add(dc); 
                                drops.Add(dc);
                            }
                        } 
                        pData.Click += (ms, me) =>
                        {
                            selectItemTemp = null;
                            lblSelectedInfo.Text = string.Empty;
                        };
                        ReCalcuY(drops); 
                    }

                    Title = String.Format(Title, $"[{((IList)data).Count}]");
                }
                else
                {
                    //数组
                }

                pData.BackColor = Color.GreenYellow;
            }

        }

        private DropControl GetGenericControl(List<DropControl> drops, int i, object item)
        {
            DropControl dc = new DropControl(genericArgument, isProperty, item);
            dc.Title = genericArgument.Name + $"'{i}'";
            //dc.Location = new Point(10, i * dc.Height +5 );
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
                    selectItemTemp = drop.Data;
                    Console.WriteLine(selectItemTemp);

                    lblSelectedInfo.Text = $"当前选中'{drop.Title}'";
                }
            };
            dc.OnFold += (s, w, h) =>
            {
                Height -= h;
                ReCalcuY(drops);
            };
            dc.OnUnfold += (s, w, h) =>
            {
                Height += h;
                ReCalcuY(drops);
            };
            dc.pData.BackColor = Color.Tan;
            return dc;
        }

        /// <summary>
        /// 创新
        /// </summary>
        /// <param name="h"></param>
        /// <param name="drops"></param>
        private void ReCalcuY(  List<DropControl> drops,int orginY = 5)
        {
          
            for (int j = 0; j < drops.Count; j++)
            {
                var drop = drops[j];
                int y  ;
                if (j == 0)
                {
                    y = orginY;
                }
                else
                {
                    y = drops[j - 1].Location.Y + drops[j - 1].Height +5 ;
                } 
                drop.Location = new Point(10, y);
            }
        }
        #endregion

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
                };
                if (typeCode == TypeCode.Object)
                {
                    bool islist = IsList(field.FieldType);
                  

                    td[i].IsList = islist;
                    td[i].IsObject = true;
                    td[i].ObjectType = field.FieldType; 

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


                //Label label = new Label();
                //label.Text = field.Name;
                //label.Location = new Point(10, 10 + i *25);
                //Control coreControl =null;

                //switch (typeCode)
                //{
                //    case TypeCode.Empty:
                //        break;
                //    case TypeCode.Object:

                //        break;
                //    case TypeCode.DBNull:
                //        break;
                //    case TypeCode.Boolean:
                //        coreControl = new CheckBox();
                //        CheckBox cb = (CheckBox)coreControl;
                //        cb.Checked = (bool)field.GetValue(target);
                //        cb.CheckedChanged += (s, e) =>
                //        {
                //            field.SetValue(target, cb.Checked);
                //        };

                //        break;
                //    case TypeCode.Char:
                //        break;
                //    case TypeCode.SByte:
                //        break;
                //    case TypeCode.Byte:
                //        break;
                //    case TypeCode.Int16:
                //        break;
                //    case TypeCode.UInt16:
                //        break;
                //    case TypeCode.Int32:
                //        coreControl = new TextBox();
                //        coreControl.Text = field.GetValue(target).ToString();
                //        //txt.KeyPress += (s, e) =>
                //        //{

                //        //    if ((e.KeyChar >= '0' && e.KeyChar <= '9') || ((Keys)e.KeyChar == Keys.Back) )
                //        //    {
                //        //        e.Handled = false;
                //        //    }
                //        //    else
                //        //    {


                //        //        if (!Regex.IsMatch(txt.Text, @"^(-?[0-9]*[.]*[0-9]*)$"))
                //        //        {
                //        //            e.Handled = true;
                //        //        }
                //        //    }

                //        //};


                //        break;
                //    case TypeCode.UInt32:
                //        break;
                //    case TypeCode.Int64:
                //        break;
                //    case TypeCode.UInt64:
                //        break;
                //    case TypeCode.Single:

                //        break;
                //    case TypeCode.Double:
                //        break;
                //    case TypeCode.Decimal:
                //        break;
                //    case TypeCode.DateTime:
                //        break;
                //    case TypeCode.String:
                //        coreControl = new TextBox();
                //        coreControl.Text = field.GetValue(target).ToString();
                //        break;
                //    default:
                //        break;
                //}
                //label.TextAlign = ContentAlignment.MiddleLeft;
                //inspector.Controls.Add(label);

                //if (coreControl != null)
                //{
                //    coreControl.Location = new Point(label.Width + 10, 10 + i * 25);
                //    coreControl.Width = 150;
                //    inspector.Controls.Add(coreControl);
                //} 
                //break;

            }
        }
        return td;
    }
}
