namespace DeploymentTools.Controls
{
    partial class COMRegistrationControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btRegisterDLLs = new System.Windows.Forms.Button();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.btExcludeHelp = new System.Windows.Forms.Button();
            this.txtExclude = new System.Windows.Forms.TextBox();
            this.lbExclude = new System.Windows.Forms.Label();
            this.lbSrc = new System.Windows.Forms.Label();
            this.txtSrc = new System.Windows.Forms.TextBox();
            this.btSrc = new System.Windows.Forms.Button();
            this.grpRemote = new System.Windows.Forms.GroupBox();
            this.pnlRemote = new System.Windows.Forms.Panel();
            this.btRegisterRemote = new System.Windows.Forms.Button();
            this.btDestRemoteHelp = new System.Windows.Forms.Button();
            this.lbDestRemote = new System.Windows.Forms.Label();
            this.grpLocal = new System.Windows.Forms.GroupBox();
            this.lbDestFolder = new System.Windows.Forms.Label();
            this.txtDestLocal = new System.Windows.Forms.TextBox();
            this.btDestLocal = new System.Windows.Forms.Button();
            this.btRegisterLocal = new System.Windows.Forms.Button();
            this.txtMessageBox = new System.Windows.Forms.RichTextBox();
            this.dlgCOMSourceFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.dlgCOMDestinationFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.chkCOMRunInParalel = new System.Windows.Forms.CheckBox();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.grpRemote.SuspendLayout();
            this.pnlRemote.SuspendLayout();
            this.grpLocal.SuspendLayout();
            this.SuspendLayout();
            // 
            // btRegisterDLLs
            // 
            this.btRegisterDLLs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btRegisterDLLs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btRegisterDLLs.Location = new System.Drawing.Point(104, 119);
            this.btRegisterDLLs.Name = "btRegisterDLLs";
            this.btRegisterDLLs.Size = new System.Drawing.Size(175, 23);
            this.btRegisterDLLs.TabIndex = 3;
            this.btRegisterDLLs.Text = "Register On Local Computer";
            this.btRegisterDLLs.UseVisualStyleBackColor = true;
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 0);
            this.splitMain.Name = "splitMain";
            this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.btExcludeHelp);
            this.splitMain.Panel1.Controls.Add(this.txtExclude);
            this.splitMain.Panel1.Controls.Add(this.lbExclude);
            this.splitMain.Panel1.Controls.Add(this.lbSrc);
            this.splitMain.Panel1.Controls.Add(this.txtSrc);
            this.splitMain.Panel1.Controls.Add(this.btSrc);
            this.splitMain.Panel1.Controls.Add(this.grpRemote);
            this.splitMain.Panel1.Controls.Add(this.grpLocal);
            this.splitMain.Panel1MinSize = 220;
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.txtMessageBox);
            this.splitMain.Size = new System.Drawing.Size(687, 525);
            this.splitMain.SplitterDistance = 220;
            this.splitMain.TabIndex = 0;
            // 
            // btExcludeHelp
            // 
            this.btExcludeHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btExcludeHelp.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btExcludeHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btExcludeHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btExcludeHelp.ForeColor = System.Drawing.Color.Silver;
            this.btExcludeHelp.Location = new System.Drawing.Point(652, 36);
            this.btExcludeHelp.Name = "btExcludeHelp";
            this.btExcludeHelp.Size = new System.Drawing.Size(29, 21);
            this.btExcludeHelp.TabIndex = 30;
            this.btExcludeHelp.TabStop = false;
            this.btExcludeHelp.Text = "?";
            this.btExcludeHelp.UseVisualStyleBackColor = true;
            this.btExcludeHelp.Click += new System.EventHandler(this.btExcludeHelp_Click);
            // 
            // txtExclude
            // 
            this.txtExclude.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExclude.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtExclude.Location = new System.Drawing.Point(104, 37);
            this.txtExclude.Name = "txtExclude";
            this.txtExclude.Size = new System.Drawing.Size(542, 20);
            this.txtExclude.TabIndex = 28;
            // 
            // lbExclude
            // 
            this.lbExclude.AutoSize = true;
            this.lbExclude.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbExclude.Location = new System.Drawing.Point(6, 40);
            this.lbExclude.Name = "lbExclude";
            this.lbExclude.Size = new System.Drawing.Size(45, 13);
            this.lbExclude.TabIndex = 29;
            this.lbExclude.Text = "Exclude";
            // 
            // lbSrc
            // 
            this.lbSrc.AutoSize = true;
            this.lbSrc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSrc.Location = new System.Drawing.Point(6, 14);
            this.lbSrc.Name = "lbSrc";
            this.lbSrc.Size = new System.Drawing.Size(73, 13);
            this.lbSrc.TabIndex = 25;
            this.lbSrc.Text = "Source Folder";
            // 
            // txtSrc
            // 
            this.txtSrc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSrc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSrc.Location = new System.Drawing.Point(104, 11);
            this.txtSrc.Name = "txtSrc";
            this.txtSrc.Size = new System.Drawing.Size(542, 20);
            this.txtSrc.TabIndex = 26;
            // 
            // btSrc
            // 
            this.btSrc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSrc.Location = new System.Drawing.Point(652, 11);
            this.btSrc.Name = "btSrc";
            this.btSrc.Size = new System.Drawing.Size(29, 23);
            this.btSrc.TabIndex = 27;
            this.btSrc.TabStop = false;
            this.btSrc.Text = "...";
            this.btSrc.UseVisualStyleBackColor = true;
            this.btSrc.Click += new System.EventHandler(this.btSrc_Click);
            // 
            // grpRemote
            // 
            this.grpRemote.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpRemote.BackColor = System.Drawing.Color.Transparent;
            this.grpRemote.Controls.Add(this.pnlRemote);
            this.grpRemote.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpRemote.Location = new System.Drawing.Point(6, 62);
            this.grpRemote.Name = "grpRemote";
            this.grpRemote.Size = new System.Drawing.Size(387, 156);
            this.grpRemote.TabIndex = 24;
            this.grpRemote.TabStop = false;
            this.grpRemote.Text = "Remote Computers Registration";
            // 
            // pnlRemote
            // 
            this.pnlRemote.AutoSize = true;
            this.pnlRemote.BackColor = System.Drawing.Color.Lavender;
            this.pnlRemote.Controls.Add(this.chkCOMRunInParalel);
            this.pnlRemote.Controls.Add(this.btRegisterRemote);
            this.pnlRemote.Controls.Add(this.btDestRemoteHelp);
            this.pnlRemote.Controls.Add(this.lbDestRemote);
            this.pnlRemote.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRemote.Location = new System.Drawing.Point(3, 16);
            this.pnlRemote.Name = "pnlRemote";
            this.pnlRemote.Size = new System.Drawing.Size(381, 137);
            this.pnlRemote.TabIndex = 23;
            // 
            // btRegisterRemote
            // 
            this.btRegisterRemote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btRegisterRemote.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btRegisterRemote.Location = new System.Drawing.Point(267, 110);
            this.btRegisterRemote.Name = "btRegisterRemote";
            this.btRegisterRemote.Size = new System.Drawing.Size(111, 23);
            this.btRegisterRemote.TabIndex = 6;
            this.btRegisterRemote.Text = "Register Remote";
            this.btRegisterRemote.UseVisualStyleBackColor = true;
            this.btRegisterRemote.Click += new System.EventHandler(this.btRegisterRemote_Click);
            // 
            // btDestRemoteHelp
            // 
            this.btDestRemoteHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btDestRemoteHelp.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btDestRemoteHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btDestRemoteHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btDestRemoteHelp.ForeColor = System.Drawing.Color.Silver;
            this.btDestRemoteHelp.Location = new System.Drawing.Point(349, 6);
            this.btDestRemoteHelp.Name = "btDestRemoteHelp";
            this.btDestRemoteHelp.Size = new System.Drawing.Size(29, 21);
            this.btDestRemoteHelp.TabIndex = 21;
            this.btDestRemoteHelp.TabStop = false;
            this.btDestRemoteHelp.Text = "?";
            this.btDestRemoteHelp.UseVisualStyleBackColor = true;
            this.btDestRemoteHelp.Click += new System.EventHandler(this.btDestRemoteHelp_Click);
            // 
            // lbDestRemote
            // 
            this.lbDestRemote.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbDestRemote.BackColor = System.Drawing.Color.Transparent;
            this.lbDestRemote.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDestRemote.Location = new System.Drawing.Point(3, 1);
            this.lbDestRemote.Name = "lbDestRemote";
            this.lbDestRemote.Size = new System.Drawing.Size(343, 132);
            this.lbDestRemote.TabIndex = 22;
            this.lbDestRemote.Text = "web1, \\\\10.0.5.10\\e$\\Migration";
            // 
            // grpLocal
            // 
            this.grpLocal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLocal.BackColor = System.Drawing.Color.Transparent;
            this.grpLocal.Controls.Add(this.lbDestFolder);
            this.grpLocal.Controls.Add(this.txtDestLocal);
            this.grpLocal.Controls.Add(this.btDestLocal);
            this.grpLocal.Controls.Add(this.btRegisterLocal);
            this.grpLocal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpLocal.Location = new System.Drawing.Point(399, 62);
            this.grpLocal.Name = "grpLocal";
            this.grpLocal.Size = new System.Drawing.Size(285, 156);
            this.grpLocal.TabIndex = 23;
            this.grpLocal.TabStop = false;
            this.grpLocal.Text = "Local Computer Registration";
            // 
            // lbDestFolder
            // 
            this.lbDestFolder.AutoSize = true;
            this.lbDestFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDestFolder.Location = new System.Drawing.Point(6, 26);
            this.lbDestFolder.Name = "lbDestFolder";
            this.lbDestFolder.Size = new System.Drawing.Size(92, 13);
            this.lbDestFolder.TabIndex = 3;
            this.lbDestFolder.Text = "Destination Folder";
            // 
            // txtDestLocal
            // 
            this.txtDestLocal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDestLocal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDestLocal.Location = new System.Drawing.Point(9, 42);
            this.txtDestLocal.Name = "txtDestLocal";
            this.txtDestLocal.Size = new System.Drawing.Size(235, 20);
            this.txtDestLocal.TabIndex = 2;
            // 
            // btDestLocal
            // 
            this.btDestLocal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btDestLocal.Location = new System.Drawing.Point(250, 39);
            this.btDestLocal.Name = "btDestLocal";
            this.btDestLocal.Size = new System.Drawing.Size(29, 23);
            this.btDestLocal.TabIndex = 5;
            this.btDestLocal.TabStop = false;
            this.btDestLocal.Text = "...";
            this.btDestLocal.UseVisualStyleBackColor = true;
            this.btDestLocal.Click += new System.EventHandler(this.btDestLocal_Click);
            // 
            // btRegisterLocal
            // 
            this.btRegisterLocal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btRegisterLocal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btRegisterLocal.Location = new System.Drawing.Point(104, 126);
            this.btRegisterLocal.Name = "btRegisterLocal";
            this.btRegisterLocal.Size = new System.Drawing.Size(175, 23);
            this.btRegisterLocal.TabIndex = 3;
            this.btRegisterLocal.Text = "Register On Local Computer";
            this.btRegisterLocal.UseVisualStyleBackColor = true;
            this.btRegisterLocal.Click += new System.EventHandler(this.btRegisterLocal_Click);
            // 
            // txtMessageBox
            // 
            this.txtMessageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessageBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMessageBox.Location = new System.Drawing.Point(0, 0);
            this.txtMessageBox.Name = "txtMessageBox";
            this.txtMessageBox.Size = new System.Drawing.Size(687, 301);
            this.txtMessageBox.TabIndex = 1;
            this.txtMessageBox.TabStop = false;
            this.txtMessageBox.Text = "";
            // 
            // chkCOMRunInParalel
            // 
            this.chkCOMRunInParalel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkCOMRunInParalel.AutoSize = true;
            this.chkCOMRunInParalel.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.chkCOMRunInParalel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkCOMRunInParalel.Location = new System.Drawing.Point(267, 87);
            this.chkCOMRunInParalel.Name = "chkCOMRunInParalel";
            this.chkCOMRunInParalel.Size = new System.Drawing.Size(93, 17);
            this.chkCOMRunInParalel.TabIndex = 37;
            this.chkCOMRunInParalel.Text = "Run in parallel";
            this.chkCOMRunInParalel.UseVisualStyleBackColor = true;
            // 
            // COMRegistrationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitMain);
            this.Name = "COMRegistrationControl";
            this.Size = new System.Drawing.Size(687, 525);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel1.PerformLayout();
            this.splitMain.Panel2.ResumeLayout(false);
            this.splitMain.ResumeLayout(false);
            this.grpRemote.ResumeLayout(false);
            this.grpRemote.PerformLayout();
            this.pnlRemote.ResumeLayout(false);
            this.pnlRemote.PerformLayout();
            this.grpLocal.ResumeLayout(false);
            this.grpLocal.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btRegisterDLLs;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.GroupBox grpRemote;
        private System.Windows.Forms.Panel pnlRemote;
        private System.Windows.Forms.Button btRegisterRemote;
        private System.Windows.Forms.Button btDestRemoteHelp;
        private System.Windows.Forms.Label lbDestRemote;
        private System.Windows.Forms.GroupBox grpLocal;
        private System.Windows.Forms.Label lbDestFolder;
        private System.Windows.Forms.TextBox txtDestLocal;
        private System.Windows.Forms.Button btDestLocal;
        private System.Windows.Forms.Button btRegisterLocal;
        private System.Windows.Forms.Button btExcludeHelp;
        private System.Windows.Forms.TextBox txtExclude;
        private System.Windows.Forms.Label lbExclude;
        private System.Windows.Forms.Label lbSrc;
        private System.Windows.Forms.TextBox txtSrc;
        private System.Windows.Forms.Button btSrc;
        private System.Windows.Forms.FolderBrowserDialog dlgCOMSourceFolder;
        private System.Windows.Forms.FolderBrowserDialog dlgCOMDestinationFolder;
        public System.Windows.Forms.RichTextBox txtMessageBox;
        private System.Windows.Forms.CheckBox chkCOMRunInParalel;
    }
}
