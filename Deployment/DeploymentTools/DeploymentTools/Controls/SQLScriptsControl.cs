using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;
using EM.Logging;

namespace DeploymentTools.Controls
{
    public partial class SQLScriptsControl : BaseControl
    {
        public SQLScriptsControl()
            : base()
        {
            InitializeComponent();
            this.msgWriter = new RichTextBoxMessageWriter(this.txtMessageBox, this);
            this.log = new RollingFileLogger("SQLScripts", new LogLevel(Level.INFO), RollingType.Weekly, RollingTypeRemove.ThreeMonthsOld);
            this.msgWriter.register(this.log);
        }
        
        public override string labelName
        {
            get { return "SQL Scripts"; }
        }
        public override void cleanMessageBox()
        {
            this.txtMessageBox.Clear();
        }


        public override void configManager_OnInitControls(ConfigManager cfg)
        {
            //SQL
            this.txtSrcFolder.Text = cfg.config.get("sql_source_folder", "");
            this.txtExceptions.Text = cfg.config.get("sql_exceptions", "");
            this.txtDBServer.Text = cfg.config.get("sql_server_name", "");
            this.txtUser.Text = cfg.config.get("sql_server_user", "");
            this.txtRollbackFolder.Text = cfg.config.get("sql_rollback_folder", this.txtSrcFolder.Text);
            this.chkUseWinAuth.Checked = cfg.config.get("sql_use_win_auth", "false").Trim().ToLower() == "true" ? true : false;

            this.radioDepthFirst.Checked = false;
            this.radioBreadthFirst.Checked = false;

            string strategy = cfg.config.get("sql_walk_strategy", "breadth");
            if (strategy == "depth") { this.radioDepthFirst.Checked = true; }
            else { this.radioBreadthFirst.Checked = true; }

            this.splitMain.SplitterDistance = int.Parse(cfg.internalConfig.get("sql_split_at", "220"));
        }

        public override void configManager_OnSave(ConfigManager cfg)
        {
            //SQL
            cfg.config["sql_source_folder"] = this.txtSrcFolder.Text;
            cfg.config["sql_exceptions"] = this.txtExceptions.Text;
            cfg.config["sql_server_name"] = this.txtDBServer.Text;
            cfg.config["sql_server_user"] = this.txtUser.Text;
            cfg.config["sql_use_win_auth"] = this.chkUseWinAuth.Checked.ToString().ToLower();
            cfg.config["sql_walk_strategy"] = this.radioBreadthFirst.Checked ? "breadth" : "depth";
            cfg.config["sql_rollback_folder"] = this.txtRollbackFolder.Text;

            cfg.internalConfig["sql_split_at"] = this.splitMain.SplitterDistance.ToString();
        }

        private void btSrcFolder_Click(object sender, EventArgs e)
        {
            this.dlgSQLSourceFolder.ShowNewFolderButton = true;
            this.dlgSQLSourceFolder.SelectedPath = this.txtSrcFolder.Text;
            if (this.dlgSQLSourceFolder.ShowDialog() == DialogResult.OK)
            {
                this.txtSrcFolder.Text = this.dlgSQLSourceFolder.SelectedPath;
                this.configManager.saveConfigFromControls();
            }
        }

        private void btExceptionsHelp_Click(object sender, EventArgs e)
        {
            string s = "A pattern for exluding scripts to be executed (or rolled back) \n";
            s += "Examples:\n";
            s += "  \"4. Data\"\n";
            s += "    - will exclude all scripts insite the \"4. Data\" folder\n";
            s += "  \"4. Data\" and not \"add page rights.sql\"\n";
            s += "    - will exclude all scripts insite the \"4. Data\" folder except for the \"add page rights.sql\" script that will be executed\n";

            MessageBox.Show(s);
        }

        private void btRollbackHelp_Click(object sender, EventArgs e)
        {
            string s = "Based on the scripts you want to run (a) it will generate rollback scripts (b) in the specified folder\n";
            s += "  1. Will only generate rollback scripts for Tables, Views, Functions and Stored Procedures\n";
            s += "  2. In order for the rollback to identify database objects, it requires that the name of the scripts in (a) end with the name of the object to be scripted";

            MessageBox.Show(s);
        }

        private void btRollbackFolder_Click(object sender, EventArgs e)
        {
            this.dlgRollbackFolder.ShowNewFolderButton = true;
            this.dlgRollbackFolder.SelectedPath = this.txtRollbackFolder.Text;
            if (this.dlgRollbackFolder.ShowDialog() == DialogResult.OK)
            {
                this.txtRollbackFolder.Text = this.dlgRollbackFolder.SelectedPath;
                this.configManager.saveConfigFromControls();
            }
        }

        private void btGenerateRollbackSQL_Click(object sender, EventArgs e)
        {
            this.runInThread(delegate()
            {
                if (this.txtSrcFolder.Text.Trim() != "" && this.txtRollbackFolder.Text.Trim() != "")
                {
                    try
                    {
                        this.btGenerateRollbackSQL.Enabled = false;
                        this.btViewTraceStrategy.Enabled = false;
                        this.btRunSQL.Enabled = false;

                        DBContext dbc = new DBContext(this.txtDBServer.Text, this.txtUser.Text, this.txtPassword.Text, this.chkUseWinAuth.Checked, this.txtSrcFolder.Text);
                        SQLDeployment sql = new SQLDeployment(dbc, this.msgWriter);
                        sql.sqlScriptExceptions = this.txtExceptions.Text;
                        DirectoryInfo rollbackFolder = new DirectoryInfo(this.txtRollbackFolder.Text);
                        sql.generateRollbackSripts(rollbackFolder);
                    }
                    catch (Exception er)
                    {
                        this.msgWriter.WriteException(er);
                    }
                    finally
                    {
                        this.btGenerateRollbackSQL.Enabled = true;
                        this.btViewTraceStrategy.Enabled = true;
                        this.btRunSQL.Enabled = true;
                    }
                }
                else
                {
                    MessageBox.Show("Please enter the path to SQL scripts and the path where to generate the rollback scripts");
                }
            });            
        }

        private void btViewTraceStrategy_Click(object sender, EventArgs e)
        {
            doSQLTasks(true);
        }

        private void btRunSQL_Click(object sender, EventArgs e)
        {
            doSQLTasks(false);
        }

        private void doSQLTasks(bool preview)
        {
            runInThread(delegate()
            {
                try
                {
                    this.btGenerateRollbackSQL.Enabled = false;
                    this.btViewTraceStrategy.Enabled = false;
                    this.btRunSQL.Enabled = false;

                    if (this.txtSrcFolder.Text.Trim() != "")
                    {
                        DBContext dbc = new DBContext(this.txtDBServer.Text, this.txtUser.Text, this.txtPassword.Text, this.chkUseWinAuth.Checked, this.txtSrcFolder.Text);
                        SQLDeployment sql = new SQLDeployment(dbc, this.msgWriter);
                        sql.sqlScriptExceptions = this.txtExceptions.Text;

                        if (this.radioBreadthFirst.Checked)
                        {
                            sql.runSriptsBreadthFirst(preview);
                        }
                        else
                        {
                            sql.runSriptsDepthFirst(preview);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please enter the path to SQL scripts");
                    }
                }
                catch (Exception er)
                {
                    this.msgWriter.WriteException(er);
                }
                finally
                {
                    this.btGenerateRollbackSQL.Enabled = true;
                    this.btViewTraceStrategy.Enabled = true;
                    this.btRunSQL.Enabled = true;
                }
            });
        }

        private void chkUseWinAuth_CheckedChanged(object sender, EventArgs e)
        {
            if (chkUseWinAuth.Checked)
            {
                this.txtUser.Enabled = false;
                this.txtPassword.Enabled = false;
                this.lbUser.Enabled = false;
                this.lbPassword.Enabled = false;
            }
            else
            {
                this.txtUser.Enabled = true;
                this.txtPassword.Enabled = true;
                this.lbUser.Enabled = true;
                this.lbPassword.Enabled = true;
            }
        }

        
    }
}
