using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using DeploymentTools.Controls;
using EM.Collections;
using EM.Util;
using System.Threading;
using EM.Logging;

namespace DeploymentTools
{
    public partial class DeployTools : Form, IDeployToolsPluginHost
    {
        private ConfigManager _cm;
        public ConfigManager cm
        {
            get { return _cm; }
            set { _cm = value; }
        }
        
        private PluginLoader _pluginLoader;
        public PluginLoader pluginLoader
        {
            get { return _pluginLoader; }
            set { _pluginLoader = value; }
        }

        public DeployTools()
        {
            InitializeComponent();
        }

        
        private void btCleanMessageBox_Click(object sender, EventArgs e)
        {
            BaseControl c = (BaseControl)this.tabDeploySelector.TabPages[this.tabDeploySelector.SelectedIndex].Controls[0];
            c.cleanMessageBox();            
        }

        private void btEditConfig_Click(object sender, EventArgs e)
        {
            this.cm.editConfig();
        }

        private void btLoadConfig_Click(object sender, EventArgs e)
        {
            this.cm_OnSave(this.cm);    //set the new config file if is a new one

            this.cm.loadConfig();
            this.cm.initControlsFromConfig();
        }

        private void btSaveConfig_Click(object sender, EventArgs e)
        {
            this.cm.saveConfigFromControls();
        }

        private void DeployTools_Load(object sender, EventArgs e)
        {
            try
            {
            //Thread.Sleep(20000);
            this.cm = new ConfigManager();
            this.cm.OnInitControls += new ConfigManager.ConfigEventHandler(cm_OnInitControls);
            this.cm.OnSave += new ConfigManager.ConfigEventHandler(cm_OnSave);
            this.sqlScriptsControl1.configManager = this.cm;
            this.prodFileDeployControl1.configManager = this.cm;
            this.foldersSyncControl1.configManager = this.cm;
            this.cm.loadConfig();
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message + "\n" + Logger.getErrorDetails(err), "Startup Error - Config Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                this.pluginLoader = new PluginLoader();
                this.pluginLoader.OnLoadPlugin += new PluginLoader.LoadHandler(pl_OnLoadPlugin);
                this.pluginLoader.load_once();  //load plugins existing at startup

                this.cm.initControlsFromConfig();

                if (!this.cm.config.ContainsKey("com_servers"))
                {   //save missing config tags with default values and then reload
                    //so when doing "edit config" the user sees all the tags
                    this.cm.saveConfigFromControls();
                    this.cm.loadConfig();
                    this.cm.initControlsFromConfig();
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message + "\n" + Logger.getErrorDetails(err), "Pugins Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            //allow loading plugins that don't exist at startup
            this.pluginLoader.startLoadingLoop();            

        }
        
        void pluginAdder(Assembly plugin)
        {
            this.pluginLoader.installPlugin(plugin, this);            
        }

        void pl_OnLoadPlugin(Assembly plugin)
        {
            //must perform the plugin instalation from the thread of the main form 
            //and not from the thread of the plugin loader that calls this method
            //because if the instalation involves adding controls to the Form they can only be added 
            //from the  thread of the form
            lock (this)
            {
                this.Invoke(new PluginAdder(pluginAdder), new object[] { plugin });                
            }
        }

        private void cm_OnSave(ConfigManager cfg)
        {
            //WIN
            int keys = cfg.internalConfig.Count;
            if (this.Top > 5)  //not maximized
            {
                cfg.internalConfig["win_left"] = this.Left.ToString();
                cfg.internalConfig["win_top"] = this.Top.ToString();
                cfg.internalConfig["win_width"] = this.Width.ToString();
                cfg.internalConfig["win_height"] = this.Height.ToString();
            }
            cfg.internalConfig["win_tab"] = this.tabDeploySelector.SelectedIndex.ToString();
            cfg.internalConfig["config_file"] = this.txtConfigFile.Text;
            if ( keys == 0 || cfg.configFilePath != this.txtConfigFile.Text)
            {
                cfg.internalConfig.save();
                cfg.configFilePath = this.txtConfigFile.Text;
            }
        }

        private void cm_OnInitControls(ConfigManager cfg)
        {
            //WIN
            this.Left = int.Parse(cfg.internalConfig.get("win_left", this.Left.ToString()));
            this.Top = int.Parse(cfg.internalConfig.get("win_top", this.Top.ToString()));
            this.Width = int.Parse(cfg.internalConfig.get("win_width", this.Width.ToString()));
            this.Height = int.Parse(cfg.internalConfig.get("win_height", this.Height.ToString()));
            this.txtConfigFile.Text = cfg.internalConfig.get("config_file", cfg.defaultConfigFilePath);

            try
            {
                this.tabDeploySelector.SelectTab(int.Parse(cfg.internalConfig.get("win_tab", this.tabDeploySelector.SelectedIndex.ToString())));
            }
            catch   //in case it was a plugin that dosen't exist anymore
            {
                this.tabDeploySelector.SelectTab(0);
            }

            this.lbCurrentLoadedConfig.Text = cfg.configFilePath;
        }

        private void btConfigSrc_Click(object sender, EventArgs e)
        {
            this.dlgConfigFile.CheckFileExists = false;
            this.dlgConfigFile.CheckPathExists = false;
            this.dlgConfigFile.Filter = "Configuration files (*.cfg)|*.cfg";
            this.dlgConfigFile.InitialDirectory = Path.GetDirectoryName(this.txtConfigFile.Text);
            if (this.dlgConfigFile.ShowDialog() == DialogResult.OK)
            {
                this.txtConfigFile.Text = this.dlgConfigFile.FileName;
                //this.configManager.saveConfigFromControls();
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            this.Hide();
            try
            {
                this.pluginLoader.stopLoadingLoop();
                this.cm.saveConfigFromControls();
            }
            catch 
            {
                this.Show();
                throw;
            }
        }


        #region IDeployToolsPluginHost Members

        public TabControl mainSelector
        {
            get { return this.tabDeploySelector; }
        }
        public void repaint()
        {
            this.PerformLayout();
        }

        #endregion

        private void sqlScriptsControl1_Load(object sender, EventArgs e)
        {

        }
    }
}
