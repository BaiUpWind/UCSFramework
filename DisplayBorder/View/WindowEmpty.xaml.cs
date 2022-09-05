using HandyControl.Controls;
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

namespace DisplayBorder.View
{
    /// <summary>
    /// WindowEmpty.xaml 的交互逻辑
    /// </summary>
    public partial class WindowEmpty : HandyControl.Controls.Window
    {
        public WindowEmpty()
        {
            InitializeComponent();
           if( this.Content is Grid g)
            {
                StackPanel sp = new StackPanel();

                sp.HorizontalAlignment = HorizontalAlignment.Right;
                sp.VerticalAlignment = VerticalAlignment.Bottom;
                sp.Margin = new Thickness(0, 0, 0, 20);
                sp.SetValue(Growl.GrowlParentProperty, true);
                g.Children.Add(sp);
            };
   
        

            Activated += (s, e) =>
            {
                Growl.SetGrowlParent(this, true);
            };
            Deactivated +=(s,e) => {

                Growl.SetGrowlParent(this, false);
            };
        }

        
    }
}
