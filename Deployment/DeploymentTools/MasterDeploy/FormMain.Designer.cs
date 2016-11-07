namespace MasterDeploy
{
    partial class FormMain
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
            this.masterDeployControl1 = new MasterDeploy.MasterDeployControl();
            this.SuspendLayout();
            // 
            // masterBuildControl1
            // 
            this.masterDeployControl1.configManager = null;
            this.masterDeployControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.masterDeployControl1.Location = new System.Drawing.Point(0, 0);
            this.masterDeployControl1.MinimumSize = new System.Drawing.Size(846, 661);
            this.masterDeployControl1.msgWriter = null;
            this.masterDeployControl1.Name = "masterBuildControl1";
            this.masterDeployControl1.Size = new System.Drawing.Size(1068, 661);
            this.masterDeployControl1.TabIndex = 0;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1068, 623);
            this.Controls.Add(this.masterDeployControl1);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(750, 537);
            this.Name = "FormMain";
            this.Text = "MasterBuild - ChangeSet(s) Viewer";
            this.Activated += new System.EventHandler(this.FormMain_Activated);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FormMain_KeyPress);
            this.ResumeLayout(false);

        }

        #endregion

        private MasterDeployControl masterDeployControl1;

    }
}

