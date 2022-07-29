namespace USC
{
    partial class FormTest
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
            this.button1 = new System.Windows.Forms.Button();
            this.cbNames = new System.Windows.Forms.ComboBox();
            this.cbFullNames = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.pAll = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(335, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // cbNames
            // 
            this.cbNames.FormattingEnabled = true;
            this.cbNames.Location = new System.Drawing.Point(25, 37);
            this.cbNames.Name = "cbNames";
            this.cbNames.Size = new System.Drawing.Size(266, 20);
            this.cbNames.TabIndex = 1;
            // 
            // cbFullNames
            // 
            this.cbFullNames.FormattingEnabled = true;
            this.cbFullNames.Location = new System.Drawing.Point(25, 72);
            this.cbFullNames.Name = "cbFullNames";
            this.cbFullNames.Size = new System.Drawing.Size(266, 20);
            this.cbFullNames.TabIndex = 2;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(25, 116);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 4;
            this.button2.Text = "button2";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // pAll
            // 
            this.pAll.Location = new System.Drawing.Point(25, 170);
            this.pAll.Name = "pAll";
            this.pAll.Size = new System.Drawing.Size(869, 455);
            this.pAll.TabIndex = 5;
            // 
            // FormTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1157, 767);
            this.Controls.Add(this.pAll);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.cbFullNames);
            this.Controls.Add(this.cbNames);
            this.Controls.Add(this.button1);
            this.Name = "FormTest";
            this.Text = "FormTest";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox cbNames;
        private System.Windows.Forms.ComboBox cbFullNames;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel pAll;
    }
}