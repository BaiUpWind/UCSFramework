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

namespace DisplayBorder.Controls
{
    /// <summary>
    /// Dot.xaml 的交互逻辑
    /// </summary>
    public partial class Dot : UserControl
    {
        public Dot()
        {
            InitializeComponent();
            CreateEvent(this); 
        } 
        private Point location;

        public event Action OnMove;

        public Point Location
        {
            get
            {
                return location = TransformToAncestor((Visual)Parent).Transform(new Point(0, 0));
            }
            set => location = value;
        }

        //鼠标是否按下
        bool _isMouseDown = false;
        //鼠标按下的位置
        Point _mouseDownPosition;
        //鼠标按下控件的位置
        Point _mouseDownControlPosition;
        private void CreateEvent(UIElement uI)
        {
            if (uI == null) return;
            uI.PreviewMouseMove += UI_PreviewMouseMove;
            uI.PreviewMouseDown += UI_PreviewMouseDown;
            uI.PreviewMouseUp += UI_PreviewMouseUp;
        }
        private void UI_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var c = sender as UIElement;
            _isMouseDown = false;
            c.ReleaseMouseCapture();
        }

        private void UI_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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
        private void UI_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown)
            {
                var c = sender as UIElement;
                var pos = e.GetPosition(this);
                var dp = pos - _mouseDownPosition;
                var transform = c.RenderTransform as TranslateTransform;
                var rx = _mouseDownControlPosition.X + dp.X;
                var ry = _mouseDownControlPosition.Y + dp.Y;

             
                transform.X = rx;
                transform.Y = ry;
                OnMove?.Invoke();
            }
        }
    }
}
