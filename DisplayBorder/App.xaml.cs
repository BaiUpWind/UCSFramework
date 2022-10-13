using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DisplayBorder
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            this.Startup += (s, e) =>
            {
                Process[] pro = Process.GetProcesses();
                int n = pro.Where(p => p.ProcessName.Equals("DisplayBorder")).Count();
                if (n > 1)
                {
                    MessageBox.Show("已经在运行了!");
                    Environment.Exit(0);
                }
            };
        }
    }
}
