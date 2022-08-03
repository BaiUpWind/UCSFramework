using DeviceConfig;
using DeviceConfig.Core;
using DisplayBorder.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DisplayBorder.View
{
    /// <summary>
    /// GroupControl.xaml 的交互逻辑
    /// </summary>
    public partial class GroupControl : UserControl, ISingleOpen
    {
        public GroupControl()
        {
            InitializeComponent();  
        } 
        GroupViewModel groupViewModel;

        public bool CanDispose => true;

        public void Ini(Group group)
        {
            DataContext = groupViewModel = new GroupViewModel(group);
        }

        private void Btn_Click_CreateConn(object sender, RoutedEventArgs e)
        {
            WindowHelper.GetObject<ConnectionConfigBase>((obj) =>
            {
                groupViewModel.DefaultConn = obj;
            });
        }

        public void Dispose()
        {
            groupViewModel = null;
        }

        private void Dgv_Selection_Changed(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
