namespace MasterDeploy
{
    partial class MasterDeployControl
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
            this.txtMessageBox = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtXMLConfiguration = new System.Windows.Forms.TextBox();
            this.btXMLConfigFile = new System.Windows.Forms.Button();
            this.dlgXMLConfigurationOpen = new System.Windows.Forms.OpenFileDialog();
            this.btPackageFolder = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtPackageFolder = new System.Windows.Forms.TextBox();
            this.btDeploy = new System.Windows.Forms.Button();
            this.dlgPackageFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.pnlMessageBox = new System.Windows.Forms.Panel();
            this.btPreview = new System.Windows.Forms.Button();
            this.flowLayoutPanelBuildSolutions = new System.Windows.Forms.FlowLayoutPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.btEditXmlConfigFile = new System.Windows.Forms.Button();
            this.btCheckMissing = new System.Windows.Forms.Button();
            this.pnlMessageBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtMessageBox
            // 
            this.txtMessageBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtMessageBox.CausesValidation = false;
            this.txtMessageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessageBox.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMessageBox.Location = new System.Drawing.Point(0, 0);
            this.txtMessageBox.Name = "txtMessageBox";
            this.txtMessageBox.Size = new System.Drawing.Size(829, 236);
            this.txtMessageBox.TabIndex = 4;
            this.txtMessageBox.TabStop = false;
            this.txtMessageBox.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(156, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Deployment Configuration:";
            // 
            // txtXMLConfiguration
            // 
            this.txtXMLConfiguration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtXMLConfiguration.Location = new System.Drawing.Point(168, 3);
            this.txtXMLConfiguration.Name = "txtXMLConfiguration";
            this.txtXMLConfiguration.Size = new System.Drawing.Size(583, 20);
            this.txtXMLConfiguration.TabIndex = 23;
            // 
            // btXMLConfigFile
            // 
            this.btXMLConfigFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btXMLConfigFile.Location = new System.Drawing.Point(757, 1);
            this.btXMLConfigFile.Name = "btXMLConfigFile";
            this.btXMLConfigFile.Size = new System.Drawing.Size(25, 23);
            this.btXMLConfigFile.TabIndex = 38;
            this.btXMLConfigFile.Text = "...";
            this.btXMLConfigFile.UseVisualStyleBackColor = true;
            this.btXMLConfigFile.Click += new System.EventHandler(this.btXMLConfigFile_Click);
            // 
            // dlgXMLConfigurationOpen
            // 
            this.dlgXMLConfigurationOpen.FileName = "GroupPatterns.xml";
            this.dlgXMLConfigurationOpen.Filter = "Config Files|*.xml|All files|*.*";
            // 
            // btPackageFolder
            // 
            this.btPackageFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btPackageFolder.Location = new System.Drawing.Point(815, 28);
            this.btPackageFolder.Name = "btPackageFolder";
            this.btPackageFolder.Size = new System.Drawing.Size(25, 23);
            this.btPackageFolder.TabIndex = 41;
            this.btPackageFolder.Text = "...";
            this.btPackageFolder.UseVisualStyleBackColor = true;
            this.btPackageFolder.Click += new System.EventHandler(this.btPackageFolder_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(5, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 13);
            this.label1.TabIndex = 39;
            this.label1.Text = "Deployment Package:";
            // 
            // txtPackageFolder
            // 
            this.txtPackageFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPackageFolder.Location = new System.Drawing.Point(167, 30);
            this.txtPackageFolder.Name = "txtPackageFolder";
            this.txtPackageFolder.Size = new System.Drawing.Size(642, 20);
            this.txtPackageFolder.TabIndex = 40;
            this.txtPackageFolder.TextChanged += new System.EventHandler(this.txtPackageFolder_TextChanged);
            // 
            // btDeploy
            // 
            this.btDeploy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btDeploy.Location = new System.Drawing.Point(766, 57);
            this.btDeploy.Name = "btDeploy";
            this.btDeploy.Size = new System.Drawing.Size(75, 23);
            this.btDeploy.TabIndex = 42;
            this.btDeploy.Text = "Deploy";
            this.btDeploy.UseVisualStyleBackColor = true;
            this.btDeploy.Click += new System.EventHandler(this.btDeploy_Click);
            // 
            // pnlMessageBox
            // 
            this.pnlMessageBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlMessageBox.Controls.Add(this.txtMessageBox);
            this.pnlMessageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMessageBox.Location = new System.Drawing.Point(0, 0);
            this.pnlMessageBox.Name = "pnlMessageBox";
            this.pnlMessageBox.Size = new System.Drawing.Size(831, 238);
            this.pnlMessageBox.TabIndex = 43;
            // 
            // btPreview
            // 
            this.btPreview.Location = new System.Drawing.Point(9, 56);
            this.btPreview.Name = "btPreview";
            this.btPreview.Size = new System.Drawing.Size(75, 23);
            this.btPreview.TabIndex = 44;
            this.btPreview.Text = "Preview";
            this.btPreview.UseVisualStyleBackColor = true;
            this.btPreview.Click += new System.EventHandler(this.btPreview_Click);
            // 
            // flowLayoutPanelBuildSolutions
            // 
            this.flowLayoutPanelBuildSolutions.AutoScroll = true;
            this.flowLayoutPanelBuildSolutions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.flowLayoutPanelBuildSolutions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelBuildSolutions.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanelBuildSolutions.Name = "flowLayoutPanelBuildSolutions";
            this.flowLayoutPanelBuildSolutions.Size = new System.Drawing.Size(831, 97);
            this.flowLayoutPanelBuildSolutions.TabIndex = 45;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(9, 86);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.flowLayoutPanelBuildSolutions);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.pnlMessageBox);
            this.splitContainer1.Size = new System.Drawing.Size(831, 339);
            this.splitContainer1.SplitterDistance = 97;
            this.splitContainer1.TabIndex = 46;
            // 
            // btEditXmlConfigFile
            // 
            this.btEditXmlConfigFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btEditXmlConfigFile.Location = new System.Drawing.Point(788, 1);
            this.btEditXmlConfigFile.Name = "btEditXmlConfigFile";
            this.btEditXmlConfigFile.Size = new System.Drawing.Size(51, 23);
            this.btEditXmlConfigFile.TabIndex = 47;
            this.btEditXmlConfigFile.Text = "Edit";
            this.btEditXmlConfigFile.UseVisualStyleBackColor = true;
            this.btEditXmlConfigFile.Click += new System.EventHandler(this.btEditXmlConfigFile_Click);
            // 
            // btCheckMissing
            // 
            this.btCheckMissing.Location = new System.Drawing.Point(167, 56);
            this.btCheckMissing.Name = "btCheckMissing";
            this.btCheckMissing.Size = new System.Drawing.Size(161, 23);
            this.btCheckMissing.TabIndex = 48;
            this.btCheckMissing.Text = "Check For Unhandled Folders";
            this.btCheckMissing.UseVisualStyleBackColor = true;
            this.btCheckMissing.Click += new System.EventHandler(this.btCheckMissing_Click);
            // 
            // MasterDeployControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btCheckMissing);
            this.Controls.Add(this.btEditXmlConfigFile);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.btPreview);
            this.Controls.Add(this.btDeploy);
            this.Controls.Add(this.btPackageFolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPackageFolder);
            this.Controls.Add(this.btXMLConfigFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtXMLConfiguration);
            this.Name = "MasterDeployControl";
            this.Size = new System.Drawing.Size(848, 440);
            this.pnlMessageBox.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btCreatePackage;
        private System.Windows.Forms.RichTextBox txtMessageBox;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txtXMLConfiguration;
        private System.Windows.Forms.Button btXMLConfigFile;
        private System.Windows.Forms.OpenFileDialog dlgXMLConfigurationOpen;
        private System.Windows.Forms.Button btPackageFolder;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox txtPackageFolder;
        private System.Windows.Forms.Button btDeploy;
        private System.Windows.Forms.FolderBrowserDialog dlgPackageFolder;
        private System.Windows.Forms.Panel pnlMessageBox;
        private System.Windows.Forms.Button btPreview;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelBuildSolutions;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Button btEditXmlConfigFile;
        private System.Windows.Forms.Button btCheckMissing;
    }
}
