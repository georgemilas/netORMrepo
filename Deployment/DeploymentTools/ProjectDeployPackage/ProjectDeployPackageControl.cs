using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EM.Logging;
using EM.Util;
using System.IO;
using TreeSync;
using ProjectDeployPackage;
using EM.Collections;

namespace DeploymentTools.Controls
{
    public partial class ProjectDeployPackageControl : BaseControl, IPlugin
    {
        public Func<string, string, string> cb = (s1, s2) => Path.Combine(s1, s2);
        public string deployFileDatabaseAndScopeSeparator = ":";

        public ProjectDeployPackageControl()
            : base()
        {
            InitializeComponent();
            this.msgWriter = new RichTextBoxMessageWriter(this.txtMessageBox, this);
            this.log = new RollingFileLogger("PrjPkgDeploy", new LogLevel(Level.INFO), RollingType.Weekly, RollingTypeRemove.ThreeMonthsOld);
            this.msgWriter.register(this.log);            
        }

        public override string labelName
        {
            get { return "Project Deploy Package"; }
        }

        public override void cleanMessageBox()
        {
            this.txtMessageBox.Clear();
        }

        private string errForCfg = null;
        public override void configManager_OnInitControls(ConfigManager cfg)
        {
            //Prj Pkg Deploy
            this.txtDeployScript.Text = cfg.config.get("prjpkg_script", "");
            this.deployFileDatabaseAndScopeSeparator = cfg.config.get("prjpkg_database_scope_separator", ":");
            this.txtSchemaFolder.Text = cfg.config.get("prjpkg_schema_folder", "");
            this.txtDataFolder.Text = cfg.config.get("prjpkg_data_folder", "");
            this.txtCRUDFolder.Text = cfg.config.get("prjpkg_crud_folder", "");
            this.txtReleaseScriptsFolder.Text = cfg.config.get("prjpkg_release_scripts_folder", "");
            this.txtReleaseScriptName.Text = cfg.config.get("prjpkg_release_script_name", "");
            this.txtDestinationFolder.Text = cfg.config.get("prjpkg_destination_folder", "");

            this.splitMain.SplitterDistance = int.Parse(cfg.internalConfig.get("prjpkg_split_at", "220"));
            
            this.cfg = new DeployScriptConfig(this.txtDeployScript.Text, this.deployFileDatabaseAndScopeSeparator);

        }

        public override void configManager_OnSave(ConfigManager cfg)
        {
            //COM
            cfg.config["prjpkg_script"] = this.txtDeployScript.Text;
            cfg.config["prjpkg_schema_folder"] = this.txtSchemaFolder.Text;
            cfg.config["prjpkg_data_folder"] = this.txtDataFolder.Text;
            cfg.config["prjpkg_crud_folder"] = this.txtCRUDFolder.Text;
            cfg.config["prjpkg_release_scripts_folder"] = this.txtReleaseScriptsFolder.Text;
            cfg.config["prjpkg_release_script_name"] = this.txtReleaseScriptName.Text;
            cfg.config["prjpkg_destination_folder"] = this.txtDestinationFolder.Text;
            
            cfg.internalConfig["prjpkg_split_at"] = this.splitMain.SplitterDistance.ToString();
            if (deployFileDatabaseAndScopeSeparator == ":")
            {
                cfg.config["prjpkg_database_scope_separator"] = ":"; 
            }
        }

  
        
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



        private void btSchemaFolder_Click(object sender, EventArgs e)
        {
            this.dlgSchemaFolder.ShowNewFolderButton = true;
            this.dlgSchemaFolder.SelectedPath = this.txtSchemaFolder.Text;
            if (this.dlgSchemaFolder.ShowDialog() == DialogResult.OK)
            {
                this.txtSchemaFolder.Text = this.dlgSchemaFolder.SelectedPath;
                this.configManager.saveConfigFromControls();
            }
        }

        private void btDataFolder_Click(object sender, EventArgs e)
        {
            this.dlgDataFolder.ShowNewFolderButton = true;
            this.dlgDataFolder.SelectedPath = this.txtDataFolder.Text;
            if (this.dlgDataFolder.ShowDialog() == DialogResult.OK)
            {
                this.txtDataFolder.Text = this.dlgDataFolder.SelectedPath;
                this.configManager.saveConfigFromControls();
            }
        }
        private void btCRUDFolder_Click(object sender, EventArgs e)
        {
            this.dlgCRUDFolder.ShowNewFolderButton = true;
            this.dlgCRUDFolder.SelectedPath = this.txtCRUDFolder.Text;
            if (this.dlgCRUDFolder.ShowDialog() == DialogResult.OK)
            {
                this.txtCRUDFolder.Text = this.dlgCRUDFolder.SelectedPath;
                this.configManager.saveConfigFromControls();
            }
        }
        private void btDestinationFolder_Click(object sender, EventArgs e)
        {
            this.dlgDestinationFolder.ShowNewFolderButton = true;
            this.dlgDestinationFolder.SelectedPath = this.txtDestinationFolder.Text;
            if (this.dlgDestinationFolder.ShowDialog() == DialogResult.OK)
            {
                this.txtDestinationFolder.Text = this.dlgDestinationFolder.SelectedPath;
                this.configManager.saveConfigFromControls();
            }
        }

        private void btReleaseScriptsFolder_Click(object sender, EventArgs e)
        {
            this.dlgReleaseScriptsFolder.ShowNewFolderButton = true;
            this.dlgReleaseScriptsFolder.SelectedPath = this.txtReleaseScriptsFolder.Text;
            if (this.dlgReleaseScriptsFolder.ShowDialog() == DialogResult.OK)
            {
                this.txtReleaseScriptsFolder.Text = this.dlgReleaseScriptsFolder.SelectedPath;
                this.configManager.saveConfigFromControls();
            }
        }


        private DeployScriptConfig cfg;
        private void btDeployScript_Click(object sender, EventArgs e)
        {
            this.dlgDeployScript.CheckFileExists = false;
            this.dlgDeployScript.CheckPathExists = false;
            this.dlgDeployScript.Filter = "Deployment scripts (*.deploy)|*.deploy";
            try
            {
                this.dlgDeployScript.InitialDirectory = Path.GetDirectoryName(this.txtDeployScript.Text);
            }
            catch
            {

            }
            if (this.dlgDeployScript.ShowDialog() == DialogResult.OK)
            {
                this.txtDeployScript.Text = this.dlgDeployScript.FileName;
                this.configManager.saveConfigFromControls();
                cfg = new DeployScriptConfig(this.txtDeployScript.Text, this.deployFileDatabaseAndScopeSeparator);                
            }
        }

        private void btGenerateDeployPkg_Click(object sender, EventArgs e)
        {
            doDeployPackade(false);            

        }

        private void btDeployPkgPreview_Click(object sender, EventArgs e)
        {
            doDeployPackade(true);
        }

        private void doDeployPackade(bool isPreview)
        {
            this.cfg = new DeployScriptConfig(this.txtDeployScript.Text, this.deployFileDatabaseAndScopeSeparator);
            
            //ProjectDeployFolder schema = new ProjectDeployFolder(this.txtSchemaFolder.Text, new OSFileSystem(), this.msgWriter, this.cfg);
            //ProjectDeployFolder data = new ProjectDeployFolder(this.txtDataFolder.Text, new OSFileSystem(), this.msgWriter, this.cfg);
            //FileSystemFolderTree dest = new FileSystemFolderTree(this.txtDestinationFolder.Text, new OSFileSystem(), this.msgWriter);

            //this.msgWriter.WriteLine(Color.Blue, "Copying files from schema: {0} ", (new DirectoryInfo(schema.node.ToString())).Name);
            //TreeSync.TreeSync ts = new TreeSync.TreeSync(schema, dest, null, this.msgWriter);
            //ts.preview = isPreview;
            //ts.doTreeSync("copy");

            //dest = new FileSystemFolderTree(this.txtDestinationFolder.Text, new OSFileSystem(), this.msgWriter);
            //this.msgWriter.WriteLine(Color.Blue, "Copying files from data: {0} ", (new DirectoryInfo(data.node.ToString())).Name);
            //ts = new TreeSync.TreeSync(data, dest, null, this.msgWriter);
            //ts.preview = isPreview;
            //ts.doTreeSync("copy");     


            string schema = this.txtSchemaFolder.Text;
            string data = this.txtDataFolder.Text;
            string crud = this.txtCRUDFolder.Text;
            string rootDest = this.txtDestinationFolder.Text;

            if (schema.Trim() == "" || data.Trim() == "" || crud.Trim() == "" || rootDest.Trim() == "")
            {
                MessageBox.Show("Please fill in all the fields, and try again.", "User Input Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            PackageDeployUtils u = new PackageDeployUtils(this.deployFileDatabaseAndScopeSeparator);
            u.copyDatabaseScriptsFromConfig(this.cfg, isPreview, schema, data, crud, rootDest, this.msgWriter);

        }        

        private void btGenerateReleaseScript_Click(object sender, EventArgs e)
        {
            string folder = this.txtReleaseScriptsFolder.Text;
            string fileName = this.txtReleaseScriptName.Text;

            if (folder.Trim() == "" || fileName.Trim() == "")
            {
                MessageBox.Show("Release scripts folder and release script name must both be specified", "User Input Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            PackageDeployUtils utils = new PackageDeployUtils(this.deployFileDatabaseAndScopeSeparator);          
            try
            {
                //the files get from GetFiles may be bottom to top so we need to resort them to make sure 
                //they are processed in the order of their name
                string[] files = Directory.GetFiles(folder, "*.deploy", SearchOption.TopDirectoryOnly);
                List<string> sortedFiles = new List<string>();
                sortedFiles.AddRange(files);
                sortedFiles.Sort();

                foreach (var src in sortedFiles)
                {
                    utils.addDeployFileToDatabases(src);
                }

                string dest = cb(folder, fileName);
                utils.saveDeployFile(dest, (key, value)=>
                {
                    this.msgWriter.WriteLine(Color.Blue, "Setting " + key + " as:", new LogLevel(Level.INFO));
                    this.msgWriter.WriteLine(value, new LogLevel(Level.DEBUG));
                });

                this.msgWriter.WriteLine(Color.Green, "DONE - Succesfully generated " + dest, new LogLevel(Level.INFO));
            }
            catch (Exception err)
            {
                this.msgWriter.WriteException(err);
            }
        }

        
    }
}
