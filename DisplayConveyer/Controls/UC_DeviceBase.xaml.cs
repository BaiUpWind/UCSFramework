using Config.DeviceConfig.Models;
using DisplayConveyer.Model;
using DisplayConveyer.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DisplayConveyer.Controls
{
    /// <summary>
    /// UC_DeviceBase.xaml 的交互逻辑
    /// </summary>
    public partial class UC_DeviceBase : UserControl
    {
        private delegate void DeleSetColor(Color color);
        private readonly DeleSetColor colorHandle;

        public string Title
        {
            get
            {
                return txtTitle.Text;
            }
            set
            {
                txtTitle.Text = value;
            }
        }
        public string Description
        {
            get
            {
                return txtDir.Text;
            }
            set
            {
                txtDir.Text = value;
            }
        }
        public Color TitleColor
        {
            get
            {
                return ((SolidColorBrush)txtTitle.Foreground).Color;
            }
            set
            {
                txtTitle.Foreground = new SolidColorBrush(value);
            }
        }
        public double TitleFontSize
        {
            get
            {
                return txtTitle.FontSize;
            }
            set
            {
                if (value <= 0) return;
                txtTitle.FontSize = value;
                txtDir.FontSize = value;
            }
        }
        private DeviceData Data { get;  }
        //public DeviceViewModel ViewModel { get; set; }

   
         
        public string Info => $"区域ID:{Data.AreaID}\r\b WorkID:{Data.WorkId} \r\n 名称:{Data.Name}\r\n宽：{Data.Width}\r\n高：{Data.Height}\r\n 位置X：{Data.PosX}\r\n位置Y：{Data.PosY} ";
        public UC_DeviceBase(DeviceData data)
        {
            InitializeComponent();
            colorHandle = SetColorThreadUnsafe;
            Data = data;

            DataContext = data;// ViewModel = new DeviceViewModel(data);
            //txtDir.Text = GetDri( Data.ArrowDir);
            Data.StatusChanged += StatusSetColor; 
           
        }
         
        /// <summary>
        /// 设置背景色为指定颜色，线程安全
        /// </summary>
        /// <param name="color"></param>
        public void SetColor(Color color)
        { 
            Application.Current?.Dispatcher.BeginInvoke(colorHandle, color);
        }
        /// <summary>
        /// 1-99报警red,100自动Lime,101手动灰色 ,线程安全
        /// </summary>  
        public void SetColor(int status)
        {
            SetColor(GetColor(status));
        }
        /// <summary>
        /// 设置背景色为指定颜色
        /// </summary>
        /// <param name="color"></param>
        public void SetColorThreadUnsafe(Color color)
        {
            borderStatus.Background = new SolidColorBrush(color);
        }
        private void StatusSetColor(StatusData state)
        {
            if(state == null) return;  

            if(state.MachineState == 100)
            {
                SetColor(state.LoadState == 0 ? 100 : 102);
            }
            else
            {
                SetColor(state.MachineState);
            }
        }
        private Color GetColor(int status)
        {
            if (status >= 1 && status <= 99)
            {
                return Colors.Red; 
            }
            else if (status == 100)
            {
                return Colors.Lime;
            }
            else if (status == 101)
            {
                return Colors.Gray;
            }
            else if(status == 102)
            {
                return Colors.Yellow;
            }

            return Colors.Gray;
        }

        private string GetDri(Direction dir)
        {
            switch (dir)
            {
                case Direction.None:
                    return "•";
                case Direction.Left:
                    return "←";
                case Direction.Up:
                    return "↑";
                case Direction.Right:
                    return "→";
                case Direction.Down:
                    return "↓";
                case Direction.LeftRight:
                    return "←→";
                case Direction.UpDown:
                    return "⇵";
                default:
                    return "•";
            }
        }
    }
}
