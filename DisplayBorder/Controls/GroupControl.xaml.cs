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
using MessageBox = HandyControl.Controls.MessageBox;

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
        public void Close()
        {
            DataContext = null;
        }
        private void Btn_Click_CreateConn(object sender, RoutedEventArgs e)
        {
            //WindowHelper.GetObject<ConnectionConfigBase>((obj) =>
            //{
            //    groupViewModel.DefaultConn = obj;
            //});
        }

        public void Dispose()
        {
            groupViewModel = null;
        }

        private void Dgv_Selection_Changed(object sender, SelectionChangedEventArgs e)
        {
            Device mys = (Device)dgv.SelectedItem;
            if (mys == null)
            {
                d1.Close();
                return;
            }
            d1.Ini(mys);
        }

        private void Btn_Click_DeleteDevice(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                int tag = (int)btn.Tag;
                var device = groupViewModel.CurrentGroup.DeviceConfigs.Where(a => a.DeviceId == tag).FirstOrDefault();
                if (device != null)
                {
                    if (btn.Content.ToString() == "删除")
                    {
                        int deviceID = device.DeviceId; 
                        var result = MessageBox.Ask($"确定删除'{deviceID}'?", "警告");
                        if (result == MessageBoxResult.OK)
                        {
                            groupViewModel.CurrentGroup.DeviceConfigs.Remove(device);
                            Growl.Success($"'{deviceID}'删除成功"); 
                        }
                        else
                        {
                            Growl.Warning($"取消删除");
                        }
                    } 
                }
                else
                {
                    Growl.Error($"未找到对应的组'{tag}'");
                }
            }
        }
    }
}
