using Config.DeviceConfig.Models;
using ControlHelper;
using DisplayConveyer.Config;
using DisplayConveyer.Model;
using DisplayConveyer.Utilities;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DisplayConveyer.Controls
{
    /// <summary>
    /// UC_ScrollCanvas.xaml 的交互逻辑
    /// </summary>
    public partial class UC_ScrollCanvas : UserControl
    {
        /// <summary>
        /// 主gird和画布的缩放比例系数
        /// </summary>
        private double CanvasHeightFactor => mainGrid.ActualHeight / ConvConfig.CanvasHeight;

        private ConveyerConfig ConvConfig => GlobalPara.ConveyerConfig;
        private double AnimationSpeed = 10d;
        private bool AnimationDircetion = true;
        //存放所有的画布
        private readonly List<Canvas> listAllCanvas = new List<Canvas>(); 
        //区域判定框
        private readonly List<FrameworkElement> listRangeRect = new List<FrameworkElement>();
        
        public static readonly DependencyProperty HorizontalOffsetProperty =
              DependencyProperty.RegisterAttached("HorizontalOffset",
            typeof(double),
            typeof(UC_ScrollCanvas),
            new UIPropertyMetadata(0d, new PropertyChangedCallback(OnHorizontalofsetchanged)));

        /// <summary>
        /// 当检测到在指定区域时发出事件
        /// </summary>
        public event Action<int> OnScaleCutImage;
        public UC_ScrollCanvas()
        {
            InitializeComponent();
            SizeChanged += (s, e) =>
            {
                CalculateCanvsSclae();
            };
        }
        /// <summary>
        /// 创建画布数据
        /// </summary>
        internal void CreateCanvasDatas(List<RangeData> listRangeDatas)
        {
            if (ConvConfig == null)
            {
                //todo:提示错误
                return;
            }
            AnimationSpeed = GlobalPara.ConveyerConfig.AnimationSpeed;
            AnimationDircetion = GlobalPara.ConveyerConfig.AnimationDircetion;
            mainGrid.Children.Clear();
            listRangeRect.Clear();
            listAllCanvas.Clear();  
            for (int i = 0; i < 2; i++)
            {
                Canvas canvas = new Canvas();
                canvas.Width = ConvConfig.CanvasWidth;
                canvas.Height = ConvConfig.CanvasHeight;
                canvas.RenderTransform = new MatrixTransform(1, 0, 0, 1, canvas.Width * i, 0);
                canvas.Name = "canvas_" + (i + 1).ToString();
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
                foreach (var range in listRangeDatas)
                {
                    Border border = new Border();
                    border.Width = range.MaxPosX - range.MinPosX;
                    border.Height = ConvConfig.CanvasHeight - 10;
                    border.Name = canvas.Name + "_" + range.MapPartId;
                    border.BorderBrush = new SolidColorBrush(Colors.Red);
                    border.BorderThickness = new Thickness(5);
                    border.Visibility = Visibility.Hidden;
                    border.SetValue(Canvas.LeftProperty, range.MinPosX);
                    border.SetValue(Canvas.TopProperty, 10d);
                    listRangeRect.Add(border);
                    canvas.Children.Add(border);
                }
                mainGrid.Children.Add(canvas);
                listAllCanvas.Add(canvas);
            }
            CalculateCanvsSclae();
        }
        public void RegisterWhellEvent() => mainGrid.MouseWheel += mainGrid_MouseWheel;
        public void UnregisterWhellEvent() => mainGrid.MouseWheel -= mainGrid_MouseWheel;
        /// <summary>
        /// 计算所有画布的比例
        /// </summary>
        private void CalculateCanvsSclae()
        {
            double nextX = 0;
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
        private bool CheckInLine(Point pointA, Point pointB, Point pLine)
        {
            return pLine.X >= pointA.X && pLine.X <= pointB.X;
        }
        private void SetWhellDelte(double delta,bool add =false)
        {
            if(!add)  delta *= (AnimationDircetion ? 1 : -1);
            for (int i = 0; i < listAllCanvas.Count; i++)
            {
                var current = listAllCanvas[i];
                if (current == null) continue;
                if (!(current.RenderTransform is MatrixTransform t))
                {
                    current.RenderTransform = t = new MatrixTransform();
                }
                var matrix = t.Value;
                if (t.Value.OffsetX + ConvConfig.CanvasWidth * CanvasHeightFactor < 0 && delta < 0)
                {
                    Canvas last = listAllCanvas[listAllCanvas.Count - 1];
                    var lastT = last.RenderTransform as MatrixTransform;
                    matrix.OffsetX = lastT.Value.OffsetX + (last.Width * CanvasHeightFactor) + 10;
                    current.RenderTransform = new MatrixTransform(matrix);
                    listAllCanvas.Remove(current);
                    listAllCanvas.Add(current); 
                    break;
                }
                else if (t.Value.OffsetX > ConvConfig.CanvasWidth * CanvasHeightFactor && delta > 0)
                {
                    Canvas first = listAllCanvas[0];
                    var firstT = first.RenderTransform;
                    matrix.OffsetX = firstT.Value.OffsetX - (first.Width * CanvasHeightFactor) - 10;
                    current.RenderTransform = new MatrixTransform(matrix);
                    listAllCanvas.Remove(current);
                    listAllCanvas.Insert(0, current); 
                    break;
                }
                else
                {
                    if (add)
                    {
                        matrix.OffsetX += delta;
                    }
                    else
                    {
                        matrix.OffsetX += AnimationSpeed * 0.2 * (AnimationDircetion ? -1 : 1);
                    }
                    current.RenderTransform = new MatrixTransform(matrix);
                }
            }
            foreach (var item in listRangeRect)
            {
                var pointA = item.TransformToAncestor(gridCore).Transform(new Point(0, 0));
                Point pointB = new Point(pointA.X + item.ActualWidth * CanvasHeightFactor, pointA.Y);

                if (CheckInLine(pointA, pointB, new Point(mainGrid.ActualWidth / 2, 0)))
                {
                    var name = item.Name;
                    var partId = name.Split('_')[2].CastTo(0);
                    OnScaleCutImage?.Invoke(partId);
                    break;
                }

            }
        }

        //-----事件 
        public static void OnHorizontalofsetchanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var scrollcanvas = (sender as UC_ScrollCanvas);
            scrollcanvas?.SetWhellDelte((double)e.NewValue); 
        }
        private void mainGrid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            SetWhellDelte(e.Delta,true);
        }
    }
}
