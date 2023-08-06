using AutoBackUpDB.Model;
using ControlHelper;
using ControlHelper.Tools;
using DBHelper;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms; 

namespace AutoBackUpDB
{
    public partial class FmMain : Form
    {
        private readonly string INFO = "1.此软件必须运行装了数据库(MySql)的电脑上,依赖安装目录(Bin)中的'mysqldump.exe'\r\n" +
                                       "2.如果数据库连接失败情况，不会进行备份，请注意网络以及目标数据库最大连接数设置\r\n" +
                                       "3.目前只支持MySql数据";
        /// <summary>
        /// 配置文件读取
        /// </summary>
        public static JsonHelper2 txtFile = new JsonHelper2(Application.StartupPath + "\\Config.ini");
        internal static  SysConfig SysConfig;
        BackUpLogic logic;
        Thread mainThread;
        bool trueExit = false;
        public FmMain()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            // 设置右键点击菜单
            ContextMenuStrip contextMenu = new ContextMenuStrip(); 
            ToolStripMenuItem menuItem1 = new ToolStripMenuItem("退出");
            ToolStripMenuItem menuItem2 = new ToolStripMenuItem("说明");

      
            menuItem1.Click += (s, e) =>
            {
               var dialog= MessageBox.Show("退出软件导致无法备份数据\r\n请确认是否退出", "询问", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dialog == DialogResult.Yes)
                {
                    trueExit = true;
                    Close();
                }
            };
            menuItem2.Click += (s, e) =>
            {
                MessageBox.Show(INFO);
            };
            contextMenu.Items.Add(menuItem2);
            contextMenu.Items.Add(menuItem1);

            notifyIcon1.ContextMenuStrip = contextMenu;
            Ini();
            this.FormClosing += (s, e) =>
            {
                Hide();
                e.Cancel = !trueExit;
            };
            Load += (s, e) =>
            {
                if (SysConfig.DBConnectConfigs.Count > 0)
                {
                    this.Close();
                } 
            };
        }
        private void Ini()
        {
            SysConfig = txtFile.ReadJson<SysConfig>(true);
            logic = new BackUpLogic(SysConfig.DBConnectConfigs);
            if(SysConfig.DeleteExpired <= 0.1)
            {
                SysConfig.DeleteExpired = 1;
            }
            if(SysConfig.BackUpInterval <= 1)
            {
                SysConfig.DeleteExpired = 1;
            }
            if(SysConfig.CheckInterval <= 100)
            {
                SysConfig.CheckInterval = 1000;
            }
            mainThread?.Abort();
            mainThread = new Thread(BackUp);
            mainThread.IsBackground = true;
            mainThread.Start();

            using (RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true))
            {
                if (SysConfig.EnableStratUp)
                {
                    // 启用开机启动，将应用程序的可执行文件路径添加到注册表
                    key.SetValue(Application.ProductName, Application.ExecutablePath);
                }
                else
                {
                    // 禁用开机启动，从注册表中移除应用程序的键值对
                    key.DeleteValue(Application.ProductName, false);
                }
            }
          
        }
        private void btnConfig_Click(object sender, EventArgs e)
        {
            try
            {
                this.Hide();
                mainThread?.Abort();
                FmSysSetting.Instance.ShowDialog();
                this.Show();
                Ini();

            }
            catch (Exception)
            { 
            }
           
        }
        void BackUp()
        {
            while (true)
            {
                try
                {
                    Thread.Sleep(SysConfig.CheckInterval);
                    if (SysConfig == null || SysConfig.DBConnectConfigs.Count == 0) continue;
                    //执行备份
                    logic.BackUp(SysConfig.BackUpPath, SysConfig.MySqlPath);
                }
                catch (ThreadAbortException) { }
                catch (Exception ex)
                {

                    var dialog = MessageBox.Show($"配位文件错误。\r\n点击[是]关闭程序 \r\n点击[否]跳转配置窗口\r\n点击[取消]继续\r\n 错误信息：{ex.Message}\r\r{ex.StackTrace}", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);
                    if(dialog == DialogResult.Yes)
                    { 
                        Invoke(new Action(() =>
                        {
                            trueExit = true;
                            Close();
                        })); 
                    }
                    if(dialog == DialogResult.No)
                    { 
                        Invoke( new Action( () =>
                        {
                            btnConfig_Click(null, null);
                        })); 
                    }
                }
            }
        }
  
         
        private void button1_Click(object sender, EventArgs e)
        { 
            //手动备份
            logic.BackUp(SysConfig.BackUpPath, SysConfig.MySqlPath);

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            WindowState = FormWindowState.Normal;
        }
    }
    public class BackUpLogic
    {
        public BackUpLogic(List<DBConnectConfig> dBConnects)
        {
            foreach (var item in dBConnects)
            {
                backUps.Add(new BackUpDB(item));
            }
        }
        List<BackUpDB> backUps = new List<BackUpDB>();


       public void BackUp(string path, string mysqlPath)
        {
            foreach (var item in backUps)
            { 
                if (!item.CheckConnect()) return;
                item.BackupDatabase(path, mysqlPath); 
            }
        }
    }

    public class BackUpDB : GetDBBase
    {
        public BackUpDB(DBConnectConfig config) : base(DBType.MySql, config.GetConnectionStr())
        {
            Config = config;
          
        } 
        public DBConnectConfig Config { get; }

        public bool CheckConnect()
        {
            return db.GetScalar("select 1 from dual;").CastTo(0) == 1;
        }

        /// <summary>
        /// 备份数据库
        /// </summary>
        /// <param name="backupPath"></param>
        /// <param name="mysqlPath"></param>
        public void BackupDatabase(string backupPath,string mysqlPath)
        {
            backupPath = $@"{backupPath}\{Config.Name}";
            if (!Directory.Exists(backupPath))
            {
                Directory.CreateDirectory(backupPath);
            }
            if (!CheckCanBackUp(backupPath)) return;
            DeleteOutTimeBackUp(backupPath);
            string backupFilePath = $@"{backupPath}\backup_{Config.DBName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.sql" ; 
            try
            {
                // 运行mysqldump命令来执行备份
                using (var process = new Process())
                {
                    process.StartInfo.FileName = $@"{mysqlPath}\mysqldump";
                    process.StartInfo.Arguments = $"--user={Config.DBUserName} --password={Config.DBPassWord} --host={Config.DBIp} --port={Config.DbPort} --routines --result-file={backupFilePath} --databases {Config.DBName}";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();

                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                   
                    }
                    else
                    {
                      
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 检查是否满足备份条件
        /// </summary>
        /// <param name="backupPath"></param>
        /// <returns></returns>
        private bool CheckCanBackUp(string  backupPath)
        {
            if (!Directory.Exists(backupPath)) return false;

            var file = new DirectoryInfo(backupPath).GetFiles().OrderByDescending(a => a.LastWriteTime).FirstOrDefault();
            if (file == null) return true;
            var span = (DateTime.Now - file.LastWriteTime).TotalMinutes;
            return span >  FmMain.SysConfig.BackUpInterval; 
        }


        /// <summary>
        /// 删除超时的备份文件，
        /// </summary>
        /// <param name="backupPath"></param>
        private void DeleteOutTimeBackUp(string backupPath)
        {
            if (!Directory.Exists(backupPath)) return;

            DirectoryInfo directoryInfo = new DirectoryInfo(backupPath);
            foreach (var flie in directoryInfo.GetFiles())
            {
                if (flie.Extension != ".sql") continue;
                var span = (DateTime.Now - flie.LastWriteTime).TotalMinutes;
                if(span > FmMain.SysConfig.DeleteExpired)
                {
                    flie.Delete();
                }
            }
        }
    }
}
