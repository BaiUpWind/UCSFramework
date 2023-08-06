using AutoBackUpDB.Model;
using ControlHelper;
using ControlHelper.Winform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoBackUpDB
{
    public partial class FmSysSetting : Form
    {
        List<DBConnectConfig> configs => config.DBConnectConfigs;
        SysConfig config = null;

        public FmSysSetting()
        {
            InitializeComponent();
            Load += (s, e) =>
            {
                listConfig.Items.Clear();

                config = FmMain.txtFile.ReadJson<SysConfig>(true);
                pSysSetting.CreateControl(config);
                listConfig.SelectedIndexChanged += ListConfig_SelectedIndexChanged;
                if (configs != null && configs.Count > 0)
                {
                    listConfig.Items.AddRange(configs.Select(a => $"{a.Name}_{a.ID}").ToArray());
                    listConfig.SelectedIndex = 0;
                }

            };
            btnAdd.Click += btnAdd_Click; 
            btnRemove.Click += btnRemove_Click; 
            btnSaveSingle.Click += btnSaveSingle_Click; 
        }
        private static FmSysSetting instance;
        public static FmSysSetting Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new FmSysSetting();
                    instance.StartPosition = FormStartPosition.CenterScreen;
                }
                return instance;
            }
        }

        private void ListConfig_SelectedIndexChanged(object sender, EventArgs e)
        {
            var listbox = sender as ListBox;
            if (listbox == null) return;
            var item = listbox.SelectedItem;
            if (item == null || configs == null) return;
            var id = item.ToString().Split('_')[1].CastTo(-1);
            if (id < 0) return;
            var result = configs.Where(a => a.ID == id).FirstOrDefault();
            if (result == null) return;
            int index = listbox.Items.IndexOf(item);
            panelFDList.CreateControl(result);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var maxId = configs.Count > 0 ? configs.Max(a => a.ID) + 1 : 1;
            var newdata = new DBConnectConfig()
            {
                ID = maxId,
                Name = $"数据库_{maxId}"
            };
            configs.Add(newdata);
            listConfig.Items.Add($"{newdata.Name}_{newdata.ID}");
            listConfig.SelectedIndex = listConfig.Items.Count - 1;
            FmMain.txtFile.WriteJson(config);

        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var item = listConfig.SelectedItem;
            if (item == null)
            {
                MessageBox.Show("请选择项");
                return;
            }
            var id = item.ToString().Split('_')[1].CastTo(-1);
            if (id < 0) return;
            var find = configs.Where(a => a.ID == id).FirstOrDefault();
            if (find == null) return;
            configs.Remove(find);
            listConfig.Items.Remove(item);
        }

        private void btnSaveSingle_Click(object sender, EventArgs e)
        {
            FmMain.txtFile.WriteJson(config);
            MessageBox.Show("保存成功");
        }
    }
}
