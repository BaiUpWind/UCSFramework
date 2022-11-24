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
using DisplayBorder.ViewModel;

namespace DisplayBorder.Controls
{
    /// <summary>
    /// TagControl.xaml 的交互逻辑
    /// </summary>
    public partial class TagControl : UserControl
    {
        public TagControl()
        {
            InitializeComponent();
           
        }
    
        //public string HeadName { get => headName.Text; set => headName.Text = value; }
        //public string Value { get => value.Text; set => this.value.Text = value; }

        public double TagFontSize
        {
            get => headName.FontSize; set
            {

                headName.FontSize = value;
                this.value.FontSize = value;
            }
        }

        public Color TagHeadColor
        {
            get
            {
                return ((SolidColorBrush)bHead.Background).Color;
            }
            set
            {
                bHead.Background = new SolidColorBrush(value);
            }
        }
        public Color TagValueColor
        {
            get
            {
                return ((SolidColorBrush)bValue.Background).Color;
            }
            set
            {
                bValue.Background = new SolidColorBrush(value);
            }
        }


        public Color TagHeadTextColor
        {
            get
            {
                return ((SolidColorBrush)headName.Foreground).Color;
            }
            set
            {
                headName.Foreground = new SolidColorBrush(value);
            }
        }
        public Color TagValueTextColor
        {
            get
            {
                return ((SolidColorBrush)value.Foreground).Color;
            }
            set
            {
                this.value.Foreground = new SolidColorBrush(value);
            }
        }

    }
}
