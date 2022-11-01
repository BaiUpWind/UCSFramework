using DeviceConfig;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WPF;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using System.Windows.Threading;

namespace DisplayBorder.Controls
{

    /// <summary>
    /// BasicDataInfo.xaml 的交互逻辑
    /// </summary>
    public partial class BasicDataInfo : UserControl
    {
        public abstract class DataInfoBase
        {
            /// <summary>
            /// 设置这个控件显示的信息数据
            /// <para>这个object 是[<see cref="DataTable"/>]暂时先不改，先用object吧，20220928</para>
            /// </summary>
            /// <param name="dataInfo"></param>
            public abstract void SetDataInfo(object dataInfo);
            /// <summary>
            /// 更新一次
            /// </summary>
            public abstract void Update();

            public abstract void Clear();
            protected abstract FrameworkElement Control { get; }

            public static implicit operator FrameworkElement(DataInfoBase baseinfo) => baseinfo.Control;
        }
        public sealed class ChartInfo : DataInfoBase
        {
            public ChartInfo(DataType dataType, List<ChartBasicInfo> infos)
            {
                Infos = infos;
                this.dataType = dataType;
                chart = GetChart(dataType, Infos);
                chart.Loaded += (s, e) =>
                {
                    if (chart.Parent is Panel p)
                    {
                        parent = p;
                    }
                }; 
            }
            private Chart chart;
            private List<ChartBasicInfo> Infos;
            private DataType dataType;
            private Panel parent;
            private double FontSize => GlobalPara.GridFontSize;
            protected override FrameworkElement Control => chart;
            public override void Update()
            {
                //IntrnalUpdate();
            }
            public override void SetDataInfo(object dataInfo)
            {
                if (dataInfo is DataTable dt  )
                { 
                    List<ChartBasicInfo> infos = dt.GetChartBasicInfos();
                  
                    if (infos == null) return;
                    if (Infos.Count == 0)
                    {
                        Infos = infos;

                        chart = GetChart(dataType, Infos);
                        parent?.Children.Add(chart);
                    } 
                    for (int i = 0; i < Infos.Count; i++)
                    { 
                        Infos[i].Value = infos[i].Value;
                    } 
                    IntrnalUpdate();
                }
            }
            public override void Clear()
            {
                Infos.Clear();

                if (parent != null)
                {
                    var pie = parent.Children.GetControl<PieChart>();
                    if (pie != null)
                    {
                        parent.Children.Remove(pie);
                    }
                    var c = parent.Children.GetControl<CartesianChart>();
                    if (c != null)
                    {
                        parent.Children.Remove(c);
                    }
                }
            }
            private void IntrnalUpdate()
            {
                if (Infos == null) return;
                if (Infos.Count == 0) return;
                if (chart == null) return;
                if (chart is CartesianChart cc)
                {
                    cc.XAxes = GetXAxis(Infos);
                    cc.YAxes = GetYAxis(Infos);
                    if (dataType == DataType.柱状图)
                    {
                        foreach (var item in cc.Series)
                        {
                            if (item is ColumnSeries<double> cs)
                            {
                                cs.Values = CreateMaxValue(Infos.Count, (int)(Infos.Max(a => a.Value) + Infos.Max(a => a.Value) * 0.4));
                            }
                        }
                    }
                }
                try
                {
                    chart.CoreChart.Update();
                }
                catch { /*这里有个CoreChart还未准备好的错误*/ }
            }
            private Chart GetChart(DataType dataType, List<ChartBasicInfo> infos = null)
            {
                Chart chart = null;
                Binding bind = new Binding();
                switch (dataType)
                {
                    case DataType.饼状图:
                        chart = new PieChart();
                        if (chart is PieChart pie)
                        {
                            pie.Series = CreateSeries(infos, dataType);  
                            pie.SizeChanged += (s, e) =>
                            {
                                foreach (var item in pie.Series)
                                {
                                    if (item is PieSeries<ChartBasicInfo> ps)
                                    {
                                        ps.DataLabelsSize = FontSize-4.5d;
                                    }
                                }
                            };
                        }
                        break;
                    case DataType.线状图:
                        chart = new CartesianChart();
                        if (chart is CartesianChart line)
                        {
                            line.Series = CreateSeries(infos, dataType); 
                            line.SizeChanged += (s, e) =>
                            {
                                if (line.Series is LineSeries<ChartBasicInfo> lineSeriser)
                                {
                                    lineSeriser.DataLabelsSize = FontSize;
                                }
                            };
                        }

                        break;
                    case DataType.柱状图:
                        chart = new CartesianChart();
                        if (chart is CartesianChart c)
                        {
                            c.Series = CreateSeries(infos, dataType); 
                            c.SizeChanged += (s, e) =>
                            {
                                foreach (var item in c.Series)
                                {
                                    if (item is ColumnSeries<double> cs)
                                    {
                                        cs.DataLabelsSize = FontSize;
                                    }
                                    else if (item is ColumnSeries<ChartBasicInfo> cb)
                                    {
                                        cb.DataLabelsSize = FontSize;
                                    }
                                }
                            };
                        }
                        break;
                    default:
                        throw new System.Exception($"不支持的类型{dataType}");
                }

                return chart;
            }
            private IEnumerable<ISeries> CreateSeries(List<ChartBasicInfo> infos, DataType dataType)
            {
                if (infos == null) return null;
                if (infos.Count == 0) return null;
                SolidColorPaint DataLabelFontFamily = new SolidColorPaint()
                {
                    Color = SKColors.Black,
                    FontFamily = "Microsoft YaHei UI",
                };
                int max = (int)(infos.Max(a => a.Value) + infos.Max(a => a.Value) * 0.4);
                IEnumerable<ISeries> result = null;
                switch (dataType)
                {
                    case DataType.饼状图:

                        //如果集合的数量发生变化 ,图像是不会发生改动的

                        result = infos.AsLiveChartsPieSeries((value, series) =>
                        {
                            series.Name = $"{value.Name}'{value.Value}'";
                            series.DataLabelsPaint = DataLabelFontFamily;
                            series.DataLabelsPosition = PolarLabelsPosition.Middle;
                            //series.DataLabelsFormatter = p => $"{value.Name}_{p.PrimaryValue} / {p.StackedValue?.Total} ({p.StackedValue.Share:P2})";
                            series.DataLabelsFormatter = p => $"{value.Name} ({p.StackedValue.Share:P1})";
                            series.TooltipLabelFormatter = p => $"{value.Name}_{p.PrimaryValue} / {p.StackedValue?.Total} ({p.StackedValue.Share:P2})";
                            series.DataLabelsSize = FontSize-1;
                        });


                        break;
                    case DataType.线状图:
                        var lineseriser = new LineSeries<ChartBasicInfo>
                        {
                            Values = infos,
                            Fill = null,
                            TooltipLabelFormatter = p => $"{p.Model.Name}'{p.PrimaryValue}'",
                            DataLabelsPosition = DataLabelsPosition.End,
                            DataLabelsPaint = DataLabelFontFamily,
                            //DataLabelsFormatter = p => $"{p.Model.Name}'{p.PrimaryValue}'",
                            DataLabelsFormatter = p => $" {p.PrimaryValue}",
                            DataLabelsSize = FontSize,
                        };
                        result = new ISeries[] { lineseriser };
                        break;
                    case DataType.柱状图:

                        var maxColumns = GetMaxColumns(infos);
                        var valueColumns = new ColumnSeries<ChartBasicInfo>
                        {
                            Values = infos,
                            Stroke = null,
                            Fill = new SolidColorPaint(SKColors.DeepSkyBlue),
                            IgnoresBarPosition = true,
                            TooltipLabelFormatter = p => $"{p.Model.Name}'{p.PrimaryValue}' ",
                            //在头部显示
                            DataLabelsPosition = DataLabelsPosition.End,
                            DataLabelsPaint = DataLabelFontFamily,
                            //DataLabelsFormatter = p => $"{p.Model.Name}'{p.PrimaryValue}' ",
                            DataLabelsFormatter = p => $"{p.PrimaryValue}",
                        };
                        result = new ISeries[] { valueColumns, maxColumns };
                        break;
                }
                return result;
            }
            private ColumnSeries<double> GetMaxColumns(List<ChartBasicInfo> infos)
             => new ColumnSeries<double>
             {
                 IsHoverable = false,
                 Values = CreateMaxValue(infos.Count, (int)(infos.Max(a => a.Value) + infos.Max(a => a.Value) * 0.4)),
                 Stroke = null,
                 Fill = new SolidColorPaint(new SKColor(30, 30, 30, 30)),
                 IgnoresBarPosition = true,
             };

            private Axis[] GetXAxis(List<ChartBasicInfo> infos)
            => new Axis[] { new Axis { NameTextSize = FontSize, TextSize = FontSize, Labels = infos.Select(a => a.Name).ToArray(), LabelsPaint = new SolidColorPaint() { Color = SKColors.Black, FontFamily = "Microsoft YaHei UI", }, } };
            private Axis[] GetYAxis(List<ChartBasicInfo> infos)
            => new Axis[] { new Axis { MinLimit = 0, MaxLimit = (int)(infos.Max(a => a.Value) + infos.Max(a => a.Value) * 0.4), MinStep = 1 } };

            private double[] CreateMaxValue(int lenght, double maxValue)
            {
                double[] result = new double[lenght];
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = maxValue;
                }
                return result;
            }
        } 
        public sealed class DataGridInfo : DataInfoBase
        {
            public DataGridInfo(DataTable dt)
            {
                grid = new DataGrid
                {
                    Visibility = Visibility.Hidden
                };
                SetDataGridItems(dt);
            }

            private double offset = 0;
            private readonly DataGrid grid;
            private ScrollViewer scrollViewer;
            private DataView dv;

            protected override FrameworkElement Control => grid;
            public override void Update()
            {
                if (grid.Items != null)
                {
                    if (scrollViewer == null)
                    {
                        scrollViewer = ControlHelper.GetVisualChild<ScrollViewer>(grid);
                        if (scrollViewer == null) return;
                        scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                        scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
                    }
                    grid.Visibility = Visibility.Visible;
                    if (scrollViewer.ScrollableHeight == 0) return;
                    if (offset >= scrollViewer.ScrollableHeight)
                    {
                        offset = 0;
                    }
                    else
                    {
                        offset += 0.2d;
                    }
                    scrollViewer.ScrollToVerticalOffset(offset);
                }
            }
            public override void SetDataInfo(object dataInfo)
            {
                if (dataInfo is DataTable dt)
                {
                    SetDataGridItems(dt);
                }
            }

            //public class datagirdviewmodle : INotifyPropertyChanged
            //{
            //    private DataTable data;

            //    public DataTable Data
            //    {
            //        get => data; set
            //        { 
            //            data = value;
            //            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Data)));
            //        } 
            //    }

            //    public event PropertyChangedEventHandler PropertyChanged;
            //}
            //datagirdviewmodle vm;
            private void SetDataGridItems(DataTable dt)
            {  
                if (grid.ItemsSource == null)
                {
                    dv = new DataView(dt);
                    grid.ItemsSource = dv;
              
                    //vm = new datagirdviewmodle();
                    //vm.Data = dt;
                    ////Binding bind = new Binding();
                    ////bind.Source = vm.Data;
                    //grid.DataContext = vm;
                    //grid.SetBinding(ItemsControl.ItemsSourceProperty, "Data");
                }
                else
                {
                   //就不做刷新了
                }
            }
            public override void Clear()
            {
                grid.ItemsSource = null;
            }
        }

        public sealed class TagInfo : DataInfoBase
        {
            protected override FrameworkElement Control => throw new NotImplementedException();

            public override void Clear()
            {
                throw new NotImplementedException();
            }

            public override void SetDataInfo(object dataInfo)
            {
                throw new NotImplementedException();
            }

            public override void Update()
            {
                throw new NotImplementedException();
            }
        }

        public BasicDataInfo()
        {
            InitializeComponent();
            Unloaded += (s, e) =>
            {
                intervalToken.Cancel();
            };
            /*
             * 显示信息  职位 人员 电话
             * 
             * 产量 , 货位剩余数量, 报警的核心重要的几个 
             *  
             * 
             */
        }
        public BasicDataInfo(DataTable data, DataType dataType, string title = null, int refreshTime = 1000)
        {
            InitializeComponent();
            this.dataType = dataType;
            Unloaded += (s, e) =>
            {
                intervalToken.Cancel();
            };
            //IsVisibleChanged += (s, e) =>
            //{
            //    if (this.Visibility  == Visibility.Visible)
            //    {
            //        IsRunning = true;
            //    }
            //    else
            //    {
            //        IsRunning = false;
            //        dataInfo.Clear();
            //    }
            //};  
            Init(data, dataType, title, refreshTime); 
        } 
        private CancellationTokenSource intervalToken = new CancellationTokenSource();
        private string title;
        private bool isRunning;
        private DataInfoBase dataInfo = null;
        private readonly DataType dataType; 
        public DataType DataType => dataType;

        public string Title
        {
            get => title; set
            {
                title = value;
                txtTitle.Text = value.Replace("\r\n", "");
            }
        }

        public bool IsRunning
        {
            get => isRunning; set
            {
                isRunning = value;
                if (!value)
                { 
                    intervalToken.Cancel();
                }
                else if(intervalToken.IsCancellationRequested)
                {
                    intervalToken = new CancellationTokenSource();
                }
            }
        }
        /// <summary>
        /// 初始化
        /// </summary> 
        private void Init(DataTable data, DataType dataType, string title, int refreshTime)
        {
            g1.Children.Clear();
            if (refreshTime == 0) refreshTime = 1000;
            Title = title;
            //这里表格暂时用固定死的时间方式
            refreshTime = dataType == DataType.表格 ? 200 : refreshTime;

            switch (dataType)
            {
                case DataType.饼状图:
                case DataType.线状图:
                case DataType.柱状图:
                    //单行的数据 才能 图表统计　
                    dataInfo = new ChartInfo(dataType, data.GetChartBasicInfos());
                 　
                    break;
                case DataType.表格:
                    dataInfo = new DataGridInfo(data);
                    break;
                case DataType.标签组:

                    break;
                default:
                    break;
            }
            if (dataInfo != null)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(200);
                    while (true)
                    {
                        if (intervalToken.IsCancellationRequested)
                        {
                            break;
                        }
                        Application.Current?.Dispatcher?.Invoke(new Action(() =>
                        {
                            dataInfo.Update();
                        }));
                        await Task.Delay(refreshTime);
                    }
                }, intervalToken.Token);
                g1.Children.Add(dataInfo);
            }
            else
            {
                //todo:创建一个默认的错误信息展示图
            }

        }
         
        public void SetDatas(object data)
        {
            if (dataInfo != null)
            { 
                dataInfo.SetDataInfo(data);
            }
        }
        public void SetDataControl(List<ChartBasicInfo> datas, DataType dataType, int refreshTime, object data = null)
        {
            g1.Children.Clear();
            if (refreshTime == 0) refreshTime = 1000;

            refreshTime = dataType == DataType.表格 ? 200 : refreshTime;
        
            switch (dataType)
            {
                case DataType.饼状图:
                case DataType.线状图:
                case DataType.柱状图:
                    dataInfo = new ChartInfo(dataType, datas); 
                    break;
                case DataType.表格:
                    if (data is DataTable dt)
                    {
                        dataInfo = new DataGridInfo(dt); 
                    }
                    break;
                case DataType.标签组:

                    break;
                default:
                    break;
            }
            //if (dataInfo != null)
            //{
            //    Task.Run(async () =>
            //    {
            //        await Task.Delay(200);
            //        while (true)
            //        {
            //            if (intervalToken.IsCancellationRequested)
            //            {
            //                break;
            //            }
            //            Application.Current?.Dispatcher?.Invoke(new Action(() =>
            //            {
            //                dataInfo.Update();
            //            }));
            //            await Task.Delay(refreshTime);
            //        }
            //    }, intervalToken.Token);
            //    g1.Children.Add(dataInfo);
            //}
        }


    }
}
