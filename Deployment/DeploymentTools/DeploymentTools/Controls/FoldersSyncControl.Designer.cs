namespace DeploymentTools.Controls
{
    partial class FoldersSyncControl
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
            this.btSyncCancel = new System.Windows.Forms.Button();
            this.chkMultiSource = new System.Windows.Forms.CheckBox();
            this.btSyncPreviw = new System.Windows.Forms.Button();
            this.btSyncRun = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.radioCopy = new System.Windows.Forms.RadioButton();
            this.radioGetNew = new System.Windows.Forms.RadioButton();
            this.radioRefresh = new System.Windows.Forms.RadioButton();
            this.radioMerge = new System.Windows.Forms.RadioButton();
            this.radioSync = new System.Windows.Forms.RadioButton();
            this.btSyncExcludeHelp = new System.Windows.Forms.Button();
            this.txtSyncExclude = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btSyncDest = new System.Windows.Forms.Button();
            this.txtSyncDest = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtSyncSrc = new System.Windows.Forms.TextBox();
            this.btSyncSrc = new System.Windows.Forms.Button();
            this.txtMessageBox = new System.Windows.Forms.RichTextBox();
            this.dlgSyncSrc = new System.Windows.Forms.FolderBrowserDialog();
            this.dlgSyncDest = new System.Windows.Forms.FolderBrowserDialog();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitMain
            // 
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitMain.Location = new System.Drawing.Point(0, 0);
            this.splitMain.Name = "splitMain";
            this.splitMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitMain.Panel1
            // 
            this.splitMain.Panel1.Controls.Add(this.btSyncCancel);
            this.splitMain.Panel1.Controls.Add(this.chkMultiSource);
            this.splitMain.Panel1.Controls.Add(this.btSyncPreviw);
            this.splitMain.Panel1.Controls.Add(this.btSyncRun);
            this.splitMain.Panel1.Controls.Add(this.groupBox5);
            this.splitMain.Panel1.Controls.Add(this.btSyncExcludeHelp);
            this.splitMain.Panel1.Controls.Add(this.txtSyncExclude);
            this.splitMain.Panel1.Controls.Add(this.label8);
            this.splitMain.Panel1.Controls.Add(this.btSyncDest);
            this.splitMain.Panel1.Controls.Add(this.txtSyncDest);
            this.splitMain.Panel1.Controls.Add(this.label9);
            this.splitMain.Panel1.Controls.Add(this.label12);
            this.splitMain.Panel1.Controls.Add(this.txtSyncSrc);
            this.splitMain.Panel1.Controls.Add(this.btSyncSrc);
            this.splitMain.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitMain_Panel1_Paint);
            this.splitMain.Panel1MinSize = 245;
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.txtMessageBox);
            this.splitMain.Size = new System.Drawing.Size(688, 543);
            this.splitMain.SplitterDistance = 245;
            this.splitMain.TabIndex = 1;
            // 
            // btSyncCancel
            // 
            this.btSyncCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btSyncCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btSyncCancel.Location = new System.Drawing.Point(15, 217);
            this.btSyncCancel.Name = "btSyncCancel";
            this.btSyncCancel.Size = new System.Drawing.Size(70, 25);
            this.btSyncCancel.TabIndex = 38;
            this.btSyncCancel.Text = "Cancel";
            this.btSyncCancel.UseVisualStyleBackColor = true;
            this.btSyncCancel.Click += new System.EventHandler(this.btSyncCancel_Click);
            // 
            // chkMultiSource
            // 
            this.chkMultiSource.AutoSize = true;
            this.chkMultiSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkMultiSource.Location = new System.Drawing.Point(110, 33);
            this.chkMultiSource.Name = "chkMultiSource";
            this.chkMultiSource.Size = new System.Drawing.Size(196, 17);
            this.chkMultiSource.TabIndex = 21;
            this.chkMultiSource.Text = "Use multiple sources one level deep";
            this.chkMultiSource.UseVisualStyleBackColor = true;
            // 
            // btSyncPreviw
            // 
            this.btSyncPreviw.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btSyncPreviw.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btSyncPreviw.Location = new System.Drawing.Point(492, 219);
            this.btSyncPreviw.Name = "btSyncPreviw";
            this.btSyncPreviw.Size = new System.Drawing.Size(89, 23);
            this.btSyncPreviw.TabIndex = 32;
            this.btSyncPreviw.Text = "Preview";
            this.btSyncPreviw.UseVisualStyleBackColor = true;
            this.btSyncPreviw.Click += new System.EventHandler(this.btSyncPreviw_Click);
            // 
            // btSyncRun
            // 
            this.btSyncRun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btSyncRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btSyncRun.Location = new System.Drawing.Point(587, 220);
            this.btSyncRun.Name = "btSyncRun";
            this.btSyncRun.Size = new System.Drawing.Size(89, 23);
            this.btSyncRun.TabIndex = 31;
            this.btSyncRun.Text = "Run";
            this.btSyncRun.UseVisualStyleBackColor = true;
            this.btSyncRun.Click += new System.EventHandler(this.btSyncRun_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.radioCopy);
            this.groupBox5.Controls.Add(this.radioGetNew);
            this.groupBox5.Controls.Add(this.radioRefresh);
            this.groupBox5.Controls.Add(this.radioMerge);
            this.groupBox5.Controls.Add(this.radioSync);
            this.groupBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.Location = new System.Drawing.Point(15, 110);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(661, 103);
            this.groupBox5.TabIndex = 30;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Sync Method";
            // 
            // radioCopy
            // 
            this.radioCopy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioCopy.Location = new System.Drawing.Point(24, 79);
            this.radioCopy.Name = "radioCopy";
            this.radioCopy.Size = new System.Drawing.Size(390, 17);
            this.radioCopy.TabIndex = 20;
            this.radioCopy.Text = "Copy: copy all from source to destination";
            this.radioCopy.UseVisualStyleBackColor = true;
            // 
            // radioGetNew
            // 
            this.radioGetNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioGetNew.Location = new System.Drawing.Point(24, 64);
            this.radioGetNew.Name = "radioGetNew";
            this.radioGetNew.Size = new System.Drawing.Size(390, 17);
            this.radioGetNew.TabIndex = 19;
            this.radioGetNew.Text = "Get New: refresh + copy files from source that don\'t exist in destination";
            this.radioGetNew.UseVisualStyleBackColor = true;
            // 
            // radioRefresh
            // 
            this.radioRefresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioRefresh.Location = new System.Drawing.Point(24, 49);
            this.radioRefresh.Name = "radioRefresh";
            this.radioRefresh.Size = new System.Drawing.Size(390, 17);
            this.radioRefresh.TabIndex = 17;
            this.radioRefresh.Text = "Refresh: reload destination files if the coresponding source file was modified";
            this.radioRefresh.UseVisualStyleBackColor = true;
            // 
            // radioMerge
            // 
            this.radioMerge.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioMerge.Location = new System.Drawing.Point(24, 34);
            this.radioMerge.Name = "radioMerge";
            this.radioMerge.Size = new System.Drawing.Size(274, 17);
            this.radioMerge.TabIndex = 16;
            this.radioMerge.Text = "Merge: sync destination to source and vice versa";
            this.radioMerge.UseVisualStyleBackColor = true;
            // 
            // radioSync
            // 
            this.radioSync.Checked = true;
            this.radioSync.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioSync.Location = new System.Drawing.Point(24, 19);
            this.radioSync.Name = "radioSync";
            this.radioSync.Size = new System.Drawing.Size(274, 17);
            this.radioSync.TabIndex = 15;
            this.radioSync.TabStop = true;
            this.radioSync.Text = "Synchronize: bring destination to the state of source";
            this.radioSync.UseVisualStyleBackColor = true;
            // 
            // btSyncExcludeHelp
            // 
            this.btSyncExcludeHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSyncExcludeHelp.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btSyncExcludeHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btSyncExcludeHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btSyncExcludeHelp.ForeColor = System.Drawing.Color.Silver;
            this.btSyncExcludeHelp.Location = new System.Drawing.Point(647, 83);
            this.btSyncExcludeHelp.Name = "btSyncExcludeHelp";
            this.btSyncExcludeHelp.Size = new System.Drawing.Size(29, 21);
            this.btSyncExcludeHelp.TabIndex = 29;
            this.btSyncExcludeHelp.TabStop = false;
            this.btSyncExcludeHelp.Text = "?";
            this.btSyncExcludeHelp.UseVisualStyleBackColor = true;
            this.btSyncExcludeHelp.Click += new System.EventHandler(this.btSyncExcludeHelp_Click);
            // 
            // txtSyncExclude
            // 
            this.txtSyncExclude.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSyncExclude.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSyncExclude.Location = new System.Drawing.Point(110, 84);
            this.txtSyncExclude.Name = "txtSyncExclude";
            this.txtSyncExclude.Size = new System.Drawing.Size(531, 20);
            this.txtSyncExclude.TabIndex = 27;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(12, 87);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 13);
            this.label8.TabIndex = 28;
            this.label8.Text = "Exclude";
            // 
            // btSyncDest
            // 
            this.btSyncDest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSyncDest.Location = new System.Drawing.Point(647, 56);
            this.btSyncDest.Name = "btSyncDest";
            this.btSyncDest.Size = new System.Drawing.Size(29, 23);
            this.btSyncDest.TabIndex = 26;
            this.btSyncDest.TabStop = false;
            this.btSyncDest.Text = "...";
            this.btSyncDest.UseVisualStyleBackColor = true;
            this.btSyncDest.Click += new System.EventHandler(this.btSyncDest_Click);
            // 
            // txtSyncDest
            // 
            this.txtSyncDest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSyncDest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSyncDest.Location = new System.Drawing.Point(110, 58);
            this.txtSyncDest.Name = "txtSyncDest";
            this.txtSyncDest.Size = new System.Drawing.Size(531, 20);
            this.txtSyncDest.TabIndex = 24;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(12, 10);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(73, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Source Folder";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(12, 61);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(92, 13);
            this.label12.TabIndex = 25;
            this.label12.Text = "Destination Folder";
            // 
            // txtSyncSrc
            // 
            this.txtSyncSrc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSyncSrc.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSyncSrc.Location = new System.Drawing.Point(110, 7);
            this.txtSyncSrc.Name = "txtSyncSrc";
            this.txtSyncSrc.Size = new System.Drawing.Size(531, 20);
            this.txtSyncSrc.TabIndex = 22;
            // 
            // btSyncSrc
            // 
            this.btSyncSrc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSyncSrc.Location = new System.Drawing.Point(647, 5);
            this.btSyncSrc.Name = "btSyncSrc";
            this.btSyncSrc.Size = new System.Drawing.Size(29, 23);
            this.btSyncSrc.TabIndex = 23;
            this.btSyncSrc.TabStop = false;
            this.btSyncSrc.Text = "...";
            this.btSyncSrc.UseVisualStyleBackColor = true;
            this.btSyncSrc.Click += new System.EventHandler(this.btSyncSrc_Click);
            // 
            // txtMessageBox
            // 
            this.txtMessageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessageBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMessageBox.Location = new System.Drawing.Point(0, 0);
            this.txtMessageBox.Name = "txtMessageBox";
            this.txtMessageBox.Size = new System.Drawing.Size(688, 294);
            this.txtMessageBox.TabIndex = 1;
            this.txtMessageBox.TabStop = false;
            this.txtMessageBox.Text = "";
            // 
            // FoldersSyncControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitMain);
            this.Name = "FoldersSyncControl";
            this.Size = new System.Drawing.Size(688, 543);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel1.PerformLayout();
            this.splitMain.Panel2.ResumeLayout(false);
            this.splitMain.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitMain;
        public System.Windows.Forms.RichTextBox txtMessageBox;
        private System.Windows.Forms.Button btSyncPreviw;
        private System.Windows.Forms.Button btSyncRun;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton radioCopy;
        private System.Windows.Forms.RadioButton radioGetNew;
        private System.Windows.Forms.RadioButton radioRefresh;
        private System.Windows.Forms.RadioButton radioMerge;
        private System.Windows.Forms.RadioButton radioSync;
        private System.Windows.Forms.Button btSyncExcludeHelp;
        private System.Windows.Forms.TextBox txtSyncExclude;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btSyncDest;
        private System.Windows.Forms.TextBox txtSyncDest;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtSyncSrc;
        private System.Windows.Forms.Button btSyncSrc;
        private System.Windows.Forms.FolderBrowserDialog dlgSyncSrc;
        private System.Windows.Forms.FolderBrowserDialog dlgSyncDest;
        private System.Windows.Forms.CheckBox chkMultiSource;
        private System.Windows.Forms.Button btSyncCancel;
    }
}
