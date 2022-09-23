using DeviceConfig;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WPF;
using SkiaSharp;
using System;
using System.Collections.Generic;
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


namespace DisplayBorder.Controls
{
    /// <summary>
    /// BasicDataInfo.xaml 的交互逻辑
    /// </summary>
    public partial class BasicDataInfo : UserControl
    {
        public class ChartInfo
        {
            public ChartInfo(DataType dataType, List<ChartBasicInfo> infos)
            {
                Infos = infos;
                this.dataType = dataType;
                chart = GetChart(dataType, infos);
            }
            private Chart chart;
            private List<ChartBasicInfo> Infos;
            private DataType dataType;

            public Chart Chart => chart;

            private static double FontSize => GlobalPara.GridFontSize  ;

            public void Update()
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
                chart.CoreChart.Update();
            }
            private Chart GetChart(DataType dataType, List<ChartBasicInfo> infos = null)
            {
                Chart chart = null;
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
                                        ps.DataLabelsSize = FontSize;
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
                            series.DataLabelsPosition = PolarLabelsPosition.Outer;
                            //series.DataLabelsFormatter = p => $"{value.Name}_{p.PrimaryValue} / {p.StackedValue?.Total} ({p.StackedValue.Share:P2})";
                            series.DataLabelsFormatter = p => $"{value.Name}({p.StackedValue.Share:P2})";
                            series.TooltipLabelFormatter = p => $"{value.Name}_{p.PrimaryValue} / {p.StackedValue?.Total} ({p.StackedValue.Share:P2})";
                            series.DataLabelsSize = FontSize;
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

            public static implicit operator FrameworkElement(ChartInfo chart) => chart.Chart;
        }

        public class DataGridInfo
        {
            public DataGridInfo(DataTable dt)
            {
                grid = new DataGrid();
                grid.Background = new SolidColorBrush(Colors.Transparent);
                //grid.AutoGenerateColumns = false;
                grid.CanUserAddRows = false;
                grid.CanUserDeleteRows = false;
                grid.CanUserResizeRows = false;
                grid.CanUserSortColumns= false;
                grid.CanUserResizeColumns= false;
                grid.CanUserReorderColumns= false;
                grid.SelectionMode = DataGridSelectionMode.Single;
                grid.SelectionUnit = DataGridSelectionUnit.FullRow;
              
                //grid.ColumnHeaderHeight = 80;
                grid.Margin = new Thickness(2);
                grid.SetBinding(FontSizeProperty, new Binding() { Source = Application.Current.Resources.FindName("TabFontSize") });
                grid.GridLinesVisibility = DataGridGridLinesVisibility.Horizontal;
                //grid.HeadersVisibility = DataGridHeadersVisibility.Column;
                 
                grid.IsReadOnly = true;
           
                grid.RowHeight = 28;
                DataView dv = new DataView(dt);
                grid.ItemsSource = dv;
            }

            private DataGrid grid;
            public DataGrid GridInfo => grid;

            public static implicit operator FrameworkElement(DataGridInfo grid) => grid.GridInfo;
        }

        private string title;
        public BasicDataInfo()
        {
            InitializeComponent();
            Unloaded += (s, e) =>
            {
                intervalToken.Cancel();
            };
        }
        CancellationTokenSource intervalToken = new CancellationTokenSource();
        public string Title
        {
            get => title; set
            {
                title = value;
                txtTitle.Text = value;
            }
        }


        public void SetDataControl(List<ChartBasicInfo> datas, DataType dataType, int refreshTime,object data =null)
        {
            g1.Children.Clear();
            if (refreshTime == 0) refreshTime = 1000;

            switch (dataType)
            {
                case DataType.饼状图: 
                case DataType.线状图: 
                case DataType.柱状图:
                    ChartInfo ci = new ChartInfo(dataType, datas);
                    g1.Children.Add(ci);
                    Task.Run(async () =>
                    {
                        await Task.Delay(1000);
                        while (true)
                        {
                            if (intervalToken.IsCancellationRequested)
                            {
                                break;
                            }
                            Console.WriteLine("更新数据");
                            Application.Current.Dispatcher.Invoke(new Action(() =>
                            {
                                ci.Update();
                            }));
                            await Task.Delay(refreshTime);
                        }
                    }, intervalToken.Token);
                    break;
                case DataType.表格:
                    if(data is DataTable dt)
                    {
                        DataGridInfo di = new DataGridInfo(dt);
                        g1.Children.Add(di);
                    } 
                    break;
                default:
                    break;
            } 
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {

        }
    }
}
