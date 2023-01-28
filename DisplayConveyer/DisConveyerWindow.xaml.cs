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
        /// <summary>
        /// 主gird和画布的缩放比例系数
        /// </summary>
        private double CanvasHeightFactor => mainGrid.ActualHeight / ConvConfig.CanvasHeight;
   
        private ConveyerConfig ConvConfig => GlobalPara.ConveyerConfig;
        private ReadStatusLogic logic;
        private double AnimationSpeed =55d;  
        private Storyboard storyboard = new Storyboard();
        private FrameworkElement feCacheScale;
        //存放所有的区域数据 小地图用
        private readonly List<RangeData> listRangeDatas = new List<RangeData>();
        //存放所有的画布
        private readonly List<Canvas> listAllCanvas = new List<Canvas>(); 
        //区域判定框
        private readonly List<FrameworkElement> listRangeRect = new List<FrameworkElement>();
        //裁切的所有图像
        private readonly List<FrameworkElement> listCutImg = new List<FrameworkElement>();
        
        DoubleAnimation animation;
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


            //DispatcherTimer timer2 = new DispatcherTimer
            //{
            //    Interval = TimeSpan.FromMilliseconds(70)
            //};
            //timer2.Start();
            //timer2.Tick += (s, e) =>
            //{
            //    mainGrid_MouseWheel(null, new MouseWheelEventArgs(Mouse.PrimaryDevice,200,-88 ) );
            //}; 

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
            };
            SizeChanged += (s, e) =>
            {
                CalculateCanvsSclae();
                Calculate(topCanvas, topSv, topGrid);
     

            };
            topGrid.Width = topCanvas.Width = ConvConfig.CanvasWidth;
            topGrid.Height = topCanvas.Height = ConvConfig.CanvasHeight; 
            txtLock.Click += TxtLock_Click;
        }
        private void SvHorizontalOffsetToRight()
        {
            ScrollHorizontalMoveTo(0, 1920, (s, e) =>
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
                Duration = new Duration(TimeSpan.FromSeconds(ConvConfig.CanvasWidth / AnimationSpeed)),
                FillBehavior = FillBehavior.Stop 
            };
      
            animation.Completed += (s, e) =>
            {
                animationCompleted?.Invoke(s, e);
                storyboard.Children.Remove(animation);
            };
            storyboard.Children.Add(animation);
            //Storyboard.SetTarget(animation,mainSv);
            Storyboard.SetTargetProperty(animation, new PropertyPath(ScrollViewerBehaviour.HorizontalOffsetProperty ));
             
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
            listRangeDatas.Clear();
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
                        var tempMin = area.Devices.Min(r => r.PosX);
                        var tempMax = area.Devices.Max(r => r.PosX + r.Width);
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
                CreateCutImage(part); 
            }
        }
        /// <summary>
        /// 创建剪切之后的图片
        /// </summary> 
        /// <param name="part"></param>
        private void CreateCutImage(MapPartData part)
        {
            var bs = imgBack.Source;
            Grid grid = new Grid();
            Image img = new Image();
            img.Height = ConvConfig.CanvasHeight;
            img.Width = part.Width;
            img.Margin = new Thickness(5);
            grid.MouseEnter += Img_MouseEnter;
            grid.MouseLeave += Img_MouseLeave;
            var rect = new Int32Rect(part.PosX.CastTo(0), 0, part.Width.CastTo(50), bs.Height.CastTo(50));
            CroppedBitmap cb = new CroppedBitmap((BitmapSource)bs, rect);
            img.Source = cb;
            TextBlock tb = new TextBlock()
            {
                Margin = new Thickness(0, 45, 0, 0),
                FontSize = 92,
                Text = part.Title,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = new SolidColorBrush(Colors.Black),
            };
            grid.SetValue(Canvas.LeftProperty, part.PosX);
            grid.SetValue(Canvas.TopProperty, part.PosY); 
            grid.Children.Add(img);
            grid.Children.Add(tb);
            grid.Tag = part.ID;
            listCutImg.Add(grid);
            topCanvas.Children.Add(grid);
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
            mainGrid.Children.Clear();
            listRangeRect.Clear(); 
            TryGetBitmapImage();
            CalculateRange();
            //todo : 这里的1,实际为:需要判断当前画布宽度是否小于屏幕尺寸 ,小于则需要额外添加一个canvas
            for (int i = 0; i < 2; i++)
            {
                Canvas canvas = new Canvas(); 
                canvas.Width = ConvConfig.CanvasWidth;
                canvas.Height = ConvConfig.CanvasHeight;
                canvas.RenderTransform = new MatrixTransform(1,0,0,1,canvas.Width * i,0);
                canvas.Name = "canvas_"+(i+1).ToString();
                foreach (var area in ConvConfig.Areas)
                {
                    foreach (var device in area.Devices)
                    {
                        var ucdevice = CreateHelper.GetDeviceBase(device);
                        canvas.Children.Add(ucdevice);
                        ucdevice.ToolTip = (ucdevice as UC_DeviceBase).Info;
                        device.StatusChanged?.Invoke(new StatusData() { MachineState = 100 });
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
                foreach (var range in listRangeDatas)
                {
                    Border border = new Border();
                    border.Width = range.MaxPosX - range.MinPosX;
                    border.Height = ConvConfig.CanvasHeight -10;
                    border.Name = canvas.Name +"_" +range.MapPartId; 
                    border.BorderBrush = new SolidColorBrush(Colors.Red);
                    border.BorderThickness = new Thickness(5);
                    border.Visibility = Visibility.Hidden;
                    border.SetValue(Canvas.LeftProperty ,range.MinPosX);
                    border.SetValue(Canvas.TopProperty, 10d);
                    listRangeRect.Add(border);
                    canvas.Children.Add(border); 
                }
                mainGrid.Children.Add(canvas);
                listAllCanvas.Add(canvas);
            }
            CalculateCanvsSclae();
            //if (logic != null) logic.Stop();
            //logic = new ReadStatusLogic(ConvConfig.Areas);
        }

        /// <summary>
        /// 计算所有画布的比例
        /// </summary>
        private void CalculateCanvsSclae()
        {
            double nextX = 0 ;
            foreach (var item in listAllCanvas)
            {
                var factor = CanvasHeightFactor;
                if (item.RenderTransform is MatrixTransform matrix)
                { 
                    item.RenderTransform = new MatrixTransform(factor, 0, 0, factor, nextX, matrix.Value.OffsetY);
                    nextX += item.Width * factor;
                }
            } 
        }
        private double GetHeightFactor(FrameworkElement father, FrameworkElement ui) => father.ActualHeight / ((ui.ActualHeight == 0 ? 1 : ui.ActualHeight) + 5);
        private bool CheckInLine(Point pointA, Point pointB, Point pLine)
        {
            return pLine.X >= pointA.X && pLine.X <= pointB.X;
        }
        private void TryGetBitmapImage() => TryGetBitmapImage(GlobalPara.ConveyerConfig.MiniMapImagePath);
        private void TryGetBitmapImage(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                Growl.Error("创建略缩图片失败,路径为空");
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

        private void DisConveyerWindow_Loaded(object sender, RoutedEventArgs e)
        { 
            BtnFullScreen_Click(btnFullScreen, null);
            CreateCanvasDatas();
       
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
                }
                else if (txt.Tag.ToString() == "解锁")
                {
                    storyboard.Resume();
                    GlobalPara.Locked = false; 
                    txt.Tag = "锁住";
                    txt.Content = "🔒";
                    txt.ToolTip = "切换手动轮播";
                }
            }
        }

        private void mainGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        { 
            for (int i = 0; i < listAllCanvas.Count; i++)
            {
                var current = listAllCanvas[i];
                if (current == null) continue;
                if (!(current.RenderTransform is MatrixTransform t))
                {
                    current.RenderTransform = t = new MatrixTransform();
                }
                var matrix = t.Value; 
                if (t.Value.OffsetX + ConvConfig.CanvasWidth * CanvasHeightFactor < 0 && e.Delta < 0)
                {
                    Canvas last = listAllCanvas[listAllCanvas.Count - 1];
                    var lastT = last.RenderTransform as MatrixTransform; 
                    matrix.OffsetX = lastT.Value.OffsetX + (last.Width * CanvasHeightFactor) + 10;
                    current.RenderTransform = new MatrixTransform(matrix);
                    listAllCanvas.Remove(current);
                    listAllCanvas.Add(current);
                    break;
                }
                else if (t.Value.OffsetX > ConvConfig.CanvasWidth * CanvasHeightFactor && e.Delta > 0)
                {
                    Canvas first = listAllCanvas[0];
                    var firstT = first.RenderTransform ; 
                    matrix.OffsetX = firstT.Value.OffsetX - (first.Width * CanvasHeightFactor) - 10;
                    current.RenderTransform = new MatrixTransform(matrix);
                    listAllCanvas.Remove(current);
                    listAllCanvas.Insert(0, current);
                    break;
                }
                else
                { 
                    matrix.OffsetX += e.Delta * 0.8d;
                    current.RenderTransform = new MatrixTransform(matrix);
                }  
            }
            txtInfo.Text = string.Empty;
           
            foreach (var item in listRangeRect)
            {
                var pointA = item.TransformToAncestor(gridCore).Transform(new Point(0, 0));
                Point pointB = new Point(pointA.X + item.ActualWidth * CanvasHeightFactor, pointA.Y);

                if (CheckInLine(pointA, pointB, new Point(mainGrid.ActualWidth / 2, 0)))
                {
                    var name = txtInfo.Text = item.Name;
                    var partId = name.Split('_')[2].CastTo(0);
                    var fe =  listCutImg.Find(a => a.Tag.CastTo(0) == partId);
                    if (fe != null && feCacheScale != fe)
                    {
                        if (feCacheScale != null)
                        {
                            ScaleEasingAnimationShow(feCacheScale, 1.5d, 1);
                        }
                        ScaleEasingAnimationShow(fe, 1, 1.5d);
                        feCacheScale = fe;
                    }
                    break;
                }
                
            }

        }
    }
    public class ScrollBehaviour
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
