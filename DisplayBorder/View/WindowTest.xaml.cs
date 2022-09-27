using DeviceConfig.Core;
using DisplayBorder.Controls;
using HandyControl.Controls;
using HandyControl.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CommonApi; 
using DeviceConfig;
using DisplayBorder.Events;
using System.Windows;
using Window = HandyControl.Controls.Window;
using DisplayBorder.ViewModel;
using System.Windows.Threading;
using System.Threading;
using ScrollViewer = System.Windows.Controls.ScrollViewer;
using LiveChartsCore; 
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WPF;
using SkiaSharp;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using System.Collections.ObjectModel;

namespace DisplayBorder.View
{
    /// <summary>
    /// WindowTest.xaml 的交互逻辑
    /// </summary>
    public partial class WindowTest :Window
    {
        public class DataGridInfo
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int Age { get; set; }
            public string Sex { get; set; }
            public string Company { get; set; }

            public string Address { get; set; }
        }

        public WindowTest()
        {
            InitializeComponent();
            //--------------------事件系统测试

            GlobalPara.EventManager.Subscribe(OnOpenNewWindowArgs.EventID, OnOpenNewWindow);
        
            //GlobalPara.EventManager.Subscribe(OnGroupChooseArgs.EventID, OnGroupChoose);
            //-----------------------
             
        
            InitTimer();
       
            Activated += (s, e) =>
            {
                Growl.SetGrowlParent(this, true);

            };
            Deactivated += (s, e) =>
            {
                Growl.SetGrowlParent(this, false);

            };
            //注册统计图的数据类型
            LiveChartsCore.LiveCharts.Configure(config =>
            config.HasMap<ChartBasicInfo>((info, point) =>
            {
                point.PrimaryValue = info.Value;
                point.SecondaryValue = point.Context.Entity.EntityIndex;
            }));
            var datas = new List<DataGridInfo>();
            for (int i = 0; i < 100; i++)
            {
                DataGridInfo info = new DataGridInfo()
                {
                    Name = $"小马{i + 1}",
                    Address = $"位置{i}层",
                    Age = i,
                    Company = "无",
                     Description ="哈哈",
                     Sex = i%2==0?"男":"女",
                };
                datas.Add(info);
            }
            dgTest.ItemsSource = datas;

        }

        

        private DispatcherTimer mDataTimer = null; //定时器
        //private long timerExeCount = 0; //定时器执行次数

        private Group currentGroup;
  
        /// <summary>
        /// 当前在读取的组
        /// </summary>
        public Group CurrentGroup
        {
            get => currentGroup; set
            {
                currentGroup = value; 
            }
        }

      

        //private int gmIndex = 0;


        private void InitTimer()
        {
            if (mDataTimer == null)
            {
                mDataTimer = new DispatcherTimer(); 
                mDataTimer.Interval = TimeSpan.FromSeconds(1);
            }
        }

     
        private void OnOpenNewWindow(object sender, BaseEventArgs e)
        {
            if (e is OnOpenNewWindowArgs args && args.NewWindow != null)
            {
                //设置弹窗新的父类容器
                //Growl.SetGrowlParent(args.NewWindow, false); 
            }
        }
         
        

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            WindowHelper.CreateComboBox<OperationBase>((obj) =>
            {
                Growl.Info($"创建成功!{obj.GetType().Name}");
            }, para: new DataBaseConnectCfg());
        }
        //string token = "wode";
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //WindowEmpty window = new WindowEmpty();
            //Window1 window = new Window1();
            
            //Growl.SetToken(window, token);

            //var result = Growl.GetToken(window);
            //GlobalPara.EventManager.Fire(this, OnOpenNewWindowArgs.Create(window, this));

            //window.Show();
        }

       

        private void Button_Start(object sender, RoutedEventArgs e)
        {
            if(sender is Button btn)
            {
                if(btn.Content.ToString() == "开始")
                {
                    btn.Content = "停止"; 
                    mDataTimer.Start();
                }
                else if(btn.Content.ToString() == "停止")
                { 
                    mDataTimer.Stop(); 
                    btn.Content = "开始";
                }
            }
        }

        private void Btn_ShowInfo(object sender, RoutedEventArgs e)
        {
            Growl.Info("创建成功!"  );
        }

        private void Btn_AddChart(object sender, RoutedEventArgs e)
        {
            sp.Children.Clear();
            gChart.Children.Clear();
            BasicDataInfo.ChartInfo ci = new BasicDataInfo.ChartInfo(DataType.柱状图, ControlHelper.StorageInfos);
            ClassControl cc = new ClassControl(ControlHelper.StorageInfos.GetType(), true, ControlHelper.StorageInfos);
            gChart.Children.Add(ci);
            sp.Children.Add(cc);
            Task.Run(async () =>
            {
                while (true)
                {
                    Console.WriteLine("更新数据");
                    Application.Current.Dispatcher.Invoke( new Action(() =>
                    {
                        ci.Update();
                    } )); 
                    await Task.Delay(5000);
                }
            });
        }

        private void hahah_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double titlesize = ((ActualWidth / 12) / 3 * 2) / 3;
            System.Windows.Application.Current.Resources.Remove("TitleFontSize");
            System.Windows.Application.Current.Resources.Add("TitleFontSize", titlesize);
            double tabsize = ((ActualWidth / 12) / 3 * 2) / 4;
            System.Windows.Application.Current.Resources.Remove("TabFontSize");
            System.Windows.Application.Current.Resources.Add("TabFontSize", tabsize);
            double gridsize = ((ActualWidth / 12) / 3 * 2) / 5 * 0.8;
            System.Windows.Application.Current.Resources.Remove("GridFontSize");
            System.Windows.Application.Current.Resources.Add("GridFontSize", gridsize);
        }
    }
     
}


namespace MyWindows
{
    public class MyWindow : System.Windows.Window
    {
        public MyWindow()
        { 
            Activated += (s, e) =>
            {
                Growl.SetGrowlParent(this, true);

            };
            Deactivated += (s, e) =>
            {
                Growl.SetGrowlParent(this, false);

            }; 
        }
     
    }
}

