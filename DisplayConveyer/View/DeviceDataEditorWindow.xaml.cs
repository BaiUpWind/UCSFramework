using ControlHelper.WPF;
using DisplayConveyer.Controls;
using DisplayConveyer.Model;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
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
using Window = System.Windows.Window;

namespace DisplayConveyer.View
{
    /// <summary>
    /// DeviceDataEditorWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceDataEditorWindow : Window
    {
        //当前选择的控件
        private FrameworkElement currentSelected;
   
        List<MapPartData> mapPartDatas =null;
        public DeviceDataEditorWindow( )
        {
            InitializeComponent(); 
            Loaded += (s, e) =>
            {
                mapPartDatas = GlobalPara.ConveyerConfig.MiniMapData;
                //rectSelector.DataContext = new MapPartData()
                //{
                //    Width = 300,
                //    EnableThumbVertical = false,
                //};
                //rectSelector.Tag = tbName;
                //rectSelector.SetValue(Canvas.LeftProperty, 110d);
                //rectSelector.SetValue(Canvas.TopProperty, 0d);
                //SetMouseDown(rectSelector);
                //AddSelector(rectSelector);

                //ClassControl cc = new ClassControl(typeof(MapPartData));
                //gridProp.Children.Add(cc);
            };
            MouseDown += (s, e) =>
            {
                if(e.Source is FrameworkElement fe  && fe.DataContext != null)
                {
                    SetSelectControl(fe);
                }
                else
                {
                    SetSelectControl(null);
                }
            };
        } 
   

        private void AddSelector(MapPartData mpd)
        {
            Border border = new Border();
            border.Background = new SolidColorBrush(Color.FromArgb(64, 255, 255, 255));
            border.Width = mpd.Width;
            border.Height = mpd.Height;
            border.BorderBrush = new SolidColorBrush(Colors.GreenYellow);
            border.BorderThickness = new Thickness(5);
          
            TextBlock tb = new TextBlock()
            {
                FontSize = 32,
                Text = mpd.Title,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment= VerticalAlignment.Top, 
            }; 
            border.SetValue(Canvas.LeftProperty, mpd.PosX);
            border.SetValue(Canvas.TopProperty, mpd.PosY);
            border.Tag = tb;
            border.DataContext = mpd;
            border.Child=(tb);
            canvasMain.Children.Add(border);
            if (!mapPartDatas.Contains(mpd))
            {
                mapPartDatas.Add(mpd);
            }
            AddSelector(border);
        }
        
        private void AddSelector(FrameworkElement fe)
        {
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
            }
            if (fe != null)
            { 
                if(fe.DataContext is MapPartData mpd)
                {

                }
                SetThumShowOrHide(currentSelected = fe, true);
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
            AddSelector(new MapPartData()
            {
                ID = mpd == null ? 1 : mpd.ID + 1,
                Width = 300,
                Height = canvasMain.ActualHeight,
                EnableThumbVertical = false,
                Title = "新建区域",
                PosX = maxPox
            });
        }
    }
}
