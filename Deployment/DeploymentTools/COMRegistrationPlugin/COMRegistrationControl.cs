using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using EM.Logging;
using EM.Util;
using System.Threading;
using DeploymentTools.Parallel;

namespace DeploymentTools.Controls
{
    public partial class COMRegistrationControl : BaseControl, IPlugin
    {
        
        private RemoteServers comServers;
        
        public COMRegistrationControl(): base()
        {
            InitializeComponent();
            this.msgWriter = new RichTextBoxMessageWriter(this.txtMessageBox, this);
            this.log = new RollingFileLogger("COMRegister", new LogLevel(Level.INFO), RollingType.Weekly, RollingTypeRemove.ThreeMonthsOld);
            this.msgWriter.register(this.log);
        }

        public override string labelName
        {
            get { return "COM Registration"; }
        }

        public override void cleanMessageBox()
        {
            this.txtMessageBox.Clear();
        }

        private string errForCfg = null;
        public override void configManager_OnInitControls(ConfigManager cfg)
        {
            //COM
            this.txtSrc.Text = cfg.config.get("com_source_folder", "");
            this.txtDestLocal.Text = cfg.config.get("com_destination_folder", "C:\\Objects");
            this.txtExclude.Text = cfg.config.get("com_exceptions", "");
            string cservers = cfg.config.get("com_servers", @"web-dev, \\web-dev\EDrive\Objects, E:\Objects
                 ts-node1, \\tsnode1-dev\c$\Objects, C:\Objects");
            this.comServers = new RemoteServers(cservers);
            if (this.comServers.ignoredServers.Count > 0)
            {
                if (errForCfg == null || errForCfg != cfg.configFilePath)
                {
                    MessageBox.Show("There are duplicated COM servers in the config file, only one instance of them will be used. The instances that are ignored are:\r\n" + this.comServers.ignoredServersConfig, "Ignored COM Servers:", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    errForCfg = cfg.configFilePath;
                }
            }

            this.lbDestRemote.Text = this.comServers.ToString();

            string runParal = cfg.config.get("com_run_parallel", "true");
            if (runParal == "true") { this.chkCOMRunInParalel.Checked = true; }
            else { this.chkCOMRunInParalel.Checked = false; }

            this.splitMain.SplitterDistance = int.Parse(cfg.internalConfig.get("com_split_at", "220"));


        }

        public override void configManager_OnSave(ConfigManager cfg)
        {
            //COM
            cfg.config["com_source_folder"] = this.txtSrc.Text;
            cfg.config["com_destination_folder"] = this.txtDestLocal.Text;
            cfg.config["com_exceptions"] = this.txtExclude.Text;
            cfg.config.setdefault("com_servers", @"web-dev, \\web-dev\EDrive\Objects, E:\Objects
                 #web-test, \\web-test\EDrive\Objects, E:\Objects
                 ts-node1, \\tsnode1-dev\c$\Objects, C:\Objects");

            cfg.config["com_run_parallel"] = this.chkCOMRunInParalel.Checked ? "true" : "false";
            cfg.internalConfig["com_split_at"] = this.splitMain.SplitterDistance.ToString();
        }


        private void btSrc_Click(object sender, EventArgs e)
        {
            this.dlgCOMSourceFolder.ShowNewFolderButton = true;
            this.dlgCOMSourceFolder.SelectedPath = this.txtSrc.Text;
            if (this.dlgCOMSourceFolder.ShowDialog() == DialogResult.OK)
            {
                this.txtSrc.Text = this.dlgCOMSourceFolder.SelectedPath;
                this.configManager.saveConfigFromControls();
            }
        }

        private void btExcludeHelp_Click(object sender, EventArgs e)
        {
            string s = "A pattern for exceptions that should not be registered to COM\n";
            s += "Examples:\n";
            s += "  dcm\n";
            s += "    - will exclude components that have dcm in the name of the file\n";
            s += "  dcm util\n";
            s += "    - exclude dcm and util\n";
            s += "  dcm (util and not utilTools)\n";
            s += "    - exclude dcm and util but if there is one called utilTools, include it\n";

            MessageBox.Show(s);
        }

        private void btDestLocal_Click(object sender, EventArgs e)
        {
            this.dlgCOMDestinationFolder.ShowNewFolderButton = true;
            this.dlgCOMDestinationFolder.SelectedPath = this.txtDestLocal.Text;
            if (this.dlgCOMDestinationFolder.ShowDialog() == DialogResult.OK)
            {
                this.txtDestLocal.Text = this.dlgCOMDestinationFolder.SelectedPath;
                this.configManager.saveConfigFromControls();
            }
        }

        private void btDestRemoteHelp_Click(object sender, EventArgs e)
        {
            string s = "This are the destination servers and the paths where the COM DLL's will be copied to and registered from. \n\n";
            s += "To change the list of servers and the paths:\n\t click \"Edit Config\" to open the file in notepad, edit the [com_servers] configuration tag and save the changes \n\t click \"Load Config\" to reload the new settings:\n";

            MessageBox.Show(s);
        }


        public void registerOnServer(RemoteServer server)
        {
            ComponentsRegistration com = new ComponentsRegistration(this.msgWriter);
            com.registrationExceptions = this.txtExclude.Text;
            this.msgWriter.WriteLine(Color.Blue, "\n" + "Thread: " + Thread.CurrentThread.ManagedThreadId.ToString() + " Start Registration for: " + server.name, new LogLevel(Level.INFO));
            com.register(this.txtSrc.Text, new COMDestination(server), false);
            this.msgWriter.WriteLine(Color.Blue, "Thread: " + Thread.CurrentThread.ManagedThreadId.ToString() + " Registration is Done for: " + server.name, new LogLevel(Level.INFO));
        }

        private void btRegisterRemote_Click(object sender, EventArgs e)
        {
            this.runInThread(delegate()
            {
                try
                {
                    this.btRegisterRemote.Enabled = false;
                    this.btRegisterLocal.Enabled = false;

                    if (this.txtSrc.Text.Trim() != "" && this.comServers.Count > 0)
                    {
                        
                        if (this.chkCOMRunInParalel.Checked)
                        {
                            this.msgWriter.WriteLine(Color.Blue, "\nStart new Components Registration on remote servers in parallel", new LogLevel(Level.INFO));
                            TimeSpan s = TimeTracker.trackTimeForAction(() =>
                            {
                                ParallelTasksRunner.runParallel(this.comServers, new WorkOnServer(registerOnServer));
                            });
                            this.msgWriter.WriteLine(Color.Green, "Done, took " + s.TotalSeconds + " seconds", new LogLevel(Level.INFO));                            
                        }
                        else
                        {
                            this.msgWriter.WriteLine(Color.Blue, "\nStart new Components Registration on remote servers", new LogLevel(Level.INFO));
                            TimeSpan s = TimeTracker.trackTimeForAction(() =>
                            {
                                foreach (RemoteServer server in this.comServers.Values)
                                {
                                    registerOnServer(server);
                                }
                            });
                            this.msgWriter.WriteLine(Color.Green, "Done, took " + s.TotalSeconds + " seconds", new LogLevel(Level.INFO));
                        }
                        this.msgWriter.WriteLine(Color.DarkGreen, "\nComponents Registration is Done for all remote servers\n", new LogLevel(Level.INFO));
                    }
                    else
                    {
                        MessageBox.Show("Please enter the path to where you have the COM components and also configure the remote servers list");
                    }

                }
                catch (System.Security.SecurityException ser)
                {
                    this.msgWriter.WriteException(ser);
                    this.msgWriter.WriteLine(Color.Brown, "You should run the program from a local drive (C:\\)\n", new LogLevel(Level.ERROR));
                }
                catch (Exception er)
                {
                    this.msgWriter.WriteException(er);
                }
                finally
                {
                    this.btRegisterRemote.Enabled = true;
                    this.btRegisterLocal.Enabled = true;
                }

            });
        }

        private void btRegisterLocal_Click(object sender, EventArgs e)
        {
            this.runInThread(delegate()
            {
                try
                {
                    this.btRegisterLocal.Enabled = false;
                    this.btRegisterRemote.Enabled = false;

                    if (this.txtSrc.Text.Trim() != "" && this.txtDestLocal.Text.Trim() != "")
                    {
                        ComponentsRegistration com = new ComponentsRegistration(this.msgWriter);
                        com.registrationExceptions = this.txtExclude.Text;
                        this.msgWriter.WriteLine(Color.Blue, "\nStart new Components Registration", new LogLevel(Level.INFO));
                        com.register(this.txtSrc.Text, new COMDestination(this.txtDestLocal.Text), false);
                        this.msgWriter.WriteLine(Color.DarkGreen, "\nComponents Registration is Done\n", new LogLevel(Level.INFO));
                    }
                    else
                    {
                        MessageBox.Show("Please enter the path to where you have the COM components and also the path to where they should be registered from");
                    }

                }
                catch (System.Security.SecurityException ser)
                {
                    this.msgWriter.WriteException(ser);
                    this.msgWriter.WriteLine(Color.Brown, "You should run the program from a local drive (C:\\)\n", new LogLevel(Level.ERROR));
                }
                catch (Exception er)
                {
                    this.msgWriter.WriteException(er);
                }
                finally
                {
                    this.btRegisterLocal.Enabled = true;
                    this.btRegisterRemote.Enabled = true;
                }
            });
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

        
    }
}
