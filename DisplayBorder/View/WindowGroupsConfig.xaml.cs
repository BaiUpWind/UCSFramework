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
using System.Windows.Threading;
using CommonApi;
using DeviceConfig;
using DeviceConfig.Core;
using DisplayBorder.Controls;
using DisplayBorder.Events;
using HandyControl.Controls;
using MessageBox = HandyControl.Controls.MessageBox;
using Window = HandyControl.Controls.Window;

namespace DisplayBorder.View
{
 
    /// <summary>
    /// WindowGroupsConfig.xaml 的交互逻辑
    /// </summary>
    public partial class WindowGroupsConfig : Window
    {
        public const double CanvasOffset = 45 * 2;
        public class TitleControls
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
            SetControlEnable(false);
             Loaded += (s, e) =>
            {
                 
                GlobalPara.EventManager.Subscribe(OnCanvasChildrenClickArgs.EventID, OnClickChildren);
               
                foreach (var item in C1.Children)
                {
                    if(item is Border border)
                    {
                        AdornerLayer.GetAdornerLayer(border).Add( new ElementAdorner(border));
                    }
                }
              

                if(img.Source is BitmapSource bitmap)
                {
                    lblresolution.Text = $"分辨率{bitmap.PixelWidth}*{bitmap.PixelHeight}";
                    img.Width = bitmap.Width;
                    img.Height = bitmap.Height;
                }

                var w = C1.ActualWidth;
                if (w <= 0)
                {
                    return;
                }
                var h = C1.ActualHeight;
                if (h <= 0)
                {
                    return;
                }
                //g3.Width = w;
                //g3.Height = h; 
            };

            Unloaded += (s, e) =>
            {  
                GlobalPara.EventManager.Unsubscribe(OnCanvasChildrenClickArgs.EventID, OnClickChildren);
            };
            #region 弃用代码


            //当窗体大小发生变化时同时改变 图像的宽度 暂时弃用
            //SizeChanged += (s, e) =>
            //{ 
            //    //var w = C1.ActualWidth;
            //    //if (w <= 0)
            //    //{
            //    //    return;
            //    //}
            //    //var h = C1.ActualHeight;
            //    //if (h <= 0)
            //    //{
            //    //    return;
            //    //}
            //    //g3.Width = w;
            //    //g3.Height = h;
            //    //g3.SetValue(Canvas.TopProperty, 45d);
            //    //g3.SetValue(Canvas.LeftProperty, 45d);

            //};
        
            //img.SizeChanged += (s, e) =>
            //{
            //    if (CacheTc != null)
            //    { 
            //        var parent = ((FrameworkElement)CacheTc.Parent);

            //        var xfactor = (BackImage.PixelWidth / img.Width);
            //        var yfactor  = (BackImage.PixelHeight / img.Height);
                
            //        var x = CacheTc.ImagePixelPoint.X / xfactor;
            //        var y = CacheTc.ImagePixelPoint.Y / yfactor;
          
            //        CacheTc.Parent.SetValue(Canvas.LeftProperty, x  );
            //        CacheTc.Parent.SetValue(Canvas.TopProperty, y  );


            //        var pos = parent.TransformToAncestor(C1).Transform(new Point(0, 0));
            //        lblTpos.Text = $"窗体 X:{pos.X:0} Y:{pos.Y:0}   ";
            //        Console.WriteLine($"x{CacheTc.ImagePixelPoint.X:0}  y{CacheTc.ImagePixelPoint.Y:0}");
            //    }
            //};
            #endregion
            GlobalPara.Init();
    
        }
        /// <summary>
        /// 当前选中的可拖拽border装饰器
        /// </summary>
        private ElementAdorner cacheEa; 
        private TitleControl cacheTc;

        private BitmapSource BackImage => img.Source as BitmapSource;
        /// <summary>
        /// 当前选中的标题控件
        /// </summary>
        public TitleControl CacheTc  {  get => cacheTc; set =>  cacheTc = value; }

        public ElementAdorner CacheEa { get => cacheEa; set => cacheEa = value; }

        private List<Group> groups = new List<Group>();
        private List<Border> cacheBorder = new List<Border>();

        private void SetControlEnable(bool value)
        {
            foreach (var item in opSP.Children)
            {
                if(item is UIElement uielement)
                {
                    uielement.IsEnabled = value;
                }
            } 
        }
        /// <summary>
        /// 创建组的标题
        /// </summary>
        /// <param name="g"></param>
        private void CreateTitle(Group g)
        {
            TitleControl tc = new TitleControl(g);
            tc.SetValue(CanvasHelper.IsSelectableProperty, true);
            tc.SwitchDricetion((TitleControl.DirectionArrow)g.Direction);
            Border border = new Border();
            border.Width = g.CWidth;
            border.Height = g.CHeight;
            border.Child = tc;
            border.ToolTip = "右键切换方向";

            double rx = 20, ry = 20;
            if (g.PosX >= 0)
            {
                rx = g.PosX / (BackImage.PixelWidth / img.Width);
            }
            if (g.PosY >= 0)
            {
                ry = g.PosY / (BackImage.PixelHeight / img.Height);
            }

            border.SetValue(Canvas.LeftProperty, rx);
            border.SetValue(Canvas.TopProperty, ry);
            C1.Children.Add(border);
            var al = AdornerLayer.GetAdornerLayer(border);
            var adorner = new ElementAdorner(border);
            al.Add(adorner); 
            g.GroupID = groups.Count + 1; 
            //添加到组的缓存
            
            groups.Add(g);
            cacheBorder.Add(border);
            #region 事件 
            //大小发生变化时
            border.SizeChanged += (bs, be) =>
            {
                g.CWidth = border.ActualWidth;
                g.CHeight = border.ActualHeight;
            };
            //切换方向事件
            border.MouseRightButtonDown += (rs, re) =>
            {
                if (CacheTc != null)
                {
                  
                    int dir = (int)CacheTc.Direction;
                    if(dir +1 <= 4)
                    {
                        dir++;
                    }
                    else
                    {
                        dir = 0;
                    } 
                    CacheTc.SwitchDricetion((TitleControl.DirectionArrow)dir);
                    g.Direction = dir;
                }
            };
            //拖放事件
            adorner.OnDrage += (ps, pe) =>
            {
                var pos = border.TransformToAncestor(C1).Transform(new Point(0, 0));

                if (CacheTc != null)
                {
                    var pos2 = CacheTc.TransformToAncestor(C1).Transform(new Point(0, 0));
                    var x = pos2.X * BackImage.PixelWidth / img.Width;
                    var y = pos2.Y * BackImage.PixelHeight / img.Height;
                    CacheTc.ImagePixelPoint = new Point(x, y);
                    lblTimgpos.Text = $"图像 X:{x:0} Y:{y:0}";
                    //保存所在的图像位置
                    g.PosX = x;
                    g.PosY = y;
                }
                //相对于父元素的位置
                lblTpos.Text = $"窗体 X:{pos.X:0} Y:{pos.Y:0}   ";
            };
            #endregion
        }


        private void SetImgSource()
        {
            if (!string.IsNullOrEmpty(GlobalPara.SysConfig.BackImagPath) )
            {
                try
                {
                    img.Source = new BitmapImage(new Uri(GlobalPara.SysConfig.BackImagPath, UriKind.Absolute));
                    if (img.Source is BitmapSource bitmap)
                    {
                        lblresolution.Text = $"分辨率{bitmap.PixelWidth}*{bitmap.PixelHeight}";
                        img.Width = bitmap.Width;
                        img.Height = bitmap.Height;
  
                    }
                   
                }
                catch (Exception ex)
                {
                    Growl.InfoGlobal($"打开图片失败!其他信息'{ex.Message}'");
                }
            }
        }
        #region 控件事件

        /// <summary>
        /// 当前缓存失去焦点
        /// </summary>
        private void CurrentCachceLostFocus()
        {
            if (CacheEa != null)
            {
                CacheEa.Hide(); 
            }
            if (CacheTc != null)
            { 
                CacheTc.HideDot();  
            }

        }

        //当点击的是子控件时
        private void OnClickChildren(object sender, BaseEventArgs e)
        {
            //Console.WriteLine("hit hit hit hit hit ");
            if (e is OnCanvasChildrenClickArgs args)
            {
                if (args.Source == null)
                {
                    CurrentCachceLostFocus();
                    SetControlEnable(false);
                }
                else
                {
                    CurrentCachceLostFocus();
                    if (args.Source is Control control && control.Parent is Border border)
                    {
                        var al = AdornerLayer.GetAdornerLayer(border);
                        var adorners = al.GetAdorners(border);
                        foreach (var adorner in adorners)
                        {
                            if (adorner is ElementAdorner ea)
                            { 
                                ea.Show();
                                CacheEa = ea;
                                break;
                            }
                        }
                        if (control is TitleControl tc)
                        {
                            tc.ShowDot();
                            CacheTc = tc;
                        }
                        SetControlEnable(true);
                    } 
                }
            }
        }
        //当添加一个组时
        private void Btn_ClickAddGroup(object sender, RoutedEventArgs e)
        {
            if (BackImage == null)
            {
                Growl.Info("请打开文件配置");
            }
            else
            { 
                WindowHelper.CreateWindow<Group, DataWindow>((g) =>
                {
                    CreateTitle(g);
                });
            }
        }

        //当删除一个组时
        private void Btn_DeleteGroup(object sender, RoutedEventArgs e)
        {
            if(CacheTc != null)
            {
                MessageBoxResult result = MessageBox.Ask($"是否删除'{CacheTc.groupViewModel.CurrentGroup.GroupName}'");
                if(result == MessageBoxResult.Yes)
                {
                    groups.Remove(cacheTc.groupViewModel.CurrentGroup);
                    C1.Children.Remove((UIElement)CacheTc.Parent);
                    Growl.Info("删除成功!");
                }
                else
                {
                    Growl.Info("取消删除");
                }

              
            }
            else
            {
                Growl.Info("选择一个组!");
            }
        }
        //当点击切换方向时
        private void Btn_SwitchDirection(object sender, RoutedEventArgs e)
        { 
            if (CacheTc != null)
            { 
                var dir = TitleControl.DirectionArrow.LeftUp;
                if(CacheTc.Direction == TitleControl.DirectionArrow.LeftUp)
                {
                    dir = TitleControl.DirectionArrow.LeftDown;
                }
                else
                {
                    dir = TitleControl.DirectionArrow.LeftUp;
                }
                CacheTc.SwitchDricetion(dir);
       
            }
        }
         
        private void Image_MouseMove(object sender, MouseEventArgs e)
        {
            //获取鼠标所在的位置
            if (e.Source is Image im && im.Source is BitmapSource image)
            {
                System.Windows.Point p = e.GetPosition(((IInputElement)e.Source));
                var x = e.GetPosition(img).X * image.PixelWidth / img.Width;
                var y = e.GetPosition(img).Y * image.PixelHeight / img.Height;
                lblxy.Text = $"鼠标位置:图像:X:{x:#0} Y:{y:#0)} || 画布: X:{p.X:0}Y:{p.Y:0}  ";
            } 
        }

        private void img_MouseLeave(object sender, MouseEventArgs e)
        {
            lblxy.Text = $"X:{0} Y:{0}";
        } 
        private void Btn_GroupsDetails(object sender, RoutedEventArgs e)
        {
            if (groups.Count > 0)
            {
                WindowHelper.CreateDataGirdWindow<DataGirdWindow>(groups, titleName: "组的详细信息",oldWindow:null, onClose: () =>
                {

                });
            }
            else
            {
                Growl.Warning($"未找到任何组的信息");
            }
        }

        private void Btn_OpenGroupsData(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBoxResult.OK;
            if (C1.Children.Count > 2  && GlobalPara.Groups != groups)
            {
                result = MessageBox.Ask($"当前操作未保存\n\r是否打开新的文件", "警告");
                if (result != MessageBoxResult.OK)
                {
                    return;
                } 
            }
            else
            { 
                if (C1.Children.Count > 2)
                {
                    foreach (var border in cacheBorder)
                    { 
                        C1.Children.Remove(border);
                    } 
                    img.Source = null;
                    groups.Clear();
                }
                try
                {
                    SetImgSource();
                    foreach (var group in GlobalPara.Groups)
                    {
                        CreateTitle(group);
                    }
                    groups =  new List<Group>(GlobalPara.Groups);
           
                    Growl.Info(GlobalPara.Groups.Count == 0 ?"创建成功":"打开成功");
                    
                }
                catch (Exception ex)
                { 
                    Growl.Error($"打开文件失败,可能是路径配置错误,其他信息'{ex.Message}'");
                }
              
            }
          
        }

        private void Btn_SaveGroupsData(object sender, RoutedEventArgs e)
        {
            if(groups.Count > 0)
            { 
                GlobalPara.Groups = groups;
                Growl.Info("保存成功!");
            }
        }

        private void Btn_OpenConfig(object sender, RoutedEventArgs e)
        {
            WindowHelper.CreateWindow<SysConfigPara, DataWindow>((a) =>
            {
                GlobalPara.SysConfig = a;
                Growl.InfoGlobal("保存成功!");
                SetImgSource();
            }, GlobalPara.SysConfig,"系统配置");
        }

        private void Btn_ModifyGroup(object sender, RoutedEventArgs e)
        {
            if (CacheTc != null && CacheTc.groupViewModel != null && CacheTc.groupViewModel.CurrentGroup != null)
            {

                WindowHelper.CreateWindow<Group, DataWindow>((g) =>
                {
                    CacheTc.groupViewModel.CurrentGroup = g;
                }, CacheTc.groupViewModel.CurrentGroup,"修改组信息" );
            }
            else
            {
                Growl.Info("选择一个组!");
            }

        }

        #endregion
      

      
    }


}
