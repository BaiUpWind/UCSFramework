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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DisplayBorder
{
    public class ChartBasicInfo
    { 
        public string Name { get; set; } = "";
        public double Value { get; set; }
    }
    public static class ControlHelper
    { 
        /// <summary>
        /// 测试数据
        /// </summary>

        public   static List<ChartBasicInfo> StorageInfos => new List<ChartBasicInfo>()
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
                Value = 135,
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

        //测试方法
        public static void CreateChartConrotl(Panel panel, DataType dataType, string title = null) => CreateChartConrotl(panel, StorageInfos, dataType, title);

        /// <summary>
        /// 创建统计图的自定义控件
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="dataType"></param>
        public static void CreateChartConrotl(Panel panel, List<ChartBasicInfo> infos, DataType dataType, string title = null,int refreshTime =1000)
        {
            if (panel == null) return;
            BasicDataInfo cc = new BasicDataInfo();
            cc.Title = title;
            cc.SetDataControl(infos, dataType, refreshTime);
            panel.Children.Add(cc);
        }

        public static void CreateDataGridConrotl(Panel panel, object dataTable, DataType dataType, string title = null, int refreshTime = 1000)
        {
            if (panel == null) return;
            BasicDataInfo cc = new BasicDataInfo();
            cc.Title = title;
            cc.SetDataControl(null, dataType, refreshTime, dataTable);
            panel.Children.Add(cc);
        }
    }

   
}
