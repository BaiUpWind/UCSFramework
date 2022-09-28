using DeviceConfig;
using DisplayBorder.Controls;
using LiveChartsCore;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.WPF;
using SkiaSharp; 
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

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
        /// <summary>
        /// 创建datagrid view 控件
        /// </summary>
        /// <param name="panel"></param>
        /// <param name="dataTable"></param>
        /// <param name="dataType"></param>
        /// <param name="title"></param>
        /// <param name="refreshTime"></param>
        public static void CreateDataGridConrotl(Panel panel, object dataTable, DataType dataType, string title = null, int refreshTime = 1000)
        {
            if (panel == null) return;
            BasicDataInfo cc = new BasicDataInfo();
            cc.Title = title;
            cc.SetDataControl(null, dataType, refreshTime, dataTable);
            panel.Children.Add(cc);
        }

        public static BasicDataInfo CreateDataControl(DataTable data,DataType dataType,string title=null,int refreshTime = 1000) 
        =>  new BasicDataInfo(data, dataType, title, refreshTime);
       

        /// <summary>
        /// 获取元素节点下的子元素
        /// </summary>
        /// <typeparam name="T">指定的类型</typeparam>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static T GetVisualChild<T>(DependencyObject parent) where T : Visual
        {
            T child = default(T);

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
    }

   
}
