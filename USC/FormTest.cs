using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CommonApi;
using CommonApi.Event;
using DeviceConfig;
using DeviceConfig.Core;
 

namespace USC
{
    public partial class FormTest : Form
    {
        public FormTest()
        {
            InitializeComponent();
            
          cbNames.DataSource =   Utility.Reflection.GetChildrenNames<Animal>().ToList();
           
            cbFullNames.DataSource = Utility.Reflection.GetChildrenFullNames<Animal>().ToList();

            cbFullNames.SelectedIndexChanged += CbNames_SelectedIndexChanged;
            Type type = Type.GetType("USC.Dog");

            //------------------ 继承多级联动测试

            cd = new ClassData();
            Utility.Reflection.GetInheritors(typeof(OperationBase), ref cd);
            Console.WriteLine("Holle world!");
            //-------------------

            
        }

    

        EventManager eventManager = new EventManager();
        ClassData cd;
        private void CbNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            animal = null;
        }

        Animal animal;
        private void button1_Click(object sender, EventArgs e)
        {
            if (animal == null)
            {
               
                animal = Utility.Reflection.CreateObject<Animal>( 
                  cbFullNames.SelectedItem.ToString()
                    , para: "麻雀");
            }
            animal.Eat();
        }
  
        private void button2_Click(object sender, EventArgs e)
        {
            pAll.Controls.Clear();
            Create(cd);
        }


        Point location = new Point(5, 15);
        public void Create(ClassData data)
        {
            if (data == null) return;
            if (data.ChildrenTypes.Count == 0) return; 
            ComboBox box1 = new ComboBox();
            box1.Name = data.ClassType.Name;
            box1.DropDownStyle = ComboBoxStyle.DropDownList;
            box1.Location = location;
            //创建这个类的所有直接实现的类型
            foreach (var item in data.ChildrenTypes)
            {
                if (item.IsAbstract)
                {
                    var reulst = data.Children.Where(a => a.ClassType == item).FirstOrDefault();
                    if (reulst != null)
                    {
                        location.X += 125;
                        Create(reulst);
                    }
                }
                box1.Items.Add($"{item.Name}{(item.IsAbstract ? "[abstract]" : "")}"); 
            } 
            location.Y += box1.Height; 
            pAll.Controls.Add(box1);

        }  
    }


    public abstract class Animal
    {
        public string name;
        public Animal(string name)
        {
            this.name = name;
        }
        public abstract void Eat();
    }


    public abstract class Dog : Animal
    {
        public Dog(string name) : base(name)
        {
        }

        public override void Eat()
        {
            MessageBox.Show($"狗{name}嘴巴吃");
        }
    }

    public abstract class Cat : Animal
    {
        protected Cat(string name) : base(name)
        {
        }
    }

    public class qqqqqqqCat : Cat
    {
        public qqqqqqqCat(string name) : base(name)
        {
        }

        public override void Eat()
        {
            throw new NotImplementedException();
        }
    }
    public class ddddddCat : Cat
    {
        public ddddddCat(string name) : base(name)
        {
        }

        public override void Eat()
        {
            throw new NotImplementedException();
        }
    }

    public class BigDog : Dog
    {
        public BigDog(string name) : base(name)
        {

        }

        public override void Eat()
        {
            MessageBox.Show("大狗吃");
        }
    }

    public class SmallDog : Dog
    {
        public SmallDog(string name) : base(name)
        {
            MessageBox.Show("小狗吃");
        }
    }


    public abstract class CutDog : Dog
    {
        protected CutDog(string name) : base(name)
        {
        }
    }

    public class LLLLLLDog : CutDog
    {
        public LLLLLLDog(string name) : base(name)
        {
        }
    }

    public class bbbbbbbDog : CutDog
    {
        public bbbbbbbDog(string name) : base(name)
        {
        }
    }


    public class Chicken : Animal
    {
        public Chicken(string name) : base(name)
        {
        }

        public override void Eat()
        {
            MessageBox.Show($"鸡{name}嘴巴啄");
        }
    }

    public class Cow : Animal
    {
        public Cow(string name) : base(name)
        {
        }

        public override void Eat()
        {
            MessageBox.Show($"牛[{name}]咀");
        }
    }
}
