
namespace DeploymentTools
{
    partial class DeployTools
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbCurrentLoadedConfig = new System.Windows.Forms.Label();
            this.lbCurConfigLabel = new System.Windows.Forms.Label();
            this.lbSrc = new System.Windows.Forms.Label();
            this.txtConfigFile = new System.Windows.Forms.TextBox();
            this.btConfigSrc = new System.Windows.Forms.Button();
            this.btEditConfig = new System.Windows.Forms.Button();
            this.btSaveConfig = new System.Windows.Forms.Button();
            this.btLoadConfig = new System.Windows.Forms.Button();
            this.btCleanMessageBox = new System.Windows.Forms.Button();
            this.dlgConfigFile = new System.Windows.Forms.OpenFileDialog();
            this.tabSyncFolders = new System.Windows.Forms.TabPage();
            this.foldersSyncControl1 = new DeploymentTools.Controls.FoldersSyncControl();
            this.tabProdDeploy = new System.Windows.Forms.TabPage();
            this.prodFileDeployControl1 = new DeploymentTools.Controls.ProdFileDeployControl();
            this.tabSQLScripts = new System.Windows.Forms.TabPage();
            this.sqlScriptsControl1 = new DeploymentTools.Controls.SQLScriptsControl();
            this.tabDeploySelector = new System.Windows.Forms.TabControl();
            this.groupBox1.SuspendLayout();
            this.tabSyncFolders.SuspendLayout();
            this.tabProdDeploy.SuspendLayout();
            this.tabSQLScripts.SuspendLayout();
            this.tabDeploySelector.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.lbCurrentLoadedConfig);
            this.groupBox1.Controls.Add(this.lbCurConfigLabel);
            this.groupBox1.Controls.Add(this.lbSrc);
            this.groupBox1.Controls.Add(this.txtConfigFile);
            this.groupBox1.Controls.Add(this.btConfigSrc);
            this.groupBox1.Controls.Add(this.btEditConfig);
            this.groupBox1.Controls.Add(this.btSaveConfig);
            this.groupBox1.Controls.Add(this.btLoadConfig);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(97, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(706, 88);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Deployment Tools Configuration";
            // 
            // lbCurrentLoadedConfig
            // 
            this.lbCurrentLoadedConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lbCurrentLoadedConfig.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCurrentLoadedConfig.Location = new System.Drawing.Point(11, 69);
            this.lbCurrentLoadedConfig.Name = "lbCurrentLoadedConfig";
            this.lbCurrentLoadedConfig.Size = new System.Drawing.Size(689, 13);
            this.lbCurrentLoadedConfig.TabIndex = 32;
            this.lbCurrentLoadedConfig.Text = "label1";
            // 
            // lbCurConfigLabel
            // 
            this.lbCurConfigLabel.AutoSize = true;
            this.lbCurConfigLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCurConfigLabel.Location = new System.Drawing.Point(8, 49);
            this.lbCurConfigLabel.Name = "lbCurConfigLabel";
            this.lbCurConfigLabel.Size = new System.Drawing.Size(134, 13);
            this.lbCurConfigLabel.TabIndex = 31;
            this.lbCurConfigLabel.Text = "Currently loaded config file:";
            // 
            // lbSrc
            // 
            this.lbSrc.AutoSize = true;
            this.lbSrc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSrc.Location = new System.Drawing.Point(8, 19);
            this.lbSrc.Name = "lbSrc";
            this.lbSrc.Size = new System.Drawing.Size(56, 13);
            this.lbSrc.TabIndex = 28;
            this.lbSrc.Text = "Config File";
            // 
            // txtConfigFile
            // 
            this.txtConfigFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConfigFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConfigFile.Location = new System.Drawing.Point(70, 16);
            this.txtConfigFile.Name = "txtConfigFile";
            this.txtConfigFile.Size = new System.Drawing.Size(598, 20);
            this.txtConfigFile.TabIndex = 29;
            // 
            // btConfigSrc
            // 
            this.btConfigSrc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btConfigSrc.Location = new System.Drawing.Point(674, 14);
            this.btConfigSrc.Name = "btConfigSrc";
            this.btConfigSrc.Size = new System.Drawing.Size(26, 23);
            this.btConfigSrc.TabIndex = 30;
            this.btConfigSrc.TabStop = false;
            this.btConfigSrc.Text = "...";
            this.btConfigSrc.UseVisualStyleBackColor = true;
            this.btConfigSrc.Click += new System.EventHandler(this.btConfigSrc_Click);
            // 
            // btEditConfig
            // 
            this.btEditConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btEditConfig.Location = new System.Drawing.Point(450, 39);
            this.btEditConfig.Name = "btEditConfig";
            this.btEditConfig.Size = new System.Drawing.Size(96, 23);
            this.btEditConfig.TabIndex = 23;
            this.btEditConfig.Text = "Edit Config File";
            this.btEditConfig.UseVisualStyleBackColor = true;
            this.btEditConfig.Click += new System.EventHandler(this.btEditConfig_Click);
            // 
            // btSaveConfig
            // 
            this.btSaveConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSaveConfig.Location = new System.Drawing.Point(629, 39);
            this.btSaveConfig.Name = "btSaveConfig";
            this.btSaveConfig.Size = new System.Drawing.Size(71, 23);
            this.btSaveConfig.TabIndex = 22;
            this.btSaveConfig.Text = "Save";
            this.btSaveConfig.UseVisualStyleBackColor = true;
            this.btSaveConfig.Click += new System.EventHandler(this.btSaveConfig_Click);
            // 
            // btLoadConfig
            // 
            this.btLoadConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btLoadConfig.Location = new System.Drawing.Point(552, 39);
            this.btLoadConfig.Name = "btLoadConfig";
            this.btLoadConfig.Size = new System.Drawing.Size(71, 23);
            this.btLoadConfig.TabIndex = 21;
            this.btLoadConfig.Text = "Load ";
            this.btLoadConfig.UseVisualStyleBackColor = true;
            this.btLoadConfig.Click += new System.EventHandler(this.btLoadConfig_Click);
            // 
            // btCleanMessageBox
            // 
            this.btCleanMessageBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btCleanMessageBox.Location = new System.Drawing.Point(12, 38);
            this.btCleanMessageBox.Name = "btCleanMessageBox";
            this.btCleanMessageBox.Size = new System.Drawing.Size(79, 49);
            this.btCleanMessageBox.TabIndex = 0;
            this.btCleanMessageBox.Text = "Clean Message Box";
            this.btCleanMessageBox.UseVisualStyleBackColor = true;
            this.btCleanMessageBox.Click += new System.EventHandler(this.btCleanMessageBox_Click);
            // 
            // tabSyncFolders
            // 
            this.tabSyncFolders.Controls.Add(this.foldersSyncControl1);
            this.tabSyncFolders.Location = new System.Drawing.Point(4, 22);
            this.tabSyncFolders.Name = "tabSyncFolders";
            this.tabSyncFolders.Size = new System.Drawing.Size(790, 400);
            this.tabSyncFolders.TabIndex = 3;
            this.tabSyncFolders.Text = "Syncronize Folders";
            this.tabSyncFolders.UseVisualStyleBackColor = true;
            // 
            // foldersSyncControl1
            // 
            this.foldersSyncControl1.configManager = null;
            this.foldersSyncControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.foldersSyncControl1.Location = new System.Drawing.Point(0, 0);
            this.foldersSyncControl1.Name = "foldersSyncControl1";
            this.foldersSyncControl1.Size = new System.Drawing.Size(790, 400);
            this.foldersSyncControl1.TabIndex = 0;
            // 
            // tabProdDeploy
            // 
            this.tabProdDeploy.Controls.Add(this.prodFileDeployControl1);
            this.tabProdDeploy.Location = new System.Drawing.Point(4, 22);
            this.tabProdDeploy.Name = "tabProdDeploy";
            this.tabProdDeploy.Size = new System.Drawing.Size(790, 400);
            this.tabProdDeploy.TabIndex = 2;
            this.tabProdDeploy.Text = "Production Files Deploy";
            this.tabProdDeploy.UseVisualStyleBackColor = true;
            // 
            // prodFileDeployControl1
            // 
            this.prodFileDeployControl1.configManager = null;
            this.prodFileDeployControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.prodFileDeployControl1.Location = new System.Drawing.Point(0, 0);
            this.prodFileDeployControl1.Name = "prodFileDeployControl1";
            this.prodFileDeployControl1.Size = new System.Drawing.Size(790, 400);
            this.prodFileDeployControl1.TabIndex = 0;
            // 
            // tabSQLScripts
            // 
            this.tabSQLScripts.Controls.Add(this.sqlScriptsControl1);
            this.tabSQLScripts.Location = new System.Drawing.Point(4, 22);
            this.tabSQLScripts.Name = "tabSQLScripts";
            this.tabSQLScripts.Padding = new System.Windows.Forms.Padding(3);
            this.tabSQLScripts.Size = new System.Drawing.Size(790, 400);
            this.tabSQLScripts.TabIndex = 1;
            this.tabSQLScripts.Text = "SQL Scripts";
            this.tabSQLScripts.UseVisualStyleBackColor = true;
            // 
            // sqlScriptsControl1
            // 
            this.sqlScriptsControl1.configManager = null;
            this.sqlScriptsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.sqlScriptsControl1.Location = new System.Drawing.Point(3, 3);
            this.sqlScriptsControl1.Name = "sqlScriptsControl1";
            this.sqlScriptsControl1.Size = new System.Drawing.Size(784, 394);
            this.sqlScriptsControl1.TabIndex = 0;
            this.sqlScriptsControl1.Load += new System.EventHandler(this.sqlScriptsControl1_Load);
            // 
            // tabDeploySelector
            // 
            this.tabDeploySelector.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabDeploySelector.Controls.Add(this.tabSQLScripts);
            this.tabDeploySelector.Controls.Add(this.tabProdDeploy);
            this.tabDeploySelector.Controls.Add(this.tabSyncFolders);
            this.tabDeploySelector.Location = new System.Drawing.Point(12, 111);
            this.tabDeploySelector.Name = "tabDeploySelector";
            this.tabDeploySelector.SelectedIndex = 0;
            this.tabDeploySelector.Size = new System.Drawing.Size(798, 426);
            this.tabDeploySelector.TabIndex = 0;
            // 
            // DeployTools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 549);
            this.Controls.Add(this.btCleanMessageBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.tabDeploySelector);
            this.Name = "DeployTools";
            this.Text = "DeploymentTools";
            this.Load += new System.EventHandler(this.DeployTools_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabSyncFolders.ResumeLayout(false);
            this.tabProdDeploy.ResumeLayout(false);
            this.tabSQLScripts.ResumeLayout(false);
            this.tabDeploySelector.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btEditConfig;
        private System.Windows.Forms.Button btSaveConfig;
        private System.Windows.Forms.Button btLoadConfig;
        private System.Windows.Forms.Button btCleanMessageBox;
        private System.Windows.Forms.Label lbSrc;
        private System.Windows.Forms.TextBox txtConfigFile;
        private System.Windows.Forms.Button btConfigSrc;
        private System.Windows.Forms.OpenFileDialog dlgConfigFile;
        private System.Windows.Forms.Label lbCurrentLoadedConfig;
        private System.Windows.Forms.Label lbCurConfigLabel;
        private System.Windows.Forms.TabPage tabSyncFolders;
        private DeploymentTools.Controls.FoldersSyncControl foldersSyncControl1;
        private System.Windows.Forms.TabPage tabProdDeploy;
        private DeploymentTools.Controls.ProdFileDeployControl prodFileDeployControl1;
        private System.Windows.Forms.TabPage tabSQLScripts;
        private DeploymentTools.Controls.SQLScriptsControl sqlScriptsControl1;
        private System.Windows.Forms.TabControl tabDeploySelector;
    }
}