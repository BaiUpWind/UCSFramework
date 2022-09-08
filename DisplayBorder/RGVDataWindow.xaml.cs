using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
using CommonApi;
using DeviceConfig;
using DisplayBorder.Controls;
using DisplayBorder.Events;
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
            DataContext = ViewModelLocator.Instance;
            Main = ViewModelLocator.Instance.Main;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ShowNonClientArea = false;

            Loaded += (s, e) =>
            {
                CreateInfos();
                GlobalPara.EventManager.Subscribe(OnValueChangedArgs.EventID, OnGroupsValueChanged);
            };
            Unloaded += (s, e) =>
            {
                GlobalPara.EventManager.Unsubscribe(OnValueChangedArgs.EventID, OnGroupsValueChanged);
            };
            #region 时间定时器
            var dateTimeTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 1)
            };
            dateTimeTimer.Tick += (s, e) =>
            {
                txtTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            };
            dateTimeTimer.Start();
            #endregion


            RecoverAutoTimer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 5)
            };
            RecoverAutoTimer.Tick += (s, e) =>
            {
                Console.WriteLine("切换成自动刷新");
                IsAuto = true;
                RecoverAutoTimer.Stop();
            };



        }

        #region 字段&属性  
        /// <summary>
        /// 检查恢复为自定定时器
        /// </summary>
        private readonly DispatcherTimer RecoverAutoTimer;
        private readonly MainViewModel Main;
        /// <summary>
        /// 存放当前所有组的控件信息
        /// </summary>
        private readonly Dictionary<int, TitleControl> DicTitleContorl = new Dictionary<int, TitleControl>();
        private bool isAuto;
        private BitmapSource BackImage => mainImg.Source as BitmapSource;
        private TitleControl currentSelectedTitle;
        private CancellationTokenSource MainCancellToken = new CancellationTokenSource();
        private Task MainTask;
        private bool isRunning;

        public bool IsAuto
        {
            get => isAuto; set
            {
                isAuto = value;
                if (!value)
                {
                    //每当有为假的值传入时 都重置定时器
                    RecoverAutoTimer.IsEnabled = false;
                    RecoverAutoTimer.Start();
                }
            }
        }
        public bool IsRunning
        {
            get => isRunning; set
            {
                isRunning = value;
                StopAllSvTask();
            }

        }
        /// <summary>
        /// 当前选中的组信息
        /// </summary>
        public TitleControl CurrentSelectedTitle
        {
            get => currentSelectedTitle;
            set
            {
                //当之前的不会为空 且 新值不等于旧值 
                if (currentSelectedTitle != null && currentSelectedTitle != value)
                {
                    //取消旧值的选择
                    currentSelectedTitle.IsSelected = false;
                }
                currentSelectedTitle = value;
                currentSelectedTitle.IsSelected = true;
                Main.RunGroup = currentSelectedTitle.GroupInfo;
                SetScrollViewerPos(value);
            }
        }
        #endregion
        #region 内部方法
        /// <summary>
        /// 异步更改UI
        /// </summary>
        /// <param name="action"></param>
        public void AsyncRunUI(Action action)
        {
            if (action == null || Application.Current == null) return;
            Application.Current.Dispatcher.BeginInvoke(action);
        }

        private async void Run()
        {
            IsRunning = true;
            while (!MainCancellToken.IsCancellationRequested)
            {
                if (DicTitleContorl == null || DicTitleContorl.Count == 0)
                {
                    continue;
                }
                if (MainCancellToken.IsCancellationRequested) break;
                foreach (var titleControl in DicTitleContorl.Values)
                {
                    if (MainCancellToken.IsCancellationRequested) break;
                    var group = titleControl.GroupInfo;
                    AsyncRunUI(() =>
                    {
                        CurrentSelectedTitle = titleControl; 
                    });
                    if (group.DeviceInfos == null) continue;
                    //await Task.Delay(3000);
                    //continue;
                    foreach (var deviceInfo in group.DeviceInfos)
                    {
                        if (MainCancellToken.IsCancellationRequested) break;
                        AsyncRunUI(() =>
                        {
                            Main.CurrentRunDeviceName = deviceInfo.DeviceInfoName;
                        });
                        CancellationTokenSource intervalToken = new CancellationTokenSource();
                        Task task = new Task(async () =>
                        {
                            Console.WriteLine($"{deviceInfo.DeviceInfoName}获取数据刷新");
                            while (true)
                            {
                                if (intervalToken.IsCancellationRequested)
                                {
                                    break;
                                }
                                //deviceInfo.Operation.GetResults(); 
                                Console.WriteLine($"[{DateTime.Now}]{deviceInfo.DeviceInfoName}刷新");
                                if (deviceInfo.RefreshInterval == 0) deviceInfo.RefreshInterval = 1000;
                                await Task.Delay(deviceInfo.RefreshInterval);
                            }
                        }, intervalToken.Token);
                        task.Start();
                        await Task.Delay(deviceInfo.StayTime);
                        intervalToken.Cancel();
                        Console.WriteLine($"{deviceInfo.DeviceInfoName}终止任务");
                    }
                }
                await Task.Delay(500);
            }
            IsRunning = false;
        }

        /// <summary>
        /// 创建显示区信息
        /// </summary>
        /// <param name="groups"></param>
        private void CreateInfos()
        {
            var groups = GlobalPara.Groups;
            if (groups == null || groups.Count == 0)
            {
                Growl.ErrorGlobal($"获取配置数据失败,请检查配置文件,或者重新创建");
                return;
            }
            //初始化
            mainImg.Source = null;
            foreach (var control in DicTitleContorl)
            {
                C1.Children.Remove((UIElement)((FrameworkElement)control.Value).Parent);
            }
            DicTitleContorl.Clear();

            //开始创建
            SetImgSource();
            if (mainImg.Source == null) return;
            foreach (var group in groups)
            {
                CreateTitle(group);
            }



            MainTask = Task.Run(() =>
            {
                Run();
            }, MainCancellToken.Token);
            void CreateTitle(Group g)
            {
                TitleControl tc = new TitleControl(g);
                tc.SwitchDricetion((TitleControl.DirectionArrow)g.Direction);
                Border border = new Border
                {
                    Width = g.CWidth,
                    Height = g.CHeight,
                    Child = tc,
                    ToolTip = "单击查看"
                };

                double rx = 20, ry = 20;
                if (g.PosX >= 0)
                {
                    rx = g.PosX / (BackImage.PixelWidth / mainImg.Width);
                }
                if (g.PosY >= 0)
                {
                    ry = g.PosY / (BackImage.PixelHeight / mainImg.Height);
                }
                border.SetValue(Canvas.LeftProperty, rx);
                border.SetValue(Canvas.TopProperty, ry);
                //添加到对应的集合
                C1.Children.Add(border);
                var al = AdornerLayer.GetAdornerLayer(border);
                var adorner = new ElementAdorner(border);
                al.Add(adorner);
                DicTitleContorl.Add(g.GroupID, tc);
                #region 事件
                border.MouseDown += (ms, me) =>
                {
                    if (ms is Border b && b.Child is TitleControl ctc)
                    {
                        CurrentSelectedTitle = ctc;
                        IsAuto = false;
                    }
                };
                #endregion
            }
        }
        /// <summary>
        /// 设置图片源
        /// </summary>
        private void SetImgSource()
        {
            if (!string.IsNullOrEmpty(GlobalPara.SysConfig.BackImagPath))
            {
                try
                {
                    mainImg.Source = new BitmapImage(new Uri(GlobalPara.SysConfig.BackImagPath, UriKind.Absolute));
                    if (mainImg.Source is BitmapSource bitmap)
                    {
                        mainImg.Width = bitmap.Width;
                        mainImg.Height = bitmap.Height;
                    }
                }
                catch (Exception ex)
                {
                    Growl.ErrorGlobal($"打开图片失败!请检查配置文件\n\r其他信息'{ex.Message}'");
                }
            }
        }

        #region 平滑的方式 定位到滚动条位置 
        /// <summary>
        /// 存放 scroll viewer 异步任务
        /// </summary>
        private readonly Dictionary<int, CancellationTokenSource> DicSvTask = new Dictionary<int, CancellationTokenSource>();
        private void StopAllSvTask()
        {
            foreach (var task in DicSvTask)
            {
                task.Value.Cancel();
            }
            DicSvTask.Clear();
        }
        /// <summary>
        /// 定位滚动条位置
        /// </summary>
        /// <param name="target"></param>
        private void SetScrollViewerPos(Visual target)
        {
            if (target == null) return;
            if (sv.ScrollableHeight == 0 && sv.ScrollableWidth ==0) return;
            var currentPosY = sv.VerticalOffset;
            var currentPosX = sv.HorizontalOffset;
            //获取目标控件相对scrollViewer位置
            var point = new Point(currentPosX - 25, currentPosY - 25);
            //获取目标相对sv的位置
            var tarPos = target.TransformToVisual(sv).Transform(point);
            CancellationTokenSource lerpToken = new CancellationTokenSource();
            Task task;
            if (DicSvTask.Count > 0)
            {
                //这里的作用是 暂停上个异步的操作
                int key = DicSvTask.First().Key;
                DicSvTask[key].Cancel();
                DicSvTask.Remove(key);
            }
            task = Task.Factory.StartNew(async () =>
            {
                while (Math.Abs((sv.HorizontalOffset + sv.VerticalOffset) - (tarPos.X + tarPos.Y)) >= 5)
                {
                    if (lerpToken.IsCancellationRequested)
                    {
                        break;
                    }
                    AsyncRunUI(() =>
                    {
                        Point newPoint = Lerp(new Point(sv.HorizontalOffset, sv.VerticalOffset), tarPos, 0.25f);
                        //垂直方向上的定位
                        sv.ScrollToVerticalOffset(newPoint.Y);
                        //水平方向上的定位
                        sv.ScrollToHorizontalOffset(newPoint.X);
                    });
                    //调整这个 可以调整帧率
                    await Task.Delay(20);
                }
            }, lerpToken.Token);
            DicSvTask[task.Id] = lerpToken;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Point Lerp(Point a, Point b, float t)
        {
            t = Clamp01(t);
            return new Point(a.X + (b.X - a.X) * t, a.Y + (b.Y - a.Y) * t);
        }
        public float Clamp01(float value)
        {
            if (value < 0f)
            {
                return 0f;
            }

            if (value > 1f)
            {
                return 1f;
            }

            return value;
        }
        #endregion


        #endregion
        #region 事件

        //全屏切换
        private void Btn_Click_FullSwitch(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                if (btn.Tag.ToString() == "全屏")
                {

                    IsFullScreen = true;

                    btn.Tag = "复原";

                    if (btn.Content is StackPanel sp)
                    {
                        sp.Children[0].Visibility = Visibility.Collapsed;
                        sp.Children[1].Visibility = Visibility.Visible;
                    }
                }
                else if (btn.Tag.ToString() == "复原")
                {
                    IsFullScreen = false;
                    btn.Tag = "全屏";
                    if (btn.Content is StackPanel sp)
                    {
                        sp.Children[0].Visibility = Visibility.Visible;
                        sp.Children[1].Visibility = Visibility.Collapsed;
                    }
                }
            }
        }
        //关闭系统
        private void Btn_Click_Close(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Ask("请确认关闭");
            if (result == MessageBoxResult.OK)
            {
                Close();
                Application.Current.Shutdown();
            }
        }
        //打开配置窗口
        private void Btn_Click_OpenConfig(object sender, RoutedEventArgs e)
        {
            WindowGroupsConfig wgc = new WindowGroupsConfig();
            wgc.WindowState = WindowState.Maximized;
            wgc.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            wgc.ShowDialog();
        }
        //鼠标拖拽
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }


        //当组集合数据发生变化时
        private async void OnGroupsValueChanged(object sender, BaseEventArgs e)
        {
            if (e is OnValueChangedArgs args && args.Value is IList<Group>)
            {
                MainCancellToken.Cancel();
                if (IsRunning)
                {
                    await Task.Run(async () =>
                     {
                         while (IsRunning) { await Task.Delay(500); }

                     });
                }
                MainCancellToken = new CancellationTokenSource();
                CreateInfos();
            }
        }
        #endregion
    }
}
