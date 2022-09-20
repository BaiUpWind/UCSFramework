using System;
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
using USC.MyContorls;

namespace USC
{
    public partial class FormUnityInspector : Form
    {
       
        public FormUnityInspector()
        {
            InitializeComponent();
            //Auto (typeof(Data),false );

            DropControl dc = new DropControl(typeof(Data), false);
            dc.Dock = DockStyle.Fill;
            inspector.Controls.Add(dc);


        }
        public class TypeData
        {
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
            /// <summary>
            /// 这个字段的名称属性或者名称
            /// </summary>
            public string Name;
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
        /// <summary>
        /// 判断该类型是否继承了<see cref="System.Collections.IList "/>
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool IsList(Type type)
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
        private TypeData[] Auto (Type type, bool checkProp = true, params object[] para)  
        {
            //创建实例
            object target  = Activator.CreateInstance(type, para);
           
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
                        var value = field.GetValue(target);

                        td[i].IsList = islist;
                        td[i].IsObject = true;
                        td[i].ObjectType = field.FieldType;
                        td[i].Name = type.Name;

                        //这里只用两种可能,因为只对未知的类型和集合类型进行拆解
                        td[i].ControlType = islist ? ControlType.List : ControlType.Class;
                  
                        //创建对应的实例 好像没啥用....
                        if (islist)
                        {
                            //对集合创建实例
                            if (value == null)
                            {
                                if (field.FieldType.IsGenericType)
                                {
                                    if (field.FieldType.GenericTypeArguments.Length != 1)
                                    {
                                        throw new Exception("目前只对单个泛型集合进行处理");
                                    }
                                    td[i].IsGeneric = true;
                                    td[i].GenericType = field.FieldType.GenericTypeArguments[0];
                                    //泛型集合 只对单个泛型进行操作
                                    var generics = typeof(List<>).MakeGenericType(field.FieldType.GenericTypeArguments);
                                    value = Activator.CreateInstance(generics);

                                }
                                else
                                {
                                    //数组
                                    value = Activator.CreateInstance(field.FieldType, 0);

                                
                                  
                                  
                                }
                            }
                        }
                        else
                        {
                            //对类型进行处理
                            if (value == null)
                            {
                                value = Activator.CreateInstance(field.FieldType);
                            }
                        }
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

    public class Person
    {
        public string Sex;
        public int Age;
        //public Monster Data;
    }

    public class Monster
    {
        public string hp;
        public string mp;
        public string attackDmg;
        public Monster2 monster2;
        public List<string> monsters;
    }
    public class Monster2
    {
        public string hp;
        public string mp;
        public string attackDmg;
    }

    public class Data : DataBase
    {
        public int ID = 1001;
        public string Name = "哈哈";
        public bool Check = true;
        public Person Charlie;
        //public Person Jsone = new Person()
        //{
        //    Age = 18,
        //    Sex = "男",
        //};
        public List<Person> Strings = new List<Person>()
        {
           new Person()
            {
                Age = 18,
                Sex = "男",
            },
           new Person()
            {
                Age = 11,
                Sex = "女",
            }
        };
        //public int[] Ints;
        private float speed;
        public string Description { get; set; }
    }

    public abstract class DataBase: IData
    {
        public string OrginName { get; set; }
        public string DataName { get  ; set  ; }
    }

    public interface IData
    {
        string DataName { get; set; }
    }
}
