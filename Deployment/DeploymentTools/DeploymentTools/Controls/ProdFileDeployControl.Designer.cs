namespace DeploymentTools.Controls
{
    partial class ProdFileDeployControl
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
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.btProdDeployCancel = new System.Windows.Forms.Button();
            this.chkProdRunInParalel = new System.Windows.Forms.CheckBox();
            this.btProdDeployPreview = new System.Windows.Forms.Button();
            this.chkProdPutIntoTeam = new System.Windows.Forms.CheckBox();
            this.chkProdRestartServer = new System.Windows.Forms.CheckBox();
            this.chkProdCopyFiles = new System.Windows.Forms.CheckBox();
            this.chkProdTakeOutTeam = new System.Windows.Forms.CheckBox();
            this.btProdDeploy = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.btHelpProdDeploy = new System.Windows.Forms.Button();
            this.lbProdWeb = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.txtProdSrc = new System.Windows.Forms.TextBox();
            this.btProdDeploySrc = new System.Windows.Forms.Button();
            this.txtMessageBox = new System.Windows.Forms.RichTextBox();
            this.dlgProdDeploySrc = new System.Windows.Forms.FolderBrowserDialog();
            this.chkProdResetIIS = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
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
            this.splitMain.Panel1.Controls.Add(this.chkProdResetIIS);
            this.splitMain.Panel1.Controls.Add(this.btProdDeployCancel);
            this.splitMain.Panel1.Controls.Add(this.chkProdRunInParalel);
            this.splitMain.Panel1.Controls.Add(this.btProdDeployPreview);
            this.splitMain.Panel1.Controls.Add(this.chkProdPutIntoTeam);
            this.splitMain.Panel1.Controls.Add(this.chkProdRestartServer);
            this.splitMain.Panel1.Controls.Add(this.chkProdCopyFiles);
            this.splitMain.Panel1.Controls.Add(this.chkProdTakeOutTeam);
            this.splitMain.Panel1.Controls.Add(this.btProdDeploy);
            this.splitMain.Panel1.Controls.Add(this.groupBox6);
            this.splitMain.Panel1.Controls.Add(this.label13);
            this.splitMain.Panel1.Controls.Add(this.txtProdSrc);
            this.splitMain.Panel1.Controls.Add(this.btProdDeploySrc);
            this.splitMain.Panel1MinSize = 206;
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.txtMessageBox);
            this.splitMain.Size = new System.Drawing.Size(666, 538);
            this.splitMain.SplitterDistance = 206;
            this.splitMain.TabIndex = 1;
            // 
            // btProdDeployCancel
            // 
            this.btProdDeployCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btProdDeployCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btProdDeployCancel.Location = new System.Drawing.Point(6, 178);
            this.btProdDeployCancel.Name = "btProdDeployCancel";
            this.btProdDeployCancel.Size = new System.Drawing.Size(70, 25);
            this.btProdDeployCancel.TabIndex = 37;
            this.btProdDeployCancel.Text = "Cancel";
            this.btProdDeployCancel.UseVisualStyleBackColor = true;
            this.btProdDeployCancel.Click += new System.EventHandler(this.btProdDeployCancel_Click);
            // 
            // chkProdRunInParalel
            // 
            this.chkProdRunInParalel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkProdRunInParalel.AutoSize = true;
            this.chkProdRunInParalel.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.chkProdRunInParalel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkProdRunInParalel.Location = new System.Drawing.Point(546, 154);
            this.chkProdRunInParalel.Name = "chkProdRunInParalel";
            this.chkProdRunInParalel.Size = new System.Drawing.Size(93, 17);
            this.chkProdRunInParalel.TabIndex = 36;
            this.chkProdRunInParalel.Text = "Run in parallel";
            this.chkProdRunInParalel.UseVisualStyleBackColor = true;
            this.chkProdRunInParalel.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // btProdDeployPreview
            // 
            this.btProdDeployPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btProdDeployPreview.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btProdDeployPreview.Location = new System.Drawing.Point(507, 179);
            this.btProdDeployPreview.Name = "btProdDeployPreview";
            this.btProdDeployPreview.Size = new System.Drawing.Size(69, 23);
            this.btProdDeployPreview.TabIndex = 35;
            this.btProdDeployPreview.Text = "Preview";
            this.btProdDeployPreview.UseVisualStyleBackColor = true;
            this.btProdDeployPreview.Click += new System.EventHandler(this.btProdDeployPreview_Click);
            // 
            // chkProdPutIntoTeam
            // 
            this.chkProdPutIntoTeam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkProdPutIntoTeam.AutoSize = true;
            this.chkProdPutIntoTeam.Location = new System.Drawing.Point(547, 114);
            this.chkProdPutIntoTeam.Name = "chkProdPutIntoTeam";
            this.chkProdPutIntoTeam.Size = new System.Drawing.Size(115, 17);
            this.chkProdPutIntoTeam.TabIndex = 34;
            this.chkProdPutIntoTeam.Text = "Put back into team";
            this.chkProdPutIntoTeam.UseVisualStyleBackColor = true;
            // 
            // chkProdRestartServer
            // 
            this.chkProdRestartServer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkProdRestartServer.AutoSize = true;
            this.chkProdRestartServer.Location = new System.Drawing.Point(547, 94);
            this.chkProdRestartServer.Name = "chkProdRestartServer";
            this.chkProdRestartServer.Size = new System.Drawing.Size(92, 17);
            this.chkProdRestartServer.TabIndex = 33;
            this.chkProdRestartServer.Text = "Restart server";
            this.chkProdRestartServer.UseVisualStyleBackColor = true;
            // 
            // chkProdCopyFiles
            // 
            this.chkProdCopyFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkProdCopyFiles.AutoSize = true;
            this.chkProdCopyFiles.Location = new System.Drawing.Point(547, 56);
            this.chkProdCopyFiles.Name = "chkProdCopyFiles";
            this.chkProdCopyFiles.Size = new System.Drawing.Size(71, 17);
            this.chkProdCopyFiles.TabIndex = 32;
            this.chkProdCopyFiles.Text = "Copy files";
            this.chkProdCopyFiles.UseVisualStyleBackColor = true;
            // 
            // chkProdTakeOutTeam
            // 
            this.chkProdTakeOutTeam.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkProdTakeOutTeam.AutoSize = true;
            this.chkProdTakeOutTeam.Location = new System.Drawing.Point(547, 37);
            this.chkProdTakeOutTeam.Name = "chkProdTakeOutTeam";
            this.chkProdTakeOutTeam.Size = new System.Drawing.Size(107, 17);
            this.chkProdTakeOutTeam.TabIndex = 31;
            this.chkProdTakeOutTeam.Text = "Take out of team";
            this.chkProdTakeOutTeam.UseVisualStyleBackColor = true;
            // 
            // btProdDeploy
            // 
            this.btProdDeploy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btProdDeploy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btProdDeploy.Location = new System.Drawing.Point(582, 177);
            this.btProdDeploy.Name = "btProdDeploy";
            this.btProdDeploy.Size = new System.Drawing.Size(72, 25);
            this.btProdDeploy.TabIndex = 30;
            this.btProdDeploy.Text = "Deploy";
            this.btProdDeploy.UseVisualStyleBackColor = true;
            this.btProdDeploy.Click += new System.EventHandler(this.btProdDeploy_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox6.Controls.Add(this.btHelpProdDeploy);
            this.groupBox6.Controls.Add(this.lbProdWeb);
            this.groupBox6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox6.Location = new System.Drawing.Point(6, 37);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(534, 137);
            this.groupBox6.TabIndex = 29;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Destination Servers";
            // 
            // btHelpProdDeploy
            // 
            this.btHelpProdDeploy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btHelpProdDeploy.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btHelpProdDeploy.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btHelpProdDeploy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btHelpProdDeploy.ForeColor = System.Drawing.Color.Silver;
            this.btHelpProdDeploy.Location = new System.Drawing.Point(499, 19);
            this.btHelpProdDeploy.Name = "btHelpProdDeploy";
            this.btHelpProdDeploy.Size = new System.Drawing.Size(29, 21);
            this.btHelpProdDeploy.TabIndex = 20;
            this.btHelpProdDeploy.TabStop = false;
            this.btHelpProdDeploy.Text = "?";
            this.btHelpProdDeploy.UseVisualStyleBackColor = true;
            this.btHelpProdDeploy.Click += new System.EventHandler(this.btHelpProdDeploy_Click);
            // 
            // lbProdWeb
            // 
            this.lbProdWeb.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbProdWeb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbProdWeb.Location = new System.Drawing.Point(6, 19);
            this.lbProdWeb.Name = "lbProdWeb";
            this.lbProdWeb.Size = new System.Drawing.Size(487, 115);
            this.lbProdWeb.TabIndex = 0;
            this.lbProdWeb.Text = "web1, \\\\10.0.5.10\\e$\\Migration";
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
            // txtProdSrc
            // 
            this.txtProdSrc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtProdSrc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtProdSrc.Location = new System.Drawing.Point(82, 8);
            this.txtProdSrc.Name = "txtProdSrc";
            this.txtProdSrc.Size = new System.Drawing.Size(537, 20);
            this.txtProdSrc.TabIndex = 27;
            // 
            // btProdDeploySrc
            // 
            this.btProdDeploySrc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btProdDeploySrc.Location = new System.Drawing.Point(625, 8);
            this.btProdDeploySrc.Name = "btProdDeploySrc";
            this.btProdDeploySrc.Size = new System.Drawing.Size(29, 23);
            this.btProdDeploySrc.TabIndex = 28;
            this.btProdDeploySrc.TabStop = false;
            this.btProdDeploySrc.Text = "...";
            this.btProdDeploySrc.UseVisualStyleBackColor = true;
            this.btProdDeploySrc.Click += new System.EventHandler(this.btProdDeploySrc_Click);
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
            // chkProdResetIIS
            // 
            this.chkProdResetIIS.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkProdResetIIS.AutoSize = true;
            this.chkProdResetIIS.Location = new System.Drawing.Point(547, 75);
            this.chkProdResetIIS.Name = "chkProdResetIIS";
            this.chkProdResetIIS.Size = new System.Drawing.Size(70, 17);
            this.chkProdResetIIS.TabIndex = 38;
            this.chkProdResetIIS.Text = "Reset IIS";
            this.chkProdResetIIS.UseVisualStyleBackColor = true;
            // 
            // ProdFileDeployControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitMain);
            this.Name = "ProdFileDeployControl";
            this.Size = new System.Drawing.Size(666, 538);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel1.PerformLayout();
            this.splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.Button btProdDeployPreview;
        private System.Windows.Forms.CheckBox chkProdPutIntoTeam;
        private System.Windows.Forms.CheckBox chkProdRestartServer;
        private System.Windows.Forms.CheckBox chkProdCopyFiles;
        private System.Windows.Forms.CheckBox chkProdTakeOutTeam;
        private System.Windows.Forms.Button btProdDeploy;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Button btHelpProdDeploy;
        private System.Windows.Forms.Label lbProdWeb;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtProdSrc;
        private System.Windows.Forms.Button btProdDeploySrc;
        private System.Windows.Forms.FolderBrowserDialog dlgProdDeploySrc;
        public System.Windows.Forms.RichTextBox txtMessageBox;
        private System.Windows.Forms.CheckBox chkProdRunInParalel;
        private System.Windows.Forms.Button btProdDeployCancel;
        private System.Windows.Forms.CheckBox chkProdResetIIS;
    }
}
