using DisplayConveyer.Model;
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
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Application;

namespace DisplayConveyer
{
    /// <summary>
    /// UC_Storages.xaml 的交互逻辑
    /// </summary>
    public partial class UC_Storages : UserControl
    {
        public bool IsFather { get; private set; }
        public string Title
        {
            get { return txtTitle.Text; }
            set { txtTitle.Text = value; }
        }
        public string ReadTableName { get; set; }
        public double MaxPosX { get; private set; }
        public double MinPosX { get; private set; }
        public double MaxPosY { get; private set; }
        public double MinPosY { get; private set; }
        public string ErrorInfo
        {
            get
            {
                return txtErrInfo.Text;
            }
            set
            {
                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        store.Visibility = Visibility.Collapsed;
                        txtErrInfo.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        txtErrInfo.Visibility = Visibility.Collapsed;
                        store.Visibility = Visibility.Visible;
                    }
                    txtErrInfo.Text = value;
                }));
            }
        }


        public bool InAnimation { get; private set; }
        /// <summary>
        /// 是否被放大了， 子类才用
        /// </summary>
        public bool Zoomed
        {
            get { return zoomed; }
            set { 
                
                btnLock.Visibility = value? Visibility.Visible: Visibility.Collapsed ;
                
                zoomed = value;
            
            }
        }
        /// <summary>
        /// 它的容器准备好了
        /// </summary>
        public bool FatherDone { get; set; }
        public Matrix CurrentMatrix
        {
            get
            {
                return (RenderTransform as MatrixTransform).Matrix;
            }
        }


        /// <summary>
        /// -1 无
        /// 0 左
        /// 1 上
        /// 2 右
        /// 3 下
        /// </summary>
        public int RunWayDirection { get; set; } = -1;
        private Matrix lastMatrix;
        private bool zoomed;

        public event Action<UC_Storages> OnZoomIn;
        public event Action<UC_Storages> OnZoomOut; 
        private readonly List<BeltEditor> beltDatas;
        private readonly Dictionary<string, Border> dicBelts = new Dictionary<string, Border>();
        public UC_Storages()
        {
            InitializeComponent();
            ErrorInfo = string.Empty;
            VerticalAlignment = VerticalAlignment.Top;
            HorizontalAlignment = HorizontalAlignment.Left;

            #region 事件 
            MouseEnter += (s, e) =>
            {
                if (FatherDone) bSelected.Visibility = Visibility.Visible;
                SaveTime();
            };
            MouseLeave += (s, e) =>
            {
                if (FatherDone) bSelected.Visibility = Visibility.Collapsed;
                SaveTime(); 
            };
            MouseDown += (s, e) =>
            {
                if (InAnimation) return;
                SaveTime(); 
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    OnZoomIn?.Invoke(this);

                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    if (IsFather)
                    {
                        foreach (var item in store.Children)
                        {
                            var usc = item as UC_Storages;
                            if (usc == null) continue;
                            usc.bSelected.Visibility = Visibility.Collapsed;
                        }
                    }
                    if (btnLock.Tag.ToString() == "解锁")
                    {
                        BtnLock_Click(btnLock, null);
                    }
                    OnZoomOut?.Invoke(this);
                }
            };
            Loaded += (s, e) =>
            {
                //if (IsFather)
                //{
                //    foreach (var children in store.Children)
                //    {
                //        var usc = children as UC_Storages;
                //        if (usc == null) continue;

                //        //当崽的大小发生变化时
                //        usc.OnChildrenSizeChanged += () =>
                //        {
                //            double width = 15d, height = 15d;
                //            //重新排列元素位置
                //            foreach (var item in store.Children)
                //            {
                //                var usc2 = item as UC_Storages;
                //                if (usc2 == null) continue;
                //                var matrix = usc2.RenderTransform as MatrixTransform;
                //                if (matrix == null) continue;

                //                usc2.RenderTransform = new MatrixTransform(
                //                           matrix.Matrix.M11, matrix.Matrix.M12,
                //                           matrix.Matrix.M21, matrix.Matrix.M22, width, 15d);
                //                width += usc2.Width + 15d;
                //                height = usc2.Height + 15d >= height ? usc2.Height + 15d : height;
                //            }
                //            Width = width;
                //            Height = height + 15d + Math.Ceiling(txtTitle.FontSize * txtTitle.FontFamily.LineSpacing);
                //        };
                //    }
                //}
            };

            btnLock.Click += BtnLock_Click;  

            btnLock.MouseEnter += (s, e) =>
            {
                btnLock.Cursor = Cursors.Hand;
            };
            btnLock.MouseLeave += (s, e) =>
            {
                btnLock.Cursor = Cursors.None;
            };
            #endregion
            btnLock.Visibility = Visibility.Collapsed;
        }



        public UC_Storages(bool isFather) : this()
        {
            FatherDone = IsFather = isFather;

        }

        public UC_Storages(List<BeltEditor> datas, double xOffSet = 0, double yOffSet = 0) : this()
        {
            if (datas == null)
            {
                Height = 600;
                Width = 600;
                return;
            }
            SetDataInfo(beltDatas = datas, xOffSet, yOffSet);

        }
        private void SetDataInfo(List<BeltEditor> beltDatas, double xOffSet, double yOffSet)
        {
            MaxPosX = beltDatas.Max(a => a.Pos_x);
            MaxPosY = beltDatas.Max(a => a.Pos_y);
            MinPosX = beltDatas.Min(a => a.Pos_x);
            MinPosY = beltDatas.Min(a => a.Pos_y);
            store.Children.Clear();
            Grid grid1 = new Grid();
            var difX = MaxPosX - MinPosX;
            var difY = MaxPosY - MinPosY;
            foreach (var editor in beltDatas)
            {
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Vertical;
                Border border = new Border();
                border.Child = new TextBlock()
                {
                    Text = editor.Work_id,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                };
                border.ToolTip = editor.S_name;
                border.Background = new SolidColorBrush(Colors.Blue);
                TextBlock txt = new TextBlock();
                txt.Text = editor.S_name;
                txt.HorizontalAlignment = HorizontalAlignment.Center;
                txt.VerticalAlignment = VerticalAlignment.Center;
                border.Width = editor.Pos_width;
                border.Height = editor.Pos_height == 0 ? 10 : editor.Pos_height - Math.Ceiling(txt.FontSize * txt.FontFamily.LineSpacing);
                border.BorderBrush = new SolidColorBrush(Colors.Transparent);
                border.BorderThickness = new Thickness(3);
                sp.RenderTransform = new TranslateTransform(editor.Pos_x - difX + xOffSet, editor.Pos_y - difY + yOffSet);
                sp.Width = editor.Pos_width;
                sp.Height = editor.Pos_height;
                sp.Children.Add(border);
                sp.Children.Add(txt);
                grid1.Children.Add(sp);
                dicBelts.Add(editor.Work_id, border);
            }
            store.Height = MaxPosY;
            store.Width = MaxPosX;
            Height = MaxPosY + Math.Ceiling(txtTitle.FontSize * txtTitle.FontFamily.LineSpacing);
            Width = MaxPosX;

            store.Children.Add(grid1);
        }
        private void BtnLock_Click(object sender, RoutedEventArgs e)
        {
            SaveTime();

            var btn = sender as Button;
            if (btn != null)
            {
                var sp = btn.Content as StackPanel;
                if (btn.Tag.ToString() == "锁住")
                {
                    GlobalPara.Locked = true;
                    btn.Tag = "解锁";

                    if (sp!= null)
                    {
                        sp.Children[0].Visibility = Visibility.Collapsed;
                        sp.Children[1].Visibility = Visibility.Visible;
                    }
                }
                else if (btn.Tag.ToString() == "解锁")
                {
                    GlobalPara.Locked = false;
                    btn.Tag = "锁住";
                    if (sp != null)
                    {
                        sp.Children[0].Visibility = Visibility.Visible;
                        sp.Children[1].Visibility = Visibility.Collapsed;
                    }
                }
            }
        }
        /// <summary>
        /// 设置工作区域的颜色 (线程安全)
        /// <para> 物流线 绿色一一物流可取、 白色一一物流可放 自动无料、 黄色一一不可取放 自动有料、 红色一  设备报警、 橙色一一设备手动、 灰色一一设备离线  </para>
        /// <para> 外部设备 灰色一一设备离线、白色自动无任务、红色一一报警、绿色一一自动有任务、黄色一一手动任务 </para>
        /// </summary>
        /// <param name="word_id"></param>
        /// <param name="color"></param>
        public void SetWorkPosColor(string word_id, Color color)
        {
            Border border;
            if (dicBelts.TryGetValue(word_id, out  border))
            {
                Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
                {
                    border.Background = new SolidColorBrush(color);
                }));
            }
        }

        public void SetWorkPosColor(string word_id, int status)
        {
            SetWorkPosColor(word_id, GetColor(status));
        }

        /// <summary>
        /// 缩放改变动画
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="onAnimationCompleted"></param>
        public void Scale(double scale,double offsetX,double offsetY, Action<object, EventArgs> onAnimationCompleted = null)
        {
            var t = RenderTransform as MatrixTransform;
            if (t == null) return;
            var ma = new Matrix(t.Matrix.M11, 0, 0, t.Matrix.M22, offsetX, offsetY);
            ma.M11 += scale;
            ma.M22 += scale;
            if (ma.M11 < .5)
            {
                ma.M11 =
                 ma.M22 = .5d;
                return;
            }
            var animation = new LinearMatrixAnimation
            {
                To = ma,
                From = t.Matrix,
                Duration = TimeSpan.FromSeconds(0.5d),
                DecelerationRatio = 0.5d,
                FillBehavior = FillBehavior.Stop
            };
            InAnimation = true;
            animation.Completed += (s, e) =>
            { 
                RenderTransform = new MatrixTransform(ma); 
                RenderSize = new Size(Width + Width * scale, Height + Height * scale);
                onAnimationCompleted?.Invoke(s, e);
                InAnimation = false;
            };
            t.BeginAnimation(
                MatrixTransform.MatrixProperty,
                animation,
                HandoffBehavior.Compose);
        }

        /// <summary>
        /// 逃逸当前位置 或者从逃逸位置返回
        /// </summary>
        /// <param name="runOrBack"></param>
        /// <param name="onAnimationCompleted"></param>
        public void RunOutSide(bool runOrBack, Action<object, EventArgs> onAnimationCompleted = null)
        {
            var from = CurrentMatrix;
            var to = runOrBack ? GetDic(RunWayDirection) : lastMatrix;
            var animation = new LinearMatrixAnimation
            {
                To = to,
                From = from,
                Duration = TimeSpan.FromSeconds(0.5d),
                DecelerationRatio = 0.5d,
                FillBehavior = FillBehavior.Stop
            };
            InAnimation = true;
            animation.Completed += (s, e) =>
            {
           
                lastMatrix = CurrentMatrix;
                var arrdist = new MatrixTransform(to);
                RenderTransform = arrdist;
                onAnimationCompleted?.Invoke(s, e);
                InAnimation = false;
            };
            this.RenderTransform.BeginAnimation(
              MatrixTransform.MatrixProperty,
              animation,
              HandoffBehavior.Compose);
        }

        /// <summary>
        /// 从这个矩阵 移动到另个矩阵
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <param name="onAnimationCompleted"></param>
        public void MoveTo(Matrix from, Matrix to, Action<object, EventArgs> onAnimationCompleted = null)
        {
            var animation = new LinearMatrixAnimation
            {
                To = to,
                From = from,
                Duration = TimeSpan.FromSeconds(0.5d),
                DecelerationRatio = 0.5d,
                FillBehavior = FillBehavior.Stop
            };
            InAnimation = true;
            animation.Completed += (s, e) =>
            { 
                RenderTransform = new MatrixTransform(to);
                onAnimationCompleted?.Invoke(s, e);
                InAnimation = false;
            };
            this.RenderTransform.BeginAnimation(
                MatrixTransform.MatrixProperty,
                animation,
                HandoffBehavior.Compose);
        }

        /// <summary>
        /// 将当前位置移动到对应的位置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="onAnimationCompleted"></param>
        public void MoveTo(double x, double y, Action<object, EventArgs> onAnimationCompleted = null)
        {
            var t = RenderTransform as MatrixTransform;

            var ma = new Matrix(t.Matrix.M11, 0, 0, t.Matrix.M22, x, y);
            var animation = new LinearMatrixAnimation
            {
                To = ma,
                From = t.Matrix,
                Duration = TimeSpan.FromSeconds(0.5d),
                //DecelerationRatio = 0.5d,
                FillBehavior = FillBehavior.Stop
            };
            InAnimation = true;
            animation.Completed += (s, e) =>
            { 
                RenderTransform = new MatrixTransform(ma);
                onAnimationCompleted?.Invoke(s, e);
                InAnimation = false;
            };
            t.BeginAnimation(
                MatrixTransform.MatrixProperty,
                animation,
                HandoffBehavior.Compose);
        }

        /// <summary>
        /// 1-99报警red,100自动Lime,101手动灰色
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private Color GetColor(int status)
        {
            if (status >= 1 && status <= 99)
            {
                return Colors.Red;
            }
            else if (status == 100)
            {
                return Colors.Lime;
            }
            else if (status == 101)
            {
                return Colors.Gray;
            }

            return Colors.Gray;
        }


        private Matrix GetDic(int dic)
        {
            double x = 0, y = 0;
            if (dic == 0)
            {
                x = -5000; ;
                y = CurrentMatrix.OffsetY;
            }
            else if (dic == 1)
            {
                x = CurrentMatrix.OffsetX;
                y = 5000;
            }
            else if (dic == 2)
            {
                x = 5000;
                y = CurrentMatrix.OffsetY;
            }
            else if (dic == 3)
            {
                x = CurrentMatrix.OffsetX;
                y = -5000;
            }

            return new Matrix(CurrentMatrix.M11, 0, 0, CurrentMatrix.M22, x, y);
        }



        private void SaveTime() => GlobalPara.RecordTime();
    }
}
