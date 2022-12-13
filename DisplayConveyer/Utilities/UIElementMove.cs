using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;

namespace DisplayConveyer.Utilities
{
    /// <summary>
    /// 对ui元素进行缩放移动操作
    /// </summary>
    public class UIElementMove
    {
        private Matrix mymat;
        private Point startpoint;
        private Point currentpoint;
        private UIElement element;
        private UIElement parent;

        public enum KeyCode
        {
            Left,
            Middle,
            Right,
        }

        public bool CanInput { get; set; }
        public KeyCode Key { get; private set; }
        public UIElementMove(UIElement element, Window window, KeyCode key = KeyCode.Middle)
        {
            mymat = new Matrix(1, 0, 0, 1, 0, 0);//存储当前控件位移和比例 
            this.element = element;
            parent = window;

            element.MouseWheel += Canvas_MouseWheel;
            element.MouseDown += Canvas_MouseDown;
            element.MouseMove += Canvas_MouseMove;
            Key = key;
        }
        public void ReSet() => MatrixChange(15, 15, 1);
  
        public void ReSet(double x,double y) => MatrixChange(x, y, 1);
        private void MatrixChange(double dx, double dy)
        {
            mymat.OffsetX = dx;
            mymat.OffsetY = dy;
            element.RenderTransform = new MatrixTransform(mymat);
        }
        private void MatrixChange(double dx, double dy, double scale)
        {
            mymat.M11 = scale;
            mymat.M22 = scale;
            mymat.OffsetX = dx;
            mymat.OffsetY = dy;
            element.RenderTransform = new MatrixTransform(mymat);
        }
        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            GlobalPara.RecordTime();
            if (!CanInput) return;
            Point p1 = e.GetPosition(element);//得当鼠标相对于控件的坐标

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
                if (scale < 0.1)
                {
                    scale = 0.1;
                    return;
                }

                dx = p1.X * (scale + 0.2) - scale * p1.X + mymat.OffsetX;
                dy = p1.Y * (scale + 0.2) - scale * p1.Y + mymat.OffsetY;
                MatrixChange(dx, dy, scale);
            }
        }
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            GlobalPara.RecordTime();
            if (!CanInput) return;

            var uiEle = sender as UIElement;
            if (uiEle == null) return;
            if (CheckPress(e))
            { 
                Point currp = e.GetPosition(parent);
                double dx = currp.X - startpoint.X + currentpoint.X;
                double dy = currp.Y - startpoint.Y + currentpoint.Y;//总位移等于当前的位移加上已有的位移
                MatrixChange(dx, dy);//移动控件，并更新总位移
            }


        }
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GlobalPara.RecordTime();
            if (!CanInput) return;
            if (CheckPress(e))
            {
                startpoint = e.GetPosition(parent);//记录开始位置
                currentpoint.X = mymat.OffsetX;//记录Canvas当前位移
                currentpoint.Y = mymat.OffsetY;
            }
        }

        private bool CheckPress(MouseEventArgs e)
        {
            switch (Key)
            {
                case KeyCode.Left:
                     return e.LeftButton == MouseButtonState.Pressed;
                case KeyCode.Middle:
                    return e.MiddleButton == MouseButtonState.Pressed;
                case KeyCode.Right:
                    return e.RightButton == MouseButtonState.Pressed;
                default:
                    return false;
            }
        }
    }
}
