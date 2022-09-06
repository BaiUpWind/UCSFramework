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
using System.Windows.Threading;
using DisplayBorder.View;
using DisplayBorder.ViewModel;
using HandyControl.Controls;
using MessageBox = HandyControl.Controls.MessageBox;

namespace DisplayBorder
{
    /// <summary>
    /// RGVDataWindow.xaml 的交互逻辑
    /// </summary>
    public partial class RGVDataWindow : HandyControl.Controls.Window
    {
        public RGVDataWindow()
        {
            InitializeComponent();
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            DataContext = ViewModelLocator.Instance ;
            ShowNonClientArea = false;

            #region 计时器
            var dt = new DispatcherTimer();
            dt.Interval = new TimeSpan(1000);
            dt.Tick += (s, e) =>
            {
                txtTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            };
            dt.Start();
            #endregion

        }
 

        private void Btn_Click_FullSwitch(object sender, RoutedEventArgs e)
        {
            if(sender is Button btn)
            {
                if (btn.Tag.ToString() == "全屏") { 

                    IsFullScreen = true;
                  
                    btn.Tag = "复原";

                    if (btn.Content is StackPanel sp )
                    {
                        sp.Children[0].Visibility = Visibility.Collapsed;
                        sp.Children[1].Visibility = Visibility.Visible;
                    }
                }
                else if(btn.Tag.ToString() == "复原")
                {
                    IsFullScreen = false;
                    btn.Tag = "全屏";
                    if (btn.Content is StackPanel sp )
                    {
                        sp.Children[0].Visibility = Visibility.Visible;
                        sp.Children[1].Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        private void Btn_Click_Close(object sender, RoutedEventArgs e)
        {
            var result=   MessageBox.Ask("请确认关闭");
            if (result == MessageBoxResult.OK)
            {
                Close();
                Application.Current.Shutdown();
            }
        }

        private void Btn_Click_OpenConfig(object sender, RoutedEventArgs e)
        {
            WindowGroupsConfig wgc = new WindowGroupsConfig();
            wgc.WindowState = WindowState.Maximized;
            wgc.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            wgc.ShowDialog();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
