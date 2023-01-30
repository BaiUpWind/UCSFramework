using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WpfTest
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            gridMain.MouseWheel += GridMain_MouseWheel;
            canvasMain.RenderTransform = new TranslateTransform(0, 0);

            Loaded += (s, e) =>
            {
                IniChildren(gridMain);

                UIElement[] temp = new UIElement[gridMain.Children.Count];
                gridMain.Children.CopyTo(temp, 0);
                listAllCanvas = temp.Where(a => a is Canvas).Select(a => a as Canvas).ToList();

                //-----------

                for (int i = 0; i < 5; i++)
                {

                    Border border = new Border();
                    border.Margin = new Thickness(15, 0, 0, 0);
                    border.Height = 300d;
                    border.Background = new SolidColorBrush(Colors.White);
                    border.Width = 300;
                    border.VerticalAlignment = VerticalAlignment.Center;
                    border.HorizontalAlignment = HorizontalAlignment.Center;
                    border.Name = "边框_" + (i + 1);

                    TextBlock tb = new TextBlock();
                    tb.Text = border.Name;
                    tb.FontSize = 48;
                    border.Child = tb;
                    spMain.Children.Add(border);
                    listTempBorder.Add(border);
                }

                pLine = lineInd.TransformToAncestor(gridMain).Transform(new Point(0, 0));
                tbInfo.Text = pLine.ToString();
                //th = new Thread(ProgressMove);
                //th.Start();
                Task.Run(async () =>
                {
                    while (true)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            progress.Value += 1;
                            if (progress.Value == 100)
                            {
                                progress.Value = 0;
                            }
                        }));
                        await Task.Delay(200);
                       
                    } 
                });
            };
            SizeChanged += (s, e) =>
            {
                tbInfo.Text = pLine.ToString();
                var t = lineInd.RenderTransform as TranslateTransform;
                if (t == null)
                {
                   
                    lineInd.RenderTransform = t = new TranslateTransform();
                }
                t.X = gridMain.ActualWidth / 2;
                pLine = lineInd.TransformToAncestor(gridMain).Transform(new Point(0, 0));
                tbInfo.Text = pLine.ToString();

            };
        }
        Point pLine;
        List<Border> listTempBorder = new List<Border>();
        List<Canvas> listAllCanvas;
        Thread th;
        private void GridMain_MouseWheel(object sender, MouseWheelEventArgs e)
        { 
            for (int i = 0; i < listAllCanvas.Count; i++)
            {
                var current = listAllCanvas[i];
                if (current == null) continue;
                var t = current.RenderTransform as TranslateTransform;
                if (t == null) continue;

                if (t.X + canvasMain.ActualWidth < 0 && e.Delta < 0)
                {
                    Canvas last = listAllCanvas[listAllCanvas.Count - 1];
                    var lastT = last.RenderTransform as TranslateTransform;
                    t.X = lastT.X + last.ActualWidth + 10;
                    listAllCanvas.Remove(current);
                    listAllCanvas.Add(current);
                    break;
                }
                else if (t.X > canvasMain.ActualWidth && e.Delta > 0)
                {
                    Canvas first = listAllCanvas[0];
                    var firstT = first.RenderTransform as TranslateTransform;
                    t.X = firstT.X - first.ActualWidth - 10;
                    listAllCanvas.Remove(current);
                    listAllCanvas.Insert(0, current);
                    break;
                }
                else
                {
                    t.X += e.Delta * 0.15d;
                }
            }
            tbCurrent.Text = string.Empty;
            foreach (var item in listTempBorder)
            {
                var pointA = item.TransformToAncestor(gridMain).Transform(new Point(0, 0));
                Point pointB = new Point(pointA.X + item.ActualWidth, pointA.Y); 
                if (item.Child is TextBlock tb)
                {
                    tb.Text = item.Name + "\r\n" + pointA + "\r\n" + pointB;
                }
                if (CheckInLine(pointA, pointB, pLine))
                {
                    tbCurrent.Text = item.Name;
                    break;
                }
            }
        }
        private void ProgressMove()
        {
            while (true)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    progress.Value += 1;
                }));
                Thread.Sleep(1000);
            }
        }
        private void IniChildren(Panel pane)
        {
            double maxWidth = 0;
            foreach (var item in pane.Children)
            {
                var canvas = item as Canvas;
                if (canvas != null)
                {
                    var t = canvas.RenderTransform as TranslateTransform;
                    if (t == null)
                    {
                        canvas.RenderTransform = new TranslateTransform(maxWidth, 0);
                    }
                    maxWidth += canvas.ActualWidth + 5;

                }
            }
        }

        private bool CheckInLine(Point pointA,Point pointB , Point pLine)
        {
            return pLine.X>= pointA.X &&   pLine.X  <= pointB.X ;
        }
    }
}
