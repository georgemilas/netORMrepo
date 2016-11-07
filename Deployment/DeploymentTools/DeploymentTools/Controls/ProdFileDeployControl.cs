using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EM.Logging;

namespace DeploymentTools.Controls
{
    public partial class ProdFileDeployControl : BaseControl
    {
        public delegate void OnCancelHandler();
        public event OnCancelHandler OnCancel;

        private RemoteServers productionServers;


        public ProdFileDeployControl()
            : base()
        {
            InitializeComponent();
            this.msgWriter = new RichTextBoxMessageWriter(this.txtMessageBox, this);
            this.log = new RollingFileLogger("ProdFileDeploy", new LogLevel(Level.INFO), RollingType.Weekly, RollingTypeRemove.ThreeMonthsOld);
            this.msgWriter.register(this.log);
            this.btProdDeployCancel.Enabled = false;
        }

        public override string labelName
        {
            get { return "Production Files Deploy"; }
        }
        public override void cleanMessageBox()
        {
            this.txtMessageBox.Clear();
        }

        private string errForCfg = null;
        public override void configManager_OnInitControls(ConfigManager cfg)
        {
            //PROD DEPLOY
            this.txtProdSrc.Text = cfg.config.get("prod_source_folder", "");

            string pservers = cfg.config.get("prod_servers", @"web1, \\10.0.5.10\e$\Migration");
            this.productionServers = new RemoteServers(pservers);
            if (this.productionServers.ignoredServers.Count > 0)
            {
                if (errForCfg == null || errForCfg != cfg.configFilePath)
                {
                    MessageBox.Show("There are duplicated servers in the config file, only one instance of them will be used. The instances that are ignored are:\r\n" + this.productionServers.ignoredServersConfig, "Ignored Production Servers:", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    errForCfg = cfg.configFilePath;
                }
            }
            this.lbProdWeb.Text = this.productionServers.ToString();

            string takeOut = cfg.config.get("prod_team_out", "false");
            if (takeOut == "true") { this.chkProdTakeOutTeam.Checked = true; }
            else { this.chkProdTakeOutTeam.Checked = false; }

            string cpf = cfg.config.get("prod_copy", "true");
            if (cpf == "true") { this.chkProdCopyFiles.Checked = true; }
            else { this.chkProdCopyFiles.Checked = false; }

            string restart = cfg.config.get("prod_restart", "false");
            if (restart == "true") { this.chkProdRestartServer.Checked = true; }
            else { this.chkProdRestartServer.Checked = false; }

            string resetiis = cfg.config.get("prod_reset_iis", "true");
            if (resetiis == "true") { this.chkProdResetIIS.Checked = true; }
            else { this.chkProdResetIIS.Checked = false; }

            string putIn = cfg.config.get("prod_team_in", "false");
            if (putIn == "true") { this.chkProdPutIntoTeam.Checked = true; }
            else { this.chkProdPutIntoTeam.Checked = false; }

            string runParal = cfg.config.get("prod_run_parallel", "true");
            if (runParal == "true") { this.chkProdRunInParalel.Checked = true; }
            else { this.chkProdRunInParalel.Checked = false; }

            this.splitMain.SplitterDistance = int.Parse(cfg.internalConfig.get("prod_split_at", "200"));
        }

        public override void configManager_OnSave(ConfigManager cfg)
        {
            //PROD DEPLOY
            cfg.config["prod_source_folder"] = this.txtProdSrc.Text;
            cfg.config["prod_team_out"] = this.chkProdTakeOutTeam.Checked ? "true" : "false";
            cfg.config["prod_copy"] = this.chkProdCopyFiles.Checked ? "true" : "false";
            cfg.config["prod_reset_iis"] = this.chkProdResetIIS.Checked ? "true" : "false";
            cfg.config["prod_restart"] = this.chkProdRestartServer.Checked ? "true" : "false";
            cfg.config["prod_team_in"] = this.chkProdPutIntoTeam.Checked ? "true" : "false";
            cfg.config.setdefault("prod_copy_method", "copy");
            cfg.config.setdefault("prod_restart_params", "-r -f \\\\computername -t 10 -c \"deployment restart\" -d up:4:2");
            cfg.config.setdefault("prod_team_out_seconds", "45");
            cfg.config.setdefault("prod_restart_minutes", "3");
            cfg.config.setdefault("prod_team_in_minutes", "5");
            cfg.config.setdefault("prod_team_file", "");
            cfg.config.setdefault("prod_root", @"\e$\IIS");
            cfg.config.setdefault("prod_servers", @"web1, \\10.0.5.10@[prod_root]
                 #web0, \\10.0.5.9@[prod_root]
                 web2, \\10.0.5.11@[prod_root]");

            cfg.config["prod_run_parallel"] = this.chkProdRunInParalel.Checked ? "true" : "false";
            cfg.internalConfig["prod_split_at"] = this.splitMain.SplitterDistance.ToString();            
        }

        private void btProdDeploySrc_Click(object sender, EventArgs e)
        {
            this.dlgProdDeploySrc.ShowNewFolderButton = true;
            this.dlgProdDeploySrc.SelectedPath = this.txtProdSrc.Text;
            if (this.dlgProdDeploySrc.ShowDialog() == DialogResult.OK)
            {
                this.txtProdSrc.Text = this.dlgProdDeploySrc.SelectedPath;
                this.configManager.saveConfigFromControls();
            }
        }

        private void btProdDeployPreview_Click(object sender, EventArgs e)
        {
            runInThread(delegate()
            {
                doProdDeploy(true);
            });     
        }

        private void btProdDeploy_Click(object sender, EventArgs e)
        {
            runInThread(delegate()
            {
                doProdDeploy(false);
            });     
        }

        private void btHelpProdDeploy_Click(object sender, EventArgs e)
        {
            string s = "This are the destination servers and the paths wehere the source folder will be copied to. \n\n";
            s += "To change the list of servers and the paths:\n\t click \"Edit Config\" to open the file in notepad, change the [prod_servers] configuration tag and save the changes \n\t click \"Load Config\" to reload the new settings:\n";

            MessageBox.Show(s);
        }


        private void doProdDeploy(bool preview)
        {
            if (this.chkProdCopyFiles.Checked && this.txtProdSrc.Text.Trim() == "")
            {
                MessageBox.Show("Please enter the path to source folder");
                return;
            }


            if (productionServers.Count > 0)
            {
                try
                {
                    this.btProdDeploy.Enabled = false;
                    this.btProdDeployPreview.Enabled = false;
                    this.btProdDeployCancel.Enabled = true;

                    FilesDeployment dep = new FilesDeployment(this.msgWriter, this.txtProdSrc.Text, productionServers, this.configManager.config);
                    dep.preview = preview;
                    dep.runInParallel = this.chkProdRunInParalel.Checked;
                    var stopper = new OnCancelHandler(() => { dep.doCancel(); });
                    this.OnCancel += stopper;

                    dep.deploy(this.chkProdTakeOutTeam.Checked,
                               this.chkProdCopyFiles.Checked,
                               this.chkProdRestartServer.Checked,
                               this.chkProdPutIntoTeam.Checked,
                               this.chkProdResetIIS.Checked);

                    this.OnCancel -= stopper;

                }
                catch (Exception er)
                {
                    this.msgWriter.WriteException(er);
                }
                finally
                {
                    this.btProdDeployCancel.Enabled = false;
                    this.btProdDeploy.Enabled = true;
                    this.btProdDeployPreview.Enabled = true;
                }
            }
            else
            {
                MessageBox.Show("Please edit the configuration file to add production servers");
            }
        }

     

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btProdDeployCancel_Click(object sender, EventArgs e)
        {
            if (this.OnCancel != null)
            {
                OnCancel();
            }
        }

    }
}
