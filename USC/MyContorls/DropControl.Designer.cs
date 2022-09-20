namespace USC.MyContorls
{
    partial class DropControl
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnShow = new System.Windows.Forms.Button();
            this.pTitle = new System.Windows.Forms.Panel();
            this.pOper = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnSub = new System.Windows.Forms.Button();
            this.pData = new System.Windows.Forms.Panel();
            this.lblSelectedInfo = new System.Windows.Forms.Label();
            this.pTitle.SuspendLayout();
            this.pOper.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Location = new System.Drawing.Point(37, 7);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(116, 18);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "datadatadata";
            // 
            // btnShow
            // 
            this.btnShow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShow.Location = new System.Drawing.Point(3, 3);
            this.btnShow.Name = "btnShow";
            this.btnShow.Size = new System.Drawing.Size(30, 30);
            this.btnShow.TabIndex = 1;
            this.btnShow.Text = "↓";
            this.btnShow.UseVisualStyleBackColor = true;
            this.btnShow.Click += new System.EventHandler(this.btnShow_Click);
            // 
            // pTitle
            // 
            this.pTitle.Controls.Add(this.btnShow);
            this.pTitle.Controls.Add(this.lblTitle);
            this.pTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pTitle.Location = new System.Drawing.Point(0, 0);
            this.pTitle.Name = "pTitle";
            this.pTitle.Size = new System.Drawing.Size(401, 35);
            this.pTitle.TabIndex = 3;
            // 
            // pOper
            // 
            this.pOper.Controls.Add(this.lblSelectedInfo);
            this.pOper.Controls.Add(this.btnAdd);
            this.pOper.Controls.Add(this.btnSub);
            this.pOper.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pOper.Location = new System.Drawing.Point(0, 387);
            this.pOper.Name = "pOper";
            this.pOper.Size = new System.Drawing.Size(401, 35);
            this.pOper.TabIndex = 5;
            // 
            // btnAdd
            // 
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(330, 2);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(30, 30);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnSub
            // 
            this.btnSub.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSub.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSub.Location = new System.Drawing.Point(367, 2);
            this.btnSub.Name = "btnSub";
            this.btnSub.Size = new System.Drawing.Size(30, 30);
            this.btnSub.TabIndex = 2;
            this.btnSub.Text = "-";
            this.btnSub.UseVisualStyleBackColor = true;
            this.btnSub.Click += new System.EventHandler(this.btnSub_Click);
            // 
            // pData
            // 
            this.pData.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.pData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pData.Location = new System.Drawing.Point(0, 35);
            this.pData.Name = "pData";
            this.pData.Size = new System.Drawing.Size(401, 352);
            this.pData.TabIndex = 6;
            // 
            // lblSelectedInfo
            // 
            this.lblSelectedInfo.AutoSize = true;
            this.lblSelectedInfo.Location = new System.Drawing.Point(3, 8);
            this.lblSelectedInfo.Name = "lblSelectedInfo";
            this.lblSelectedInfo.Size = new System.Drawing.Size(116, 18);
            this.lblSelectedInfo.TabIndex = 4;
            this.lblSelectedInfo.Text = "选择的对象是";
            // 
            // DropControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pData);
            this.Controls.Add(this.pOper);
            this.Controls.Add(this.pTitle);
            this.Name = "DropControl";
            this.Size = new System.Drawing.Size(401, 422);
            this.Load += new System.EventHandler(this.DropControl_Load);
            this.pTitle.ResumeLayout(false);
            this.pTitle.PerformLayout();
            this.pOper.ResumeLayout(false);
            this.pOper.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnShow;
        private System.Windows.Forms.Panel pTitle;
        private System.Windows.Forms.Panel pOper;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnSub;
        private System.Windows.Forms.Panel pData;
        private System.Windows.Forms.Label lblSelectedInfo;
    }
}
