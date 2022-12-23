using DisplayConveyer.Controls;
using DisplayConveyer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DisplayConveyer.Utilities
{
    public static class CreateHelper
    {
        public static FrameworkElement GetRect(RectData data)
        {
            var rect = new Rectangle()
            {
                Width = data.Width,
                Height = data.Height,
                Stroke = new SolidColorBrush(Colors.LightGreen),
                StrokeThickness = data.StrokeThickness,
            };
            rect.DataContext = data;
            rect.SetValue(Canvas.LeftProperty, data.PosX);
            rect.SetValue(Canvas.TopProperty, data.PosY);
            Panel.SetZIndex(rect, -100);
            return rect;
        }
        public static FrameworkElement GetTextBlock(LabelData data)
        {
            var tb = new TextBlock
            {
                Text = data.Text,
                FontSize = data.FontSize,
                Background = new SolidColorBrush(Colors.Transparent)
            };
            //data.EnableThumbVertical = false;
            //data.EnableThumbHorizontal = false;
            tb.DataContext = data;
            tb.SetValue(Canvas.LeftProperty, data.PosX);
            tb.SetValue(Canvas.TopProperty, data.PosY);
            return tb;
        }
        public static FrameworkElement GetDeviceBase(DeviceData data)
        {
            var udc = new UC_DeviceBase(data)
            {
                Width = data.Width,
                Height = data.Height,
                Title = data.Name,
                FontSize = data.FontSize,
                Description = data.Direction,
            };
            udc.DataContext = data;
            udc.SetValue(Canvas.LeftProperty, data.PosX);
            udc.SetValue(Canvas.TopProperty, data.PosY);
            return udc;
        }
    }
}
