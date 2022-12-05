using ControlHelper;
using ControlHelper.WPF;
using DisplayConveyer.Controls;
using DisplayConveyer.Model;
using DisplayConveyer.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
using System.Windows.Shapes;
using System.Xml.Linq;
 

namespace DisplayConveyer.View
{
    /// <summary>
    /// EditorWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditorWindow : Window
    {
        private readonly UIElementMove canvasMove;
        private readonly List<UC_DeviceBase> selectorList = new List<UC_DeviceBase>(); 
        private readonly List<UC_DeviceBase> devicesList = new List<UC_DeviceBase>();
        private Point startPoint;
        private Point currentPoint; 
        private Rectangle selectorRect; 
        private UC_DeviceBase selectedDevice;
        private bool inSelector;
        private ClassControl singleDevice  ;

        public List<AreaData> Areas { get;   set; }   
        public EditorWindow()
        {
            InitializeComponent();
            canvasMove = new UIElementMove(canvas, this)
            {
                CanInput = true,
            };
            btnTest.Click += (s, e) =>
            {
                Window window = new Window();
                window.Title = "编辑对应信息";
                Grid grid = new Grid();
                grid.Margin = new Thickness(5);
                window.Content = grid;
                ScrollViewer sv = new ScrollViewer();
                sv.Margin = new Thickness(5);
                grid.Children.Add(sv);
                sv.Content = new ClassControl(typeof(AreaData));
                window.ShowDialog();
            };
            btnAddDevice.Click += (s, e) =>
            {
                for (int i = 0; i < 4; i++)//测试代码
                {
                    CreateDeviceControl(new DeviceData()
                    {
                        ID = $"A000{i+1}",
                        Name = $"{i + 1}号啊啊",
                        PosX = (120) * i + 155,
                        PosY = 55,
                        Width = 120,
                        Height = 50,
                        FontSize = 9,
                    }) ;
                }
            };
            MouseRightButtonDown += (s, e) =>
            {
                ClearSelector();
                SetSelectControl(null);
                if (singleDevice != null)
                {
                    gOper.Children.Remove(singleDevice);
                }
            };
            btnCvLock.Click += (s, e) =>
            {
                if (s is Button btn)
                {
                    var tag = btn.Tag.ToString();
                    if (tag == "Lock")
                    {
                        canvasMove.CanInput = false; 
                        btn.Content = "解锁";
                        btn.Background = new SolidColorBrush(Colors.LightGray);
                        btn.Tag = "Unlock";
                    }
                    else if (tag == "Unlock")
                    {
                        canvasMove.CanInput = true; 
                        btn.Background = new SolidColorBrush(Colors.White);
                        btn.Content = "锁定";
                        btn.Tag = "Lock";
                    }
                }
            };
            btnCvReset.Click += (s, e) =>
            {
                canvasMove.ReSet(0, 0);
            };
            btnCopy.Click += (s, e) =>
            {
                double copyOffset = 25;
                if (selectorList.Count == 0 && selectedDevice != null   )
                {
                    //单个复制 
                    var clone = selectedDevice.Data.Clone();
                    clone.Name = $"{clone.Name}_clone";
                    clone.ID = $"{clone.ID}_clone";
                    clone.PosX += copyOffset;
                    clone.PosY += copyOffset;
                    SetSelectControl(CreateDeviceControl(clone));
                }
                else if (selectorList.Count > 0)
                { 
                    //多选复制
                    List<DeviceData> temp = selectorList.Select(a => a.Data).ToList().Clone();
                    ClearSelector();
                    for (int i = 0; i < temp.Count; i++)
                    { 
                        var clone = temp[i];
                        clone.Name = $"{clone.Name}_clone{(i + 1)}";
                        clone.ID = $"{clone.ID}_clone{(i + 1)}";
                        clone.PosX += copyOffset;
                        clone.PosY += copyOffset;
                        AddSelector(CreateDeviceControl(clone));
                    } 
                }
            };
            btnEditAreas.Click += (s, e) =>
            {
                DrawerTopInContainer.IsOpen ^= true;
            };
            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseUp += Canvas_MouseUp; 
        }
        private ClassControl ShowDataInfo(Type type, object data)
        {
            if (singleDevice != null)
            {
                gOper.Children.Remove(singleDevice);
            }
            if (cbShowDetail.IsChecked == false) return null;
            singleDevice = new ClassControl(type, true, data);
            singleDevice.HorizontalAlignment = HorizontalAlignment.Left;
            singleDevice.Width = 300;
            singleDevice.Height = gOper.ActualHeight;
            var t = singleDevice.RenderTransform = new TranslateTransform(0, 15);

            DoubleAnimation animation = new DoubleAnimation()
            {
                From = -singleDevice.Width,
                To = 15,
                DecelerationRatio = .5d,
                Duration = new Duration(TimeSpan.FromSeconds(0.5d)),
            };
            t.BeginAnimation(TranslateTransform.XProperty, animation);
            //gOper.Children.Add(singleDevice);
            Panel.SetZIndex(singleDevice, 99);
            return singleDevice;
        }
        private ComboBox CreateAreaSelector()
        {
            ComboBox cmb = new ComboBox();
            cmb.ItemsSource = Areas.Select(a => $"{a.ID}_{a.Name}").ToArray();
            cmb.SelectionChanged += (s, e) =>
            {
                var selectItem = cmb.SelectedItem.ToString().Split('_')[0];
                if(selectedDevice != null)
                {
                    selectedDevice.Data.AreaID =int.Parse(selectItem);
                }
            };
            return cmb;
        }
        private UC_DeviceBase CreateDeviceControl(DeviceData data)
        { 
            UC_DeviceBase device = new UC_DeviceBase(data); 
            device.MouseDown += (ds, de) =>
            {
                if(selectedDevice != null && selectedDevice != ds)
                {
                    return;
                }
                if (selectorList.Count == 0)
                {
                    var ucdb = ds as UC_DeviceBase;
                    SetSelectControl(ucdb); 
                }
            };
            device.MouseEnter += (ds, de) =>
            {
                device.borderSelect.Visibility = Visibility.Visible;
            }; 
            device.MouseLeave += (ds, de) =>
            {
                device.borderSelect.Visibility = Visibility.Collapsed;

            };
            device.SizeChanged += (ds, de) =>
            { 
                device.ToolTip = device.Info;
            };  
            canvas.Children.Add(device);
            //装饰器 一定要添加到父容器之后再执行
            var al = AdornerLayer.GetAdornerLayer(device);
            var adorner = new ElementAdorner(device);
            adorner.OnThumbMouseDown += (ds, de) =>
            {
                if (selectorList.Count > 1)
                    selectedDevice = de as UC_DeviceBase;
            };
            adorner.OnDrage += (ds, de) =>
            { 
                var pos = device.TransformToAncestor(canvas).Transform(new Point(0, 0));
                data.PosX = pos.X;
                data.PosY = pos.Y;
                device.ToolTip = device.Info;
                if (selectorList.Count <= 1) return;
                foreach (var item in selectorList)
                {
                    if (item == selectedDevice) continue;
                    Canvas.SetLeft(item, Canvas.GetLeft(item) + de.HorizontalChange);
                    Canvas.SetTop(item, Canvas.GetTop(item) + de.VerticalChange);
                    pos = item.TransformToAncestor(canvas).Transform(new Point(0, 0));
                    item.Data.PosX = pos.X;
                    item.Data.PosY = pos.Y;
                    item.ToolTip = item.Info;
                }  
            };
            al.Add(adorner);
            devicesList.Add(device);
            return device;
        } 
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //if (!isLocked) return;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                inSelector = true;
                startPoint = e.GetPosition(canvas);
                canvas.Children.Remove(selectorRect);
                selectorRect = new Rectangle();
                selectorRect.Fill = new SolidColorBrush(Color.FromArgb(96, 0, 0, 255));
                selectorRect.RenderTransform = new TranslateTransform(startPoint.X, startPoint.Y);
                canvas.Children.Add(selectorRect);

            }
        }
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            //if (!isLocked) return;
            if (e.LeftButton == MouseButtonState.Pressed && selectorRect != null)
            {
                if (selectedDevice != null)
                { 
                    //确保在画框选定时 单个选中被清空
                    SetThumShowOrHide(selectedDevice, false);
                    if (singleDevice != null)
                    {
                        gOper.Children.Remove(singleDevice);
                    }
                    selectedDevice = null;
                }
                currentPoint = e.GetPosition(canvas);
                selectorRect.Width = Math.Abs(currentPoint.X - startPoint.X);
                selectorRect.Height = Math.Abs(currentPoint.Y - startPoint.Y);
                CheckInSelector(startPoint, currentPoint);
                SetTxtInfo($"startPoint:{startPoint},currentPoint:{currentPoint} ");
            }
        }
        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //if (!isLocked) return;
            if (e.LeftButton == MouseButtonState.Released)
            {
                canvas.Children.Remove(selectorRect);
                inSelector = false;
                SetTxtInfo(string.Empty);
            }
        }
        private void CheckInSelector(Point startPoint,Point currentPoint)
        {
            if (!inSelector) return  ;
            foreach (var item in devicesList)
            {
                var point = item.TransformToVisual(canvas).Transform(new Point(0, 0));
                if(point.X >= startPoint.X && point.X <= currentPoint.X  
                    && point.Y >= startPoint.Y && point.Y <= currentPoint.Y  )
                {
                    AddSelector(item  );
                }
                else
                {
                    RemoveSelector(item ); 
                }
            }
        }   
        private void ClearSelector()
        {
            foreach (var item in selectorList)
            {
                SetThumShowOrHide(item, false);
            }
            selectorList.Clear();
        }
        private void AddSelector(UC_DeviceBase uiele)
        {
            if (!selectorList.Contains(uiele))
            {
                SetThumShowOrHide(uiele, true);
                selectorList.Add(uiele);
            } 
        }
        private void RemoveSelector(UC_DeviceBase uiele)
        {
            if (selectorList.Contains(uiele))
            {
                SetThumShowOrHide(uiele, false);
                selectorList.Remove(uiele);
            }
        } 
        /// <summary>
        /// 设置当前选中的控件 单击
        /// </summary>
        /// <param name="ucdb"></param>
        private void SetSelectControl(UC_DeviceBase ucdb)
        {
            if (selectedDevice != null)
            {
                SetThumShowOrHide(selectedDevice, false);
                selectedDevice = null;
            }
            if (ucdb != null)
            {
                selectedDevice = ucdb;
                var control = ShowDataInfo(ucdb.Data.GetType(), ucdb.Data);
                control.NewData += (o) =>
                {
                    selectedDevice.ViewModel.Data = o as DeviceData;
                };
               gOper.Children.Add(control);
             
                SetThumShowOrHide(selectedDevice, true);
            }
        }
        /// <summary>
        /// 设置可缩放显示/隐藏
        /// </summary>
        /// <param name="uiele"></param>
        /// <param name="showOrHide"></param>
        private void SetThumShowOrHide(UIElement uiele,bool showOrHide)
        {
            var al = AdornerLayer.GetAdornerLayer(uiele);
            var adorners = al.GetAdorners(uiele);
            if(adorners==null)
            {
                //todo:记录错误
                return;
            }
            foreach (var adorner in adorners)
            {
                if (adorner is ElementAdorner ea)
                {
                    if (showOrHide) 
                        ea.Show();
                    else
                        ea.Hide();
                    break;
                }
            }
        } 
        private void SetTxtInfo(string info) => txtInfo.Text = info;
    }
}
