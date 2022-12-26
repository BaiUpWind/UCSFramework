using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DisplayConveyer
{
    /// <summary>
    /// 可拖拽的核心
    /// </summary>
    public class ElementAdorner : Adorner
    {
        private const double ThumbSize = 5, ElementMiniSize = 20;
        private readonly Thumb tLeft;
        private readonly Thumb tRight;
        private readonly Thumb bLeftBottom;
        private readonly Thumb bRightBottom;
        private readonly Thumb tMove;
        private readonly VisualCollection visualCollection;

        public bool EnableHorizontal { get; set; } = true;
        public bool EnableVertical { get; set; } = true;

        public ElementAdorner(UIElement adornedElement) : base(adornedElement)
        {
            visualCollection = new VisualCollection(this);
            visualCollection.Add(tMove = CreateMoveThumb());
            visualCollection.Add(tLeft = CreateThumb(Cursors.SizeNWSE, HorizontalAlignment.Left,
                VerticalAlignment.Top));
            visualCollection.Add(tRight =
                CreateThumb(Cursors.SizeNESW, HorizontalAlignment.Right, VerticalAlignment.Top));
            visualCollection.Add(bLeftBottom =
                CreateThumb(Cursors.SizeNESW, HorizontalAlignment.Left, VerticalAlignment.Bottom));
            visualCollection.Add(bRightBottom =
                CreateThumb(Cursors.SizeNWSE, HorizontalAlignment.Right, VerticalAlignment.Bottom));
            //this.MouseLeftButtonDown += ElementAdorner_MouseLeftButtonDown;

            Hide();
        }
        /// <summary>
        /// 当拖动时发生
        /// </summary>
        public event Action<object, DragDeltaEventArgs> OnDrage;
        /// <summary>
        /// 当拖动时发生
        /// <para>判断是否超过边界,当返回为真时，拖动无效</para>
        /// </summary>
        public event Func<object, DragDeltaEventArgs, bool> DrageMove;

        /// <summary>
        /// 当移动四个角时发生
        /// </summary> 
        public event Action<object, DragDeltaEventArgs> OnMoveDrage;
        /// <summary>
        /// 当移动四个角时发生
        /// <para>判断是否超过边界,当返回为真时，拖动无效</para>
        /// </summary>
        public event Func<object, DragDeltaEventArgs,bool> ThumbMove;
        /// <summary>
        /// 当开始移动时发生
        /// <para>UIElement 是被装饰的元素</para>
        /// </summary>

        public event Action<object, UIElement> DragStarted;
         
        public void Show()
        { 
            foreach (var item in visualCollection)
            {
                if (item is Thumb thumb)
                {
                    thumb.Visibility = Visibility.Visible;
                }
            } 
        }

        public void Hide()
        {
            foreach (var item in visualCollection)
            {
                if(item is Thumb thumb)
                {
                    thumb.Visibility = Visibility.Hidden;
                }
            }
        }
       

        protected override int VisualChildrenCount => visualCollection.Count;

        protected override void OnRender(DrawingContext drawingContext)
        {
            var offset = ThumbSize / 2;
            var sz = new Size(ThumbSize, ThumbSize);
            //var renderPen = new Pen(new SolidColorBrush(Colors.White), 2.0);
            //var startPoint = new Point(AdornedElement.RenderSize.Width / 2,
            //    AdornedElement.RenderSize.Height - AdornedElement.RenderSize.Height);
            //var endPoint = new Point(AdornedElement.RenderSize.Width / 2,
            //    AdornedElement.RenderSize.Height - AdornedElement.RenderSize.Height - 16);
            //drawingContext.DrawLine(renderPen, startPoint, endPoint);
            tMove.Arrange(new Rect(new Point(0, 0), new Size(RenderSize.Width, RenderSize.Height)));
            tLeft.Arrange(new Rect(new Point(-offset, -offset), sz));
            tRight.Arrange(new Rect(new Point(AdornedElement.RenderSize.Width - offset, -offset), sz));
            bLeftBottom.Arrange(new Rect(new Point(-offset, AdornedElement.RenderSize.Height - offset), sz));
            bRightBottom.Arrange(new Rect(
                new Point(AdornedElement.RenderSize.Width - offset, AdornedElement.RenderSize.Height - offset), sz));
        }

        private Thumb CreateMoveThumb()
        {
            var thumb = new Thumb
            {
                Cursor = Cursors.SizeAll,
                Template = new ControlTemplate(typeof(Thumb))
                {
                    VisualTree = GetFactory(GetMoveEllipse())
                }
            };
            thumb.DragStarted += (s, e) => { DragStarted?.Invoke(s, AdornedElement); };
            thumb.DragDelta += (s, e) =>
            {
                OnDrage?.Invoke(AdornedElement, e);
                var check = DrageMove?.Invoke(AdornedElement, e);
                if(check != null && (bool)check)
                {
                    return;
                }
                var element = AdornedElement as FrameworkElement;
                if (element == null)
                    return;
                if(EnableHorizontal)
                Canvas.SetLeft(element, Canvas.GetLeft(element) + e.HorizontalChange);
                if(EnableVertical)
                Canvas.SetTop(element, Canvas.GetTop(element) + e.VerticalChange);
            };
            return thumb;
        }

        private Brush GetMoveEllipse()
        {
            return new DrawingBrush(new GeometryDrawing(Brushes.Transparent, null, null));
        }

        /// <summary>
        /// 创建Thumb
        /// </summary>
        /// <param name="cursor">鼠标</param>
        /// <param name="horizontal">水平</param>
        /// <param name="vertical">垂直</param>
        /// <returns></returns>
        private Thumb CreateThumb(Cursor cursor, HorizontalAlignment horizontal, VerticalAlignment vertical)
        {
            var thumb = new Thumb
            {
                Cursor = cursor,
                Width = ThumbSize,
                Height = ThumbSize,
                HorizontalAlignment = horizontal,
                VerticalAlignment = vertical,
                Template = new ControlTemplate(typeof(Thumb))
                {
                    VisualTree = GetFactory(new SolidColorBrush(Colors.White))
                }
            };

            thumb.DragDelta += (s, e) =>
            {
                var element = AdornedElement as FrameworkElement;
                if (element == null) return;
               
                Resize(element);
                var check = ThumbMove?.Invoke(AdornedElement, e);
                if (check != null && (bool)check)
                {
                    return;
                }
                if (EnableVertical)
                {
                    switch (thumb.VerticalAlignment)
                    {
                        case VerticalAlignment.Bottom:
                            if (element.Height + e.VerticalChange > ElementMiniSize) element.Height += e.VerticalChange;
                            break;
                        case VerticalAlignment.Top:
                            if (element.Height - e.VerticalChange > ElementMiniSize)
                            {
                                element.Height -= e.VerticalChange;
                                Canvas.SetTop(element, Canvas.GetTop(element) + e.VerticalChange);
                            }

                            break;
                    }
                }

                if (EnableHorizontal)
                {
                    switch (thumb.HorizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            if (element.Width - e.HorizontalChange > ElementMiniSize)
                            {
                                element.Width -= e.HorizontalChange;
                                Canvas.SetLeft(element, Canvas.GetLeft(element) + e.HorizontalChange);
                            }

                            break;
                        case HorizontalAlignment.Right:
                            if (element.Width + e.HorizontalChange > ElementMiniSize) element.Width += e.HorizontalChange;
                            break;
                    }
                } 
                e.Handled = true;
                OnMoveDrage?.Invoke(element, e);
            };
            return thumb;
        }

        private void Resize(FrameworkElement fElement)
        {
            if (double.IsNaN(fElement.Width))
                fElement.Width = fElement.RenderSize.Width;
            if (double.IsNaN(fElement.Height))
                fElement.Height = fElement.RenderSize.Height;
        }

        private FrameworkElementFactory GetFactory(Brush back)
        {
            var elementFactory = new FrameworkElementFactory(typeof(Ellipse));
            elementFactory.SetValue(Shape.FillProperty, back);
            return elementFactory;
        }


        protected override Visual GetVisualChild(int index)
        {
            return visualCollection[index];
        }

        #region 可拖放

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

        private void RemoveEvent(UIElement uI)
        {
            if (uI == null) return;
            uI.PreviewMouseMove -= UI_PreviewMouseMove;
            uI.PreviewMouseDown -= UI_PreviewMouseDown;
            uI.PreviewMouseUp -= UI_PreviewMouseUp;
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
                transform.X = _mouseDownControlPosition.X + dp.X;
                transform.Y = _mouseDownControlPosition.Y + dp.Y;
            }
        }
        #endregion
    }
}
