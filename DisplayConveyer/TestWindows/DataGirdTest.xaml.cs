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
using System.Reflection;
using System.Collections;

namespace DisplayConveyer.TestWindows
{
    /// <summary>
    /// DataGirdTest.xaml 的交互逻辑
    /// </summary>
    public partial class DataGirdTest : Window
    {
        private class Student
        {
            public int ID { get; set; }
            public string Name { get; set; }    
            public string NickName { get; set; }

        }

        private List<Student> students = new List<Student>(); 
        public DataGirdTest()
        {
            InitializeComponent();
            for (int i = 0; i < 10; i++)
            {
                students.Add(new Student()
                {
                    ID = i + 1,
                    Name = $"name_{i + 1}",
                    NickName = $"nickName_{i + 1}"

                }); ;
            }
            dgv.ItemsSource = students;
            dgv.SelectedCellsChanged += (s, e) =>
            {
                Console.WriteLine(s.GetHashCode());
            };

        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        { 
            if (e.Command.Equals(ApplicationCommands.Paste))
            { 
                string pasteText = Clipboard.GetText(); 
                if (string.IsNullOrEmpty(pasteText))
                    return;
                var cells = dgv.SelectedCells;
                if (cells==null || cells.Count == 0) return;
                var cell = cells.First();
                int rowIndex = 0, columnIndex = 0;
                if (!GetCellXY(dgv, ref rowIndex, ref columnIndex)) return;
                //获取当前所有的内容
 
                var list = dgv.ItemsSource as IList ;
                //获取集合中元素的类型
                var type = list.Count > 0?list[0].GetType() : cell.GetType();
                
                string[] allRow = pasteText.Trim().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < allRow.Length; i++)
                {
                    if (string.IsNullOrEmpty(allRow[i]))
                    {
                        continue;
                    }
                    if (rowIndex >= list.Count)
                    {
                        //超过索引就添加新的行数
                        try
                        {
                            var newItem = Activator.CreateInstance(type);
                            if (newItem != null)
                                list.Add(newItem);
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    }
                 
                    var row = allRow[i].Split(new string[] { "\t" }, StringSplitOptions.RemoveEmptyEntries); 
                    int tempColumnIndex = 0;
                    for (int j = columnIndex; j < dgv.Columns.Count; j++)
                    {
                        var item = list[rowIndex];
                        var column = dgv.Columns[j];
                        var prop = type.GetProperty(column.Header.ToString());
                       
                        try
                        {
                            prop.SetValue(item, Convert.ChangeType(row[tempColumnIndex++], prop.PropertyType));
                        }
                        catch  
                        {
                           
                        } 
                    }
                    rowIndex++;
                }
               
            
                dgv.ItemsSource = null;
                dgv.ItemsSource = list ;
            }
        }

      

        private bool GetCellXY(DataGrid dg, ref int rowIndex, ref int columnIndex)
        {
            var _cells = dg.SelectedCells;
            if (_cells.Any())
            {
                rowIndex = dg.Items.IndexOf(_cells.First().Item);
                //这里默认+1是因为有删除这行
                columnIndex = _cells.First().Column.DisplayIndex+1;
                return true;
            }
            return false;
        }
        private void CommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var cells = dgv.SelectedCells;
            if (cells == null || cells.Count == 0) return;

            var list = dgv.ItemsSource as IList;
            if (list.Count == 0) return;
            IList tempList = Activator.CreateInstance(typeof(List<>).MakeGenericType(new Type[] { list[0].GetType() })) as IList;

            for (int i = 0; i < cells.Count; i++)
            {
                var cell = cells[i];
                if(tempList  != null && !tempList.Contains(cell.Item))
                {
                    tempList.Add(cell.Item); 
                } 
            }
            foreach (var item in tempList)
            {
                list.Remove(item);
            }
            dgv.ItemsSource = null;
            dgv.ItemsSource = list;
          

        }
    }
}
