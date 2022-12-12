using ControlHelper;
using ControlHelper.WPF;
using DisplayConveyer.Config;
using DisplayConveyer.Controls;
using DisplayConveyer.Model;
using DisplayConveyer.Utilities;
using HandyControl.Controls;
using HandyControl.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ComboBox = HandyControl.Controls.ComboBox;
using MessageBox = HandyControl.Controls.MessageBox;
using ScrollViewer = System.Windows.Controls.ScrollViewer;
using TextBox = System.Windows.Controls.TextBox;
using Window = System.Windows.Window;

namespace DisplayConveyer.View
{
    /// <summary>
    /// EditorWindow.xaml 的交互逻辑
    /// </summary>
    public partial class EditorWindow : Window
    {
        private readonly UIElementMove canvasMove; 
        //所有框选的设备集合
        private readonly List<UC_DeviceBase> selectorList = new List<UC_DeviceBase>();
        //当前所有设备的集合 存在的目的为了框选时计算是否在框内
        private readonly List<UC_DeviceBase> devicesList = new List<UC_DeviceBase>();
        private List<Line> allLines = new List<Line>();
        private Point startPoint;
        private Point currentPoint; 
        private Rectangle selectorRect; 
        private UC_DeviceBase selectedDevice;
        //同时最多只能有一个新建在场
        private bool haveNew;
        private bool inSelector; 
        //当前选择的设备
        private FrameworkElement singleDevice  ;
        //新增新建设备界面
        private StackPanel newCreate; 
        //物流线配置 
        private ConveyerConfig ConvConfig;
        private Rectangle rectangleCache;

        private List<AreaData> Areas => ConvConfig?.Areas;



        private double CanvasWidth
        { 
            set
            {
                txtCanvasWidth.Text = value.ToString();
                canvas.Width = value;
                if (ConvConfig != null) ConvConfig.CanvasWidth = value;
            }
        } 
        private double CanvasHeight
        { 
            set
            {
                txtCanvasHeight.Text = value.ToString();
                canvas.Height = value;
                if (ConvConfig != null) ConvConfig.CanvasHeight = value;
            }
        }

        internal bool HaveNew
        {
            get => haveNew; set { 
                
                haveNew = value;

                canvasMove.CanInput = !value;
            }
        }

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
            //添加一个设备
            btnAddDevice.Click += (s, e) =>
            {
                if (!CheckConveyerConfig()) return;
                if (HaveNew) return;
                HaveNew = true; 
                var orginData = new DeviceData()
                {
                    AreaID = 0,
                    Name = $"新建设备",
                    Width = 50,
                    Height = 30,
                    FontSize = 9,
                };
                newCreate = null;
                newCreate = new StackPanel(); 
                #region
                StackPanel spOp = new StackPanel();
                spOp.HorizontalAlignment = HorizontalAlignment.Right;
                spOp.Orientation = Orientation.Horizontal;
                Button btnAdd = new Button()
                {
                    Content = "添加",
                    Margin = new Thickness(0, 0, 15, 0)
                };

                Button btnClose = new Button()
                {
                    Content = "关闭",
                    Margin = new Thickness(0, 0, 15, 0)
                };
                spOp.Children.Add(btnAdd);
                spOp.Children.Add(btnClose);

                #endregion
                newCreate.Children.Add(spOp);
                newCreate.Background = new SolidColorBrush(Color.FromArgb(125, 255, 255, 255));
                newCreate.Width = 300; 
                newCreate.HorizontalAlignment = HorizontalAlignment.Left;
                newCreate.Height = gOper.ActualHeight;
                newCreate.Orientation = Orientation.Vertical; 
                UIElementMove um = new UIElementMove(newCreate, this, UIElementMove.KeyCode.Middle);
                um.CanInput = true;
               var moup= Mouse.GetPosition(canvas);
                um.ReSet(moup.X+100, moup.Y);
                var cc = new ClassControl(typeof(DeviceData), true, orginData);
                cc.Margin = new Thickness(0, 5, 0, 5);
                var cmb = CreateAreaIDSelector(cc);
                cmb.VerticalAlignment = VerticalAlignment.Center;
                cmb.Width = 290;
                cmb.Margin = new Thickness(0, 5, 0, 5);
           
                newCreate.Children.Add(cmb);
                newCreate.Children.Add(cc);
                btnAdd.Click += (bs, be) =>
                {
                  
                    if(cmb.Tag != null && cmb.Tag.ToString() == "useless")
                    {
                        MessageBox.Show("请先添加区域!");
                        canvas.Children.Remove(newCreate);
                        return;
                    }
                    var data = cc.Data as DeviceData;
                    if(data.AreaID == 0)
                    {
                        MessageBox.Show("请选择对应的区域", "错误");
                        return;
                    }
                    var cdc =   CreateDeviceControl(data );
                    var point = btnAdd.TransformToAncestor(canvas).Transform(new Point(0, 0));
                    if(point.X > canvas.Width)
                    {
                        point.X = canvas.Width - cdc.Width;
                    }
                    if(point.X < 0)
                    {
                        point.X = 0;
                    }
                    if(point.Y > canvas.Height  )
                    {
                        point.Y = canvas.Height - cdc.Height;
                    }
                    if(point.Y <= 0)
                    {
                        point.Y = 0;
                    }
                    cdc.ViewModel.Data.PosX = point.X;
                    cdc.ViewModel.Data.PosY = point.Y;
                    HaveNew = false;
                    canvas.Children.Remove(newCreate);
                };
                btnClose.Click += (bs, be) =>
                {
                    HaveNew = false;
                    canvas.Children.Remove(newCreate);
                }; 
                canvas.Children.Add(newCreate);
            };
            //删除一个设备
            btnRemoveDevice.Click += (s, e) =>
            {
                if (!CheckConveyerConfig()) return;
                if (selectedDevice != null && selectorList.Count == 0)
                {
                    AreasRemoveDevice(selectedDevice.Data);
                    devicesList.Remove(selectedDevice);
                    canvas.Children.Remove(selectedDevice);
                    if (singleDevice != null)
                    {
                        gOper.Children.Remove(singleDevice);
                    }
                    selectedDevice = null; 
                }
                else if (selectorList.Count > 0)
                {
                    foreach (var item in selectorList)
                    {
                        devicesList.Remove(item);
                        AreasRemoveDevice(item.Data);
                        canvas.Children.Remove(item);
                    }
                    selectorList.Clear();
                    selectedDevice = null;
                }
            };
            //鼠标右键按下
            MouseRightButtonDown += (s, e) =>
            {
                ClaerAllSelection();
            };
            //锁定画布
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
            //画布复原
            btnCvReset.Click += (s, e) =>
            {
                canvasMove.ReSet(0, 0);
            };
            //复制
            btnCopy.Click += BtnCopy_Click;
            //打开区域编辑器
            btnEditAreas.Click += (s, e) =>
            {
                if (!CheckConveyerConfig()) return;
                if (newCreate != null) canvas.Children.Remove(newCreate);
            
                ClaerAllSelection();
                DrawerTopInContainer.IsOpen ^= true;
                HaveNew = false;
                if (DrawerTopInContainer.IsOpen)
                {
                    svArea.Content = null;
                    var cc = new ClassControl(typeof(List<AreaData>), true, Areas);
                    cc.NewData += (o) =>
                    {
                        ConvConfig.Areas = o as List<AreaData>;
                    };
                    svArea.Content = cc;
                }
            };
            Loaded += (s, e) =>
            {
                AddRect();
                Draw(canvas);
                EnableMyStackPanel(false);
            };
            canvas.SizeChanged += (s, e) =>
            {
                if (canvas != null) Draw(canvas);
            };
            canvas.MouseDown += Canvas_MouseDown;
            canvas.MouseMove += Canvas_MouseMove;
            canvas.MouseUp += Canvas_MouseUp;
            cbShowGrid.Checked += CbShowGrid_Checked;
            cbShowGrid.Unchecked += CbShowGrid_Checked;
            btnOpenCfg.Click += OpenAndSaveConfig;
            btnSaveConfig.Click += (s, e) =>
            {
                GlobalPara.ConveyerConfig = ConvConfig;
            };

            this.KeyDown += (s, e) =>
            {
                if(e.Key == Key.C)
                {
                    inSelector = false;
                    BtnCopy_Click(null, null);
                }
            };
            
        }
        #region 配置文件打开和保存

        private void OpenAndSaveConfig(object sender,RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                var tag = btn.Tag.ToString();
                if (tag.Equals("open"))
                {
                    if (ConvConfig != null)
                    {
                        //todo:提示当前已经有打开但是未保存是否覆盖
                    }
                    ConvConfig = GlobalPara.ConveyerConfig.Clone();
                    if (ConvConfig == null) return;
                    CanvasHeight = ConvConfig.CanvasHeight;
                    CanvasWidth = ConvConfig.CanvasWidth; 
                    EnableMyStackPanel(true);
                    canvas.Children.Clear();
                 
                    foreach (var area in Areas)
                    {
                        foreach (var device in area.Devices)
                        {
                            CreateDeviceControl(device);
                        }
                    }
                    Draw(canvas);
                    btn.Background = new SolidColorBrush(Colors.OrangeRed);
                    btn.Content = "关闭配置";
                    btn.Tag = "close";
                }
                else if (tag.Equals("close"))
                {
                    DrawerTopInContainer.IsOpen  = false;
                    GlobalPara.ConveyerConfig = ConvConfig;
                    ClaerAllSelection();
                    EnableMyStackPanel(false);
                    //todo:提示保存成功
                    canvas.Children.Clear();
                    Draw(canvas);
                    ConvConfig = null;
                    btn.Background = new SolidColorBrush(Colors.White);
                    btn.Content = "打开配置";
                    btn.Tag = "open";
                }
            }
      
        }
   
        #endregion

        private void EnableMyStackPanel(bool enable)
        {
            EnablePanelChindren(spCanvOp, enable);
            EnablePanelChindren(spDeviceOp, enable);
        }
        private void EnablePanelChindren (Panel panel, bool enable)
        {
            foreach (FrameworkElement fe in panel.Children)
            {
                fe.IsEnabled = enable;
            }
        }
       
        private bool CheckConveyerConfig()
        {
            if (ConvConfig == null )
            {
                MessageBox.Show("请先打开配置文件");
                return false;
            };
            return true;
        }
        private void ClaerAllSelection()
        {
            ClearSelector();
            SetSelectControl(null);
            if (singleDevice != null)
            {
                gOper.Children.Remove(singleDevice);
            }
        }
        private void AddRect()
        {

            UIElement rect = GetRect(new RectData()
            {
                Width = 380,
                Height = 380,
                PosX = 15,
                PosY= 15,
                StrokeThickness =2,
                Name ="高温一"
            });
            rect = GetTextBlock(new LabelData()
            {
                Text="入化成段上层",
                FontSize = 16,
            });

            canvas.Children.Add(rect);

            rect.SetValue(Canvas.LeftProperty, 15d);
            rect.SetValue(Canvas.TopProperty, 15d);
            var al = AdornerLayer.GetAdornerLayer(rect);
            var adorner = new ElementAdorner(rect); 
            al.Add(adorner);
   
            adorner.OnMoveDrage += (s, e) =>
            {
                if (rect is TextBlock tb)
                {
                    tb.FontSize = (tb.ActualWidth) / 6;
                }
            };
            rect.MouseLeftButtonDown += (s, e) =>
            {
                adorner.Show();
            };
            rect.MouseRightButtonDown += (s, e) =>
            {
                adorner.Hide();
            };
        }
        private TextBlock GetTextBlock(LabelData data)
        {
            return new TextBlock()
            {
                Text = data.Text,
                FontSize = data.FontSize,
            };
        }
        private Rectangle GetRect(RectData data)
        {
            return new Rectangle()
            {
                Width = data.Width,
                Height = data.Height,
                Stroke = new SolidColorBrush(Colors.Red),
                StrokeThickness = data.StrokeThickness,
        
            };
        }
        private FrameworkElement DrawRect(RectData data)
        {
            Grid grid = new Grid(); 
            grid.Width = data.Width;
            grid.Height = data.Height;
            //grid.Background = new SolidColorBrush(Colors.Black);

            TextBlock tb = new TextBlock();
            tb.Text = data.Name;
            tb.HorizontalAlignment = HorizontalAlignment.Center;
            tb.VerticalAlignment = VerticalAlignment.Top;
            tb.Margin = new Thickness(0, 5, 0, 5);
            tb.FontSize = 36;
            Border border = new Border();
            border.BorderThickness = new Thickness(2);
            border.BorderBrush = new SolidColorBrush(Colors.Red);
            border.Child = tb;
            grid.Children.Add(border);



            //Rectangle rect = new Rectangle
            //{
            //    Stroke = new SolidColorBrush(Colors.Red),
            //    StrokeThickness = data.StrokeThickness,
              
            //};
            //grid.Children.Add(rect);
            grid.RenderTransform = new TranslateTransform(data.PosX, data.PosY);
            Panel.SetZIndex(grid, -99);
            return grid;

        }

        //创建一个选中的设备的信息
        private FrameworkElement CreateDetail(Type type,object data)
        {
            if (cbShowDetail.IsChecked == false) return null; 
            StackPanel sp = new StackPanel();
            sp.Background = new SolidColorBrush(Color.FromArgb(125,255,255,255));
            sp.Width = 300;
            sp.HorizontalAlignment = HorizontalAlignment.Left;
            sp.Height = gOper.ActualHeight;
            sp.Orientation = Orientation.Vertical;
            var cc = new ClassControl(type, true, data);
            cc.NewData += (o) =>
            {
                if (selectedDevice != null) selectedDevice.ViewModel.Data = o as DeviceData;
            }; 
            var cmb = CreateAreaIDSelector(cc,(data as DeviceData).AreaID);
            cmb.VerticalAlignment = VerticalAlignment.Center;
            cmb.Width = 290;
            cmb.Margin = new Thickness(0, 5, 0, 5);
            sp.Children.Add(cmb);
            if (!inSelector && !selectorList.Any())
            {
                //这里做多选的准备 
                sp.Children.Add(cc);
            } 
            Panel.SetZIndex(sp, 99);
            return sp;
        }  
        private ComboBox CreateAreaIDSelector(  ClassControl cc= null,uint  areaid = 0)
        {
            ComboBox cmb = new ComboBox();
            cmb.SetValue(InfoElement.TitleProperty, "所处区域");
            cmb.SetValue(InfoElement.TitleWidthProperty, new GridLength(85d));
            cmb.SetValue(InfoElement.TitlePlacementProperty, TitlePlacementType.Left);
        
            if (Areas != null && Areas.Any())
            {
                cmb.ItemsSource = Areas.Select(a => $"{a.ID}_{a.Name}").ToArray();
                if (areaid > 0)
                {
                    cmb.SelectedIndex = Areas.Select(a => a.ID).ToList().IndexOf(areaid);
                }
                cmb.SelectionChanged += (s, e) =>
                {
                    var selectItem = cmb.SelectedItem.ToString().Split('_')[0];
                    if (cc != null)
                    {
                        var data = cc.Data as DeviceData;
                        if (data.AreaID != 0)
                        {
                            AreasRemoveDevice(data);
                        }
                        cc.SetUIValue("AreaID", selectItem); 
                        if (data != null)
                        {
                        
                            AreasAddDevice(data);
                        }
                    }
                };
            
            }
            else
            {
                cmb.Tag = "useless";
                cmb.Items.Add("请建立区域再对单个分配");
                cmb.SelectedIndex = 0;
            } 
            return cmb;
        }
        private UC_DeviceBase CreateDeviceControl(DeviceData data) => CreateDeviceControl(data, canvas);
        private UC_DeviceBase CreateDeviceControl(DeviceData data,Panel panel = null)
        { 
            UC_DeviceBase device = new UC_DeviceBase(data); 
            device.MouseDown += (ds, de) =>
            {
                if(inSelector)
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
                data.Width = device.Width;
                data.Height = device.Height;
                device.ToolTip = device.Info; 
            };
            devicesList.Add(device);

            if (panel != null)
            {
                panel.Children.Add(device);
                //装饰器 一定要添加到父容器之后再执行，否则获取不到装饰器的
                var al = AdornerLayer.GetAdornerLayer(device);
                var adorner = new ElementAdorner(device);
                adorner.DragStarted += (ds, de) =>
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
                adorner.OnMoveDrage += (ds, de) =>
                {
                    if (selectorList.Count <= 1) return;
                    foreach (var item in selectorList)
                    {
                        if (item == ds) continue;
                        item.Width += de.HorizontalChange;
                        item.Height += de.VerticalChange;
                        item.ToolTip = item.Info;
                    }
                };
                al.Add(adorner);
            }
     
        
            return device;
        }

        private void AreasAddDevice(  DeviceData data)
        {
            if(data == null)
            { 
                return;
            }
            var area = Areas.Find(a => a.ID == data.AreaID);
            if(area == null)
            { 
                return;
            }
            if (area.Devices.Contains(data))
            { 
                return;
            } 
            area.Devices.Add(data);
        }
        private void AreasRemoveDevice(DeviceData data)
        {
            if (data == null)
            {
              
                return;
            }
            var area = Areas.Find(a => a.ID == data.AreaID);
            if (area == null)
            { 
                return;
            }
            if (!area.Devices.Contains(data))
            { 
                return;
            }
            area.Devices.Remove(data);
        }
        #region 界面 操作 
        /// <summary> 复制当前选择的设置 </summary> 
        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckConveyerConfig()) return;
            double copyOffset = 25;
            if (selectorList.Count == 0 && selectedDevice != null)
            {
                //单个复制 
                var clone = selectedDevice.Data.Clone();
                clone.Name = $"{clone.Name}_clone";
                clone.ID = $"{clone.ID}_clone";
                clone.PosX += copyOffset;
                clone.PosY += copyOffset;
                AreasAddDevice(clone);
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
                    AreasAddDevice(clone);
                    AddSelector(CreateDeviceControl(clone));
                }
            }
        }
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (HaveNew) return;
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
            if (HaveNew) return;
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
            if (HaveNew) return;
            if (e.LeftButton == MouseButtonState.Released)
            {
                canvas.Children.Remove(selectorRect);
                inSelector = false;
                SetTxtInfo(string.Empty);
            }
        }
        /// <summary>  检查是否在选择区域内  </summary> >
        private void CheckInSelector(Point startPoint,Point currentPoint)
        {
            if (!inSelector) return  ;
            foreach (var item in devicesList)
            {
                if (item.Parent == null) continue;
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
        /// <summary>  清除选择器  </summary>
        private void ClearSelector()
        {
            foreach (var item in selectorList)
            {
                SetThumShowOrHide(item, false);
            }
            selectorList.Clear();
        }
        /// <summary> 添加设备到选择器 </summary> 
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
        /// <summary> 设置当前选中的控件 单击  </summary> 
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
                if (singleDevice != null)
                {
                    gOper.Children.Remove(singleDevice);
                }
                singleDevice = CreateDetail(ucdb.Data.GetType(), ucdb.Data);
                if (singleDevice != null)
                { 
                    TryAnimation(singleDevice);
                    gOper.Children.Add(singleDevice);
                }
                SetThumShowOrHide(selectedDevice, true);
            }
        } 
        /// <summary> 动画从左边往右边插入 </summary> 
        private void TryAnimation(FrameworkElement uiele)
        {
            var t = uiele.RenderTransform = new TranslateTransform(0, 15);
            DoubleAnimation animation = new DoubleAnimation()
            {
                From = -uiele.Width,
                To = 15,
                DecelerationRatio = .5d,
                Duration = new Duration(TimeSpan.FromSeconds(0.5d)),
            };
            t.BeginAnimation(TranslateTransform.XProperty, animation);
        }

        /// <summary> 绘制线条 </summary> 
        public void Draw(Canvas canvas, double scaleX = 15, double scaleY = 15)
        {
            var gridBrush = new SolidColorBrush { Color = Colors.Red };
            foreach (var item in allLines)
            {
                canvas.Children.Remove(item);
            }
            allLines.Clear();
            //gridPoints.Clear();
            double currentPosY = 0;
            currentPosY += scaleY;
            int yCount = 0, xCount = 0;
            while (currentPosY < canvas.ActualHeight)
            {
                Line line = new Line
                {
                    X1 = 0,
                    Y1 = currentPosY,
                    X2 = canvas.ActualWidth,
                    Y2 = currentPosY,
                    Stroke = gridBrush,
                    StrokeThickness = 0.2d
                };
                canvas.Children.Add(line);
                Panel.SetZIndex(line, -100);
                allLines.Add(line);
                currentPosY += scaleY;
                yCount++;
            }


            double currentPosX = 0;
            currentPosX += scaleX;
            while (currentPosX < canvas.ActualWidth)
            {
                Line line = new Line
                {
                    X1 = currentPosX,
                    Y1 = 0,
                    X2 = currentPosX,
                    Y2 = canvas.ActualHeight,
                    Stroke = gridBrush,
                    StrokeThickness = 0.2d
                };
                Panel.SetZIndex(line, -100); 
                canvas.Children.Add(line);
                allLines.Add(line);
                currentPosX += scaleX;
                xCount++;
            } 
        }
        /// <summary>  设置可缩放显示/隐藏 / </summary> 
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
        private void CbShowGrid_Checked(object sender, RoutedEventArgs e)
        {
            foreach (var item in allLines)
            {
                item.Visibility = cbShowGrid.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        private void SetTxtInfo(string info) => txtInfo.Text = info;
        #endregion

        private void txtInput_Width(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox txt)
            {
                try
                {
                    var width = txt.Text;
                    if (string.IsNullOrWhiteSpace(width))
                    {
                        return;
                    }
                    if (txt.Tag?.ToString() == "width")
                    {
                        CanvasWidth = double.Parse(width);
                    }
                    else if (txt.Tag?.ToString() == "height")
                    {
                        CanvasHeight = double.Parse(width);
                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show($"请输入正确的数字类型 ,\n\r 错误信息:'{ex.Message}' \r\n 内部信息:'{ex?.InnerException?.Message}'", "输入提示");
                    txt.Text = string.Empty;
                }
            }
  
        }

      
    }
}
