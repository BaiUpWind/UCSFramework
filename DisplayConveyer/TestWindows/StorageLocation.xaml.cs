 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Policy;
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

namespace DisplayConveyer
{
    /// <summary>
    /// StorageLocation.xaml 的交互逻辑
    /// </summary>
    public partial class StorageLocation : Window
    {
        private Matrix mymat;
        private Point startpoint;
        private Point currentpoint;
        private Point cacheSelectPonint; 
        private double gridX =30,gridY =30,st = 0.2d;
        private List<Line> allLines = new List<Line>();
        private Rectangle selectPoint = new Rectangle();
        private List<GridPoint> gridPoints = new List<GridPoint>();
        private List<Device> devices = new List<Device>();
        private int selectedMultiX  =1;
        private int selectedMultiY  =1;
        

        public StorageLocation()
        {
            InitializeComponent();
            mymat = new Matrix(1, 0, 0, 1, 0, 0);//存储当前控件位移和比例 
            Loaded += (s, e) =>
            {
                inside.Height = 600;
                inside.Width = 800;


                txtCHeight.Text = inside.Height.ToString();
                txtCWidth.Text = inside.Width.ToString();
                txtGridX.Text = gridX.ToString();
                txtGridY.Text = gridY.ToString();
                txtGridST.Text = st.ToString();
                txtMultiX.Text = "1";
                txtMultiY.Text = "1";

            };
            inside.SizeChanged += (s, e) =>
            {
                Draw(inside, gridX, gridY);
            };

            txtCHeight.KeyDown += TextChanged;
            txtCWidth.KeyDown += TextChanged;
            txtGridX.KeyDown += TextChanged;
            txtGridY.KeyDown += TextChanged;
            txtGridST.KeyDown += TextChanged;
            txtMultiX.KeyDown += TextChanged;
            txtMultiY.KeyDown += TextChanged;
        


          
        }
        void TextChanged(object s, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            double inputValue = 10d;
            var txt = s as TextBox;
            if (txt != null)
            {
                try
                {
                    inputValue = double.Parse(txt.Text);
                    if (inputValue <= 0)
                    {
                        txt.Text = "600";
                        MessageBox.Show("必须大于0");
                    }
                }
                catch (Exception ex)
                {
                    txt.Text = "600";
                    MessageBox.Show($"错误的输入,{ex.Message}");
                }

                if (s == txtCHeight)
                {
                    inside.Height = inputValue;
                    Draw(inside, gridX, gridY);
                }
                if (s == txtCWidth)
                {
                    inside.Width = inputValue;
                    Draw(inside, gridX, gridY);
                }
                if (s == txtGridX)
                {
                    Draw(inside, gridX = inputValue, gridY);
                }
                if (s == txtGridY)
                {
                    Draw(inside, gridX, gridY = inputValue);
                }
                if (s == txtGridST)
                {
                    foreach (var item in allLines)
                    {
                        item.StrokeThickness = inputValue;
                    }
                }
                if (s == txtMultiX)
                {
                    selectedMultiX = (int)inputValue;
                    AddSelectPoint(GetGridPoint(cacheSelectPonint));
                }
                if (s == txtMultiY)
                {
                    selectedMultiY = (int)inputValue;
                    AddSelectPoint(GetGridPoint(cacheSelectPonint));
                }
                FocusManager.SetFocusedElement(FocusManager.GetFocusScope(txt), null);
                Keyboard.ClearFocus();
            }
        }
        public struct GridPoint 
        {  
            public GridPoint(double x,double y)
            {
                X = x;
                Y = y;
            }
            public double X;
            public double Y;
             
            public static bool operator >(GridPoint left, GridPoint right)
            {
                return left.X > right.X && left.Y > right.Y;
            } 
            public static bool operator <(GridPoint left, GridPoint right)
            {
                return left.X < right.X && left.Y < right.Y;
            }
            public static bool operator >=(GridPoint left, GridPoint right)
            {
                return left.X >= right.X && left.Y >= right.Y;
            }
            public static bool operator <=  (GridPoint left, GridPoint right)
            {
                return left.X <= right.X && left.Y <= right.Y;
            }
        }

        public class Device
        {
            public GridPoint Point { get; set; }//所在整个布局的位置信息
            public string Name { get; set; }
            public double Width { get; set; }

            public double Height { get; set; }
        }
        /// <summary>
        /// 获取当前鼠标所在Canvs的位置
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        private GridPoint GetGridPoint(Point point)
        {
            var result = gridPoints.Where(a => a.X - point.X > 0 && a.X - point.X < gridX && a.Y - point.Y > 0 && a.Y - point.Y < gridY).ToList();
            if (result == null || !result.Any()) return new GridPoint(-1, -1);
            return result.FirstOrDefault();
        }
        /// <summary>
        /// 添加选择块
        /// </summary>
        /// <param name="gridPoint"></param>
        private void AddSelectPoint(GridPoint gridPoint)
        {
            if (gridPoint.X + gridPoint.Y < 0) return;
            if (selectedMultiX <= 0) selectedMultiX = 1;
            if (selectedMultiY <= 0) selectedMultiY = 1;
            selectPoint.RenderTransform = new TranslateTransform(gridPoint.X - gridX, gridPoint.Y - gridY);
            selectPoint.Fill = Brushes.Blue;
            selectPoint.Width = gridX * selectedMultiX;
            selectPoint.Height = gridY * selectedMultiY;
            selectPoint.Opacity = 0.6d;
            inside.Children.Remove(selectPoint);
            inside.Children.Add(selectPoint);

        }
        /// <summary>
        /// 绘制线条
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="scaleX"></param>
        /// <param name="scaleY"></param>
        public void Draw(Canvas canvas, double scaleX = 30, double scaleY = 30 )
        {
            var gridBrush = new SolidColorBrush { Color = Colors.Red };
            foreach (var item in allLines)
            {
                canvas.Children.Remove(item);
            }
            allLines.Clear();
            gridPoints.Clear();
            double currentPosY = 0;
            currentPosY += scaleY;
            int yCount =0, xCount =0;
            while (currentPosY < canvas.ActualHeight)
            {
                Line line = new Line
                {
                    X1 = 0,
                    Y1 = currentPosY,
                    X2 = canvas.ActualWidth,
                    Y2 = currentPosY,
                    Stroke = gridBrush,
                    StrokeThickness = st
                };
                canvas.Children.Add(line);
                allLines.Add(line); 
                currentPosY += scaleY;
                yCount++;
            }
        
           
            double currentPosX = 0;
            currentPosX += scaleX;
            while (currentPosX < canvas.ActualWidth)
            {
                Line line = new Line
                {
                    X1 = currentPosX,
                    Y1 = 0,
                    X2 = currentPosX,
                    Y2 = canvas.ActualHeight,
                    Stroke = gridBrush,
                    StrokeThickness = st
                };
                canvas.Children.Add(line);
                allLines.Add(line);  
                currentPosX += scaleX;
                xCount++;
            }
            xCount++;yCount++;
            for (int x = 1; x <= xCount; x++)
            {
                for (int y = 1; y <= yCount; y++)
                {
                    gridPoints.Add(new GridPoint(x* scaleX, y*scaleY)); 
                }
            }
        } 
       
        private void MatrixChange(double dx, double dy, double scale)
        {
            mymat.M11 = scale;
            mymat.M22 = scale;
            mymat.OffsetX = dx;
            mymat.OffsetY = dy;
            inside.RenderTransform = new MatrixTransform(mymat);
        }
        private void MatrixChange(double dx, double dy)
        { 
            mymat.OffsetX = dx;
            mymat.OffsetY = dy;
            inside.RenderTransform = new MatrixTransform(mymat);
        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point p1 = e.GetPosition(inside);//得当鼠标相对于控件的坐标

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
        public void ResetPosition()
        {
            MatrixChange(0, 0, 1);
        }

        private void btn_ReSet(object sender, RoutedEventArgs e)
        {
            ResetPosition();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                Point currp = e.GetPosition(this);
                double dx = currp.X - startpoint.X + currentpoint.X;
                double dy = currp.Y - startpoint.Y + currentpoint.Y;//总位移等于当前的位移加上已有的位移
                MatrixChange(dx, dy);//移动控件，并更新总位移
            } 
            else if (e.LeftButton    == MouseButtonState.Pressed)
            {
                var startPoint = GetGridPoint(cacheSelectPonint);
                var endPoint = GetGridPoint(e.GetPosition(inside));
                //计算最后鼠标抬起的位置
                selectedMultiX = (int)(Math.Abs(endPoint.X - startPoint.X) / gridX) + 1;
                selectedMultiY = (int)(Math.Abs(endPoint.Y - startPoint.Y) / gridY) + 1;

                GridPoint now = startPoint;
                if (endPoint <= startPoint)
                {
                    now = endPoint;
                }
                AddSelectPoint(now);
            }

        }
        private void inside_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //按键送起来时才进行绘制
            //if (e.ChangedButton == MouseButton.Left && e.LeftButton == MouseButtonState.Released)
            //{
            //    var startPoint = GetGridPoint(cacheSelectPonint);
            //    var endPoint = GetGridPoint(e.GetPosition(inside));
            //    //计算最后鼠标抬起的位置
            //    selectedMultiX = (int)(Math.Abs(endPoint.X - startPoint.X) / gridX) + 1;
            //    selectedMultiY = (int)(Math.Abs(endPoint.Y - startPoint.Y) / gridY) + 1;

            //    GridPoint now = startPoint;
            //    if (endPoint <= startPoint)
            //    {
            //        now = endPoint;
            //    }
            //    AddSelectPoint(now);
            //}
        }

        private void inside_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                startpoint = e.GetPosition(this);//记录开始位置
                currentpoint.X = mymat.OffsetX;//记录Canvas当前位移
                currentpoint.Y = mymat.OffsetY;
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                cacheSelectPonint = e.GetPosition(inside); 
            }
            else if(e.RightButton == MouseButtonState.Pressed)
            {
                inside.Children.Remove(selectPoint);
                selectPoint = new Rectangle();
                cacheSelectPonint = new Point();
            }
      
        }

        private void btn_AddDevice(object sender, RoutedEventArgs e)
        {
            inside.Children.Remove(selectPoint);
            selectPoint.Fill = Brushes.Pink;
            inside.Children.Add(selectPoint);
            selectPoint = new Rectangle();
            cacheSelectPonint = new Point();
        }

    
        private void cb_ShowLine(object sender, RoutedEventArgs e)
        {
            foreach (var item in allLines)
            {
                item.Visibility = cbShowLine.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void outside_MouseDown(object sender, MouseButtonEventArgs e)
        {
        
        }

    }
}
