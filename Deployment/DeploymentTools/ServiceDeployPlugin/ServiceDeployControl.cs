using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EM.Logging;
using EM.Util;

namespace DeploymentTools.Controls
{
    public partial class ServiceDeployControl : BaseControl, IPlugin 
    {
        private RemoteServers serviceServers;

        public ServiceDeployControl()
            : base()
        {
            InitializeComponent();
            this.msgWriter = new RichTextBoxMessageWriter(this.txtMessageBox, this);
            this.log = new RollingFileLogger("ServiceDeploy", new LogLevel(Level.INFO), RollingType.Weekly, RollingTypeRemove.ThreeMonthsOld);
            this.msgWriter.register(this.log);
        }

        public override string labelName
        {
            get { return "Service Deploy"; }
        }
        public override void cleanMessageBox()
        {
            this.txtMessageBox.Clear();
        }

        private string errForCfg = null;
        public override void configManager_OnInitControls(ConfigManager cfg)
        {
            //PROD DEPLOY
            this.txtServiceSrc.Text = cfg.config.get("service_source_folder", "");

            string pservers = cfg.config.get("service_servers", @"devserv1, \\PayrollServ1-Dev\c$\Program Files\surepayroll\SurePayrollWCFPayrollEngineService");
            this.serviceServers = new RemoteServers(pservers);
            if (this.serviceServers.ignoredServers.Count > 0)
            {
                if (errForCfg == null || errForCfg != cfg.configFilePath)
                {
                    MessageBox.Show("There are duplicated servers in the config file, only one instance of them will be used. The instances that are ignored are:\r\n" + this.serviceServers.ignoredServersConfig, "Ignored Service Servers:", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    errForCfg = cfg.configFilePath;
                }
            }
            this.lbServiceServers.Text = this.serviceServers.ToString();

            string serviceStop = cfg.config.get("service_stop", "false");
            if (serviceStop == "true") { this.chkStopService.Checked = true; }
            else { this.chkStopService.Checked = false; }

            string cpf = cfg.config.get("service_copy", "true");
            if (cpf == "true") { this.chkServiceCopyFiles.Checked = true; }
            else { this.chkServiceCopyFiles.Checked = false; }

            string serviceStart = cfg.config.get("service_start", "false");
            if (serviceStart == "true") { this.chkStartService.Checked = true; }
            else { this.chkStartService.Checked = false; }

            this.txtServiceName.Text = cfg.config.get("service_name", "SurePayrollWCFPayrollEngineService");
            this.txtServiceFileExclude.Text = cfg.config.get("service_copy_exclude", "bak");

            string runParal = cfg.config.get("prod_run_parallel", "true");
            if (runParal == "true") { this.chkServiceRunInParalel.Checked = true; }
            else { this.chkServiceRunInParalel.Checked = false; }

            this.splitMain.SplitterDistance = int.Parse(cfg.internalConfig.get("service_split_at", "200"));
        }

        public override void configManager_OnSave(ConfigManager cfg)
        {
            //PROD DEPLOY
            cfg.config["service_source_folder"] = this.txtServiceSrc.Text;
            cfg.config["service_stop"] = this.chkStopService.Checked ? "true" : "false";
            cfg.config["service_copy"] = this.chkServiceCopyFiles.Checked ? "true" : "false";
            cfg.config["service_start"] = this.chkStartService.Checked ? "true" : "false";
            cfg.config["service_name"] = this.txtServiceName.Text;
            cfg.config["service_copy_exclude"] = this.txtServiceFileExclude.Text;
            cfg.config.setdefault("service_servers",
               @"PayrollCalc1-dev, \\PayrollCalc1-dev\c$\Program Files\surepayroll\SurePayrollWCFPayrollEngineService
                 PayrollCalc2-dev, \\PayrollCalc2-dev\c$\Program Files\surepayroll\SurePayrollWCFPayrollEngineService
                 PayrollServ1-dev, \\PayrollServ1-dev\c$\Program Files\surepayroll\SurePayrollWCFPayrollEngineService
                 PayrollServ2-dev, \\PayrollServ2-dev\c$\Program Files\surepayroll\SurePayrollWCFPayrollEngineService");
            
            cfg.config["service_run_parallel"] = this.chkServiceRunInParalel.Checked ? "true" : "false";
            cfg.internalConfig["service_split_at"] = this.splitMain.SplitterDistance.ToString();
        }

        private void btServiceDeploySrc_Click(object sender, EventArgs e)
        {
            this.dlgServiceDeploySrc.ShowNewFolderButton = true;
            this.dlgServiceDeploySrc.SelectedPath = this.txtServiceSrc.Text;
            if (this.dlgServiceDeploySrc.ShowDialog() == DialogResult.OK)
            {
                this.txtServiceSrc.Text = this.dlgServiceDeploySrc.SelectedPath;
                this.configManager.saveConfigFromControls();
            }
        }

        private void btServiceDeployPreview_Click(object sender, EventArgs e)
        {
            runInThread(delegate()
            {
                doServiceDeploy(true);
            });
        }

        private void btServiceDeploy_Click(object sender, EventArgs e)
        {
            runInThread(delegate()
            {
                doServiceDeploy(false);
            });     
        }

        private void btHelpServiceDeploy_Click(object sender, EventArgs e)
        {
            string s = "This are the destination servers and the paths wehere the source folder will be copied to. \n\n";
            s += "To change the list of servers and the paths:\n\t click \"Edit Config\" to open the file in notepad, change the [service_servers] configuration tag and save the changes \n\t click \"Load Config\" to reload the new settings:\n";

            MessageBox.Show(s);
        }


        private void doServiceDeploy(bool preview)
        {
            if (this.chkServiceCopyFiles.Checked && this.txtServiceSrc.Text.Trim() == "")
            {
                MessageBox.Show("Please enter the path to source folder");
                return;
            }


            if (serviceServers.Count > 0)
            {
                try
                {
                    this.btServiceDeploy.Enabled = false;
                    this.btServiceDeployPreview.Enabled = false;
                    this.configManager.saveConfigFromControls();

                    ServiceDeployment dep = new ServiceDeployment(this.msgWriter, this.txtServiceSrc.Text, serviceServers, this.configManager.config);
                    dep.preview = preview;
                    dep.runInParallel = this.chkServiceRunInParalel.Checked;

                    dep.deploy(this.chkStopService.Checked,
                               this.chkServiceCopyFiles.Checked,
                               this.chkStartService.Checked);

                }
                catch (Exception er)
                {
                    this.msgWriter.WriteException(er);
                }
                finally
                {
                    this.btServiceDeploy.Enabled = true;
                    this.btServiceDeployPreview.Enabled = true;
                }
            }
            else
            {
                MessageBox.Show("Please edit the configuration file to add service servers");
            }
        }


        #region IPlugin Members

        public void installPlugin(IPluginHost hostProgram)
        {
            IDeployToolsPluginHost host = (IDeployToolsPluginHost)hostProgram;

            this.configManager = host.cm;
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Location = new System.Drawing.Point(3, 3);
            //instance.Name = "plugin" + (this.pluginLoader.loadedPugins.Keys.Count + 1).ToString();
            this.Size = new System.Drawing.Size(784, 423);

            TabPage tp = new TabPage();
            tp.Controls.Add(this);
            tp.Location = new System.Drawing.Point(4, 22);
            tp.Name = "tabPlugin" + (host.pluginLoader.loadedPugins.Keys.Count + 1).ToString();
            tp.Padding = new System.Windows.Forms.Padding(3);
            tp.Size = new System.Drawing.Size(790, 429);
            tp.Text = this.labelName;
            tp.UseVisualStyleBackColor = true;
            host.mainSelector.Controls.Add(tp);
            host.repaint();

            //read the configuration and set GUI controls
            this.configManager_OnInitControls(host.cm);
        }

        #endregion

        private void btServiceFileExcludeHelp_Click(object sender, EventArgs e)
        {
            string s = "A pattern for exluding files/folders from the copy process \n";
            s += "Examples:\n";
            s += "  \"Bak\"\n";
            s += "    - will exclude all files insite the \"Bak\" folder\n";
            s += "  \"Bak\" and not \"WCFEngineService.exe.config\"\n";
            s += "    - will exclude all files inside the \"Bak\" folder except for the \"WCFEngineService.exe.config\" file which will be considered\n";

            MessageBox.Show(s);
        }
    }
}
