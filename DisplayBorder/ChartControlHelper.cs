using DeviceConfig;
using DisplayBorder.Controls;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WPF;
using SkiaSharp; 
using System.Collections.Generic;
using System.Linq; 
using System.Windows.Controls;


namespace DisplayBorder
{
    public class ChartBasicInfo
    {
     
        public string Name { get; set; }
        public double Value { get; set; }
    }
    public static class ChartControlHelper
    {
         
        /// <summary>
        /// 测试数据
        /// </summary>

       readonly static List<ChartBasicInfo> StorageInfos = new List<ChartBasicInfo>()
        {
             new ChartBasicInfo()
            {
               
                Name = "总数量",
                Value = 26,
            },
            new ChartBasicInfo()
            {
               
                Name = "其他数量",
                Value = 4,
            },
            new ChartBasicInfo()
            {
                
                Name = "工装数量",
                Value = 2,
            },
            new ChartBasicInfo()
            {
                
                Name = "货位数量",
                Value = 35,
            },
            new ChartBasicInfo()
            {
               
                Name = "静止架数量",
                Value = 2,
            },
            new ChartBasicInfo()
            {
                 
                Name = "空闲数量",
                Value = 12,
            },
        };

    
        public static void CreateChartConrotl(Panel panel,  DataType dataType, string title = null) => CreateChartConrotl(panel,StorageInfos,dataType, title);

        /// <summary>
        /// 创建饼状图的自定义控件
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="dataType"></param>
        public static void CreateChartConrotl(Panel panel,List<ChartBasicInfo> infos,DataType dataType,string title = null)
        {
            if (panel == null) return;
            ChartControl cc = new ChartControl();
            cc.Title = title;
            cc.SetChart(infos, dataType);
            panel.Children.Add(cc);
        }


        public static void CreateChart(Panel dataGrid, DataType dataTypes) => CreateChart(dataGrid, StorageInfos, dataTypes);

        /// <summary>
        /// 在父窗体上创建
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataGrid"></param>
        /// <param name="storageInfos"></param>
        /// <param name="dataTypes"></param>
        public static void CreateChart (Panel dataGrid, List<ChartBasicInfo> storageInfos, DataType dataTypes) 
        {
            dataGrid.Children.Clear();
            int max = (int)(storageInfos.Max(a => a.Value) + storageInfos.Max(a => a.Value) * 0.4);
            SolidColorPaint DataLabelFontFamily = new SolidColorPaint()
            {
                Color = SKColors.Black,
                FontFamily = "Microsoft YaHei UI"
            };
            switch (dataTypes)
            {
                case DataType.饼状图:
                    PieChart pie = new PieChart();
                    pie.Series = storageInfos.AsLiveChartsPieSeries((value, series) =>
                    {
                        series.Name = $"{value.Name}'{value.Value}'";
                        series.DataLabelsPaint = DataLabelFontFamily;
                        series.DataLabelsPosition = PolarLabelsPosition.Outer;
                        series.DataLabelsFormatter = p => $"{value.Name}_{p.PrimaryValue} / {p.StackedValue?.Total} ({p.StackedValue.Share:P2})"; 
                        series.TooltipLabelFormatter = p => $"{value.Name}_{p.PrimaryValue} / {p.StackedValue?.Total} ({p.StackedValue.Share:P2})";

                    });

                    //  显示饼状图的总统计 如果需要显示就放开这行
                    //pie.LegendPosition = LiveChartsCore.Measure.LegendPosition.Right;

                    dataGrid.Children.Add(pie);
                    break;
                case DataType.线状图:
                    CartesianChart cline = new CartesianChart();

                    cline.Series = new ISeries[] {
                        new LineSeries<ChartBasicInfo>
                        {
                            Values =storageInfos,
                            Fill = null,
                            TooltipLabelFormatter= p => $"{p.Model .Name}'{p.PrimaryValue}'", 
                            DataLabelsPosition = DataLabelsPosition.End,
                            DataLabelsPaint = DataLabelFontFamily,
                            DataLabelsFormatter = p => $"{p.Model .Name}'{p.PrimaryValue}'",
                        }
                    };

                    cline.XAxes = new Axis[] { new Axis { Labels = storageInfos.Select(a => a.Name).ToArray(), LabelsPaint = DataLabelFontFamily } };
                    cline.YAxes = new Axis[] { new Axis { MinLimit = 0, MaxLimit = max, MinStep = 1 } };
                    dataGrid.Children.Add(cline);
                    break;
                case DataType.柱状图:
                    CartesianChart cc = new CartesianChart(); 
                    cc.Series = new ISeries[]{
                        //最大值
                        new ColumnSeries<double>
                        {
                            IsHoverable = false,
                            Values = CreateMaxValue(storageInfos.Count,max),
                            Stroke = null,
                            Fill = new SolidColorPaint(new SKColor(30, 30, 30, 30)),
                            IgnoresBarPosition = true
                        }, 
                        //当前值
                        new ColumnSeries<ChartBasicInfo>
                        {
                            Values = storageInfos,
                            Stroke = null,
                            Fill = new SolidColorPaint(SKColors.DeepSkyBlue),

                            IgnoresBarPosition = true,
                            TooltipLabelFormatter= p => $"{p.Model .Name}'{p.PrimaryValue}' ",

                            //在头部显示
                            DataLabelsPosition = DataLabelsPosition.End,
                            DataLabelsPaint = DataLabelFontFamily,
                            DataLabelsFormatter = p => $"{p.Model .Name}'{p.PrimaryValue}' ",
                        }
                    };
                    cc.YAxes = new Axis[] { new Axis { MinLimit = 0, MaxLimit = max, MinStep = 1 }, };
                    cc.XAxes = new Axis[] { new Axis { Labels = storageInfos.Select(a => a.Name).ToArray(), LabelsPaint = DataLabelFontFamily } };

                    double[] CreateMaxValue(int lenght, double maxValue)
                    {
                        double[] result = new double[lenght];
                        for (int i = 0; i < result.Length; i++)
                        {
                            result[i] = maxValue;
                        }
                        return result;
                    }

                    dataGrid.Children.Add(cc);
                    break;
            }


        }
        
    }
}
