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
using HandyControl.Controls;
using MessageBox = HandyControl.Controls.MessageBox;
using Window = System.Windows.Window;
using DisplayConveyer.View;
using DisplayConveyer.Config;
using DisplayConveyer.Utilities;
using DisplayConveyer.Logic;

namespace DisplayConveyer
{ 
    /// <summary>
    /// DisConveyerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DisConveyerWindow : Window
    {
        private ConveyerConfig ConvConfig => GlobalPara.ConveyerConfig;
        private ReadStatusLogic logic;
        public DisConveyerWindow()
        {
            InitializeComponent();
            Loaded += DisConveyerWindow_Loaded;
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 1)
            };
            timer.Tick += (s, e) =>
            {
                txtTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            };
            timer.Start();
            btnClose.Click += (s, e) =>
            {
                var result = MessageBox.Ask("请确认是否关闭","询问");
                if (result == MessageBoxResult.OK)
                {
                    Close();
                    Application.Current.Shutdown();
                }
            };
            MouseLeftButtonDown += (s, e) =>
            {
                DragMove();
            };
            btn_OpenConfig.Click += (s, e) =>
            {
                EditorWindow ew = new EditorWindow();
                ew.WindowState = WindowState.Maximized;
                ew.ShowDialog();
                ReLoad(gd);
                ReLoad(topCanvas);
            }; 
            SizeChanged += (s, e) =>
            {
                Calculate(gd,sv);
                Calculate(topCanvas,topGrid);
            };
            gd.SizeChanged += (s, e) =>
            {
                Calculate(gd,sv); 
            };
            topCanvas.SizeChanged += (s, e) =>
            {
                Calculate(topCanvas, topGrid);
                //var factor = gd.ActualWidth / ((ui.ActualHeight == 0 ? 1 : ui.ActualHeight) + 5);
            };
        }
        //重新计算缩放比例
        private void Calculate(Canvas gd,FrameworkElement father)
        {
            double x = 0; 
            var factor = GetHeightFactor(father, gd);
            gd.RenderTransform = new MatrixTransform(factor, 0, 0, factor, x, 0);
        
        }
        private void ReLoad(Canvas canvas)
        {
            if (ConvConfig == null)
            {
                //todo:提示错误
                return;
            }
            canvas.Children.Clear();
            canvas.Width = ConvConfig.CanvasWidth;
            canvas.Height = ConvConfig.CanvasHeight;
            foreach (var area in ConvConfig.Areas)
            {
                foreach (var device in area.Devices)
                {
                    canvas.Children.Add(CreateHelper.GetDeviceBase(device));
                }
            }
            foreach (var rect in ConvConfig.RectDatas)
            {
                canvas.Children.Add(CreateHelper.GetRect(rect));
            }
            foreach (var label in ConvConfig.Labels)
            {
                canvas.Children.Add(CreateHelper.GetTextBlock(label));
            }
            //if (logic != null) logic.Stop();
            //logic = new ReadStatusLogic(ConvConfig.Areas);
        }
        private double GetHeightFactor(FrameworkElement father, FrameworkElement ui) => father.ActualHeight / ((ui.ActualHeight == 0 ? 1 : ui.ActualHeight) + 5);
        private void DisConveyerWindow_Loaded(object sender, RoutedEventArgs e)
        { 
            ReLoad(gd);
            ReLoad(topCanvas);
             
        }

        private void BtnFullScreen_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn != null)
            {
                if (btn.Tag.ToString() == "全屏")
                {
                    this.WindowState = WindowState.Maximized;

                    btn.Tag = "复原";

                    if (btn.Content is StackPanel sp)
                    {
                        sp.Children[0].Visibility = Visibility.Collapsed;
                        sp.Children[1].Visibility = Visibility.Visible;
                    }
                }
                else if (btn.Tag.ToString() == "复原")
                {
                    this.WindowState = WindowState.Normal;
                    btn.Tag = "全屏";
                    if (btn.Content is StackPanel sp)
                    {
                        sp.Children[0].Visibility = Visibility.Visible;
                        sp.Children[1].Visibility = Visibility.Collapsed;
                    }
                }
            }
        }
    }
}
