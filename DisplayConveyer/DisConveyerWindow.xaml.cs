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
using System.Windows.Media.Animation;
using Config.DeviceConfig.Models;
using System.Net.NetworkInformation;
using DisplayConveyer.Controls;
using ControlHelper;
using ScrollViewer = System.Windows.Controls.ScrollViewer;

namespace DisplayConveyer
{ 
    /// <summary>
    /// DisConveyerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DisConveyerWindow : Window
    {
        /// <summary>
        /// 子滚动 与 主滚动的水平比例系数
        /// </summary>
        private double ScrollHorizontalOffSetFactor => topSv.ScrollableWidth / mainSv.ScrollableWidth;
        /// <summary>
        /// 子gird 与 主gird的宽度比例系数
        /// </summary>
        private double RectWidthFactor => (topGrid.Width / mainGrid.Width);
        private ConveyerConfig ConvConfig => GlobalPara.ConveyerConfig;
        private ReadStatusLogic logic;
        private double AnimationSpeed =55d;
        private Storyboard storyboard = new Storyboard();
        public DisConveyerWindow()
        {
            InitializeComponent();
            Loaded += DisConveyerWindow_Loaded;
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 1)
            };
            timer.Start();
            timer.Tick += (s, e) =>
            {
                txtTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            };
            timer.Start();
            selectRect.RenderTransform = new TranslateTransform(0, 0);
            ReLoad(mainCanvas, mainGrid);
            ReLoad(topCanvas, topGrid);
            mainSv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            btnClose.Click += (s, e) =>
            {
                //Close();//调试代码
                var result = MessageBox.Ask("请确认是否关闭", "询问");
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
                storyboard.Children.Remove(animation);
                storyboard.Stop();
                animation = null;
                mainSv.ScrollToHorizontalOffset(0);
                topSv.ScrollToHorizontalOffset(0);
                logic.Stop();
                logic = null;
                ew.ShowDialog(); 
       
                ReLoad(mainCanvas, mainGrid);
                ReLoad(topCanvas, topGrid);
                Calculate(mainCanvas, mainSv, mainGrid);
                Calculate(topCanvas, topSv, topGrid);
                SvHorizontalOffsetToRight();
            };
            SizeChanged += (s, e) =>
            {
                Calculate(mainCanvas, mainSv, mainGrid);
                Calculate(topCanvas, topSv, topGrid);
                if (mainCanvas.Width != 0)
                { 
                    selectRect.Width = mainSv.ActualWidth * RectWidthFactor; 
                }
            };
          
            mainSv.ScrollChanged += (s, e) =>
            {
                var offset = e.HorizontalOffset * RectWidthFactor;
                if (selectRect.RenderTransform is TranslateTransform t)
                {
                    //当前框的位置偏移
                    var tempOffset = offset ; 
                    //当前主滚动相对于top滚动已经偏移了多少
                    var lastOffset = ( mainSv.HorizontalOffset) * ScrollHorizontalOffSetFactor;
                    //topSv.ScrollToHorizontalOffset(e.HorizontalOffset * ScrollFactor); 
                    if (tempOffset + selectRect.Width >= topSv.ActualWidth)
                    {
                        //如果超过边界
                        topSv.ScrollToHorizontalOffset(e.HorizontalOffset * ScrollHorizontalOffSetFactor );
                    }
                    else
                    {
                    
                        if (topSv.HorizontalOffset  > 0  && e.HorizontalChange < 0)
                        {
                            topSv.ScrollToHorizontalOffset(e.HorizontalOffset * ScrollHorizontalOffSetFactor - lastOffset);
                        }
                        else
                        {
                            t.X = tempOffset;
                        }
                           
                    }
                }
            };
         
            txtLock.Click += (s, e) =>
            {
                if (s is Button txt)
                { 
                    if (txt.Tag.ToString() == "锁住")
                    {
                        storyboard.Pause();
                        GlobalPara.Locked = true;
                        mainSv.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                        txt.Tag = "解锁";
                        txt.Content = "🔓";
                        txt.ToolTip = "切换自动轮播";
                    }
                    else if (txt.Tag.ToString() == "解锁")
                    {
                        storyboard.Resume();
                        GlobalPara.Locked = false; 
                        mainSv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                        txt.Tag = "锁住";
                        txt.Content = "🔒";
                        txt.ToolTip = "切换手动轮播";
                    }
                }
            };
        }

        private void ScrollTimer_Tick(object sender, EventArgs e)
        {
            mainSv.ScrollToHorizontalOffset(mainSv.HorizontalOffset +1.5d);
        }

        DoubleAnimation animation;
        private void SvHorizontalOffsetToRight()
        {
            ScrollHorizontalMoveTo(0, mainSv.ScrollableWidth, (s, e) =>
            {
                HorizontalOffsetToLeft();
            });
        }
        private void HorizontalOffsetToLeft()
        {
            ScrollHorizontalMoveTo(mainSv.ScrollableWidth, 0, (s, e) =>
            {
                SvHorizontalOffsetToRight();
            });
        }
     
        private void ScrollHorizontalMoveTo(double from,double to , Action<object, EventArgs> animationCompleted = null)
        {
            animation = new DoubleAnimation()
            {
                From = from,
                To = to,
                Duration = new Duration(TimeSpan.FromSeconds(mainSv.ScrollableWidth / AnimationSpeed)),
                FillBehavior = FillBehavior.Stop 
            };
      
            animation.Completed += (s, e) =>
            {
                animationCompleted?.Invoke(s, e);
                storyboard.Children.Remove(animation);
            };
            //var HorizontalOffsetPropertyKey = DependencyProperty.RegisterReadOnly("HorizontalOffset", typeof(double), typeof(ScrollViewer), new FrameworkPropertyMetadata(0.0));

            //mainSv.BeginAnimation(ScrollViewerBehaviour.HorizontalOffsetProperty, animation);
 
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation,mainSv);
            Storyboard.SetTargetProperty(animation, new PropertyPath(ScrollViewerBehaviour.HorizontalOffsetProperty ));
            storyboard.Begin();
        }
        //重新计算缩放比例
        private void Calculate(Canvas mainCanvas,FrameworkElement father,Grid grid)
        {
            double x = 0; 
            var factor = GetHeightFactor(father, mainCanvas);
            mainCanvas.RenderTransform = new MatrixTransform(factor, 0, 0, factor, x, 0);
            if (!double.IsNaN(grid.Width) && grid.Width > 0) grid.Width = ConvConfig.CanvasWidth * factor; 

        }
        private void ReLoad(Canvas canvas,Grid grid)
        {
            if (ConvConfig == null)
            {
                //todo:提示错误
                return;
            }
            canvas.Children.Clear();
            grid.Width = canvas.Width = ConvConfig.CanvasWidth;
            grid.Height=  canvas.Height = ConvConfig.CanvasHeight;
            foreach (var area in ConvConfig.Areas)
            {
                foreach (var device in area.Devices)
                {
                    var ucdevice = CreateHelper.GetDeviceBase(device);
                    canvas.Children.Add(ucdevice);
                    ucdevice.ToolTip = (ucdevice as UC_DeviceBase).Info;
                    //device.StatusChanged?.Invoke(new StatusData() { MachineState = 100 });
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
            if (logic != null) logic.Stop();
            logic = new ReadStatusLogic(ConvConfig.Areas);
        }
        private double GetHeightFactor(FrameworkElement father, FrameworkElement ui) => father.ActualHeight / ((ui.ActualHeight == 0 ? 1 : ui.ActualHeight) + 5);
        private void DisConveyerWindow_Loaded(object sender, RoutedEventArgs e)
        {

            BtnFullScreen_Click(btnFullScreen, null);
            SvHorizontalOffsetToRight();
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

        private void btn_TestStatus(object sender, RoutedEventArgs e)
        {
            var areaId = txtAreaID.Text.CastTo<uint>(1001);
            var workid = txtWorkID.Text ;
            var machineState = txtMachineStatus.Text.CastTo(100);
            var loadState = txtLoadStatus.Text.CastTo(1);
            var areas = ConvConfig.Areas.Find(a => a.ID == areaId);
            if (areas != null)
            {
                var device = areas.Devices.Find(a => a.WorkId == workid); 
                device?.StatusChanged?.Invoke(new StatusData()
                {
                    MachineState = machineState,
                    LoadState = loadState,
                });
            }
        
        }

        private void sliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sender is Slider slider)
            {
                if (txtSpeed != null) txtSpeed.Text = $"速度:{slider.Value:#00.0}";
                if (txtInfo != null) txtInfo.Text = slider.Value.ToString();
                AnimationSpeed = slider.Value;
                if (mainSv != null)
                {
                    //animation.SetValue(DoubleAnimation.DurationProperty, new Duration(TimeSpan.FromSeconds(mainSv.ScrollableWidth / AnimationSpeed)));
                    //storyboard.Duration = new Duration(TimeSpan.FromSeconds(mainSv.ScrollableWidth / AnimationSpeed));
                    storyboard.SetValue(DoubleAnimation.DurationProperty, new Duration(TimeSpan.FromSeconds(mainSv.ScrollableWidth / AnimationSpeed)));
                    //animation.Duration = new Duration(TimeSpan.FromSeconds(mainSv.ScrollableWidth / AnimationSpeed));
                }
            }
        }
    }
    public class ScrollViewerBehaviour
    {

        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.RegisterAttached("Horizontalofset",
          typeof(double),
          typeof(ScrollViewerBehaviour),
          new UIPropertyMetadata(0d, new PropertyChangedCallback(OnHorizontalofsetchanged)));

        public static void SetHorizontalofset(ScrollViewer element, double value)
        {
            element.SetValue(HorizontalOffsetProperty, value);
        }
        public static double GetHorizontalofset(ScrollViewer element)
        {
            return (double)element.GetValue(HorizontalOffsetProperty);
        }

        public static void OnHorizontalofsetchanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var scrollviewer = (sender as ScrollViewer);
            scrollviewer?.ScrollToHorizontalOffset((double)e.NewValue);
            //scrollviewer.ChangeView((double)e.NewValue, scrollviewer.VerticalOffset, scrollviewer.ZoomFactor);
        }

    }
}
