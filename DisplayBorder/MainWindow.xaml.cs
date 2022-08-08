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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DisplayBorder
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void BackRun()
        {



        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double titlesize = ((ActualWidth / 12) / 3 * 2) / 3;
            System.Windows.Application.Current.Resources.Remove("TitleFontSize");
            System.Windows.Application.Current.Resources.Add("TitleFontSize", titlesize);
            //double tabsize = ((SystemParameters.PrimaryScreenWidth / 12) / 3 * 2) / 5 * 0.9;
            //System.Windows.Application.Current.Resources.Remove("TabFontSize");
            //System.Windows.Application.Current.Resources.Add("TabFontSize", tabsize);
            double gridsize = ((ActualWidth / 12) / 3 * 2) / 5 * 0.8;
            System.Windows.Application.Current.Resources.Remove("GridFontSize");
            System.Windows.Application.Current.Resources.Add("GridFontSize", gridsize);
            //double controlsize = ((SystemParameters.PrimaryScreenWidth / 12) / 3 * 2) / 5 * 0.7;
            //System.Windows.Application.Current.Resources.Remove("ControlFontSize");
            //System.Windows.Application.Current.Resources.Add("ControlFontSize", controlsize);

        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
