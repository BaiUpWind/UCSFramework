using ControlHelper;
using ControlHelper.WPF;
using HandyControl;
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
using System.Windows.Data;
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
        //当前所有控件的集合 存在的目的为了框选时计算是否在框内
        private readonly List<UC_DeviceBase> controlList = new List<UC_DeviceBase>();
        private List<Line> allLines = new List<Line>();
        private Point startPoint;
        private Point currentPoint;
        private Rectangle selectorRect;
        //当前选择的控件
        private FrameworkElement currentSelected;
        //同时最多只能有一个新建在场
        private bool haveNew;
        private bool inSelector;
        //当前选择的设备
        private FrameworkElement controlDetail;

        //新增新建设备界面
        private StackPanel newCreate;
        //物流线配置 
        private ConveyerConfig ConvConfig;
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
            get => haveNew; set
            {

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
            MouseRightButtonDown += (s, e) =>
            {
                ClaerAllSelection();
            };
            Loaded += (s, e) =>
            {
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
            this.KeyDown += (s, e) =>
            {
                if (e.Key == Key.LeftAlt)
                {
                    inSelector = false;
                    BtnCopy_Click(null, null);
                }
            };

        }

        private string LabelOrRect;
        //添加一个Label
        private void BtnAddLable_Click(object sender, RoutedEventArgs e)
        {
            LabelOrRect = "Label";
            EnableMyStackPanel(false);
            SetTxtInfo("添加标签中...");
        }
        //添加一个矩形选择框
        private void BtnAddRect_Click(object sender, RoutedEventArgs e)
        {
            LabelOrRect = "Rect";
            EnableMyStackPanel(false);
            SetTxtInfo("添加区域框中...");
        }

        //移除一个设备
        private void BtnRemoveDevice_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckConveyerConfig()) return;

            var result = MessageBox.Ask("请确认是否删除", "询问");
            if (result == MessageBoxResult.OK)
            {
                if (currentSelected != null && selectorList.Count == 0)
                {
                    if (currentSelected is UC_DeviceBase udb)
                    {
                        AreasRemoveDevice(udb.Data);
                        controlList.Remove(udb);
                    }
                    else if (currentSelected.DataContext is RectData rd)
                    {
                        var rect = ConvConfig.RectDatas.Find(a => a.ID == rd.ID);
                        if (rect == null)
                        {
                            Growl.Info("未找到对应区域框的数据");
                            return;
                        }
                        else
                        {
                            ConvConfig.RectDatas.Remove(rect);
                        }
                    }
                    else if (currentSelected.DataContext is LabelData ld)
                    {
                        var label = ConvConfig.Labels.Find(a => a.ID == ld.ID);
                        if (label == null)
                        {
                            Growl.Info("未找到对应标签的数据");
                            return;
                        }
                        else
                        {
                            ConvConfig.Labels.Remove(label);
                        }
                    }
                    canvas.Children.Remove(currentSelected);
                    if (controlDetail != null)
                    {
                        gOper.Children.Remove(controlDetail);
                    }
                    currentSelected = null;
                }
                else if (selectorList.Count > 0)
                {
                    foreach (var item in selectorList)
                    {
                        controlList.Remove(item);
                        AreasRemoveDevice(item.Data);
                        canvas.Children.Remove(item);
                    }
                    selectorList.Clear();
                    currentSelected = null;
                }
            }
        }

        private void EnableMyStackPanel(bool enable)
        {
            EnablePanelChindren(spCanvOp, enable);
            EnablePanelChindren(spDeviceOp, enable);
        }
        private void EnablePanelChindren(Panel panel, bool enable)
        {
            foreach (FrameworkElement fe in panel.Children)
            {
                fe.IsEnabled = enable;
            }
        }

        private bool CheckConveyerConfig()
        {
            if (ConvConfig == null)
            {
                MessageBox.Show("请先打开配置文件");
                return false;
            };
            return true;
        }
        //清除所有的选择
        private void ClaerAllSelection()
        {
            ClearSelector();
            SetSelectControl(null);
            if (controlDetail != null)
            {
                gOper.Children.Remove(controlDetail);
            }
            if (LabelOrRect != null)
            {
                LabelOrRect = null;
                EnableMyStackPanel(true);
            }
        }
        private void AddControl(FrameworkElement fe)
        {
            canvas.Children.Add(fe);
            var cacheDataContext = fe.DataContext as ControlDataBase;
            var al = AdornerLayer.GetAdornerLayer(fe);
            var adorner = new ElementAdorner(fe);
            al.Add(adorner);

            if (cacheDataContext != null)
            {
                adorner.EnableHorizontal = cacheDataContext.EnableThumbHorizontal;
                adorner.EnableVertical = cacheDataContext.EnableThumbVertical;
            }
            fe.SizeChanged += (s, e) =>
            {
                if (cacheDataContext != null)
                {
                    cacheDataContext.Width = fe.Width;
                    cacheDataContext.Height = fe.Height;
                }
            };
            adorner.OnDrage += (s, e) =>
            {
                if (cacheDataContext != null)
                {
                    var pos = fe.TransformToAncestor(canvas).Transform(new Point(0, 0));
                    cacheDataContext.PosX = pos.X;
                    cacheDataContext.PosY = pos.Y;
                }
            };

        }
        private FrameworkElement GetTextBlock(LabelData data)
        {
            var tb = CreateHelper.GetTextBlock(data);
            SetMouseDown(tb);
            return tb;
        }
        private FrameworkElement GetRect(RectData data)
        {
            var rect = CreateHelper.GetRect(data);
            SetMouseDown(rect);
            return rect;
        }
        //创建一个选中的控件的信息
        private FrameworkElement CreateDetail(Type type, object data, bool haveCmb = false, Action<object> newData = null)
        {
            if (cbShowDetail.IsChecked == false) return null;
            StackPanel sp = new StackPanel();
            sp.Background = new SolidColorBrush(Color.FromArgb(125, 255, 255, 255));
            sp.Width = 300;
            sp.HorizontalAlignment = HorizontalAlignment.Left;
            sp.Height = gOper.ActualHeight;
            sp.Orientation = Orientation.Vertical;
            var cc = new ClassControl(type, true, data);
            cc.NewData += (o) =>
            {
                newData?.Invoke(o);
                if (currentSelected != null && currentSelected is UC_DeviceBase udb)
                    udb.ViewModel.Data = o as DeviceData;
            };
            if (haveCmb)
            {
                //haveCmb 是特殊
                var cmb = GetAreaIDCombBox(cc, (data as DeviceData).AreaID);
                cmb.VerticalAlignment = VerticalAlignment.Center;
                cmb.Width = 290;
                cmb.Margin = new Thickness(0, 5, 0, 5);
                sp.Children.Add(cmb);
            }
            if (!inSelector && !selectorList.Any())
            {
                //这里做多选的准备 
                sp.Children.Add(cc);
            }
            Panel.SetZIndex(sp, 99);
            return sp;
        }
        private ComboBox GetAreaIDCombBox(ClassControl cc = null, uint areaid = 0)
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
        private UC_DeviceBase GetDeviceBase(DeviceData data) => GetDeviceBase(data, canvas);
        private UC_DeviceBase GetDeviceBase(DeviceData data, Panel panel = null)
        {
            UC_DeviceBase device = CreateHelper.GetDeviceBase(data) as UC_DeviceBase;
            SetMouseDown(device);
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
            controlList.Add(device);

            if (panel != null)
            {
                panel.Children.Add(device);
                //装饰器 一定要添加到父容器之后再执行，否则获取不到装饰器的
                var al = AdornerLayer.GetAdornerLayer(device);
                var adorner = new ElementAdorner(device);
                adorner.DragStarted += (ds, de) =>
                {
                    if (selectorList.Count > 1)
                        currentSelected = de as UC_DeviceBase;
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
                        if (item == currentSelected) continue;
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
        /// <summary>  设置左键按下事件 </summary> 
        private void SetMouseDown(FrameworkElement fe)
        {
            fe.MouseDown += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (inSelector)
                    {
                        return;
                    }
                    if (selectorList.Count == 0)
                    {
                        SetSelectControl(s as FrameworkElement);
                    }
                }
            };
        }
        private void AreasAddDevice(DeviceData data)
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
            if (selectorList.Count == 0 && currentSelected != null && currentSelected is UC_DeviceBase udb)
            {
                //单个复制 
                var clone = udb.Data.Clone();
                clone.Name = $"{clone.Name}Clone";
                clone.ID = $"{clone.ID}Clone";
                clone.PosX += copyOffset;
                clone.PosY += copyOffset;
                AreasAddDevice(clone);
                SetSelectControl(GetDeviceBase(clone));
            }
            else if (selectorList.Count > 0)
            {
                //多选复制
                List<DeviceData> temp = selectorList.Select(a => a.Data).ToList().Clone();
                ClearSelector();
                for (int i = 0; i < temp.Count; i++)
                {
                    var clone = temp[i];
                    clone.Name = $"{clone.Name}Clone{(i + 1)}";
                    clone.ID = $"{clone.ID}Clone{(i + 1)}";
                    clone.PosX += copyOffset;
                    clone.PosY += copyOffset;
                    AreasAddDevice(clone);
                    AddSelector(GetDeviceBase(clone));
                }
            }
            else if (currentSelected != null && currentSelected.DataContext is RectData rd)
            {
                var clone = rd.Clone();
                clone.ID++;
                clone.PosX += copyOffset;
                clone.PosY += copyOffset;
                var fe = GetRect(clone);
                AddControl(fe);
                ConvConfig.RectDatas.Add(clone);
                SetSelectControl(fe);
            }
            else if (currentSelected != null && currentSelected.DataContext is LabelData ld)
            {
                var clone = ld.Clone();
                clone.ID++;
                clone.Text += "Clone";
                clone.PosX += copyOffset;
                clone.PosY += copyOffset;
                var fe = GetTextBlock(clone);
                AddControl(fe);
                ConvConfig.Labels.Add(clone);
                SetSelectControl(fe);
            }
        }
        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (HaveNew) return;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                startPoint = e.GetPosition(canvas);

                if (LabelOrRect == "Label" && ConvConfig != null)
                {
                    var labelData = new LabelData()
                    {
                        ID = ConvConfig.Labels.Any() ? ConvConfig.Labels.Max(a => a.ID) + 1 : 1,
                        Text = "新建标签",
                        FontSize = 16,
                        PosX = startPoint.X,
                        PosY = startPoint.Y,
                    };
                    AddControl(GetTextBlock(labelData));
                    ConvConfig.Labels.Add(labelData);
                    EnableMyStackPanel(true);
                    LabelOrRect = null;
                }
                else if (LabelOrRect == "Rect" && ConvConfig != null)
                {
                    var rectData = new RectData()
                    {
                        ID = ConvConfig.RectDatas.Any() ? ConvConfig.RectDatas.Max(a => a.ID) + 1 : 1,
                        Width = 400,
                        Height = 400,
                        StrokeThickness = 6,
                        PosX = startPoint.X,
                        PosY = startPoint.Y,
                    };
                    AddControl(GetRect(rectData));
                    ConvConfig.RectDatas.Add(rectData);
                    EnableMyStackPanel(true);
                    LabelOrRect = null;
                }
                else
                {
                    inSelector = true;
                    canvas.Children.Remove(selectorRect);
                    selectorRect = new Rectangle();
                    selectorRect.Fill = new SolidColorBrush(Color.FromArgb(96, 0, 0, 255));
                    selectorRect.RenderTransform = new TranslateTransform(startPoint.X, startPoint.Y);
                    canvas.Children.Add(selectorRect);
                }
            }
        }
        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (HaveNew) return;
            if (e.LeftButton == MouseButtonState.Pressed && selectorRect != null)
            {
                if (currentSelected != null)
                {
                    //确保在画框选定时 单个选中被清空
                    SetThumShowOrHide(currentSelected, false);
                    if (controlDetail != null)
                    {
                        gOper.Children.Remove(controlDetail);
                    }
                    currentSelected = null;
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
        private void CheckInSelector(Point startPoint, Point currentPoint)
        {
            if (!inSelector) return;
            foreach (var item in controlList)
            {
                if (item.Parent == null) continue;
                var point = item.TransformToVisual(canvas).Transform(new Point(0, 0));
                if (point.X >= startPoint.X && point.X <= currentPoint.X
                    && point.Y >= startPoint.Y && point.Y <= currentPoint.Y)
                {
                    AddSelector(item);
                }
                else
                {
                    RemoveSelector(item);
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
        private void SetSelectControl(FrameworkElement fe)
        {
            if (currentSelected != null)
            {
                SetThumShowOrHide(currentSelected, false);
                currentSelected = null;
            }
            if (fe != null)
            {
                if (controlDetail != null)
                {
                    gOper.Children.Remove(controlDetail);
                }
                if (fe is UC_DeviceBase udb)
                {
                    controlDetail = CreateDetail(udb.Data.GetType(), udb.Data, true);
                }
                else if (fe.DataContext is LabelData ld)
                {
                    controlDetail = CreateDetail(typeof(LabelData), ld, false, (o) =>
                    {
                        if (fe is TextBlock tb && o is LabelData newld)
                        {
                            tb.Text = newld.Text;
                            tb.FontSize = newld.FontSize;
                        }
                    });
                }
                else if (fe.DataContext is RectData rd)
                {
                    controlDetail = CreateDetail(typeof(RectData), rd, false, (o) =>
                    {
                        if (fe is Rectangle rect && o is RectData newRd)
                        {
                            rect.StrokeThickness = newRd.StrokeThickness <= 0 ? 1 : newRd.StrokeThickness;
                        }
                    });
                }
                else
                {
                    controlDetail = null;
                }
                if (controlDetail != null)
                {
                    TryAnimation(controlDetail);
                    gOper.Children.Add(controlDetail);
                }

                SetThumShowOrHide(currentSelected = fe, true);
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
        private void SetThumShowOrHide(UIElement uiele, bool showOrHide)
        {
            var al = AdornerLayer.GetAdornerLayer(uiele);
            var adorners = al.GetAdorners(uiele);
            if (adorners == null)
            {
                Growl.Info("对应元素未绑定任何装饰器");
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
        //配置文件打开和保存
        private void btnOpenCfg_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                var tag = btn.Tag.ToString();
                if (tag.Equals("open"))
                {
                    ConvConfig = GlobalPara.ConveyerConfig.Clone();
                    if (ConvConfig == null) return;
                    CanvasHeight = ConvConfig.CanvasHeight;
                    CanvasWidth = ConvConfig.CanvasWidth;
                    EnableMyStackPanel(true);
                    LabelOrRect = null;
                    canvas.Children.Clear();

                    foreach (var area in Areas)
                    {
                        foreach (var device in area.Devices)
                        {
                            GetDeviceBase(device);
                        }
                    }
                    foreach (var label in ConvConfig.Labels)
                    {
                        AddControl(GetTextBlock(label));
                    }
                    foreach (var rect in ConvConfig.RectDatas)
                    {
                        AddControl(GetRect(rect));
                    }
                    Draw(canvas);
                    btn.Background = new SolidColorBrush(Colors.OrangeRed);
                    btn.Content = "关闭配置";
                    btn.Tag = "close";
                }
                else if (tag.Equals("close"))
                {
                    DrawerTopInContainer.IsOpen = false;
                    GlobalPara.ConveyerConfig = ConvConfig;
                    ClaerAllSelection();
                    EnableMyStackPanel(false);
                    Growl.Info("保存成功");
                    canvas.Children.Clear();
                    Draw(canvas);
                    ConvConfig = null;
                    btn.Background = new SolidColorBrush(Colors.White);
                    btn.Content = "打开配置";
                    btn.Tag = "open";
                }
            }
        }
        //保存配置
        private void btnSaveConfig_Click(object sender, RoutedEventArgs e)
        {
            GlobalPara.ConveyerConfig = ConvConfig;
            Growl.Info("保存成功");
        }
        //打开区域编辑器
        private void btnEditAreas_Click(object sender, RoutedEventArgs e)
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
        }
        //添加一个设备
        private void BtnAddDevice_Click(object sender, RoutedEventArgs e)
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

            newCreate.RenderTransform =new  TranslateTransform(0, 0);///////

            #endregion
            newCreate.Children.Add(spOp);
            newCreate.Background = new SolidColorBrush(Color.FromArgb(125, 255, 255, 255));
            newCreate.Width = 300;
            newCreate.HorizontalAlignment = HorizontalAlignment.Left;
            newCreate.Height = gOper.ActualHeight;
            newCreate.Orientation = Orientation.Vertical;
            UIElementMove um = new UIElementMove(newCreate, this, UIElementMove.KeyCode.Middle);
            um.CanInput = true;
            var moup = Mouse.GetPosition(canvas);
            um.ReSet(moup.X + 100, moup.Y);
            var cc = new ClassControl(typeof(DeviceData), true, orginData);
            cc.Margin = new Thickness(0, 5, 0, 5);
            var cmb = GetAreaIDCombBox(cc);
            cmb.VerticalAlignment = VerticalAlignment.Center;
            cmb.Width = 290;
            cmb.Margin = new Thickness(0, 5, 0, 5);

            newCreate.Children.Add(cmb);
            newCreate.Children.Add(cc);
            btnAdd.Click += (bs, be) =>
            {

                if (cmb.Tag != null && cmb.Tag.ToString() == "useless")
                {
                    MessageBox.Show("请先添加区域!");
                    canvas.Children.Remove(newCreate);
                    return;
                }
                var data = cc.Data as DeviceData;
                if (data.AreaID == 0)
                {
                    MessageBox.Show("请选择对应的区域", "错误");
                    return;
                }
                var cdc = GetDeviceBase(data);
                var point = btnAdd.TransformToAncestor(canvas).Transform(new Point(0, 0));
                if (point.X > canvas.Width)
                {
                    point.X = canvas.Width - data.Width;
                }
                if (point.X < 0)
                {
                    point.X = 0;
                }
                if (point.Y > canvas.Height)
                {
                    point.Y = canvas.Height - data.Height;
                }
                if (point.Y <= 0)
                {
                    point.Y = 0;
                }
                cdc.ViewModel.Data.PosX = point.X + 300;
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
        }
        //画布复原
        private void btnCvReset_Click(object sender, RoutedEventArgs e)
        {
            canvasMove.ReSet(0, 0);
        }
        //锁定画布
        private void btnCvLock_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
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
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (ConvConfig != null)
            {
                var result = MessageBox.Show("是:保存且关闭\n\r 否:关闭不保存 \n\r 取消:不保存不关闭", "是否保存已经修改的信息", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    GlobalPara.ConveyerConfig = ConvConfig;
                }
                else if (result == MessageBoxResult.Cancel || result == MessageBoxResult.None)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
