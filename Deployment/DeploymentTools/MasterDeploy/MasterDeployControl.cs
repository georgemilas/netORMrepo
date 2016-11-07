using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DeploymentTools.Controls;
using EM.Util;
using System.IO;
using Common.SurePayroll.Resource.Reader;
using System.Threading.Tasks;
using EM.Logging;
using Common.SurePayroll.TeamFoundationServer;
using System.Threading;
using EM.Collections;
using System.Diagnostics;
using DeploymentTools;
using System.Xml;
using TreeSync;


namespace MasterDeploy
{
    public partial class MasterDeployControl : BaseControl, IPlugin
    {
        public MasterDeployControl(): base()
        {
            InitializeComponent();
            this.msgWriter = new RichTextBoxMessageWriter(this.txtMessageBox, this);
            this.configManager_OnInitControls(this.configManager);
        }

        public override string labelName
        {
            get { return "Master Deploy"; }
        }

        public override void cleanMessageBox()
        {
            this.txtMessageBox.Clear();
        }

        public override void configManager_OnInitControls(DeploymentTools.ConfigManager cfg)
        {
            var defaultPath = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "DEV_Deployment.xml");
            if (cfg != null)
            {
                this.txtXMLConfiguration.Text = cfg.config.get("masterdeploy_deploy_config_file", defaultPath);
                this.txtPackageFolder.Text = cfg.config.get("masterdeploy_deploy_root", @"C:\Test\DeployTest");
            }
            else
            {
                this.txtXMLConfiguration.Text = defaultPath;
                this.txtPackageFolder.Text = @"C:\Test\DeployTest";
            }
            
        }

        public override void configManager_OnSave(DeploymentTools.ConfigManager cfg)
        {
            cfg.config["masterdeploy_deploy_config_file"] = this.txtXMLConfiguration.Text;
            cfg.config["masterdeploy_deploy_root"] = this.txtPackageFolder.Text;
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

        
        private void btXMLConfigFile_Click(object sender, EventArgs e)
        {
            this.dlgXMLConfigurationOpen.CheckFileExists = false;
            this.dlgXMLConfigurationOpen.CheckPathExists = false;
            this.dlgXMLConfigurationOpen.Filter = @"Configuration files (*.xml)|*.xml|All Files|*.*";
            this.dlgXMLConfigurationOpen.InitialDirectory = this.txtXMLConfiguration.Text;
            if (this.dlgXMLConfigurationOpen.ShowDialog() == DialogResult.OK)
            {
                this.txtXMLConfiguration.Text = this.dlgXMLConfigurationOpen.FileName;
                try
                {
                    this.configManager.saveConfigFromControls();
                }
                catch(Exception err) 
                {
                    //running in isolation not as part of Deployment Tools so there is no Config Manager file
                }
            }
        }

        private void btPackageFolder_Click(object sender, EventArgs e)
        {
            //this.dlgPackageFolder.RootFolder = Path.GetDirectoryName(this.txtPackageFolder.Text);
            this.dlgPackageFolder.ShowNewFolderButton = true;
            this.dlgPackageFolder.SelectedPath = this.txtPackageFolder.Text;
            if (this.dlgPackageFolder.ShowDialog() == DialogResult.OK)
            {
                this.txtPackageFolder.Text = this.dlgPackageFolder.SelectedPath;
                try
                {
                    this.configManager.saveConfigFromControls();
                }
                catch (Exception err)
                {
                    //running in isolation not as part of Deployment Tools so there is no Config Manager file
                }
            }
            showDeployablePackages();
        }

        private object _lockObj = new object();
        private void showDeployablePackages()
        {
            this.flowLayoutPanelBuildSolutions.Controls.Clear();
            var threadStartValue = txtPackageFolder.Text;
            if (!Directory.Exists(threadStartValue)) 
            {
                return;    
            }

            if (this.msgWriter == null)
            {
                this.msgWriter = new RichTextBoxMessageWriter(this.txtMessageBox, this);
            }

            
            Task.Factory.StartNew(() =>
            {                
                if (threadStartValue == txtPackageFolder.Text)
                {
                    try
                    {
                        //Thread.SpinWait(1000); //allow user to type

                        xml = null; //reset the config
                        bool doit = parseConfig(false);                        
                        if (doit)
                        {
                            IEnumerable<DeployAction> actions = new List<DeployAction>();
                            actions = xml.DeployActions;

                            lock (_lockObj)
                            {
                                this.Invoke(new ThreadStart(delegate
                                {
                                    this.flowLayoutPanelBuildSolutions.Controls.Clear();
                                }));

                                //bool something = false;
                                foreach (var g in actions)
                                {
                                    CheckBox cb = new CheckBox()
                                    {
                                        Text = g.name.Trim(),
                                        AutoSize = true,
                                        Checked = g.disable ? false : Directory.Exists(g.source),
                                        Enabled = !g.disable
                                    };
                                    DeployAction g1 = g;
                                    cb.CheckedChanged += (o, s) =>
                                    {
                                        g1.skipDeploy = !cb.Checked;
                                    };
                                    //if (cb.Checked) { something = true; }
                                    this.Invoke(new ThreadStart(delegate
                                    {
                                        this.flowLayoutPanelBuildSolutions.Controls.Add(cb);
                                    }));
                                }
                            }

                            //the intention is to make sure that the deploy script has all posible packages
                            //so we don't try on whatever folder out there to get missing stuff (for exaple C:)
                            //so we only try if we do actualy have something to deploy but may be missing others
                            //if (something)
                            //{
                            //    CheckForUnhandledFolders(threadStartValue, actions);
                            //}
                        }
                        else
                        {
                            this.Invoke(new ThreadStart(delegate
                            {
                                this.flowLayoutPanelBuildSolutions.Controls.Clear();
                            }));
                        }
                    }
                    catch (Exception err)
                    {
                        this.msgWriter.WriteException(err);
                    }
                }                
            });            
        }

        private void CheckForUnhandledFolders(string threadStartValue, IEnumerable<DeployAction> actions)
        {
            this.btDeploy.Enabled = false;
            this.btPreview.Enabled = false;
            this.btCheckMissing.Enabled = false;
            if (this.msgWriter == null)
            {
                this.msgWriter = new RichTextBoxMessageWriter(this.txtMessageBox, this);
            }

            Task.Factory.StartNew(() =>
            {
                try
                {
                    var root = new TreeSync.FileSystemFolderTree(threadStartValue, new OSFileSystem(), null);
                    MissingDeployFinder df = new MissingDeployFinder(root, actions);
                    df.walk();
                    if (df.missing.Count > 0)
                    {
                        this.Invoke(new ThreadStart(delegate
                        {
                            this.flowLayoutPanelBuildSolutions.Controls.Add(new Label()
                            {
                                Text = "Missing: ",
                                ForeColor = Color.Red,
                                AutoSize = true
                            });
                        }));

                        bool added = false;
                        foreach (var m in df.missing)
                        {
                            if (added)
                            {
                                this.Invoke(new ThreadStart(delegate
                                {
                                    this.flowLayoutPanelBuildSolutions.Controls.Add(new Label()
                                    {
                                        Text = " | ",
                                        ForeColor = Color.Red,
                                        AutoSize = true
                                    });
                                }));
                            }
                            this.Invoke(new ThreadStart(delegate
                            {
                                this.flowLayoutPanelBuildSolutions.Controls.Add(new Label()
                                {
                                    Text = m.Replace(threadStartValue, "").Trim(),
                                    ForeColor = Color.Red,
                                    AutoSize = true
                                });
                            }));
                            added = true;
                        }
                    }
                }
                catch (Exception err)
                {
                    this.msgWriter.WriteException(err);                    
                }
                finally
                {
                    this.Invoke(new ThreadStart(delegate
                    {
                        this.btDeploy.Enabled = true;
                        this.btPreview.Enabled = true;
                        this.btCheckMissing.Enabled = true;
                    }));
                }
            });
        }

        private void btDeploy_Click(object sender, EventArgs e)
        {
            doMasterDeploy(false);

        }

        private void btPreview_Click(object sender, EventArgs e)
        {
            doMasterDeploy(true);
        }

        private DeploymentXMLConfiguration xml = null;
        
        private bool parseConfig(bool showErrors)
        {            
            if (xml != null)
            {
                xml.reload();
                return true;
            }

            string conf = this.txtXMLConfiguration.Text;
            string root = this.txtPackageFolder.Text;
            try
            {
                xml = new DeploymentXMLConfiguration(conf);
                xml.rootFolder = root;
                xml.parse();
                return true;
            }
            catch (Exception err)
            {
                this.msgWriter.WriteException(err);

                xml = null;
                if (showErrors)
                {
                    this.msgWriter.WriteLine(Color.Red, "Master Depploy did not finish, there were errors.", new LogLevel(Level.ERROR));
                }
                return false;
            }                        
        }

        
        private ErrorsLogScreen errOnly;
        public void doShowErrors()
        {
            if (errOnly.HasErrors)
            {
                errOnly.Show();
            }
            else
            {
                errOnly.Close();
            }
        }
        
        private void doMasterDeploy(bool preview)
        {
            this.btDeploy.Enabled = false;
            this.btPreview.Enabled = false;
            this.btCheckMissing.Enabled = false;
            string conf = this.txtXMLConfiguration.Text;
            string root = this.txtPackageFolder.Text;
            if (this.msgWriter == null)
            {
                this.msgWriter = new RichTextBoxMessageWriter(this.txtMessageBox, this);
            }

            string logFilePath =  Path.Combine(root, "MasterDeploy_" + DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss") + ".log");
            var fileLogger = new FileLogger("MasterDeploy", Level.DEBUG, logFilePath);
            var fileErrorsLogger = new FileLogger("MasterDeploy_ErrorsOnly", Level.ERROR, logFilePath.Replace("MasterDeploy_", "MasterDeploy_ErrorsOnly_"));
            this.msgWriter.register(fileLogger);
            this.msgWriter.register(fileErrorsLogger);

            errOnly = new ErrorsLogScreen(this.msgWriter);
            //need to do show/hide so that doShowErrors does not throw cross thread errors when called (InvokeRequired is false, and even if we do call Invoke, it fails anyway again with cross thread errors again)
            errOnly.Show();
            errOnly.Hide();            
            
            Task.Factory.StartNew(() =>
            {                
                try
                {                    
                    bool doit = parseConfig(true);
                    if (doit)
                    {
                        this.msgWriter.WriteLine(Color.Blue, "The following will be deployed:", new LogLevel(Level.INFO));
                        foreach (var ac in xml.DeployActions)
                        {
                            if (Directory.Exists(ac.source))
                            {
                                if (!ac.skipDeploy)
                                {
                                    this.msgWriter.WriteLine(Color.Blue, ac.GetType().Name + " from: " + ac.name, new LogLevel(Level.INFO));
                                }
                                else
                                {
                                    this.msgWriter.WriteLine(Color.DarkGray, "SKIP " + ac.GetType().Name + " from: " + ac.name, new LogLevel(Level.DEBUG));
                                }
                            }
                        }
                        this.msgWriter.WriteLine("", new LogLevel(Level.DEBUG));

                        bool ok = true;
                        foreach (var ac in xml.DeployActions)
                        {
                            if (Directory.Exists(ac.source))
                            {
                                if (!ac.skipDeploy)
                                {
                                    if (preview)
                                    {
                                        this.msgWriter.WriteLine(Color.Blue, "Preview " + ac.GetType().Name + " from: " + ac.source, new LogLevel(Level.INFO));
                                        ok = ac.PreviewDeploy(this.msgWriter) && ok;
                                    }
                                    else
                                    {
                                        this.msgWriter.WriteLine(Color.Blue, ac.GetType().Name + " deploy from: " + ac.source, new LogLevel(Level.INFO));
                                        ok = ac.Deploy(this.msgWriter) && ok;
                                    }
                                    if (!ok) { break; }
                                }
                                else
                                {
                                    this.msgWriter.WriteLine(Color.DarkGray, "SKIP " + ac.GetType().Name + " from: " + ac.source, new LogLevel(Level.DEBUG));
                                }
                            }
                        }
                        if (ok)
                        {
                            this.msgWriter.WriteLine(Color.DarkGreen, "Master Depploy finished successfuly", new LogLevel(Level.INFO));
                        }
                        else
                        {
                            this.msgWriter.WriteLine(Color.Red, "Master Depploy did not finish, there were errors.", new LogLevel(Level.ERROR));
                        }
                        this.msgWriter.WriteLine(Color.Blue, "Master Depploy details are in " + logFilePath, new LogLevel(Level.DEBUG));
                    }
                }
                catch (Exception err)
                {
                    this.msgWriter.WriteException(err);
                    this.msgWriter.WriteLine(Color.Red, "Master Depploy did not finish, there were errors.", new LogLevel(Level.ERROR));
                }
                finally
                {
                    this.Invoke(new ThreadStart(delegate
                    {
                        this.btDeploy.Enabled = true;
                        this.btPreview.Enabled = true;
                        this.btCheckMissing.Enabled = true;
                        this.msgWriter.unregister(fileLogger);
                        this.msgWriter.unregister(fileErrorsLogger);
                        doShowErrors();
                    }));                 
                }               

            });
        }

        
        public bool calculating = false;
        public int changeCnt = 0;
        private void txtPackageFolder_TextChanged(object sender, EventArgs e)
        {
            showDeployablePackages();
        }

        private void btEditXmlConfigFile_Click(object sender, EventArgs e)
        {
            Process proc = new Process();
            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.FileName = txtXMLConfiguration.Text; // "notepad";
            procInfo.UseShellExecute = true;
            proc.StartInfo = procInfo;
            try
            {
                proc.Start();
            }
            catch
            {
                try
                {
                    //no program was associated with this file type, so use notepad
                    procInfo.FileName = "notepad";
                    procInfo.Arguments = txtXMLConfiguration.Text;
                    proc.StartInfo = procInfo;
                    proc.Start();
                }
                catch (Exception err)
                {
                    this.msgWriter.WriteLine(Color.Red, "Could not open configuration", new LogLevel(Level.ERROR));
                    this.msgWriter.WriteException(err);      
                }
            }
        }

        private void btCheckMissing_Click(object sender, EventArgs e)
        {
            var threadStartValue = txtPackageFolder.Text;
            if (!Directory.Exists(threadStartValue))
            {
                return;
            }
            if (xml == null || xml.DeployActions == null)
            {
                return;
            }
            CheckForUnhandledFolders(threadStartValue, xml.DeployActions);
        }



    }
}
