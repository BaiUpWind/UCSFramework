using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoBackUpDB
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Mutex mutex = new Mutex(true,Application.ProductName, out bool createdNew);

            if (!createdNew)
            {
                // 程序已经在运行，可以选择显示提示信息或直接退出
               MessageBox.Show("程序已经在运行！");
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FmMain()); 
            mutex.ReleaseMutex();
        }
    }
}
