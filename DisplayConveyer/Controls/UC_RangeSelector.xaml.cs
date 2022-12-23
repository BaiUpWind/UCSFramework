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

namespace DisplayConveyer.Controls
{
    /// <summary>
    /// UC_RangeSelector.xaml 的交互逻辑
    /// </summary>
    public partial class UC_RangeSelector : UserControl
    {
        public UC_RangeSelector()
        {
            InitializeComponent();
            spMain.RenderTransform = new TranslateTransform();
            tbLeft.RenderTransform = new TranslateTransform();
            tbMid.RenderTransform = new TranslateTransform();
            tbRight.RenderTransform = new TranslateTransform();

            Loaded += (s, e) =>
            {
            parent = this.Parent as FrameworkElement;
            };
        }
        public event Action<object, Point> OnMove;
        UIElement cacheUielm;
        FrameworkElement parent;
        bool _isMouseDown = false;
        //鼠标按下的位置
        Point _mouseDownPosition;
        //鼠标按下控件的位置
        Point _mouseDownControlPosition;

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_isMouseDown) return;
            IInputElement ie = this;
          
            var c = sender as UIElement;
            cacheUielm = c;
            if (cacheUielm == this)
            {
                ie = parent;
            }
            _isMouseDown = true;
            _mouseDownPosition = e.GetPosition(ie);
            var transform = c.RenderTransform as TranslateTransform;
            //if (transform == null)
            //{
            //    transform = new TranslateTransform();
            //    c.RenderTransform = transform;
            //}
            _mouseDownControlPosition = new Point(transform.X, transform.Y);
            c.CaptureMouse();
        }

        private void TextBlock_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown && cacheUielm == sender)
            {
                IInputElement ie = this; 
                if(cacheUielm== this)
                {
                    ie = parent;
                }
                var c = sender as UIElement;
                var pos = e.GetPosition(ie);
                var dp = pos - _mouseDownPosition;
                var transform = c.RenderTransform as TranslateTransform;
                //if (CheckOutSide(dp.X,tbRight)) return;
                //if (CheckOutSide(dp.X,tbLeft)) return;
                if (CheckOutSide(dp.X, tbLeft)  ||   CheckOutSide(dp.X, this))
                {
                    //transform.X = 0;
                    return;
                }
                if (cacheUielm != spMain)
                {
                    if (cacheUielm == tbLeft && _mouseDownControlPosition.X + dp.X >= ((TranslateTransform)tbMid.RenderTransform).X)
                    {
                        return;
                    }
                    else if (cacheUielm == tbRight && _mouseDownControlPosition.X + dp.X <= ((TranslateTransform)tbMid.RenderTransform).X)
                    {
                        return;
                    }
                }
                else
                {
                    OnMove?.Invoke(this, new Point(_mouseDownControlPosition.X + dp.X, 0));
                }
               
                
                transform.X = _mouseDownControlPosition.X + dp.X;
                transform.Y = 0;
            }
        }

        private void TextBlock_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (cacheUielm != sender) return;
            cacheUielm = null; 
            var c = sender as UIElement;
            _isMouseDown = false;
            c.ReleaseMouseCapture();

        }

        private bool CheckOutSide(double offset, UIElement uielm)
        {
            //检查是否超出父边界
            if (parent == null || uielm == null) return false;
            Point tempPoint = uielm.TransformToVisual(parent).Transform(new Point(0, 0)); 
            bool temp = (tempPoint.X+offset < 0);
            return temp;
        }
    }
}
