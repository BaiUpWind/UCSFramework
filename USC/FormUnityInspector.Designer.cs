namespace USC
{
    partial class FormUnityInspector
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
            this.inspector = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // inspector
            // 
            this.inspector.Dock = System.Windows.Forms.DockStyle.Right;
            this.inspector.Location = new System.Drawing.Point(320, 0);
            this.inspector.Name = "inspector";
            this.inspector.Size = new System.Drawing.Size(510, 575);
            this.inspector.TabIndex = 0;
            // 
            // FormUnityInspector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(830, 575);
            this.Controls.Add(this.inspector);
            this.Name = "FormUnityInspector";
            this.Text = "FormUnityInspector";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel inspector;
    }
}