using ControlHelper;
using ControlHelper.WPF;
using DisplayConveyer.Config;
using DisplayConveyer.Controls;
using DisplayConveyer.Model;
using DisplayConveyer.Utilities;
using HandyControl.Controls;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;
using TextBox = System.Windows.Controls.TextBox;
using Window = System.Windows.Window;

namespace DisplayConveyer.View
{
    /// <summary>
    /// MapDataEditorWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MapDataEditorWindow : Window
    {
        //当前选择的控件
        private FrameworkElement currentSelected;

        private MapPartData SelectData => currentSelected?.DataContext as MapPartData;
        private List<MapPartData> mapPartDatas = null;
        //物流线配置 
        private ConveyerConfig ConvConfig;
        //存放所有的区域数据
        private List<string> listAllAreas;
        public MapDataEditorWindow()
        {
            InitializeComponent();
            Loaded += (s, e) =>
            {
                ConvConfig = GlobalPara.ConveyerConfig.Clone();
                SetImage(ConvConfig.MiniMapImagePath);
                mapPartDatas = ConvConfig.MiniMapData;
                if (mapPartDatas == null)
                {
                    mapPartDatas = new List<MapPartData>();
                }
                foreach (var item in mapPartDatas)
                {
                    AddSelector(item);
                }

                if (ConvConfig.Areas != null && ConvConfig.Areas.Any())
                {
                    //已经被添加过的id不再添加
                    //获取所有被绑定的ID
                    List<uint> ids = (from item in ConvConfig.MiniMapData
                                      from id in item.AreaIDs
                                      select id).ToList();
                    var tempAreas = ConvConfig.Areas.Clone();
                    listAllAreas = tempAreas.Select(a => a.Name + "_" + a.ID).ToList();
                    foreach (var id in ids)
                    {
                        var area = tempAreas.Find(a => a.ID == id);
                        if (area != null)
                        {
                            tempAreas.Remove(area);
                        }
                    } 
                    cmbIDs.ItemsSource = tempAreas.Select(a => a.Name + "_" + a.ID).ToList();

                }

                UIElementMove uI = new UIElementMove(canvasMain, this, UIElementMove.KeyCode.Middle)
                {
                    CanInput = true,
                };
            };
            MouseDown += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    if (e.Source is FrameworkElement fe && fe.DataContext != null)
                    {
                        SetSelectControl(fe);
                    }
                    else
                    {
                        SetSelectControl(null);
                    }
                }

            };
        }
        private void RemoveSelector(ref FrameworkElement fe)
        {
            if (fe == null) return;
            if (fe.DataContext is MapPartData data)
            { 
                if (mapPartDatas.Contains(data))
                {
                    mapPartDatas.Remove(data);
                }
            }
            canvasMain.Children.Remove(fe);
            fe = null; 
        }
        
        private void AddSelector(MapPartData mpd)
        {
            Border border = new Border();
            border.Background = new SolidColorBrush(Color.FromArgb(64, 255, 255, 255));
            border.Width = mpd.Width;
            border.Height = mpd.Height;
            border.BorderBrush = new SolidColorBrush(Colors.GreenYellow);
            border.BorderThickness = new Thickness(1);
          
            TextBlock tb = new TextBlock()
            {
                FontSize = 72,
                Text = mpd.Title,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment= VerticalAlignment.Top, 
            };
            tb.SetValue(TextBlock.IsHitTestVisibleProperty, false);
            border.SetValue(Canvas.LeftProperty, mpd.PosX);
            border.SetValue(Canvas.TopProperty, mpd.PosY);
            border.Tag = tb;
            border.DataContext = mpd;
            border.Child=(tb); 
            if (!mapPartDatas.Contains(mpd))
            {
                mapPartDatas.Add(mpd);
            }
            AddControl(border);
        }
        
        private void AddControl(FrameworkElement fe)
        {
            canvasMain.Children.Add(fe);
            var al = AdornerLayer.GetAdornerLayer(fe);
            var adorner = new ElementAdorner(fe);
            al.Add(adorner);
            var cacheDataContext = fe.DataContext as ControlDataBase;
            if (cacheDataContext == null)
            { 
                Growl.Error("错误的DataContext,添加控件失败!");
                return;
              
            }
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
            //拖动发生的时候
            adorner.OnDrage += (s, e) =>
            {
                if (cacheDataContext != null)
                {
                    var pos = fe.TransformToAncestor(canvasMain).Transform(new Point(0, 0));
                    cacheDataContext.PosX = pos.X;
                    cacheDataContext.PosY = pos.Y;  
                }
            };
            adorner.DrageMove += Adorner_CheckOutRange;
            adorner.ThumbMove += Adorner_CheckOutRange;


        }

        private bool Adorner_CheckOutRange(object s, DragDeltaEventArgs e)
        {
            bool result = false;
            if (SelectData != null)
            {
                //判断是否超过最大编辑
                var leftX = Canvas.GetLeft(currentSelected) + e.HorizontalChange;
                var rightX = Canvas.GetLeft(currentSelected) + e.HorizontalChange + currentSelected.Width;
                if (leftX < 0 || rightX > imgBack.ActualWidth)
                {
                      result = true;
                }
                //判断是否碰撞
                if(e.HorizontalChange < 0)
                {
                    var last = mapPartDatas?.Find(a => a.ID == SelectData.ID - 1);
                    if (last != null && leftX <= last.PosX + last.Width)
                    {
                        result = true;
                    }
                }
              
                if(e.HorizontalChange > 0)
                {
                    var next = mapPartDatas?.Find(a => a.ID == SelectData.ID + 1);
                    if (next != null && rightX >= next.PosX)
                    {
                        result = true;
                    }
                } 
            }
            return result;
        }
        private uint TryGetAreaID( object item)
        { 
            if (item == null) return 0;
            var split = item.ToString().Split('_');
            var id = split[split.Length - 1].CastTo<uint>(0);
            if (id == 0)
            {
                Growl.Error($"获取组合框中的ID值失败,'{item}'");
                return 0;
            }
            return id;
        }
        private void SetImage(string path )
        {
            if (!File.Exists(path))
            {
                Growl.Info($"图片文件不存在，请重新配置!\r\n{path}");
            }
            else
            {
                imgSelector.SetValue(ImageSelector.UriPropertyKey, new Uri(ConvConfig.MiniMapImagePath, UriKind.RelativeOrAbsolute));
                ImageBrush ib = new ImageBrush(BitmapFrame.Create(imgSelector.Uri, BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.None))
                {
                    Stretch = Stretch.Fill
                };
                imgSelector.SetValue(ImageSelector.PreviewBrushPropertyKey, ib);
                imgBack.Source = ib.ImageSource;
                imgSelector .SetValue(ImageSelector.HasValuePropertyKey, true); 
            }
        }
        /// <summary>  设置左键按下事件 </summary> 
        private void SetMouseDown(FrameworkElement fe)
        {
            fe.MouseDown += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    SetSelectControl(fe);
                }
            };
        }
        /// <summary> 设置当前选中的控件 单击  </summary> 
        private void SetSelectControl(FrameworkElement fe)
        {
            if (currentSelected != null)
            {
                SetThumShowOrHide(currentSelected, false);
                currentSelected = null;
                gridDetail.IsEnabled = false;
            }
            if (fe != null)
            {
                if (fe.DataContext is MapPartData mpd)
                {
                    listIDs.ItemsSource = null;
                    listIDs.ItemsSource = mpd.AreaIDs;
                    txtTitle.Text = mpd.Title;
                }
                SetThumShowOrHide(currentSelected = fe, gridDetail.IsEnabled = true);
            }
        }
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

        private void btnAddSelector_Click(object sender, RoutedEventArgs e)
        {
            var mpd = mapPartDatas.Any() ? mapPartDatas.Where(a => a.ID == mapPartDatas.Max(b => b.ID)).FirstOrDefault() : null;
            var maxPox = mpd == null ? 0 : mpd.PosX + mpd.Width;
            var leftWidth = canvasMain.Width - maxPox;
            if(leftWidth <= 10)
            {
                Growl.Error("添加失败，最左侧剩余宽度不足10");
                return;
            }
            AddSelector(new MapPartData()
            {
                ID = mpd == null ? 1 : mpd.ID + 1,
                Width = leftWidth> 300 ? 300 : leftWidth,
                Height = canvasMain.ActualHeight,
                EnableThumbVertical = false,
                Title = "新建区域",
                PosX = maxPox+5
            });
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            GlobalPara.ConveyerConfig  = ConvConfig;
            Growl.Info("保存成功");
        }

        private void imgSelector_ImageSelected(object sender, RoutedEventArgs e)
        {
            ConvConfig.MiniMapImagePath = imgSelector.Uri.LocalPath;
            imgBack.Source = new BitmapImage(new Uri(imgSelector.Uri.AbsolutePath, UriKind.Absolute));
        }
 

        private void imgSelector_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (imgSelector.HasValue)
                {
                    imgBack.Source = null;

                }
            }
        }

        private void btnRemoveSelector_Click(object sender, RoutedEventArgs e)
        {
            RemoveSelector(ref currentSelected);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var txt = sender as TextBox;
            if(currentSelected != null)
            {
                if(currentSelected.Tag is TextBlock tb)
                {
                   SelectData.Title = tb.Text = txt.Text;
                }
            }
        }

        private void btnAddID_Click(object sender, RoutedEventArgs e)
        {
            if (SelectData != null)
            {
                var id = TryGetAreaID(cmbIDs.SelectedItem);
                if (id == 0)
                {
                    return;
                }
                if (SelectData.AreaIDs.Contains(id))
                {
                    Growl.Info($"已经添加对应的ID,'{id}'");
                    return;
                }
                SelectData.AreaIDs.Add(id); 
                (cmbIDs.ItemsSource as IList)?.Remove(cmbIDs.SelectedItem);
                cmbIDs.Items.Refresh();
                listIDs.ItemsSource = null;
                listIDs.ItemsSource = SelectData.AreaIDs;
            }
        }

        private void btnRemoveID_Click(object sender, RoutedEventArgs e)
        {
            if(listIDs.SelectedIndex < 0)
            {
                Growl.Info("请选中区域ID");
                return;
            }
            if (SelectData != null)
            {
                var id = listIDs.SelectedItem.CastTo<uint>(0);
                if (id == 0)
                {
                    return;
                }
                if (!SelectData.AreaIDs.Contains(id))
                {
                    Growl.Info($"未找到对应的ID,'{id}'");
                    return;
                }

                var result = listAllAreas.Where(a =>  a.Split('_')[a.Split('_').Length -1].CastTo(0) == id).FirstOrDefault();
                if(result != null)
                {
                    (cmbIDs.ItemsSource as IList)?.Add(result);
                    cmbIDs.Items.Refresh();
                }
                SelectData.AreaIDs.Remove(id);
                listIDs.ItemsSource = null; 
                listIDs.ItemsSource = SelectData.AreaIDs;
            }
        }

        private void cmbIDs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
          

        }
    }
}
