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
using DisplayConveyer.Controls;
using ControlHelper;
using ScrollViewer = System.Windows.Controls.ScrollViewer;
using DisplayConveyer.Model;  

namespace DisplayConveyer
{ 
    /// <summary>
    /// DisConveyerWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DisConveyerWindow : Window
    { 
        
        private ConveyerConfig ConvConfig => GlobalPara.ConveyerConfig;
        private ReadStatusLogic logic; 
        private Storyboard storyboard = new Storyboard();
        private FrameworkElement feCacheScale;
        //存放所有的区域数据 小地图用
        private readonly List<RangeData> listRangeDatas = new List<RangeData>(); 
        //区域判定框
        private readonly List<FrameworkElement> listRangeRect = new List<FrameworkElement>();
        //裁切的所有图像
        private readonly List<FrameworkElement> listCutImg = new List<FrameworkElement>();
        
        DoubleAnimation animation;
        public DisConveyerWindow()
        {
            InitializeComponent(); 
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

            Loaded += (s, e) =>
            {
                BtnFullScreen_Click(btnFullScreen, null);
                TxtLock_Click(txtLock, null);
                CreateCanvasDatas();
                SvHorizontalOffsetToRight();
            };
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
                ew.WindowStyle = WindowStyle.None;
                ew.WindowState = WindowState.Maximized;
                storyboard.Children.Remove(animation);
                storyboard.Stop();
                animation = null; 
                topSv.ScrollToHorizontalOffset(0);
                logic?.Stop();
                logic = null;
                ew.ShowDialog(); 
                CreateCanvasDatas();
                SvHorizontalOffsetToRight();
                txtLock.Tag = "解锁";
                TxtLock_Click(txtLock, null);
            };
            SizeChanged += (s, e) =>
            { 
                Calculate(topCanvas, topSv, topGrid); 
            };
            topGrid.Width = topCanvas.Width = ConvConfig.CanvasWidth;
            topGrid.Height = topCanvas.Height = ConvConfig.CanvasHeight; 
            txtLock.Click += TxtLock_Click;
        }
        private void SvHorizontalOffsetToRight()
        {
            ScrollHorizontalMoveTo(0, -1920, (s, e) =>
            {
                
            });
        } 

        private void ScrollHorizontalMoveTo(double from,double to , Action<object, EventArgs> animationCompleted = null)
        {
            animation = new DoubleAnimation()
            {
                From = from,
                To = to,
                 RepeatBehavior =    RepeatBehavior.Forever 
            };
      
            animation.Completed += (s, e) =>
            {
                animationCompleted?.Invoke(s, e);
                storyboard.Children.Remove(animation);
            };
            storyboard.Children.Add(animation);
            Storyboard.SetTarget(animation, uc_scMain);
            Storyboard.SetTargetProperty(animation, new PropertyPath(UC_ScrollCanvas.HorizontalOffsetProperty ));
             
            storyboard.Begin();
        }
        //重新计算缩放比例
        private void Calculate(Canvas mainCanvas,FrameworkElement father,Grid grid)
        {
            double x = 0; 
            var factor = GetHeightFactor(father, mainCanvas);
            mainCanvas.RenderTransform = new MatrixTransform(factor, 0, 0, factor, x, 0);
            if (!double.IsNaN(grid.ActualWidth) && grid.ActualWidth > 0) grid.Width = ConvConfig.CanvasWidth * factor; 

        }
        /// <summary>
        /// 计算小地图选中框
        /// </summary>
        private void CalculateRange()
        {
            if (imgBack.Source == null)
            {
                Growl.Error("略缩图未能正常获取,请在[设置-编辑略缩图-选择图片]中进行配置!");
                return;
            }
            listRangeRect.Clear();
            listRangeDatas.Clear(); 

            foreach (var item in listCutImg)
            {
                topCanvas.Children.Remove(item);
            }
            listCutImg.Clear();
            foreach (var part in ConvConfig.MiniMapData)
            {
                #region 获取区域数据 
                List<AreaData> areas = null;
                foreach (var ids in part.AreaIDs)
                {
                    var area = ConvConfig.Areas.Find(a => a.ID == ids);
                    if (area != null)
                    {
                        if (areas == null) areas = new List<AreaData>();
                        areas.Add(area);
                    }
                }
                if (areas != null && areas.Any())
                {
                    RangeData rd = new RangeData();
                    rd.MapPartId = part.ID;
                    foreach (var area in areas)
                    {
                        var tempMin = area.Devices.Min(r => r.PosX - 50);
                        var tempMax = area.Devices.Max(r => r.PosX + r.Width + 50);
                        if (rd.MinPosX == 0 || rd.MinPosX >= tempMin)
                        {
                            rd.MinPosX = tempMin;
                        }
                        if (rd.MaxPosX == 0 || rd.MaxPosX <= tempMax)
                        {
                            rd.MaxPosX = tempMax;
                        }
                    }
                    listRangeDatas.Add(rd);
                }
                #endregion
                topCanvas.Children.Add(CreateCutImage(part)) ; 
            }
        }
        /// <summary>
        /// 创建剪切之后的图片
        /// </summary> 
        /// <param name="part"></param>
        private FrameworkElement CreateCutImage(MapPartData part)
        {
            var bs = imgBack.Source; 
            Grid grid = new Grid();
            Image img = new Image();
            img.Height = bs.Height.CastTo(50);
            img.Width = part.Width;
            img.Margin = new Thickness(5);
            //grid.MouseEnter += Img_MouseEnter;
            //grid.MouseLeave += Img_MouseLeave;
            var rect = new Int32Rect(part.PosX.CastTo(0), 0, part.Width.CastTo(50), bs.Height.CastTo(50));
            CroppedBitmap cb = new CroppedBitmap((BitmapSource)bs, rect);
            img.Source = cb;
            TextBlock tb = new TextBlock()
            {
                Margin = new Thickness(0, 0, 0, 0),
                FontSize = 92,
                Text = part.Title,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.Black),
            };
            grid.SetValue(Canvas.LeftProperty, part.PosX);
            grid.SetValue(Canvas.TopProperty, part.PosY); 
            grid.Children.Add(img);
            //grid.Children.Add(tb);
            grid.Tag = part.ID;
            listCutImg.Add(grid);
            return grid; 
        }

        private void Img_MouseLeave(object sender, MouseEventArgs e)
        {
            if(sender is UIElement ele)
            {
                //ele.SetValue(Panel.ZIndexProperty, -1);

                ScaleEasingAnimationShow(ele, 1.5d, 1);
            }
        }

        private void Img_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is UIElement ele)
            {
                //ele.SetValue(Panel.ZIndexProperty, 99);
                ScaleEasingAnimationShow(ele, 1, 1.5d);
            }
        }
   
        private void ScaleEasingAnimationShow(UIElement ele, double Sizefrom, double Sizeto)
            => ScaleEasingAnimationShow(ele, 0.5, 0.5, Sizefrom, Sizeto, 5, TimeSpan.FromSeconds(0.2));
        /// <summary>
        /// 缩放动画
        /// </summary>
        /// <param name="element">控件名</param>
        /// <param name="RenderX">变换起点X坐标</param>
        /// <param name="RenderY">变换起点Y坐标</param>
        /// <param name="Sizefrom">开始大小</param>
        /// <param name="Sizeto">结束大小</param>
        /// <param name="power">过渡强度</param>
        /// <param name="time">持续时间，例如3秒： TimeSpan(0,0,3) </param>
        public void ScaleEasingAnimationShow(UIElement element, double RenderX, double RenderY, double Sizefrom, double Sizeto, int power, TimeSpan time)
        {
            ScaleTransform scale = new ScaleTransform();  //旋转
            element.RenderTransform = scale;
            //定义圆心位置
            element.RenderTransformOrigin = new System.Windows.Point(RenderX, RenderY);
            //定义过渡动画,power为过度的强度
            EasingFunctionBase easeFunction = new PowerEase()
            {
                EasingMode = EasingMode.EaseInOut,
                Power = power
            };

            DoubleAnimation scaleAnimation = new DoubleAnimation()
            {
                From = Sizefrom,                                   //起始值
                To = Sizeto,                                     //结束值
                FillBehavior = FillBehavior.HoldEnd,
                Duration = time,                                 //动画播放时间
                EasingFunction = easeFunction,                   //缓动函数
            };
            scale.BeginAnimation(ScaleTransform.ScaleXProperty, scaleAnimation);
            scale.BeginAnimation(ScaleTransform.ScaleYProperty, scaleAnimation);
        }

        /// <summary>
        /// 创建画布数据
        /// </summary>
        private void CreateCanvasDatas( )
        {
            if (ConvConfig == null)
            {
                //todo:提示错误
                return;
            }  
            TryGetBitmapImage();
            CalculateRange();
            uc_scMain.CreateCanvasDatas(listRangeDatas);
            uc_scMain.OnScaleCutImage += (s) => {
                var fe = listCutImg.Find(a => a.Tag.CastTo(0) == s);
                if (fe != null && feCacheScale != fe)
                {
                    if (feCacheScale != null)
                    {
                        if(feCacheScale is Grid grid)
                        {
                            grid.Background = new SolidColorBrush(Colors.Transparent);
                        }
                        ScaleEasingAnimationShow(feCacheScale, 1.5d, 1);
                    }

                    if (fe is Grid grid2)
                    {
                        grid2.Background = new SolidColorBrush( Color.FromArgb(155,255,255,255));
                    }
                    ScaleEasingAnimationShow(fe, 1, 1.5d);
                  
                    feCacheScale = fe;
                }
            };
            txtInfo.Text = ConvConfig.DemoMode ? "演示模式" : "";
            if (logic != null) logic.Stop();
            logic = new ReadStatusLogic(ConvConfig.Areas);
        } 
        private double GetHeightFactor(FrameworkElement father, FrameworkElement ui) => father.ActualHeight / ((ui.ActualHeight == 0 ? 1 : ui.ActualHeight) + 5);
   
        private void TryGetBitmapImage() => TryGetBitmapImage(GlobalPara.ConveyerConfig.MiniMapImagePath);
        private void TryGetBitmapImage(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Growl.Error("创建略缩图片失败,路径为空");
                return;
            }
            try
            {
                imgBack.Source = new BitmapImage(new Uri(path, UriKind.Absolute));
                //topGrid.Width = imgBack.Source.Width;
            }
            catch (Exception ex)
            {
                Growl.Error($"创建图片失败,'{ex.Message}'");
            }
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
         
        private void TxtLock_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button txt)
            {
                if (txt.Tag.ToString() == "锁住")
                {
                    storyboard.Pause();
                    GlobalPara.Locked = true; 
                    txt.Tag = "解锁";
                    txt.Content = "🔓";
                    txt.ToolTip = "切换自动轮播";
                    uc_scMain.RegisterWhellEvent();

                }
                else if (txt.Tag.ToString() == "解锁")
                {
                    storyboard.Resume();
                    GlobalPara.Locked = false; 
                    txt.Tag = "锁住";
                    txt.Content = "🔒";
                    txt.ToolTip = "切换手动轮播";
                    uc_scMain.UnregisterWhellEvent();


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
