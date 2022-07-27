namespace USC
{
    partial class FmSetting
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gpDb = new System.Windows.Forms.GroupBox();
            this.txtUserPassWord = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDbIp = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbDBtype = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbldatype = new System.Windows.Forms.Label();
            this.btnDbTest = new System.Windows.Forms.Button();
            this.gpDb.SuspendLayout();
            this.SuspendLayout();
            // 
            // gpDb
            // 
            this.gpDb.Controls.Add(this.btnDbTest);
            this.gpDb.Controls.Add(this.txtUserPassWord);
            this.gpDb.Controls.Add(this.txtUserName);
            this.gpDb.Controls.Add(this.label5);
            this.gpDb.Controls.Add(this.txtName);
            this.gpDb.Controls.Add(this.label4);
            this.gpDb.Controls.Add(this.txtPort);
            this.gpDb.Controls.Add(this.label3);
            this.gpDb.Controls.Add(this.txtDbIp);
            this.gpDb.Controls.Add(this.label2);
            this.gpDb.Controls.Add(this.cmbDBtype);
            this.gpDb.Controls.Add(this.label1);
            this.gpDb.Controls.Add(this.lbldatype);
            this.gpDb.Location = new System.Drawing.Point(12, 12);
            this.gpDb.Name = "gpDb";
            this.gpDb.Size = new System.Drawing.Size(420, 140);
            this.gpDb.TabIndex = 9;
            this.gpDb.TabStop = false;
            this.gpDb.Text = "数据库接口";
            // 
            // txtUserPassWord
            // 
            this.txtUserPassWord.Location = new System.Drawing.Point(284, 51);
            this.txtUserPassWord.Name = "txtUserPassWord";
            this.txtUserPassWord.Size = new System.Drawing.Size(121, 21);
            this.txtUserPassWord.TabIndex = 18;
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(284, 19);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(121, 21);
            this.txtUserName.TabIndex = 17;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(234, 54);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 7;
            this.label5.Text = "密码：";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(89, 48);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(121, 21);
            this.txtName.TabIndex = 16;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(222, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "用户名：";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(284, 83);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(121, 21);
            this.txtPort.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(27, 51);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 9;
            this.label3.Text = "实例名：";
            // 
            // txtDbIp
            // 
            this.txtDbIp.Location = new System.Drawing.Point(89, 77);
            this.txtDbIp.Name = "txtDbIp";
            this.txtDbIp.Size = new System.Drawing.Size(121, 21);
            this.txtDbIp.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(234, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "端口：";
            // 
            // cmbDBtype
            // 
            this.cmbDBtype.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDBtype.Font = new System.Drawing.Font("微软雅黑", 8F, System.Drawing.FontStyle.Bold);
            this.cmbDBtype.FormattingEnabled = true;
            this.cmbDBtype.Items.AddRange(new object[] {
            "MySql",
            "Oracle",
            "SQLServer"});
            this.cmbDBtype.Location = new System.Drawing.Point(89, 18);
            this.cmbDBtype.Name = "cmbDBtype";
            this.cmbDBtype.Size = new System.Drawing.Size(121, 24);
            this.cmbDBtype.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(27, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 11;
            this.label1.Text = "IP地址：";
            // 
            // lbldatype
            // 
            this.lbldatype.AutoSize = true;
            this.lbldatype.ForeColor = System.Drawing.Color.Black;
            this.lbldatype.Location = new System.Drawing.Point(3, 22);
            this.lbldatype.Name = "lbldatype";
            this.lbldatype.Size = new System.Drawing.Size(77, 12);
            this.lbldatype.TabIndex = 12;
            this.lbldatype.Text = "数据库类型：";
            // 
            // btnDbTest
            // 
            this.btnDbTest.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDbTest.Location = new System.Drawing.Point(330, 111);
            this.btnDbTest.Name = "btnDbTest";
            this.btnDbTest.Size = new System.Drawing.Size(75, 23);
            this.btnDbTest.TabIndex = 19;
            this.btnDbTest.Text = "连接测试";
            this.btnDbTest.UseVisualStyleBackColor = true;
            // 
            // FmSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(848, 489);
            this.Controls.Add(this.gpDb);
            this.Name = "FmSetting";
            this.Text = "FmSetting";
            this.gpDb.ResumeLayout(false);
            this.gpDb.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gpDb;
        private System.Windows.Forms.Button btnDbTest;
        private System.Windows.Forms.TextBox txtUserPassWord;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDbIp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbDBtype;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbldatype;
    }
}