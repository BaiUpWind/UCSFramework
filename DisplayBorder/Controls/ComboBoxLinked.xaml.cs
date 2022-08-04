using CommonApi;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ComboBox = HandyControl.Controls.ComboBox;

namespace DisplayBorder.Controls
{
    /// <summary>
    /// ComboBoxLinked.xaml 的交互逻辑
    /// </summary>
    public partial class ComboBoxLinked : UserControl, ISingleOpen
    {

        public sealed class ComboBoxData
        {
           public  TextBlock Text;
           public ComboBox Combo;

           public void SetVisiable(Visibility value)
            {
                Text.Visibility = value;
                Combo.Visibility = value;
                if(value == Visibility.Visible)
                {
                    Combo.SelectedIndex = 0;
                } 
            }
        }
        public ComboBoxLinked()
        {
            InitializeComponent(); 
            Datas = new List<ComboBoxData>()
            {
                new ComboBoxData()
                {
                    Text= t1,
                    Combo = c1
                },
                new ComboBoxData()
                {
                    Text= t2,
                    Combo = c2
                }, 
                new ComboBoxData()
                {
                    Text= t3,
                    Combo = c3
                },
            };
        }

        private ClassData classData;
        private List<ComboBoxData> Datas ;
        private ComboBoxData currenData; 
        public event Action OnEnter;
        public event Action OnCancel;
        public  object Result { get; private set; }
       

        public bool CanDispose => true;

        public void Dispose()
        {
            currenData = null;
            classData = null;
            OnEnter = null;
            OnCancel = null;
            Datas.Clear();
        }

        public void SetType<T>() where T : class
        {
            try
            {
                classData = Utility.Reflection.GetClassData<T>();
                if (classData != null)
                {

                    Create(classData);
                    GetComboBox(1).SetVisiable(Visibility.Hidden);
                    GetComboBox(2).SetVisiable(Visibility.Hidden);
                 
                }
                else
                {
                    Growl.ErrorGlobal($"未能正确获取到'{typeof(T)}'类型的数据");
                }

            }
            catch (Exception ex)
            {
                Growl.ErrorGlobal(ExceptionOperater.GetSaveStringFromException("设置组合框内容时出现未知异常:", ex));  
            } 
        }
        public T GetType<T>(params object[] para) where T : class
        {  
            if (currenData == null)
            {
                return default;
            }  
            var result = Utility.Reflection.CreateObjectShortName<T>(currenData.Combo.SelectedItem.ToString(), para);
            if (result == null)
            {
                return default;
            }
            else
            {
                return result;
            }
        }

        public void Create(ClassData data)
        {
            if (data == null) return;
            if (data.ChildrenTypes.Count == 0) return;  

            ComboBoxData comboBoxData = GetComboBox(data.LayerIndex-1); 
            comboBoxData.Text.Text = data.ClassType.Name;
            
            comboBoxData.Combo.SelectionChanged += Combo_SelectionChanged;
            foreach (var item in data.ChildrenTypes)
            {
                
                if (item.IsAbstract)
                { 
                    var reulst = data.Children.Where(a => a.ClassType == item).FirstOrDefault();
                    if (reulst != null)
                    {
                        comboBoxData.Combo.Tag = reulst.LayerIndex-1;
                        Create(reulst);
                    }
                }
                comboBoxData.Combo.Items.Add(item.Name+(item.IsAbstract ? "_[A]":"")); 
            } 
        }

        private void Combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(sender is ComboBox combo)
            {
                if (combo.TabIndex == 1 && !combo.SelectedItem.ToString().Contains("_[A]"))
                {
                    GetComboBox(1).SetVisiable(Visibility.Hidden);
                    GetComboBox(2).SetVisiable(Visibility.Hidden);
                }
                else if (combo.SelectedItem.ToString().Contains("_[A]"))
                { 
                    GetComboBox(int.Parse(combo.Tag.ToString())).SetVisiable(Visibility.Visible); 
                }
                else if (!combo.SelectedItem.ToString().Contains("_[A]") && combo.Tag !=null)
                {
                    GetComboBox(int.Parse(combo.Tag.ToString())).SetVisiable(Visibility.Hidden);
                }
            }
           
        }

        private void Btn_Click_Canecl(object sender, RoutedEventArgs e)
        {
            if(sender  is Button btn)
            {
                if (btn.Content.ToString() == "确认") 
                { 
                    currenData = GetLastData();
                    if (currenData != null   &&  currenData.Combo.SelectedIndex <= -1)
                    {
                        Growl.WarningGlobal("请选择类型进行创建!");
                        return ;
                    }
                    OnEnter?.Invoke();
                }
                else if(btn .Content.ToString() == "取消")
                { 
                    OnCancel?.Invoke();
                }
            }
        }

        private ComboBoxData GetComboBox(int layerIndex)
        {
            if(layerIndex >= Datas.Count)
            {
                Growl.ErrorGlobal($"获取组合控件数据时,索引越界'{layerIndex}';检查继承类的层数,是否与控件配置嵌套!");
                return null;
            } 
            return Datas[layerIndex]; 
        }

        private ComboBoxData GetLastData()
        {
            ComboBoxData data = Datas.Where(a=> a.Combo.TabIndex == Datas.Where(b => b.Combo.Visibility == Visibility.Visible).Max(c => c.Combo.TabIndex)).FirstOrDefault(); 
            if (data == null )
            {
                Growl.ErrorGlobal("获取组件数据时出现异常 'GetLastData' 未能获取最后选中的项");
            } 
            return data;
        }

     
    }

 
}
