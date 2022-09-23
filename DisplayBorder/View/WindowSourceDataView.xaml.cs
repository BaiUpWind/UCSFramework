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

namespace DisplayBorder.View
{
    /// <summary>
    /// WindowSourceDataView.xaml 的交互逻辑
    /// </summary>
    public partial class WindowSourceDataView : Window
    {
        public WindowSourceDataView()
        {
            InitializeComponent();
            KeyDown += (s, e) =>
            {
                if(e.Key == Key.Escape)
                {
                    Hide();
                }
                if(e.SystemKey == Key.F10)
                {
                    var result=   MessageBox.Show("是否清空所有控件", "问一下", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        spFather.Children.Clear();
                    }
                }
            };
            Closing += (s, e) =>
            {
                this.Hide();
                e.Cancel = true;
            };
        }
        public void AddControl(FrameworkElement control)
        {
            if (control == null) return;
            spFather.Children.Add(control);
        }

        public void Claer()
        {
            spFather.Children.Clear();
        }
    }
}
