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
using CommonApi;
using DeviceConfig;
using DisplayBorder.Controls;
using DisplayBorder.Events;
using HandyControl.Controls; 
using Window = HandyControl.Controls.Window;

namespace DisplayBorder.View
{
    /// <summary>
    /// WindowGroupsConfig.xaml 的交互逻辑
    /// </summary>
    public partial class WindowGroupsConfig : Window
    {

        public class GroupTitleControls
        {
            public Canvas Canvas { get; set; }
            public Border Border   { get; set; }

            public TitleControl TitleControl { get; set; }

            public Point GetPoint 
            {
                get
                {
                    if (Canvas == null || Border == null || TitleControl == null)
                    {
                        return new Point(0, 0);
                    }
                    return Border.TransformToAncestor(Canvas).Transform(new Point(0, 0));
                } 
            }

            public Size GetTitleSize
            {
                get
                {
                    if(TitleControl == null)
                    {
                        return new Size(0, 0);
                    }

                    return new Size(TitleControl.ActualWidth, TitleControl.ActualHeight);
                }
            }

        }

        public WindowGroupsConfig()
        {
            InitializeComponent(); 
             Loaded += (s, e) =>
            {

                //-----
                GlobalPara.EventManager.Subscribe(OnCanvasChildrenClickArgs.EventID, OnClickChildren);
                //----
                foreach (var item in C1.Children)
                {
                    if(item is Border border)
                    {
                        AdornerLayer.GetAdornerLayer(border).Add( new ElementAdorner(border));
                    }
                } 
            };

            Unloaded += (s, e) =>
            {  
                GlobalPara.EventManager.Unsubscribe(OnCanvasChildrenClickArgs.EventID, OnClickChildren);
            };
           
        }
        private ElementAdorner cacheEa;
        private TitleControl cacheTc; 
        private List<Group> groups = new List<Group>(); 
        private void OnClickChildren(object sender, BaseEventArgs e)
        {
            Console.WriteLine("hit hit hit hit hit ");
            if (e is OnCanvasChildrenClickArgs args)
            {
                if(args.Source == null  )
                {
                    if (cacheEa != null)
                         cacheEa.Hide();
                    if (cacheTc != null)
                        cacheTc.HideDot();
                }
                else
                {
                    if (cacheEa != null)
                    {
                        cacheEa.Hide();
                    }
                    if (cacheTc != null)
                        cacheTc.HideDot();
                    if (args.Source is Control control && control.Parent is Border border)
                    { 
                        var al = AdornerLayer.GetAdornerLayer(border);
                        var adorners = al.GetAdorners(border); 
                        foreach (var adorner in adorners)
                        {
                            if (adorner is ElementAdorner ea)
                            {
                                ea.Show();
                                cacheEa = ea;
                                break;
                            }
                        }
                        if(control is TitleControl tc)
                        {
                            tc.ShowDot();
                            cacheTc = tc;
                        }
                    }
                } 
            }
        }
         
        private void Btn_ClickAddGroup(object sender, RoutedEventArgs e)
        {
            WindowHelper.Create<Group, DataControl>((g) =>
            {

                TitleControl tc = new TitleControl(g); 
                tc.SetValue(CanvasHelper.IsSelectableProperty, true);
                Border border = new Border(); 
                border.Width = 100;
                border.Height = 100;
                border.Child = tc;
                border.SetValue(Canvas.LeftProperty, 20d);
                border.SetValue(Canvas.TopProperty, 20d);
                
             
                C1.Children.Add(border);
                var al = AdornerLayer.GetAdornerLayer(border);
                al.Add(new ElementAdorner(border));  
            }, null, this);
        }

        private void Btn_SwitchDirection(object sender, RoutedEventArgs e)
        {
        
            if (cacheTc != null)
            {
             
                var dir = TitleControl.DirectionArrow.Up;
                if(cacheTc.Direction == TitleControl.DirectionArrow.Up)
                {
                    dir = TitleControl.DirectionArrow.Down;
                }
                else
                {
                    dir = TitleControl.DirectionArrow.Up;
                }
                cacheTc.SwitchDricetion(dir); 
            }
        }

        #region 可拖拽

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
                transform.X = _mouseDownControlPosition.X + dp.X;
                transform.Y = _mouseDownControlPosition.Y + dp.Y;
            }
        }
        #endregion
    }


}
