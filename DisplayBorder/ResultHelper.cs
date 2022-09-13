using DeviceConfig;
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
    public class StorageInfo
    {
        public int Key { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
    }
    public class ResultHelper
    {
       
        
      
            static List<StorageInfo> StorageInfos = new List<StorageInfo>()
        {
             new StorageInfo()
            {
                 Key =1,
                Name = "总数量",
                Value = 26,
            },
            new StorageInfo()
            {
                  Key =2,
                Name = "其他数量",
                Value = 4,
            },
            new StorageInfo()
            {
                  Key =3,
                Name = "工装数量",
                Value = 2,
            },
            new StorageInfo()
            {
                Key = 4,
                Name = "货位数量",
                Value = 35,
            },
            new StorageInfo()
            {
                Key = 5,
                Name = "静止架数量",
                Value = 2,
            },
            new StorageInfo()
            {
                Key = 6,
                Name = "空闲数量",
                Value = 12,
            },
        };

             public static void CreateDataControl(Panel dataGrid, DataType dataTypes) => CreateDataControl(dataGrid, StorageInfos, dataTypes);
            public static void CreateDataControl(Panel dataGrid, List<StorageInfo> storageInfos, DataType dataTypes)
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
                    case  DataType.饼状图:
                        PieChart pie = new PieChart();
                        pie.Series = storageInfos.AsLiveChartsPieSeries((value, series) =>
                        {
                            series.Name = $"{value.Name}'{value.Value}'";
                            series.DataLabelsPaint = DataLabelFontFamily;
                            series.DataLabelsPosition = PolarLabelsPosition.Outer;
                            series.DataLabelsFormatter = p => $"{value.Name}_{p.PrimaryValue} / {p.StackedValue?.Total} ({p.StackedValue.Share:P2})";

                            series.TooltipLabelFormatter = p => $"{value.Name}_{p.PrimaryValue} / {p.StackedValue?.Total} ({p.StackedValue.Share:P2})";

                        });

                        //pie.LegendPosition = LiveChartsCore.Measure.LegendPosition.Right;

                        dataGrid.Children.Add(pie);
                        break;
                    case  DataType.线状图 :
                        CartesianChart cline = new CartesianChart();

                        cline.Series = new ISeries[] {
                        new LineSeries<StorageInfo>
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
                    case  DataType.柱状图 :
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
                        new ColumnSeries<StorageInfo>
                        {
                            Values = storageInfos,
                            Stroke = null,
                            Fill = new SolidColorPaint(SKColors.CornflowerBlue),

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
