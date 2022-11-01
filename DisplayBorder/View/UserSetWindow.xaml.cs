using DisplayBorder.Model;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ComboBox = System.Windows.Controls.ComboBox;
using TabItem = HandyControl.Controls.TabItem;
using Window = System.Windows.Window;

namespace DisplayBorder.View
{
    /// <summary>
    /// UserSetWindow.xaml 的交互逻辑
    /// </summary>
    public partial class UserSetWindow : Window
    {
        public UserSetWindow()
        {
            InitializeComponent();
            InitializeComponent();
            KeyDown += (s, e) =>
            {
                if (e.IsKeyDown(Key.Escape))
                {
                    Hide();
                } 
            };
            Closing += (s, e) =>
            {
                this.Hide();
                e.Cancel = true;
            };

            if (GlobalPara.Groups!= null && GlobalPara.Groups.Count > 0)
            {
                cloneClassInfo = GlobalPara.ClassInfos.ToList().Clone();
                cmbGroupNames.ItemsSource = GlobalPara.Groups.Select(a => a.GroupName+"_"+a.GroupID  ).ToArray();
                cmbGroupNames.SelectionChanged += CmbGroupNames_SelectionChanged;
                cmbGroupNames.SelectedIndex = 0;
             
            }
        
        }
        private int selectedId;
        private List<ClassInfo> cloneClassInfo;

        private void CmbGroupNames_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox cb)
            {
                selectedId = int.Parse(cb.SelectedItem.ToString().Split('_')[1]);
                var result = cloneClassInfo.Where(a => a.GroupID == selectedId);
                if (result.Count() != 2)
                {
                    if (result.Count() > 2)
                    {
                        foreach (var item in result)
                        {
                            cloneClassInfo.Remove(item);
                        }
                    }
                    for (int i = 0; i < 2; i++)
                    {
                        ClassInfo classInfo = new ClassInfo()
                        {
                            GroupID = selectedId,
                            Name = "张三" + (i + 1),
                            Position = "ME",
                            TelelPhone = "12345678910",
                        };
                        cloneClassInfo.Add(classInfo);
                    }
                }
                 
                int index = 1;
                foreach (var item in tabC.Items)
                {
                    if(item is TabItem tabItem)
                    {
                        if(index == 1)
                        {
                            tabItem.IsSelected = true;
                        }

                        tabItem.Content = null;
                        tabItem.Content = CreateTabItem(result.ToArray()[index - 1]);
                    }
                    index++;
                } 
            }
        }

        private StackPanel CreateTabItem(ClassInfo result)
        {
            StackPanel sp = new StackPanel
            {
                Background = new SolidColorBrush(Colors.White)
            };
            WindowHelper.CreateControl(typeof(ClassInfo), result, sp, this);
            return sp;
        }

        private void Btn_Save(object sender, RoutedEventArgs e)
        {
            GlobalPara.ClassInfos = cloneClassInfo;
            Growl.Info("保存成功");
        }
    }
}
