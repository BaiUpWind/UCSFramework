using AutoBackUpDB.Model;
using ControlHelper;
using ControlHelper.Tools;
using DBHelper;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoBackUpDB
{
    public partial class FmMain : Form
    {
        /// <summary>
        /// 配置文件读取
        /// </summary>
        public static JsonHelper2 txtFile = new JsonHelper2(Application.StartupPath + "\\Config.ini");
        SysConfig SysConfig;
        BackUpLogic logic;
        public FmMain()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            SysConfig = txtFile.ReadJson<SysConfig>(true);
            Ini();
            this.FormClosing += (s, e) =>
            {
                Hide();
                e.Cancel = true;
            };
        }
        private void Ini()
        {
            logic = new BackUpLogic(SysConfig.DBConnectConfigs);
        }
        private void btnConfig_Click(object sender, EventArgs e)
        {
            this.Hide();
            FmSysSetting.Instance.ShowDialog();
            this.Show();
            Ini();

        }

        void StartBackUp()
        {
            if (SysConfig == null || SysConfig.DBConnectConfigs.Count == 0) return;


        }

         
        private void button1_Click(object sender, EventArgs e)
        {
            logic.BackUp(SysConfig.BackUpPath);
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


       public void BackUp(string path)
        {
            foreach (var item in backUps)
            {
                if (!item.CheckConnect()) continue;
                item.BackupDatabase(path);
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

        public void BackupDatabase(string backupPath)
        {
            if (!Directory.Exists(backupPath))
            {
                Directory.CreateDirectory(backupPath);
            }
            //string backupFileName = "backup_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".sql";
            string backupFilePath = $@"{backupPath}\backup_{Config.DBName}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.sql" ;

            try
            {
                // 运行mysqldump命令来执行备份
                using (var process = new Process())
                {
                    process.StartInfo.FileName = "mysqldump";
                    process.StartInfo.Arguments = $"--user={Config.DBUserName} --password={Config.DBPassWord} --host={Config.DBIp} --port={Config.DbPort} --routines --result-file={backupFilePath} --databases {Config.DBName}";
                      //Config.DBUserName, Config.DBPassWord, Config.DBName, "3306", backupFilePath, "your_database");
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
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
            
            }
        }
    }
}
