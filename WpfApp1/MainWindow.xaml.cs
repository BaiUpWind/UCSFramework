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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFDevelopers.Controls;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += (s, e) =>
            {
                var al = AdornerLayer.GetAdornerLayer(_border);
                al.Add(new ElementAdorner(_border));
            };

        }
        //鼠标是否按下
        bool _isMouseDown = false;
        //鼠标按下的位置
        Point _mouseDownPosition;
        //鼠标按下控件的位置
        Point _mouseDownControlPosition;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //var layer = AdornerLayer.GetAdornerLayer(border);
            //border.PreviewMouseDown += Layer_PreviewMouseDown;
            //border.PreviewMouseMove += Layer_PreviewMouseMove;
            //border.PreviewMouseUp += Layer_PreviewMouseUp;
            //layer.PreviewMouseDown += Layer_PreviewMouseDown;
            //layer.PreviewMouseMove += Layer_PreviewMouseMove;
            //layer.PreviewMouseUp += Layer_PreviewMouseUp;
            //layer.Add(new CanvasAdorner(border));
        }

        private void Layer_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var c = sender as UIElement;
            _isMouseDown = false;
            c.ReleaseMouseCapture();
        }

        private void Layer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                var c = sender as UIElement;
                var pos = e.GetPosition(this);
                var dp = pos - _mouseDownPosition;
                var transform = c.RenderTransform as TranslateTransform;
                transform.X = _mouseDownControlPosition.X + dp.X;
                transform.Y = _mouseDownControlPosition.Y + dp.Y;
            }
        }

        private void Layer_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var c = sender as UIElement;
            _isMouseDown = true;
            _mouseDownPosition = e.GetPosition(this);
            var transform = c.RenderTransform as TranslateTransform;
            if (transform == null)
            {
                transform = new TranslateTransform();
                c.RenderTransform = transform;
            }
            _mouseDownControlPosition = new Point(transform.X, transform.Y);
            c.CaptureMouse();
        }

        private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            UIElement thumb = e.Source as UIElement;

            //    防止Thumb控件被拖出容器。  
            //    if (nTop <= 0)
            //        nTop = 0;
            //    if (nTop >= (g.Height - myThumb.Height))
            //        nTop = g.Height - myThumb.Height;
            //    if (nLeft <= 0)
            //        nLeft = 0;
            //    if (nLeft >= (g.Width - myThumb.Width))
            //        nLeft = g.Width - myThumb.Width;
            //    Canvas.SetTop(myThumb, nTop);
            //    Canvas.SetLeft(myThumb, nLeft);
            //    tt.Text = "Top:" + nTop.ToString() + "\nLeft:" + nLeft.ToString();


            Canvas.SetLeft(thumb, Canvas.GetLeft(thumb) + e.HorizontalChange);
            Canvas.SetTop(thumb, Canvas.GetTop(thumb) + e.VerticalChange);
        }
    }
}
