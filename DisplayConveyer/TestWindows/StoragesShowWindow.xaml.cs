using DisplayConveyer.Config;
using DisplayConveyer.Logic;
using ControlHelper.WPF;
using ControlHelper;
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
using MessageBox = HandyControl.Controls.MessageBox;
using System.Diagnostics;

namespace DisplayConveyer
{
    /// <summary>
    /// StoragesShowWindow.xaml 的交互逻辑
    /// </summary>
    public partial class StoragesShowWindow : Window
    {
  
        private readonly List<UC_Storages> listUscs = new List<UC_Storages>();
        private UC_Storages lastUsc;
        private float speed = 20;
        private List<BeltLogic> logics;
        private Stopwatch stopwatch = new Stopwatch();
        private TimeSpan prevTime = TimeSpan.Zero;
        bool mouseEnter = false;
        public StoragesShowWindow()
        {
            InitializeComponent();
            IniLogics(); 
            BtnFullScreen_Click(btnFullScreen, null);

            btn_OpenConfig.Click += (s, e) =>
            {
                GlobalPara.RecordTime();
                Stop();
                gd.Children.Clear();
                var window = new Window();
                window.Title = "编辑对应信息";
                Grid grid = new Grid();
                grid.Margin = new Thickness(5);
                window.Content = grid;
                ScrollViewer sv = new ScrollViewer();
                sv.Margin = new Thickness(5);
                grid.Children.Add(sv);
                var cc = new ClassControl(typeof(MainConfig), true, GlobalPara.Config.Clone());
                sv.Content = cc;
                window.Closing += (ws, we) =>
                {
                    GlobalPara.RecordTime();
                    var dig = MessageBox.Show("是:保存且关闭\n\r 否:关闭不保存 \n\r 取消:不保存不关闭", "是否保存已经修改的信息", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (dig == MessageBoxResult.Yes)
                    {
                        GlobalPara.Config = (MainConfig)cc.Data;

                    }
                    else if (dig == MessageBoxResult.Cancel)
                    {
                        we.Cancel = true;
                    }
                    IniLogics();
                    Start();
                };
                window.ShowDialog();
            };
            btnFullScreen.Click += BtnFullScreen_Click;
            btnClose.Click += (s, e) =>
            {
                var result = MessageBox.Ask("请确认关闭");
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
            SizeChanged += (s, e) =>
            {
            
                Calculate(); 
            };
            Loaded += (s, e) =>
            {
                Start();
            };
            stopwatch.Start();
        }
         
        private void IniLogics()
        {
            logics = new List<BeltLogic>(); 
            gd.Children.Clear();
            listUscs.Clear();
            speed = GlobalPara.Config.SlideSpeed;
            foreach (var item in GlobalPara.Config.Belts)
            {
                var logic = new BeltLogic(item);  
                logics.Add(logic);
                gd.Children.Add(logic.WholeBelts);
                listUscs.Add(logic.WholeBelts);
            }
            gd.RenderTransform = new TranslateTransform(0, 0);
            if(listUscs.Count > 0)
            {
                listUscs[0].Loaded += (s, e) =>
                {
                    Calculate();
                }; 
            }
         
            GlobalPara.RecordTime();
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 1)
            };
            timer.Tick += (s, e) =>
            {
                CheckTimeOut();
                txtTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            };
            timer.Start();
  
        }

        public void Stop()
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering; 
            logics.ForEach(a =>
            {
                a.Stop();
                a.WholeBelts.OnMouseSelected -= WholeBelts_OnMouseSelected;
                a.WholeBelts.OnMouseUnselect -= WholeBelts_OnMouseUnselect;
            });
        }

  

        public void Start()
        {
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            logics.ForEach(a =>
            {
                a.Start();
                a.WholeBelts.OnMouseSelected += WholeBelts_OnMouseSelected;
                a.WholeBelts.OnMouseUnselect += WholeBelts_OnMouseUnselect;
            });
        }
        //重新计算缩放比例
        private void Calculate()
        {
            double x = 0;
            foreach (var item in listUscs)
            {
                var factor = GetHeightFactor(item);// gd.ActualHeight / (usc.ActualHeight == 0 ? 1 : usc.ActualHeight);
                item.RenderTransform = new MatrixTransform(factor, 0, 0, factor, x, 0);
                x += factor * item.Width;
            }
            gd.Width = x + 15;
        }
        private double GetHeightFactor(FrameworkElement ui) => gd.ActualHeight  / ((ui.ActualHeight == 0 ? 1 : ui.ActualHeight)+5);
        private void WholeBelts_OnMouseUnselect()
        {
            mouseEnter = false;
        }

        private void WholeBelts_OnMouseSelected()
        {
            mouseEnter = true;
        }
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            TimeSpan currentTime = this.stopwatch.Elapsed;
            double elapsedTime = (currentTime - this.prevTime).TotalSeconds;
            this.prevTime = currentTime;
            if (!mouseEnter && listUscs.Count > 1)
            {
                foreach (var usc in listUscs)
                {
                    var matrix = (usc.RenderTransform as MatrixTransform)?.Matrix;
                    if (matrix != null)
                    {
                        var m = matrix.Value;
                        double nextX = m.OffsetX - speed * elapsedTime;
                        double factor = GetHeightFactor(usc);
                        if (nextX <= -usc.Width * factor)
                        {
                            //意味超出边界 出现的位置靠近最后的地方
                            if (lastUsc == null)
                            {
                                lastUsc = listUscs[listUscs.Count - 1]; 
                            }
                            var lastMatrix = lastUsc.RenderTransform as MatrixTransform;
                            var lastFactor = GetHeightFactor(lastUsc);
                            var offsetX = lastMatrix.Matrix.OffsetX + (lastUsc.Width * lastFactor)+15;
                            usc.RenderTransform = new MatrixTransform(m.M11, 0, 0, m.M22, offsetX, m.OffsetY);
                            lastUsc = usc;
                        }
                        else
                            usc.RenderTransform = new MatrixTransform(m.M11, 0, 0, m.M22, nextX, m.OffsetY);
                    };
                }
            }
        }
        private void CheckTimeOut()
        {
            if (!GlobalPara.Config.EnableTimeOut || GlobalPara.Locked) return;
         
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
