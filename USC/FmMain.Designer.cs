namespace USC
{
    partial class FmMain
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
            this.btnSend = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.cbChosseType = new System.Windows.Forms.ComboBox();
            this.gbConnType = new System.Windows.Forms.GroupBox();
            this.gbConninfo = new System.Windows.Forms.GroupBox();
            this.btnAddGroup = new System.Windows.Forms.Button();
            this.gbGroups = new System.Windows.Forms.GroupBox();
            this.pGroup = new System.Windows.Forms.Panel();
            this.pGroupInfo = new System.Windows.Forms.Panel();
            this.gbConninfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(73, 20);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 0;
            this.btnSend.Text = "send event";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(174, 21);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(102, 22);
            this.button1.TabIndex = 1;
            this.button1.Text = "get info";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(297, 20);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(102, 22);
            this.button2.TabIndex = 2;
            this.button2.Text = "create";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // cbChosseType
            // 
            this.cbChosseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbChosseType.FormattingEnabled = true;
            this.cbChosseType.Location = new System.Drawing.Point(31, 45);
            this.cbChosseType.Name = "cbChosseType";
            this.cbChosseType.Size = new System.Drawing.Size(121, 20);
            this.cbChosseType.TabIndex = 3;
            this.cbChosseType.SelectedIndexChanged += new System.EventHandler(this.cbChosseType_SelectedIndexChanged);
            // 
            // gbConnType
            // 
            this.gbConnType.Location = new System.Drawing.Point(31, 71);
            this.gbConnType.Name = "gbConnType";
            this.gbConnType.Size = new System.Drawing.Size(444, 318);
            this.gbConnType.TabIndex = 4;
            this.gbConnType.TabStop = false;
            this.gbConnType.Text = "None";
            // 
            // gbConninfo
            // 
            this.gbConninfo.Controls.Add(this.cbChosseType);
            this.gbConninfo.Controls.Add(this.gbConnType);
            this.gbConninfo.Location = new System.Drawing.Point(544, 206);
            this.gbConninfo.Name = "gbConninfo";
            this.gbConninfo.Size = new System.Drawing.Size(509, 411);
            this.gbConninfo.TabIndex = 5;
            this.gbConninfo.TabStop = false;
            this.gbConninfo.Text = "None";
            // 
            // btnAddGroup
            // 
            this.btnAddGroup.Location = new System.Drawing.Point(124, 99);
            this.btnAddGroup.Name = "btnAddGroup";
            this.btnAddGroup.Size = new System.Drawing.Size(75, 23);
            this.btnAddGroup.TabIndex = 6;
            this.btnAddGroup.Text = "add group";
            this.btnAddGroup.UseVisualStyleBackColor = true;
            this.btnAddGroup.Click += new System.EventHandler(this.btnAddGroup_Click);
            // 
            // gbGroups
            // 
            this.gbGroups.Location = new System.Drawing.Point(12, 99);
            this.gbGroups.Name = "gbGroups";
            this.gbGroups.Size = new System.Drawing.Size(94, 413);
            this.gbGroups.TabIndex = 7;
            this.gbGroups.TabStop = false;
            this.gbGroups.Text = "Groups";
            // 
            // pGroup
            // 
            this.pGroup.BackColor = System.Drawing.SystemColors.Control;
            this.pGroup.Location = new System.Drawing.Point(205, 99);
            this.pGroup.Name = "pGroup";
            this.pGroup.Size = new System.Drawing.Size(194, 136);
            this.pGroup.TabIndex = 8;
            // 
            // pGroupInfo
            // 
            this.pGroupInfo.Location = new System.Drawing.Point(161, 332);
            this.pGroupInfo.Name = "pGroupInfo";
            this.pGroupInfo.Size = new System.Drawing.Size(211, 158);
            this.pGroupInfo.TabIndex = 9;
            // 
            // FmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1109, 702);
            this.Controls.Add(this.pGroupInfo);
            this.Controls.Add(this.pGroup);
            this.Controls.Add(this.gbGroups);
            this.Controls.Add(this.btnAddGroup);
            this.Controls.Add(this.gbConninfo);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnSend);
            this.Name = "FmMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.FmMain_Load);
            this.gbConninfo.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox cbChosseType;
        private System.Windows.Forms.GroupBox gbConnType;
        private System.Windows.Forms.GroupBox gbConninfo;
        private System.Windows.Forms.Button btnAddGroup;
        private System.Windows.Forms.GroupBox gbGroups;
        private System.Windows.Forms.Panel pGroup;
        private System.Windows.Forms.Panel pGroupInfo;
    }
}

