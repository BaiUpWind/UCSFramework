using DeviceConfig;
using DeviceConfig.Core;
using DisplayBorder.View;
using DisplayBorder.ViewModel;
using HandyControl.Controls;
using HandyControl.Tools;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MessageBox = HandyControl.Controls.MessageBox;

namespace DisplayBorder.Controls
{
    /// <summary>
    /// DeviceInfoControl.xaml 的交互逻辑
    /// </summary>
    public partial class DeviceControl : UserControl
    {
        public DeviceControl()
        {
            InitializeComponent();
            g1.Visibility = Visibility.Hidden;
            tc.Visibility = Visibility.Hidden;
        }
        private DeviceViewModel deviceView;

        public void Ini( Device device)
        {
            g1.Visibility = Visibility.Visible;
            t1.Visibility = Visibility.Hidden;

            DataContext = deviceView = new DeviceViewModel(device);
        }
        public void Close()
        {
            g1.Visibility = Visibility.Hidden;
            t1.Visibility = Visibility.Visible;
            DataContext = deviceView = null;
        }
        private void Btn_Click_CreateConn(object sender, RoutedEventArgs e)
        {
            //if (deviceView == null) return;
            //WindowHelper.GetObject<ConnectionConfigBase>((obj) =>
            //{
            //    deviceView.DefaultConn = obj;
            //});
        }

        private void Btn_Click_CreateOperation(object sender, RoutedEventArgs e)
        {
            if (deviceView == null) return;
            WindowHelper.CreateComboBox<OperationBase>((obj) =>
            {
                deviceView.Operation = obj;
            });
           
        }

        private void Dgv_Selection_Changed(object sender, SelectionChangedEventArgs e)
        {
            DeviceInfo deviceinfo = (DeviceInfo)dgv.SelectedItem;
            if (deviceinfo == null)
            {
                t2.Visibility = Visibility.Visible;
                tc.Visibility = Visibility.Hidden;
                if (deviceView != null) deviceView.CurrenDeviceInfo = null;
                //op1.Close();
                return;
            }
            else
            {
                var cbw = new WindowOperation();
                cbw.WindowStyle = WindowStyle.SingleBorderWindow;
                cbw.ResizeMode = ResizeMode.NoResize;
                cbw.WindowState = WindowState.Maximized;
                t2.Visibility = Visibility.Hidden;
                tc.Visibility = Visibility.Visible;
                deviceView.CurrenDeviceInfo = deviceinfo;
                cbw.Init(deviceView.CurrenDeviceInfo.Operation, deviceView.CurrenDeviceInfo);

                cbw.OnEnter += () =>
                {
                    deviceView.CurrenDeviceInfo.Operation = cbw.GetResult(); 
                    cbw.Close() ;
                   
                };

                cbw.OnCancel += () =>
                {
                    cbw.Close();
                };

                cbw.ShowDialog();
            } 
        }

        private void Btn_Click_DelteInfo(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            {
                int tag = (int)btn.Tag;
                var info = deviceView.CurrentDevice.DeviceInfos.Where(a => a.DeviceInfoID == tag).FirstOrDefault();
                if (info != null)
                {
                    if (btn.Content.ToString() == "删除")
                    {
                        int deviceID = info.DeviceInfoID;
                        var result = MessageBox.Ask($"确定删除'{deviceID}'?", "警告");
                        if (result == MessageBoxResult.OK)
                        {
                            deviceView.CurrentDevice.DeviceInfos.Remove(info);
                            deviceView.Infos.Remove(info);
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
