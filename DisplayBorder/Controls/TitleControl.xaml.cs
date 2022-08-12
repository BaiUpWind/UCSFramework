using DeviceConfig;
using DisplayBorder.ViewModel;
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
    /// TitleControl.xaml 的交互逻辑
    /// </summary>
    public partial class TitleControl : UserControl
    { 
        public enum DirectionArrow
        {
            Up,
            Down,
        }
        public TitleControl()
        {
            InitializeComponent();
            Init();
        }

        public TitleControl(Group group)
        {
            InitializeComponent();
            Init();
          DataContext =   groupViewModel = new GroupViewModel(group);
        }
        GroupViewModel groupViewModel;
        public event Action<object,Group> OnClick;
         
        public DirectionArrow Direction { get; private set; }
        private void Init()
        {
            SizeChanged += (s, e) =>
            {
                line1.StartPoint = d1.Location;
                line1.EndPoint = d2.Location;

                line2.StartPoint = d2.Location;
                line2.EndPoint = d3.Location;
            };
            d1.OnMove += () =>
            {
                line1.StartPoint = d1.Location;
            };
            d2.OnMove += () =>
            {
                line1.EndPoint = d2.Location;
                line2.StartPoint = d2.Location;
            };

            d3.OnMove += () =>
            {
                line2.EndPoint = d3.Location;
            };
            HideDot();
            SwitchDricetion(DirectionArrow.Up);
        }
 

        public void SwitchDricetion(DirectionArrow dir)
        {
            switch (dir)
            {
                case DirectionArrow.Up:
                    d1.HorizontalAlignment = HorizontalAlignment.Right;
                    d1.VerticalAlignment = VerticalAlignment.Bottom;
                    d1.Margin = new Thickness(0, 0, 15, 15);

                    d2.HorizontalAlignment = HorizontalAlignment.Right;
                    d2.VerticalAlignment = VerticalAlignment.Top;
                    d2.Margin = new Thickness(0, 16, 25, 0);


                    d3.HorizontalAlignment = HorizontalAlignment.Left;
                    d3.VerticalAlignment = VerticalAlignment.Top;
                    d3.Margin = new Thickness(0, 16, 0, 0);

                    txb1.HorizontalAlignment = HorizontalAlignment.Left;
                    txb1.VerticalAlignment = VerticalAlignment.Top;
                    txb1.Margin = new Thickness(0);


                    break;
                case DirectionArrow.Down:

                    d2.HorizontalAlignment = HorizontalAlignment.Left;
                    d2.VerticalAlignment = VerticalAlignment.Bottom;
                    d2.Margin = new Thickness(15, 0, 0, 15);

                    txb1.HorizontalAlignment = HorizontalAlignment.Left;
                    txb1.VerticalAlignment = VerticalAlignment.Bottom;
                    txb1.Margin = new Thickness(20, 0, 0, 25);

                    break;
            }
            Direction = dir;

            //linePath.Visibility = Visibility.Hidden;
            //linePath.Visibility = Visibility.Visible;


        }
         
        public void HideDot()
        {
            d1.Visibility = Visibility.Hidden;
            d2.Visibility = Visibility.Hidden;
            d3.Visibility = Visibility.Hidden;
        }

        public void ShowDot()
        {
            d1.Visibility = Visibility.Visible;
            d2.Visibility = Visibility.Visible;
            d3.Visibility = Visibility.Visible;
        }

   
        private void txb1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(groupViewModel != null && groupViewModel.CurrentGroup !=null)
            {
                OnClick?.Invoke(this, groupViewModel.CurrentGroup);
            }
        }

    
    } 
}


