﻿using DeviceConfig;
using DisplayBorder.ViewModel;
using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Tools;
using System;
using System.Collections.Generic;
using System.Data;
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
using MessageBox = HandyControl.Controls.MessageBox;
using Window = System.Windows.Window;

namespace DisplayBorder.View
{
    /// <summary>
    /// WindowConfig.xaml 的交互逻辑
    /// </summary>
    public partial class WindowConfig : Window
    {
        public WindowConfig()
        {
            InitializeComponent(); 
            DataContext = model;
        }
        GroupsViewModel model = new GroupsViewModel();
     
        private void Button_Click_Delete(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn)
            { 
                int tag = (int)btn.Tag;
                var group = model.Groups.Where(a => a.GroupID == tag).FirstOrDefault(); 
                if(group != null)
                {
                    if (btn.Content.ToString() == "删除")
                    {
                        int groupID = group.GroupID; 
                        var result = MessageBox.Ask($"确定删除'{groupID}'?", "警告");
                        if (result == MessageBoxResult.OK)
                        {
                            model.Groups.Remove(group);
                            Growl.Success($"'{groupID}'删除成功");

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

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Group mys = (Group)dgv.SelectedItem;
            if (mys == null)
            {
                g1.Close();
                return;
            }
            g1.Ini(mys); 
        }
    }
}