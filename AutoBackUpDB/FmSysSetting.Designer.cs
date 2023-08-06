namespace AutoBackUpDB
{
    partial class FmSysSetting
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FmSysSetting));
            this.pSysSetting = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSaveSingle = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.listConfig = new System.Windows.Forms.ListBox();
            this.panelFDList = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // pSysSetting
            // 
            this.pSysSetting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pSysSetting.AutoScroll = true;
            this.pSysSetting.BackColor = System.Drawing.Color.White;
            this.pSysSetting.Location = new System.Drawing.Point(27, 64);
            this.pSysSetting.Margin = new System.Windows.Forms.Padding(4);
            this.pSysSetting.Name = "pSysSetting";
            this.pSysSetting.Size = new System.Drawing.Size(1102, 249);
            this.pSysSetting.TabIndex = 25;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 13F);
            this.label4.Location = new System.Drawing.Point(23, 28);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(116, 26);
            this.label4.TabIndex = 24;
            this.label4.Text = "系统配置";
            // 
            // btnSaveSingle
            // 
            this.btnSaveSingle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveSingle.Location = new System.Drawing.Point(1017, 23);
            this.btnSaveSingle.Margin = new System.Windows.Forms.Padding(4);
            this.btnSaveSingle.Name = "btnSaveSingle";
            this.btnSaveSingle.Size = new System.Drawing.Size(112, 34);
            this.btnSaveSingle.TabIndex = 23;
            this.btnSaveSingle.Text = "保存";
            this.btnSaveSingle.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 13F);
            this.label2.Location = new System.Drawing.Point(367, 317);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(194, 26);
            this.label2.TabIndex = 22;
            this.label2.Text = "数据库配置信息";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 13F);
            this.label1.Location = new System.Drawing.Point(30, 317);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 26);
            this.label1.TabIndex = 21;
            this.label1.Text = "数据库集合";
            // 
            // btnRemove
            // 
            this.btnRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemove.Location = new System.Drawing.Point(156, 735);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(4);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(112, 34);
            this.btnRemove.TabIndex = 20;
            this.btnRemove.Text = "删除";
            this.btnRemove.UseVisualStyleBackColor = true;
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAdd.Location = new System.Drawing.Point(35, 735);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(4);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(112, 34);
            this.btnAdd.TabIndex = 19;
            this.btnAdd.Text = "添加";
            this.btnAdd.UseVisualStyleBackColor = true;
            // 
            // listConfig
            // 
            this.listConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.listConfig.Font = new System.Drawing.Font("宋体", 9F);
            this.listConfig.FormattingEnabled = true;
            this.listConfig.ItemHeight = 18;
            this.listConfig.Items.AddRange(new object[] {
            "1212",
            "123",
            "123",
            "123",
            "123",
            "123",
            "123",
            "123",
            "123"});
            this.listConfig.Location = new System.Drawing.Point(27, 352);
            this.listConfig.Margin = new System.Windows.Forms.Padding(4);
            this.listConfig.Name = "listConfig";
            this.listConfig.Size = new System.Drawing.Size(326, 364);
            this.listConfig.TabIndex = 18;
            // 
            // panelFDList
            // 
            this.panelFDList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelFDList.AutoScroll = true;
            this.panelFDList.BackColor = System.Drawing.Color.White;
            this.panelFDList.Location = new System.Drawing.Point(365, 352);
            this.panelFDList.Margin = new System.Windows.Forms.Padding(4);
            this.panelFDList.Name = "panelFDList";
            this.panelFDList.Size = new System.Drawing.Size(768, 374);
            this.panelFDList.TabIndex = 17;
            // 
            // FmSysSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1156, 793);
            this.Controls.Add(this.pSysSetting);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnSaveSingle);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.listConfig);
            this.Controls.Add(this.panelFDList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FmSysSetting";
            this.Text = "配置";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pSysSetting;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSaveSingle;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.ListBox listConfig;
        private System.Windows.Forms.Panel panelFDList;
    }
}

