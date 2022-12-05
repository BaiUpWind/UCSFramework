using DisplayConveyer.Model;
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

namespace DisplayConveyer.View
{
    /// <summary>
    /// DeviceDataEditorWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceDataEditorWindow : Window
    {
        public DeviceDataEditorWindow(DeviceData  data)
        {
            InitializeComponent();
            Data = data;
        }

        public DeviceData  Data { get; set; }

      
    }
}
