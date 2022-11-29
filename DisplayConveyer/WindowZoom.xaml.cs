using DisplayConveyer.Config;
using DisplayConveyer.Logic;
using ControlHelper.WPF;
using ControlHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DisplayConveyer
{
    /// <summary>
    /// WindowZoom.xaml 的交互逻辑
    /// </summary>
    public partial class WindowZoom : Window
    {
        /// <summary>
        /// 缩放倍数
        /// </summary>
        private  double ScaleMultipty = .5d;
        private List<BeltLogic> logics;
        private UC_Storages currentFather;
        private UC_Storages currentChildren;  
        private Matrix cacheMatirx;
        private Matrix cacheChildrenMatrix;
        private UIElementMove dpMove;
        private UIElementMove cvMove;
        public WindowZoom()
        {
            InitializeComponent();
            IniLogics();
            btn_OpenConfig.Click += (s, e) =>
            {
                GlobalPara.RecordTime();
                Stop();
                cv.Children.Clear();
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
            btnFullScreen.Click += (s, e) =>
            {
                if (s is Button btn)
                {
                    if (btn.Tag.ToString() == "全屏")
                    {
                        this.WindowState = WindowState.Maximized;
                        WindowStyle = WindowStyle.None;
                        //IsFullScreen = true; 
                        btn.Tag = "复原";

                        if (btn.Content is StackPanel sp)
                        {
                            sp.Children[0].Visibility = Visibility.Collapsed;
                            sp.Children[1].Visibility = Visibility.Visible;
                        }
                    }
                    else if (btn.Tag.ToString() == "复原")
                    {
                        //this.WindowState = WindowState.Normal;
                        WindowStyle = WindowStyle.SingleBorderWindow;
                        //IsFullScreen = false;
                        btn.Tag = "全屏";
                        if (btn.Content is StackPanel sp)
                        {
                            sp.Children[0].Visibility = Visibility.Visible;
                            sp.Children[1].Visibility = Visibility.Collapsed;
                        }
                    }
                }
            };
        }

  
        private void IniLogics()
        {
            logics = new List<BeltLogic>();
            double maxW = 0, maxH = 0;
            double x = 15d, y = 15d;
            double tempWidth =0d;
            int count = 0;//换行索引
            bool lastMid = GlobalPara.Config.Belts.Count % 2 == 1; 
            int index = 0;//当前到第几个
            gd.Children.Clear();
            ScaleMultipty = GlobalPara.Config.SacleRatio;
            foreach (var item in GlobalPara.Config.Belts)
            {
                var logic = new BeltLogic(item); 
                //计算逃逸的方向
                logic.WholeBelts.RunWayDirection = count == 0? 0 :2;  
                //如果是最后一个时 则让它居中显示
                if (lastMid && index == GlobalPara.Config.Belts.Count -1)
                {
                    x = 1920 / 2 - (logic.WholeBelts.Width * ScaleMultipty / 2);
                    count = 1;
                } 
                logic.WholeBelts.RenderTransform = new MatrixTransform(ScaleMultipty, 0, 0, ScaleMultipty, x, y);
                x += (logic.WholeBelts.Width + 15) * ScaleMultipty;
                tempWidth += (logic.WholeBelts.Width + 15) * ScaleMultipty;
                logics.Add(logic);
                if(count == 1)
                { 
                    x = 15d;
                    y += (logic.WholeBelts.Height + 15) * ScaleMultipty;
                    maxW = tempWidth > maxW ? tempWidth : maxW;
                    count = 0;
                    tempWidth = 0;
                }
                else
                { 
                    count++;
                } 
                maxH += (logic.WholeBelts.Height + 15) * ScaleMultipty;
                index++;
                gd.Children.Add(logic.WholeBelts);
            }
            gd.Width = maxW+15;
            gd.Height = maxH;
            gd.RenderTransform = new TranslateTransform(0, 0);
            Start();
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

            dpMove = new UIElementMove(dp,this);
            cvMove = new UIElementMove(cv,this);
        }
         
        public void Stop()
        {
            logics.ForEach(a =>
            {
                a.Stop();
                a.UnRegisterEvent(OnZoomIn, OnZoomOut );
            });
        } 
        public void Start()
        {
            logics.ForEach(a =>
            {
                a.Start();
                a.RegisterEvent(OnZoomIn, OnZoomOut);
            });
        }
        private void CheckTimeOut()
        {
            if (!GlobalPara.Config.EnableTimeOut || GlobalPara.Locked) return;
            if ((DateTime.Now - GlobalPara.LastMotionTime).TotalMilliseconds > GlobalPara.Config.TimeOut)
            {
                if (currentChildren != null)
                {
                    OnZoomOut(currentChildren);
                }
                else if (currentFather != null)
                {
                    OnZoomOut(currentFather);
                }
            }
        }
        private void OnZoomIn(  UC_Storages store)
        {
            try
            { 
                if (store.InAnimation) return;
                if (!store.Zoomed)
                {
                    if (store.IsFather)
                    {
                        cacheMatirx = (store.RenderTransform as MatrixTransform).Matrix;
                        foreach (var item in logics)
                        {
                            if (item.WholeBelts == store) continue;
                            item.WholeBelts.RunOutSide(true, (s, e) =>
                            {
                                gd.Visibility = Visibility.Collapsed;
                            });
                        }
                        gd.Children.Remove(store);
                        dp.Children.Add(store);
                        dp.Visibility = Visibility.Visible;

                        store.Scale(0.5, 15, 15, (s, e) =>
                        {

                            store.Zoomed = true;
                            dpMove.CanInput = true;
                            currentFather = store;

                            foreach (var item in store.store.Children)
                            {
                                var usc = item as UC_Storages;
                                if (usc == null) continue;
                                usc.FatherDone = true;
                            }

                        });
                    }
                    else if (store.FatherDone)
                    {
                        cacheChildrenMatrix = store.CurrentMatrix;
                        dp.Visibility = Visibility.Collapsed;
                        currentFather.store.Children.Remove(store);
                        cv.Children.Add(store);
                        currentChildren = store;
                        cv.Visibility = Visibility.Visible;
                
                        store.MoveTo(15, 15, (s, e) =>
                        {
                            cvMove.CanInput = true;
                            store.Zoomed = true;
                        });
                    }
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show($"出现未知异常，点击确认重新加载。\r\n错误信息{ex.Message}");
                IniLogics();
            }

        }

        private void OnZoomOut(  UC_Storages store)
        {
            try
            {
                if (store.InAnimation) return;
                if (store.Zoomed)
                {
                    if (store.IsFather)
                    {
                        foreach (var item in logics)
                        {
                            if (item.WholeBelts == store) continue;
                            item.WholeBelts.RunOutSide(false, (s, e) =>
                            {
                                dp.Visibility = Visibility.Collapsed;
                            });
                        }
                        dp.Children.Remove(store);
                        gd.Children.Add(store);
                        gd.Visibility = Visibility.Visible;
                        dpMove.ReSet();
                        store.Scale(-0.5, cacheMatirx.OffsetX, cacheMatirx.OffsetY, (s, e) =>
                        {
                            store.Zoomed = false;
                            currentFather = null;
                            foreach (var item in store.store.Children)
                            {
                                var usc = item as UC_Storages;
                                if (usc == null) continue;
                                usc.FatherDone = false;
                                dpMove.CanInput = false;
                            }
                        });
                    }
                    else if (store.FatherDone)
                    {
                        cv.Visibility = Visibility.Collapsed;
                        cv.Children.Remove(store);
                        currentFather.store.Children.Add(store);
                        dp.Visibility = Visibility.Visible;
                        store.Zoomed = false;
                        cvMove.ReSet();
                        store.MoveTo(store.CurrentMatrix, cacheChildrenMatrix, (s, e) =>
                        {
                            cvMove.CanInput = false;
                            currentChildren = null;
                        });

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"出现未知异常，点击确认重新加载。\r\n错误信息{ex.Message}");
                IniLogics();
            }
         
     
        }

       

 

    }
    public class UIElementMove
    {
        private Matrix mymat;
        private Point startpoint;
        private Point currentpoint; 
        private UIElement element;
        private UIElement parent;

        public bool CanInput { get  ; set  ; }

        public UIElementMove(UIElement element, Window window)
        {
            mymat = new Matrix(1, 0, 0, 1, 0, 0);//存储当前控件位移和比例 
            this.element = element;
            parent = window;

            element.MouseWheel += Canvas_MouseWheel;
            element.MouseDown += Canvas_MouseDown;
            element.MouseMove += Canvas_MouseMove;
        }
        public void ReSet() => MatrixChange(15, 15, 1);
        private void MatrixChange(double dx, double dy)
        {
            mymat.OffsetX = dx;
            mymat.OffsetY = dy;
            element.RenderTransform = new MatrixTransform(mymat);
        }
        private void MatrixChange(double dx, double dy, double scale)
        {
            mymat.M11 = scale;
            mymat.M22 = scale;
            mymat.OffsetX = dx;
            mymat.OffsetY = dy;
            element.RenderTransform = new MatrixTransform(mymat);
        }
        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            GlobalPara.RecordTime();
            if (!CanInput) return;
            Point p1 = e.GetPosition(element);//得当鼠标相对于控件的坐标

            double dx, dy;
            double scale = mymat.M11;
            if (e.Delta > 0)
            {
                scale += 0.2;
                if (scale > 4)
                {
                    scale = 4;
                    return;
                }

                dx = p1.X * (scale - 0.2) - scale * p1.X + mymat.OffsetX;
                dy = p1.Y * (scale - 0.2) - scale * p1.Y + mymat.OffsetY;//放大本质是 移动和缩放两个步骤 
                                                                         //
                MatrixChange(dx, dy, scale);

            }
            else
            {
                scale -= 0.2;
                if (scale < 0.5)
                {
                    scale = 0.5;
                    return;
                }

                dx = p1.X * (scale + 0.2) - scale * p1.X + mymat.OffsetX;
                dy = p1.Y * (scale + 0.2) - scale * p1.Y + mymat.OffsetY;
                MatrixChange(dx, dy, scale);
            }
        }
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            GlobalPara.RecordTime();
            if (!CanInput) return;

            var uiEle = sender as UIElement;
            if (uiEle == null) return;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point currp = e.GetPosition(parent);
                double dx = currp.X - startpoint.X + currentpoint.X;
                double dy = currp.Y - startpoint.Y + currentpoint.Y;//总位移等于当前的位移加上已有的位移
                MatrixChange(dx, dy);//移动控件，并更新总位移
            }


        }
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GlobalPara.RecordTime();
            if (!CanInput) return;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                startpoint = e.GetPosition(parent);//记录开始位置
                currentpoint.X = mymat.OffsetX;//记录Canvas当前位移
                currentpoint.Y = mymat.OffsetY;
            }
        }
    }
    public class LinearMatrixAnimation : AnimationTimeline
    {

        public Matrix? From
        {
            set { SetValue(FromProperty, value); }
            get { return (Matrix)GetValue(FromProperty); }
        }
        public static DependencyProperty FromProperty = DependencyProperty.Register("From", typeof(Matrix?), typeof(LinearMatrixAnimation), new PropertyMetadata(null));

        public Matrix? To
        {
            set { SetValue(ToProperty, value); }
            get { return (Matrix)GetValue(ToProperty); }
        }
        public static DependencyProperty ToProperty = DependencyProperty.Register("To", typeof(Matrix?), typeof(LinearMatrixAnimation), new PropertyMetadata(null));

        public LinearMatrixAnimation()
        {
        }

        public LinearMatrixAnimation(Matrix from, Matrix to, Duration duration)
        {
            Duration = duration;
            From = from;
            To = to;
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue, AnimationClock animationClock)
        {
            if (animationClock.CurrentProgress == null)
            {
                return null;
            }

            double progress = animationClock.CurrentProgress.Value;
            Matrix from = From ?? (Matrix)defaultOriginValue;

            if (To.HasValue)
            {
                Matrix to = To.Value;
                Matrix newMatrix = new Matrix(((to.M11 - from.M11) * progress) + from.M11, 0, 0, ((to.M22 - from.M22) * progress) + from.M22,
                                              ((to.OffsetX - from.OffsetX) * progress) + from.OffsetX, ((to.OffsetY - from.OffsetY) * progress) + from.OffsetY);
                return newMatrix;
            }

            return Matrix.Identity;
        }

        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new LinearMatrixAnimation();
        }

        public override System.Type TargetPropertyType
        {
            get { return typeof(Matrix); }
        }
    }
}
