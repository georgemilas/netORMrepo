namespace DeploymentTools.Controls
{
    partial class ServiceDeployControl
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
            System.Windows.Forms.Button btHelpServiceDeploy;
            this.lbServiceServers = new System.Windows.Forms.Label();
            this.txtServiceSrc = new System.Windows.Forms.TextBox();
            this.btServiceDeployPreview = new System.Windows.Forms.Button();
            this.btServiceDeploy = new System.Windows.Forms.Button();
            this.chkServiceCopyFiles = new System.Windows.Forms.CheckBox();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.btServiceFileExcludeHelp = new System.Windows.Forms.Button();
            this.txtServiceFileExclude = new System.Windows.Forms.TextBox();
            this.lbExclude = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtServiceName = new System.Windows.Forms.TextBox();
            this.chkStartService = new System.Windows.Forms.CheckBox();
            this.chkStopService = new System.Windows.Forms.CheckBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label13 = new System.Windows.Forms.Label();
            this.btServiceDeploySrc = new System.Windows.Forms.Button();
            this.txtMessageBox = new System.Windows.Forms.RichTextBox();
            this.dlgServiceDeploySrc = new System.Windows.Forms.FolderBrowserDialog();
            this.chkServiceRunInParalel = new System.Windows.Forms.CheckBox();
            btHelpServiceDeploy = new System.Windows.Forms.Button();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // btHelpServiceDeploy
            // 
            btHelpServiceDeploy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            btHelpServiceDeploy.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            btHelpServiceDeploy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btHelpServiceDeploy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            btHelpServiceDeploy.ForeColor = System.Drawing.Color.Silver;
            btHelpServiceDeploy.Location = new System.Drawing.Point(518, 11);
            btHelpServiceDeploy.Name = "btHelpServiceDeploy";
            btHelpServiceDeploy.Size = new System.Drawing.Size(29, 21);
            btHelpServiceDeploy.TabIndex = 20;
            btHelpServiceDeploy.TabStop = false;
            btHelpServiceDeploy.Text = "?";
            btHelpServiceDeploy.UseVisualStyleBackColor = true;
            btHelpServiceDeploy.Click += new System.EventHandler(this.btHelpServiceDeploy_Click);
            // 
            // lbServiceServers
            // 
            this.lbServiceServers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbServiceServers.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbServiceServers.Location = new System.Drawing.Point(6, 16);
            this.lbServiceServers.Name = "lbServiceServers";
            this.lbServiceServers.Size = new System.Drawing.Size(506, 90);
            this.lbServiceServers.TabIndex = 0;
            this.lbServiceServers.Text = "web1, \\\\10.0.5.10\\e$\\Migration";
            // 
            // txtServiceSrc
            // 
            this.txtServiceSrc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServiceSrc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtServiceSrc.Location = new System.Drawing.Point(82, 8);
            this.txtServiceSrc.Name = "txtServiceSrc";
            this.txtServiceSrc.Size = new System.Drawing.Size(537, 20);
            this.txtServiceSrc.TabIndex = 27;
            // 
            // btServiceDeployPreview
            // 
            this.btServiceDeployPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btServiceDeployPreview.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btServiceDeployPreview.Location = new System.Drawing.Point(566, 151);
            this.btServiceDeployPreview.Name = "btServiceDeployPreview";
            this.btServiceDeployPreview.Size = new System.Drawing.Size(88, 23);
            this.btServiceDeployPreview.TabIndex = 35;
            this.btServiceDeployPreview.Text = "Preview";
            this.btServiceDeployPreview.UseVisualStyleBackColor = true;
            this.btServiceDeployPreview.Click += new System.EventHandler(this.btServiceDeployPreview_Click);
            // 
            // btServiceDeploy
            // 
            this.btServiceDeploy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btServiceDeploy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btServiceDeploy.Location = new System.Drawing.Point(565, 180);
            this.btServiceDeploy.Name = "btServiceDeploy";
            this.btServiceDeploy.Size = new System.Drawing.Size(89, 25);
            this.btServiceDeploy.TabIndex = 30;
            this.btServiceDeploy.Text = "Deploy";
            this.btServiceDeploy.UseVisualStyleBackColor = true;
            this.btServiceDeploy.Click += new System.EventHandler(this.btServiceDeploy_Click);
            // 
            // chkServiceCopyFiles
            // 
            this.chkServiceCopyFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkServiceCopyFiles.AutoSize = true;
            this.chkServiceCopyFiles.Location = new System.Drawing.Point(567, 83);
            this.chkServiceCopyFiles.Name = "chkServiceCopyFiles";
            this.chkServiceCopyFiles.Size = new System.Drawing.Size(71, 17);
            this.chkServiceCopyFiles.TabIndex = 32;
            this.chkServiceCopyFiles.Text = "Copy files";
            this.chkServiceCopyFiles.UseVisualStyleBackColor = true;
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
            this.splitMain.Panel1.Controls.Add(this.chkServiceRunInParalel);
            this.splitMain.Panel1.Controls.Add(this.btServiceFileExcludeHelp);
            this.splitMain.Panel1.Controls.Add(this.txtServiceFileExclude);
            this.splitMain.Panel1.Controls.Add(this.lbExclude);
            this.splitMain.Panel1.Controls.Add(this.label1);
            this.splitMain.Panel1.Controls.Add(this.txtServiceName);
            this.splitMain.Panel1.Controls.Add(this.btServiceDeployPreview);
            this.splitMain.Panel1.Controls.Add(this.chkStartService);
            this.splitMain.Panel1.Controls.Add(this.chkServiceCopyFiles);
            this.splitMain.Panel1.Controls.Add(this.chkStopService);
            this.splitMain.Panel1.Controls.Add(this.btServiceDeploy);
            this.splitMain.Panel1.Controls.Add(this.groupBox6);
            this.splitMain.Panel1.Controls.Add(this.label13);
            this.splitMain.Panel1.Controls.Add(this.txtServiceSrc);
            this.splitMain.Panel1.Controls.Add(this.btServiceDeploySrc);
            this.splitMain.Panel1MinSize = 206;
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.txtMessageBox);
            this.splitMain.Size = new System.Drawing.Size(666, 538);
            this.splitMain.SplitterDistance = 206;
            this.splitMain.TabIndex = 1;
            // 
            // btServiceFileExcludeHelp
            // 
            this.btServiceFileExcludeHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btServiceFileExcludeHelp.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btServiceFileExcludeHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btServiceFileExcludeHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btServiceFileExcludeHelp.ForeColor = System.Drawing.Color.Silver;
            this.btServiceFileExcludeHelp.Location = new System.Drawing.Point(625, 33);
            this.btServiceFileExcludeHelp.Name = "btServiceFileExcludeHelp";
            this.btServiceFileExcludeHelp.Size = new System.Drawing.Size(29, 21);
            this.btServiceFileExcludeHelp.TabIndex = 40;
            this.btServiceFileExcludeHelp.TabStop = false;
            this.btServiceFileExcludeHelp.Text = "?";
            this.btServiceFileExcludeHelp.UseVisualStyleBackColor = true;
            this.btServiceFileExcludeHelp.Click += new System.EventHandler(this.btServiceFileExcludeHelp_Click);
            // 
            // txtServiceFileExclude
            // 
            this.txtServiceFileExclude.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServiceFileExclude.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtServiceFileExclude.Location = new System.Drawing.Point(82, 34);
            this.txtServiceFileExclude.Name = "txtServiceFileExclude";
            this.txtServiceFileExclude.Size = new System.Drawing.Size(537, 20);
            this.txtServiceFileExclude.TabIndex = 38;
            // 
            // lbExclude
            // 
            this.lbExclude.AutoSize = true;
            this.lbExclude.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbExclude.Location = new System.Drawing.Point(3, 37);
            this.lbExclude.Name = "lbExclude";
            this.lbExclude.Size = new System.Drawing.Size(45, 13);
            this.lbExclude.TabIndex = 39;
            this.lbExclude.Text = "Exclude";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 36;
            this.label1.Text = "Service Name";
            // 
            // txtServiceName
            // 
            this.txtServiceName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServiceName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtServiceName.Location = new System.Drawing.Point(82, 60);
            this.txtServiceName.Name = "txtServiceName";
            this.txtServiceName.Size = new System.Drawing.Size(477, 20);
            this.txtServiceName.TabIndex = 37;
            // 
            // chkStartService
            // 
            this.chkStartService.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkStartService.AutoSize = true;
            this.chkStartService.Location = new System.Drawing.Point(567, 106);
            this.chkStartService.Name = "chkStartService";
            this.chkStartService.Size = new System.Drawing.Size(87, 17);
            this.chkStartService.TabIndex = 33;
            this.chkStartService.Text = "Start Service";
            this.chkStartService.UseVisualStyleBackColor = true;
            // 
            // chkStopService
            // 
            this.chkStopService.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkStopService.AutoSize = true;
            this.chkStopService.Location = new System.Drawing.Point(567, 60);
            this.chkStopService.Name = "chkStopService";
            this.chkStopService.Size = new System.Drawing.Size(87, 17);
            this.chkStopService.TabIndex = 31;
            this.chkStopService.Text = "Stop Service";
            this.chkStopService.UseVisualStyleBackColor = true;
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox6.Controls.Add(btHelpServiceDeploy);
            this.groupBox6.Controls.Add(this.lbServiceServers);
            this.groupBox6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox6.Location = new System.Drawing.Point(6, 86);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(553, 118);
            this.groupBox6.TabIndex = 29;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Destination Servers";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(3, 11);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(73, 13);
            this.label13.TabIndex = 26;
            this.label13.Text = "Source Folder";
            // 
            // btServiceDeploySrc
            // 
            this.btServiceDeploySrc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btServiceDeploySrc.Location = new System.Drawing.Point(625, 8);
            this.btServiceDeploySrc.Name = "btServiceDeploySrc";
            this.btServiceDeploySrc.Size = new System.Drawing.Size(29, 23);
            this.btServiceDeploySrc.TabIndex = 28;
            this.btServiceDeploySrc.TabStop = false;
            this.btServiceDeploySrc.Text = "...";
            this.btServiceDeploySrc.UseVisualStyleBackColor = true;
            this.btServiceDeploySrc.Click += new System.EventHandler(this.btServiceDeploySrc_Click);
            // 
            // txtMessageBox
            // 
            this.txtMessageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessageBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMessageBox.Location = new System.Drawing.Point(0, 0);
            this.txtMessageBox.Name = "txtMessageBox";
            this.txtMessageBox.Size = new System.Drawing.Size(666, 328);
            this.txtMessageBox.TabIndex = 1;
            this.txtMessageBox.TabStop = false;
            this.txtMessageBox.Text = "";
            // 
            // chkServiceRunInParalel
            // 
            this.chkServiceRunInParalel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkServiceRunInParalel.AutoSize = true;
            this.chkServiceRunInParalel.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.chkServiceRunInParalel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkServiceRunInParalel.Location = new System.Drawing.Point(567, 128);
            this.chkServiceRunInParalel.Name = "chkServiceRunInParalel";
            this.chkServiceRunInParalel.Size = new System.Drawing.Size(93, 17);
            this.chkServiceRunInParalel.TabIndex = 41;
            this.chkServiceRunInParalel.Text = "Run in parallel";
            this.chkServiceRunInParalel.UseVisualStyleBackColor = true;
            // 
            // ServiceDeployControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitMain);
            this.Name = "ServiceDeployControl";
            this.Size = new System.Drawing.Size(666, 538);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel1.PerformLayout();
            this.splitMain.Panel2.ResumeLayout(false);
            this.splitMain.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.CheckBox chkStartService;
        private System.Windows.Forms.CheckBox chkStopService;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button btServiceDeploySrc;
        private System.Windows.Forms.FolderBrowserDialog dlgServiceDeploySrc;
        public System.Windows.Forms.RichTextBox txtMessageBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtServiceName;
        private System.Windows.Forms.CheckBox chkServiceCopyFiles;
        private System.Windows.Forms.Button btServiceDeploy;
        private System.Windows.Forms.Button btServiceDeployPreview;
        private System.Windows.Forms.TextBox txtServiceSrc;
        private System.Windows.Forms.Label lbServiceServers;
        private System.Windows.Forms.Button btServiceFileExcludeHelp;
        private System.Windows.Forms.TextBox txtServiceFileExclude;
        private System.Windows.Forms.Label lbExclude;
        private System.Windows.Forms.CheckBox chkServiceRunInParalel;
    }
}
