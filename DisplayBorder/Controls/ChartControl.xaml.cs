using DeviceConfig;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DisplayBorder.Controls
{
    /// <summary>
    /// 图标控制逻辑
    /// </summary>
    public partial class ChartControl : UserControl
    {
        private string title;

        public ChartControl()
        {
            InitializeComponent();
        }
        public string Title
        {
            get => title; set
            {
                title = value; 
                txtTitle.Text = value;
            } 
        }

        public void SetChart(List<ChartBasicInfo> datas, DataType dataType)
        {
            ChartControlHelper.CreateChart(g1, datas, dataType);
        }
    }


}
