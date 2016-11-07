namespace DeploymentTools.Controls
{
    partial class SQLScriptsControl
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
            this.btExceptionsHelp = new System.Windows.Forms.Button();
            this.txtExceptions = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btRollbackHelp = new System.Windows.Forms.Button();
            this.btRollbackFolder = new System.Windows.Forms.Button();
            this.txtRollbackFolder = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btGenerateRollbackSQL = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkUseWinAuth = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.lbUser = new System.Windows.Forms.Label();
            this.txtUser = new System.Windows.Forms.TextBox();
            this.txtDBServer = new System.Windows.Forms.TextBox();
            this.lbPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioDepthFirst = new System.Windows.Forms.RadioButton();
            this.radioBreadthFirst = new System.Windows.Forms.RadioButton();
            this.btViewTraceStrategy = new System.Windows.Forms.Button();
            this.btRunSQL = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtSrcFolder = new System.Windows.Forms.TextBox();
            this.btSrcFolder = new System.Windows.Forms.Button();
            this.txtMessageBox = new System.Windows.Forms.RichTextBox();
            this.dlgSQLSourceFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.dlgRollbackFolder = new System.Windows.Forms.FolderBrowserDialog();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
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
            this.splitMain.Panel1.Controls.Add(this.btExceptionsHelp);
            this.splitMain.Panel1.Controls.Add(this.txtExceptions);
            this.splitMain.Panel1.Controls.Add(this.label11);
            this.splitMain.Panel1.Controls.Add(this.groupBox4);
            this.splitMain.Panel1.Controls.Add(this.groupBox3);
            this.splitMain.Panel1.Controls.Add(this.groupBox2);
            this.splitMain.Panel1.Controls.Add(this.label3);
            this.splitMain.Panel1.Controls.Add(this.txtSrcFolder);
            this.splitMain.Panel1.Controls.Add(this.btSrcFolder);
            this.splitMain.Panel1MinSize = 220;
            // 
            // splitMain.Panel2
            // 
            this.splitMain.Panel2.Controls.Add(this.txtMessageBox);
            this.splitMain.Size = new System.Drawing.Size(700, 535);
            this.splitMain.SplitterDistance = 220;
            this.splitMain.TabIndex = 1;
            // 
            // btExceptionsHelp
            // 
            this.btExceptionsHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btExceptionsHelp.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btExceptionsHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btExceptionsHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btExceptionsHelp.ForeColor = System.Drawing.Color.Silver;
            this.btExceptionsHelp.Location = new System.Drawing.Point(653, 32);
            this.btExceptionsHelp.Name = "btExceptionsHelp";
            this.btExceptionsHelp.Size = new System.Drawing.Size(29, 21);
            this.btExceptionsHelp.TabIndex = 27;
            this.btExceptionsHelp.TabStop = false;
            this.btExceptionsHelp.Text = "?";
            this.btExceptionsHelp.UseVisualStyleBackColor = true;
            this.btExceptionsHelp.Click += new System.EventHandler(this.btExceptionsHelp_Click);
            // 
            // txtExceptions
            // 
            this.txtExceptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExceptions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtExceptions.Location = new System.Drawing.Point(88, 32);
            this.txtExceptions.Name = "txtExceptions";
            this.txtExceptions.Size = new System.Drawing.Size(559, 20);
            this.txtExceptions.TabIndex = 25;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(12, 32);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(45, 13);
            this.label11.TabIndex = 26;
            this.label11.Text = "Exclude";
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.BackColor = System.Drawing.Color.Transparent;
            this.groupBox4.Controls.Add(this.btRollbackHelp);
            this.groupBox4.Controls.Add(this.btRollbackFolder);
            this.groupBox4.Controls.Add(this.txtRollbackFolder);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.btGenerateRollbackSQL);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(201, 67);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(281, 151);
            this.groupBox4.TabIndex = 24;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Rollback";
            // 
            // btRollbackHelp
            // 
            this.btRollbackHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btRollbackHelp.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.btRollbackHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btRollbackHelp.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btRollbackHelp.ForeColor = System.Drawing.Color.Silver;
            this.btRollbackHelp.Location = new System.Drawing.Point(246, 13);
            this.btRollbackHelp.Name = "btRollbackHelp";
            this.btRollbackHelp.Size = new System.Drawing.Size(29, 21);
            this.btRollbackHelp.TabIndex = 21;
            this.btRollbackHelp.TabStop = false;
            this.btRollbackHelp.Text = "?";
            this.btRollbackHelp.UseVisualStyleBackColor = true;
            this.btRollbackHelp.Click += new System.EventHandler(this.btRollbackHelp_Click);
            // 
            // btRollbackFolder
            // 
            this.btRollbackFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btRollbackFolder.Location = new System.Drawing.Point(249, 55);
            this.btRollbackFolder.Name = "btRollbackFolder";
            this.btRollbackFolder.Size = new System.Drawing.Size(29, 23);
            this.btRollbackFolder.TabIndex = 18;
            this.btRollbackFolder.TabStop = false;
            this.btRollbackFolder.Text = "...";
            this.btRollbackFolder.UseVisualStyleBackColor = true;
            this.btRollbackFolder.Click += new System.EventHandler(this.btRollbackFolder_Click);
            // 
            // txtRollbackFolder
            // 
            this.txtRollbackFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRollbackFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtRollbackFolder.Location = new System.Drawing.Point(6, 57);
            this.txtRollbackFolder.Name = "txtRollbackFolder";
            this.txtRollbackFolder.Size = new System.Drawing.Size(237, 20);
            this.txtRollbackFolder.TabIndex = 17;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(3, 40);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 14);
            this.label7.TabIndex = 16;
            this.label7.Text = "Rollback Folder:";
            // 
            // btGenerateRollbackSQL
            // 
            this.btGenerateRollbackSQL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btGenerateRollbackSQL.Location = new System.Drawing.Point(119, 116);
            this.btGenerateRollbackSQL.Name = "btGenerateRollbackSQL";
            this.btGenerateRollbackSQL.Size = new System.Drawing.Size(159, 23);
            this.btGenerateRollbackSQL.TabIndex = 8;
            this.btGenerateRollbackSQL.Text = "Generate Backup Scripts";
            this.btGenerateRollbackSQL.UseVisualStyleBackColor = true;
            this.btGenerateRollbackSQL.Click += new System.EventHandler(this.btGenerateRollbackSQL_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.groupBox3.Controls.Add(this.chkUseWinAuth);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.lbUser);
            this.groupBox3.Controls.Add(this.txtUser);
            this.groupBox3.Controls.Add(this.txtDBServer);
            this.groupBox3.Controls.Add(this.lbPassword);
            this.groupBox3.Controls.Add(this.txtPassword);
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(12, 68);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(183, 150);
            this.groupBox3.TabIndex = 23;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Database Connection";
            // 
            // chkUseWinAuth
            // 
            this.chkUseWinAuth.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkUseWinAuth.Location = new System.Drawing.Point(6, 115);
            this.chkUseWinAuth.Name = "chkUseWinAuth";
            this.chkUseWinAuth.Size = new System.Drawing.Size(171, 22);
            this.chkUseWinAuth.TabIndex = 14;
            this.chkUseWinAuth.Text = "Use Windows Authentication";
            this.chkUseWinAuth.UseVisualStyleBackColor = true;
            this.chkUseWinAuth.CheckedChanged += new System.EventHandler(this.chkUseWinAuth_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(6, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Server";
            // 
            // lbUser
            // 
            this.lbUser.AutoSize = true;
            this.lbUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbUser.Location = new System.Drawing.Point(6, 59);
            this.lbUser.Name = "lbUser";
            this.lbUser.Size = new System.Drawing.Size(29, 13);
            this.lbUser.TabIndex = 8;
            this.lbUser.Text = "User";
            // 
            // txtUser
            // 
            this.txtUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtUser.Location = new System.Drawing.Point(65, 56);
            this.txtUser.Name = "txtUser";
            this.txtUser.Size = new System.Drawing.Size(112, 20);
            this.txtUser.TabIndex = 6;
            // 
            // txtDBServer
            // 
            this.txtDBServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDBServer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDBServer.Location = new System.Drawing.Point(65, 27);
            this.txtDBServer.Name = "txtDBServer";
            this.txtDBServer.Size = new System.Drawing.Size(112, 20);
            this.txtDBServer.TabIndex = 5;
            // 
            // lbPassword
            // 
            this.lbPassword.AutoSize = true;
            this.lbPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPassword.Location = new System.Drawing.Point(6, 85);
            this.lbPassword.Name = "lbPassword";
            this.lbPassword.Size = new System.Drawing.Size(53, 13);
            this.lbPassword.TabIndex = 10;
            this.lbPassword.Text = "Password";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPassword.Location = new System.Drawing.Point(65, 82);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(112, 20);
            this.txtPassword.TabIndex = 7;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.radioDepthFirst);
            this.groupBox2.Controls.Add(this.radioBreadthFirst);
            this.groupBox2.Controls.Add(this.btViewTraceStrategy);
            this.groupBox2.Controls.Add(this.btRunSQL);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(488, 68);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(194, 150);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Run SQL Scripts ";
            // 
            // radioDepthFirst
            // 
            this.radioDepthFirst.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioDepthFirst.Location = new System.Drawing.Point(6, 49);
            this.radioDepthFirst.Name = "radioDepthFirst";
            this.radioDepthFirst.Size = new System.Drawing.Size(132, 33);
            this.radioDepthFirst.TabIndex = 16;
            this.radioDepthFirst.Text = "Depth First Strategy";
            this.radioDepthFirst.UseVisualStyleBackColor = true;
            // 
            // radioBreadthFirst
            // 
            this.radioBreadthFirst.Checked = true;
            this.radioBreadthFirst.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioBreadthFirst.Location = new System.Drawing.Point(6, 27);
            this.radioBreadthFirst.Name = "radioBreadthFirst";
            this.radioBreadthFirst.Size = new System.Drawing.Size(132, 30);
            this.radioBreadthFirst.TabIndex = 15;
            this.radioBreadthFirst.TabStop = true;
            this.radioBreadthFirst.Text = "Breadth First Strategy";
            this.radioBreadthFirst.UseVisualStyleBackColor = true;
            // 
            // btViewTraceStrategy
            // 
            this.btViewTraceStrategy.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btViewTraceStrategy.Location = new System.Drawing.Point(6, 115);
            this.btViewTraceStrategy.Name = "btViewTraceStrategy";
            this.btViewTraceStrategy.Size = new System.Drawing.Size(70, 23);
            this.btViewTraceStrategy.TabIndex = 14;
            this.btViewTraceStrategy.Text = "Preview";
            this.btViewTraceStrategy.UseVisualStyleBackColor = true;
            this.btViewTraceStrategy.Click += new System.EventHandler(this.btViewTraceStrategy_Click);
            // 
            // btRunSQL
            // 
            this.btRunSQL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btRunSQL.Location = new System.Drawing.Point(99, 115);
            this.btRunSQL.Name = "btRunSQL";
            this.btRunSQL.Size = new System.Drawing.Size(89, 23);
            this.btRunSQL.TabIndex = 9;
            this.btRunSQL.Text = "Run Scripts";
            this.btRunSQL.UseVisualStyleBackColor = true;
            this.btRunSQL.Click += new System.EventHandler(this.btRunSQL_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "Source Folder";
            // 
            // txtSrcFolder
            // 
            this.txtSrcFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSrcFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSrcFolder.Location = new System.Drawing.Point(88, 6);
            this.txtSrcFolder.Name = "txtSrcFolder";
            this.txtSrcFolder.Size = new System.Drawing.Size(559, 20);
            this.txtSrcFolder.TabIndex = 21;
            // 
            // btSrcFolder
            // 
            this.btSrcFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSrcFolder.Location = new System.Drawing.Point(653, 4);
            this.btSrcFolder.Name = "btSrcFolder";
            this.btSrcFolder.Size = new System.Drawing.Size(29, 23);
            this.btSrcFolder.TabIndex = 22;
            this.btSrcFolder.TabStop = false;
            this.btSrcFolder.Text = "...";
            this.btSrcFolder.UseVisualStyleBackColor = true;
            this.btSrcFolder.Click += new System.EventHandler(this.btSrcFolder_Click);
            // 
            // txtMessageBox
            // 
            this.txtMessageBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMessageBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMessageBox.Location = new System.Drawing.Point(0, 0);
            this.txtMessageBox.Name = "txtMessageBox";
            this.txtMessageBox.Size = new System.Drawing.Size(700, 311);
            this.txtMessageBox.TabIndex = 1;
            this.txtMessageBox.TabStop = false;
            this.txtMessageBox.Text = "";
            // 
            // SQLScriptsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitMain);
            this.Name = "SQLScriptsControl";
            this.Size = new System.Drawing.Size(700, 535);
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel1.PerformLayout();
            this.splitMain.Panel2.ResumeLayout(false);
            this.splitMain.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.Button btExceptionsHelp;
        private System.Windows.Forms.TextBox txtExceptions;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btRollbackHelp;
        private System.Windows.Forms.Button btRollbackFolder;
        private System.Windows.Forms.TextBox txtRollbackFolder;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btGenerateRollbackSQL;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkUseWinAuth;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lbUser;
        private System.Windows.Forms.TextBox txtUser;
        private System.Windows.Forms.TextBox txtDBServer;
        private System.Windows.Forms.Label lbPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioDepthFirst;
        private System.Windows.Forms.RadioButton radioBreadthFirst;
        private System.Windows.Forms.Button btViewTraceStrategy;
        private System.Windows.Forms.Button btRunSQL;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSrcFolder;
        private System.Windows.Forms.Button btSrcFolder;
        private System.Windows.Forms.FolderBrowserDialog dlgSQLSourceFolder;
        private System.Windows.Forms.FolderBrowserDialog dlgRollbackFolder;
        public System.Windows.Forms.RichTextBox txtMessageBox;

    }
}
