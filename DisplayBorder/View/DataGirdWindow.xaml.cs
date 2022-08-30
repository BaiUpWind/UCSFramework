using DisplayBorder.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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

namespace DisplayBorder.View
{
    /// <summary>
    /// DataGirdWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DataGirdWindow : Window, IControlData
    {
        public DataGirdWindow()
        {
            InitializeComponent();
        }

        public event Action<object> OnSet;
        public event Action OnClose;

        public object GetData() => null;

        public void SetData(object data)
        {
            OnSet?.Invoke(data);
            if (data != null && data is DataTable dt)
            {
                DataView dv = new DataView(dt);
                dg1.ItemsSource = dv;
            }
            else if (data is IEnumerable list)
            {
                dg1.ItemsSource = list;
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            OnClose?.Invoke();
        }
    }
}
