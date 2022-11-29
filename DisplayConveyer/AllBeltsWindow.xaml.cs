using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using DisplayConveyer.Model;
using DisplayConveyer.Config;
using DisplayConveyer.DA;
using DisplayConveyer.Logic;
using ControlHelper;
using ControlHelper.WPF; 

namespace DisplayConveyer
{
    /// <summary>
    /// AllBeltsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class AllBeltsWindow : Window
    {
        public AllBeltsWindow()
        {
            InitializeComponent();
            btn_OpenConfig.Click += (s, e) =>
            {
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
                var cc =  new ClassControl(typeof(MainConfig), true, GlobalPara.Config.Clone());
                sv.Content = cc;
                
                window.Closing += (ws, we) =>
                {
                    var dig = MessageBox.Show("是:保存且关闭\n\r 否:关闭不保存 \n\r 取消:不保存不关闭", "是否保存已经修改的信息", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (dig == MessageBoxResult.Yes)
                    {
                        GlobalPara.Config = (MainConfig)cc.Data;
                          
                    }
                    else if (dig == MessageBoxResult.Cancel)
                    {
                        we.Cancel = true;
                    }
                    LogicIni();
                    Start();
                }; 
                window.Show();  
            };
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 1)
            };
            timer.Tick += (s, e) =>
            {
                txtTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            };
            timer.Start();

            LogicIni();
            IniCanvsMove();
            Start();

        }
        private List<BeltLogic> logics;
        private void LogicIni()
        {
            logics = new List<BeltLogic>();
            double maxW =0,maxH =0;
            double x = 15d, y = 15d; 
            foreach (var item in GlobalPara.Config.Belts)
            {
                var logic = new BeltLogic(item); 
                cv.Children.Add(logic.WholeBelts);
                logic.WholeBelts.SetValue(Canvas.TopProperty, y);
                logic.WholeBelts.SetValue(Canvas.LeftProperty, x);
                cv.Width += logic.WholeBelts.Width;
                cv.Height += logic.WholeBelts.Height;
                y += logic.WholeBelts.Height + 15;
                logics.Add(logic);
                maxW = logic.WholeBelts.Width > maxW ? logic.WholeBelts.Width + logic.WholeBelts.store.Children.Count*15 : maxW;
                maxH += logic.WholeBelts.Height; 
            }
            cv.Width = maxW ;
            cv.Height = maxH + GlobalPara.Config.Belts.Count * 15; 
        }

        public void Stop()
        {
            logics.ForEach(a =>
            {
                a.Stop();
                 
            }); 
        }

        public void Start()
        {
            logics.ForEach(a =>
            {
                a.Start(); 
            });
        }

        private void OnSelected(int id,UC_Storages store)
        {

        }

        private void OnUnSelect(int id,UC_Storages store)
        {

        }

        public void OnChoose()
        {

        }
        public void OnBack()
        {

        }

        #region 缩放 移动canvas
        private Matrix mymat;
        private Point startpoint;
        private Point currentpoint;

        private void IniCanvsMove()
        {
            mymat = new Matrix(1, 0, 0, 1, 0, 0);//存储当前控件位移和比例 
            cv.MouseWheel += Canvas_MouseWheel;
            cv.MouseDown += Canvas_MouseDown;
            cv.MouseMove += Canvas_MouseMove;
        }
        private void MatrixChange(double dx, double dy)
        {
            mymat.OffsetX = dx;
            mymat.OffsetY = dy;
            cv.RenderTransform = new MatrixTransform(mymat);
        }
        private void MatrixChange(double dx, double dy, double scale)
        {
            mymat.M11 = scale;
            mymat.M22 = scale;
            mymat.OffsetX = dx;
            mymat.OffsetY = dy;
            cv.RenderTransform = new MatrixTransform(mymat);
        }
        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point p1 = e.GetPosition(cv);//得当鼠标相对于控件的坐标

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
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point currp = e.GetPosition(this);
                double dx = currp.X - startpoint.X + currentpoint.X;
                double dy = currp.Y - startpoint.Y + currentpoint.Y;//总位移等于当前的位移加上已有的位移
                MatrixChange(dx, dy);//移动控件，并更新总位移
            }
          

        }
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                startpoint = e.GetPosition(this);//记录开始位置
                currentpoint.X = mymat.OffsetX;//记录Canvas当前位移
                currentpoint.Y = mymat.OffsetY;
            } 
        }
       
        #endregion
    }
}
