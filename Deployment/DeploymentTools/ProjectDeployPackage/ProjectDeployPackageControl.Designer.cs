namespace DeploymentTools.Controls
{
    partial class ProjectDeployPackageControl
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btGenerateReleaseScript = new System.Windows.Forms.Button();
            this.lbReleaseScriptsFolder = new System.Windows.Forms.Label();
            this.btReleaseScriptsFolder = new System.Windows.Forms.Button();
            this.lbReleaseScriptName = new System.Windows.Forms.Label();
            this.txtReleaseScriptsFolder = new System.Windows.Forms.TextBox();
            this.txtReleaseScriptName = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lbDeployScript = new System.Windows.Forms.Label();
            this.btDeployScript = new System.Windows.Forms.Button();
            this.btGenerateDeployPkg = new System.Windows.Forms.Button();
            this.btDeployPkgPreview = new System.Windows.Forms.Button();
            this.txtDeployScript = new System.Windows.Forms.TextBox();
            this.lbSchemaFolder = new System.Windows.Forms.Label();
            this.txtSchemaFolder = new System.Windows.Forms.TextBox();
            this.btDataFolder = new System.Windows.Forms.Button();
            this.txtDataFolder = new System.Windows.Forms.TextBox();
            this.lbDestinationFolder = new System.Windows.Forms.Label();
            this.lbDataFolder = new System.Windows.Forms.Label();
            this.txtDestinationFolder = new System.Windows.Forms.TextBox();
            this.btSchemaFolder = new System.Windows.Forms.Button();
            this.btDestinationFolder = new System.Windows.Forms.Button();
            this.txtMessageBox = new System.Windows.Forms.RichTextBox();
            this.dlgSchemaFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.dlgDestinationFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.dlgDeployScript = new System.Windows.Forms.OpenFileDialog();
            this.dlgDataFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.btCRUDFolder = new System.Windows.Forms.Button();
            this.txtCRUDFolder = new System.Windows.Forms.TextBox();
            this.lbCRUDFolder = new System.Windows.Forms.Label();
            this.dlgCRUDFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.dlgReleaseScriptsFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox4.SuspendLayout();
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
            this.splitMain.Panel1.Controls.Add(this.groupBox1);
            this.splitMain.Panel1.Controls.Add(this.groupBox4);
            this.splitMain.Panel1MinSize = 220;
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.txtMessageBox);
            this.splitMain.Size = new System.Drawing.Size(717, 525);
            this.splitMain.SplitterDistance = 220;
            this.splitMain.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.btGenerateReleaseScript);
            this.groupBox1.Controls.Add(this.lbReleaseScriptsFolder);
            this.groupBox1.Controls.Add(this.btReleaseScriptsFolder);
            this.groupBox1.Controls.Add(this.lbReleaseScriptName);
            this.groupBox1.Controls.Add(this.txtReleaseScriptsFolder);
            this.groupBox1.Controls.Add(this.txtReleaseScriptName);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(449, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(261, 206);
            this.groupBox1.TabIndex = 46;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Script Merging Generator";
            // 
            // btGenerateReleaseScript
            // 
            this.btGenerateReleaseScript.Location = new System.Drawing.Point(95, 177);
            this.btGenerateReleaseScript.Name = "btGenerateReleaseScript";
            this.btGenerateReleaseScript.Size = new System.Drawing.Size(158, 23);
            this.btGenerateReleaseScript.TabIndex = 45;
            this.btGenerateReleaseScript.Text = "Generate Release Script";
            this.btGenerateReleaseScript.UseVisualStyleBackColor = true;
            this.btGenerateReleaseScript.Click += new System.EventHandler(this.btGenerateReleaseScript_Click);
            // 
            // lbReleaseScriptsFolder
            // 
            this.lbReleaseScriptsFolder.AutoSize = true;
            this.lbReleaseScriptsFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbReleaseScriptsFolder.Location = new System.Drawing.Point(15, 25);
            this.lbReleaseScriptsFolder.Name = "lbReleaseScriptsFolder";
            this.lbReleaseScriptsFolder.Size = new System.Drawing.Size(116, 13);
            this.lbReleaseScriptsFolder.TabIndex = 40;
            this.lbReleaseScriptsFolder.Text = "Release Scripts Folder ";
            // 
            // btReleaseScriptsFolder
            // 
            this.btReleaseScriptsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btReleaseScriptsFolder.Location = new System.Drawing.Point(228, 37);
            this.btReleaseScriptsFolder.Name = "btReleaseScriptsFolder";
            this.btReleaseScriptsFolder.Size = new System.Drawing.Size(27, 23);
            this.btReleaseScriptsFolder.TabIndex = 42;
            this.btReleaseScriptsFolder.TabStop = false;
            this.btReleaseScriptsFolder.Text = "...";
            this.btReleaseScriptsFolder.UseVisualStyleBackColor = true;
            this.btReleaseScriptsFolder.Click += new System.EventHandler(this.btReleaseScriptsFolder_Click);
            // 
            // lbReleaseScriptName
            // 
            this.lbReleaseScriptName.AutoSize = true;
            this.lbReleaseScriptName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbReleaseScriptName.Location = new System.Drawing.Point(16, 69);
            this.lbReleaseScriptName.Name = "lbReleaseScriptName";
            this.lbReleaseScriptName.Size = new System.Drawing.Size(98, 13);
            this.lbReleaseScriptName.TabIndex = 43;
            this.lbReleaseScriptName.Text = "Result Script Name";
            // 
            // txtReleaseScriptsFolder
            // 
            this.txtReleaseScriptsFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReleaseScriptsFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReleaseScriptsFolder.Location = new System.Drawing.Point(18, 39);
            this.txtReleaseScriptsFolder.Name = "txtReleaseScriptsFolder";
            this.txtReleaseScriptsFolder.Size = new System.Drawing.Size(204, 20);
            this.txtReleaseScriptsFolder.TabIndex = 41;
            // 
            // txtReleaseScriptName
            // 
            this.txtReleaseScriptName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReleaseScriptName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReleaseScriptName.Location = new System.Drawing.Point(18, 85);
            this.txtReleaseScriptName.Name = "txtReleaseScriptName";
            this.txtReleaseScriptName.Size = new System.Drawing.Size(237, 20);
            this.txtReleaseScriptName.TabIndex = 44;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.BackColor = System.Drawing.Color.Transparent;
            this.groupBox4.Controls.Add(this.btCRUDFolder);
            this.groupBox4.Controls.Add(this.txtCRUDFolder);
            this.groupBox4.Controls.Add(this.lbCRUDFolder);
            this.groupBox4.Controls.Add(this.lbDeployScript);
            this.groupBox4.Controls.Add(this.btDeployScript);
            this.groupBox4.Controls.Add(this.btGenerateDeployPkg);
            this.groupBox4.Controls.Add(this.btDeployPkgPreview);
            this.groupBox4.Controls.Add(this.txtDeployScript);
            this.groupBox4.Controls.Add(this.lbSchemaFolder);
            this.groupBox4.Controls.Add(this.txtSchemaFolder);
            this.groupBox4.Controls.Add(this.btDataFolder);
            this.groupBox4.Controls.Add(this.txtDataFolder);
            this.groupBox4.Controls.Add(this.lbDestinationFolder);
            this.groupBox4.Controls.Add(this.lbDataFolder);
            this.groupBox4.Controls.Add(this.txtDestinationFolder);
            this.groupBox4.Controls.Add(this.btSchemaFolder);
            this.groupBox4.Controls.Add(this.btDestinationFolder);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(6, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(437, 206);
            this.groupBox4.TabIndex = 45;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Script Package Deployment";
            // 
            // lbDeployScript
            // 
            this.lbDeployScript.AutoSize = true;
            this.lbDeployScript.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDeployScript.Location = new System.Drawing.Point(6, 23);
            this.lbDeployScript.Name = "lbDeployScript";
            this.lbDeployScript.Size = new System.Drawing.Size(96, 13);
            this.lbDeployScript.TabIndex = 25;
            this.lbDeployScript.Text = "Deployment Script ";
            // 
            // btDeployScript
            // 
            this.btDeployScript.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btDeployScript.Location = new System.Drawing.Point(402, 20);
            this.btDeployScript.Name = "btDeployScript";
            this.btDeployScript.Size = new System.Drawing.Size(29, 23);
            this.btDeployScript.TabIndex = 27;
            this.btDeployScript.TabStop = false;
            this.btDeployScript.Text = "...";
            this.btDeployScript.UseVisualStyleBackColor = true;
            this.btDeployScript.Click += new System.EventHandler(this.btDeployScript_Click);
            // 
            // btGenerateDeployPkg
            // 
            this.btGenerateDeployPkg.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btGenerateDeployPkg.Location = new System.Drawing.Point(267, 177);
            this.btGenerateDeployPkg.Name = "btGenerateDeployPkg";
            this.btGenerateDeployPkg.Size = new System.Drawing.Size(164, 23);
            this.btGenerateDeployPkg.TabIndex = 38;
            this.btGenerateDeployPkg.Text = "Generate Deploy Package";
            this.btGenerateDeployPkg.UseVisualStyleBackColor = true;
            this.btGenerateDeployPkg.Click += new System.EventHandler(this.btGenerateDeployPkg_Click);
            // 
            // btDeployPkgPreview
            // 
            this.btDeployPkgPreview.Location = new System.Drawing.Point(9, 177);
            this.btDeployPkgPreview.Name = "btDeployPkgPreview";
            this.btDeployPkgPreview.Size = new System.Drawing.Size(89, 23);
            this.btDeployPkgPreview.TabIndex = 39;
            this.btDeployPkgPreview.Text = "Preview";
            this.btDeployPkgPreview.UseVisualStyleBackColor = true;
            this.btDeployPkgPreview.Click += new System.EventHandler(this.btDeployPkgPreview_Click);
            // 
            // txtDeployScript
            // 
            this.txtDeployScript.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDeployScript.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDeployScript.Location = new System.Drawing.Point(118, 20);
            this.txtDeployScript.Name = "txtDeployScript";
            this.txtDeployScript.Size = new System.Drawing.Size(278, 20);
            this.txtDeployScript.TabIndex = 26;
            // 
            // lbSchemaFolder
            // 
            this.lbSchemaFolder.AutoSize = true;
            this.lbSchemaFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSchemaFolder.Location = new System.Drawing.Point(6, 49);
            this.lbSchemaFolder.Name = "lbSchemaFolder";
            this.lbSchemaFolder.Size = new System.Drawing.Size(78, 13);
            this.lbSchemaFolder.TabIndex = 29;
            this.lbSchemaFolder.Text = "Schema Folder";
            // 
            // txtSchemaFolder
            // 
            this.txtSchemaFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSchemaFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSchemaFolder.Location = new System.Drawing.Point(118, 46);
            this.txtSchemaFolder.Name = "txtSchemaFolder";
            this.txtSchemaFolder.Size = new System.Drawing.Size(278, 20);
            this.txtSchemaFolder.TabIndex = 28;
            // 
            // btDataFolder
            // 
            this.btDataFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btDataFolder.Location = new System.Drawing.Point(402, 69);
            this.btDataFolder.Name = "btDataFolder";
            this.btDataFolder.Size = new System.Drawing.Size(29, 23);
            this.btDataFolder.TabIndex = 33;
            this.btDataFolder.TabStop = false;
            this.btDataFolder.Text = "...";
            this.btDataFolder.UseVisualStyleBackColor = true;
            this.btDataFolder.Click += new System.EventHandler(this.btDataFolder_Click);
            // 
            // txtDataFolder
            // 
            this.txtDataFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDataFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDataFolder.Location = new System.Drawing.Point(118, 72);
            this.txtDataFolder.Name = "txtDataFolder";
            this.txtDataFolder.Size = new System.Drawing.Size(278, 20);
            this.txtDataFolder.TabIndex = 32;
            // 
            // lbDestinationFolder
            // 
            this.lbDestinationFolder.AutoSize = true;
            this.lbDestinationFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDestinationFolder.Location = new System.Drawing.Point(6, 153);
            this.lbDestinationFolder.Name = "lbDestinationFolder";
            this.lbDestinationFolder.Size = new System.Drawing.Size(92, 13);
            this.lbDestinationFolder.TabIndex = 35;
            this.lbDestinationFolder.Text = "Destination Folder";
            // 
            // lbDataFolder
            // 
            this.lbDataFolder.AutoSize = true;
            this.lbDataFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbDataFolder.Location = new System.Drawing.Point(6, 75);
            this.lbDataFolder.Name = "lbDataFolder";
            this.lbDataFolder.Size = new System.Drawing.Size(62, 13);
            this.lbDataFolder.TabIndex = 31;
            this.lbDataFolder.Text = "Data Folder";
            // 
            // txtDestinationFolder
            // 
            this.txtDestinationFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDestinationFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDestinationFolder.Location = new System.Drawing.Point(118, 150);
            this.txtDestinationFolder.Name = "txtDestinationFolder";
            this.txtDestinationFolder.Size = new System.Drawing.Size(278, 20);
            this.txtDestinationFolder.TabIndex = 36;
            // 
            // btSchemaFolder
            // 
            this.btSchemaFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSchemaFolder.Location = new System.Drawing.Point(402, 44);
            this.btSchemaFolder.Name = "btSchemaFolder";
            this.btSchemaFolder.Size = new System.Drawing.Size(29, 23);
            this.btSchemaFolder.TabIndex = 34;
            this.btSchemaFolder.TabStop = false;
            this.btSchemaFolder.Text = "...";
            this.btSchemaFolder.UseVisualStyleBackColor = true;
            this.btSchemaFolder.Click += new System.EventHandler(this.btSchemaFolder_Click);
            // 
            // btDestinationFolder
            // 
            this.btDestinationFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btDestinationFolder.Location = new System.Drawing.Point(402, 148);
            this.btDestinationFolder.Name = "btDestinationFolder";
            this.btDestinationFolder.Size = new System.Drawing.Size(29, 23);
            this.btDestinationFolder.TabIndex = 37;
            this.btDestinationFolder.TabStop = false;
            this.btDestinationFolder.Text = "...";
            this.btDestinationFolder.UseVisualStyleBackColor = true;
            this.btDestinationFolder.Click += new System.EventHandler(this.btDestinationFolder_Click);
            // 
            // txtMessageBox
            // 
            this.txtMessageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessageBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMessageBox.Location = new System.Drawing.Point(0, 0);
            this.txtMessageBox.Name = "txtMessageBox";
            this.txtMessageBox.Size = new System.Drawing.Size(717, 301);
            this.txtMessageBox.TabIndex = 1;
            this.txtMessageBox.TabStop = false;
            this.txtMessageBox.Text = "";
            // 
            // btCRUDFolder
            // 
            this.btCRUDFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btCRUDFolder.Location = new System.Drawing.Point(402, 95);
            this.btCRUDFolder.Name = "btCRUDFolder";
            this.btCRUDFolder.Size = new System.Drawing.Size(29, 23);
            this.btCRUDFolder.TabIndex = 42;
            this.btCRUDFolder.TabStop = false;
            this.btCRUDFolder.Text = "...";
            this.btCRUDFolder.UseVisualStyleBackColor = true;
            this.btCRUDFolder.Click += new System.EventHandler(this.btCRUDFolder_Click);
            // 
            // txtCRUDFolder
            // 
            this.txtCRUDFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCRUDFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCRUDFolder.Location = new System.Drawing.Point(118, 98);
            this.txtCRUDFolder.Name = "txtCRUDFolder";
            this.txtCRUDFolder.Size = new System.Drawing.Size(278, 20);
            this.txtCRUDFolder.TabIndex = 41;
            // 
            // lbCRUDFolder
            // 
            this.lbCRUDFolder.AutoSize = true;
            this.lbCRUDFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCRUDFolder.Location = new System.Drawing.Point(6, 101);
            this.lbCRUDFolder.Name = "lbCRUDFolder";
            this.lbCRUDFolder.Size = new System.Drawing.Size(106, 13);
            this.lbCRUDFolder.TabIndex = 40;
            this.lbCRUDFolder.Text = "CRUD Relase Folder";
            // 
            // ProjectDeployPackageControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitMain);
            this.Name = "ProjectDeployPackageControl";
            this.Size = new System.Drawing.Size(717, 525);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            this.splitMain.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btRegisterDLLs;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.TextBox txtSchemaFolder;
        private System.Windows.Forms.Label lbSchemaFolder;
        private System.Windows.Forms.Label lbDeployScript;
        private System.Windows.Forms.TextBox txtDeployScript;
        private System.Windows.Forms.Button btDeployScript;
        private System.Windows.Forms.FolderBrowserDialog dlgSchemaFolder;
        private System.Windows.Forms.FolderBrowserDialog dlgDestinationFolder;
        public System.Windows.Forms.RichTextBox txtMessageBox;
        private System.Windows.Forms.Label lbDataFolder;
        private System.Windows.Forms.TextBox txtDataFolder;
        private System.Windows.Forms.Button btDataFolder;
        private System.Windows.Forms.Button btSchemaFolder;
        private System.Windows.Forms.Label lbDestinationFolder;
        private System.Windows.Forms.TextBox txtDestinationFolder;
        private System.Windows.Forms.Button btDestinationFolder;
        private System.Windows.Forms.Button btGenerateDeployPkg;
        private System.Windows.Forms.OpenFileDialog dlgDeployScript;
        private System.Windows.Forms.FolderBrowserDialog dlgDataFolder;
        private System.Windows.Forms.Button btDeployPkgPreview;
        private System.Windows.Forms.Label lbReleaseScriptsFolder;
        private System.Windows.Forms.TextBox txtReleaseScriptsFolder;
        private System.Windows.Forms.Button btReleaseScriptsFolder;
        private System.Windows.Forms.Label lbReleaseScriptName;
        private System.Windows.Forms.TextBox txtReleaseScriptName;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btGenerateReleaseScript;
        private System.Windows.Forms.Button btCRUDFolder;
        private System.Windows.Forms.TextBox txtCRUDFolder;
        private System.Windows.Forms.Label lbCRUDFolder;
        private System.Windows.Forms.FolderBrowserDialog dlgCRUDFolder;
        private System.Windows.Forms.FolderBrowserDialog dlgReleaseScriptsFolder;
    }
}
