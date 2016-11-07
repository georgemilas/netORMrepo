using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using TreeSync;
using EM.Logging;
using System.IO;

namespace DeploymentTools.Controls
{
    public partial class FoldersSyncControl : BaseControl
    {
        public delegate void OnCancelHandler();
        public event OnCancelHandler OnCancel;

        public FoldersSyncControl()
            : base()
        {
            InitializeComponent();
            this.msgWriter = new RichTextBoxMessageWriter(this.txtMessageBox, this);
            this.log = new RollingFileLogger("FolderSync", new LogLevel(Level.INFO), RollingType.Weekly, RollingTypeRemove.ThreeMonthsOld);
            this.msgWriter.register(this.log);
            this.btSyncCancel.Enabled = false;
        }

        public override string labelName
        {
            get { return "Syncronize Folders"; }
        }
        public override void cleanMessageBox()
        {
            this.txtMessageBox.Clear();
        }

        public override void configManager_OnSave(ConfigManager cfg)
        {
            //SYNC
            cfg.config["sync_source_folder"] = this.txtSyncSrc.Text;
            cfg.config["sync_destination_folder"] = this.txtSyncDest.Text;
            cfg.config["sync_exceptions"] = this.txtSyncExclude.Text;

            if (this.radioSync.Checked) { cfg.config["sync_method"] = "synchronize"; }
            else if (this.radioMerge.Checked) { cfg.config["sync_method"] = "merge"; }
            else if (this.radioRefresh.Checked) { cfg.config["sync_method"] = "refresh"; }
            else if (this.radioGetNew.Checked) { cfg.config["sync_method"] = "getNew"; }
            else if (this.radioCopy.Checked) { cfg.config["sync_method"] = "copy"; }
            
            cfg.config["sync_multi_source"] = this.chkMultiSource.Checked ? "true" : "false";
            cfg.internalConfig["sync_split_at"] = this.splitMain.SplitterDistance.ToString();
        }

        public override void configManager_OnInitControls(ConfigManager cfg)
        {
            //SYNC
            this.txtSyncSrc.Text = cfg.config.get("sync_source_folder", "");
            this.txtSyncDest.Text = cfg.config.get("sync_destination_folder", "");
            this.txtSyncExclude.Text = cfg.config.get("sync_exceptions", "");

            string sm = cfg.config.get("sync_method", "refresh");
            if (sm == "synchronize" || sm == "s") { this.radioSync.Checked = true; }
            else if (sm == "merge" || sm == "m") { this.radioMerge.Checked = true; }
            else if (sm == "refresh" || sm == "r") { this.radioRefresh.Checked = true; }
            else if (sm == "getNew" || sm == "g") { this.radioGetNew.Checked = true; }
            else if (sm == "copy" || sm == "c") { this.radioCopy.Checked = true; }

            if ( cfg.config.get("sync_multi_source", "false").ToLower().Trim() == "true")
            {
                this.chkMultiSource.Checked = true;
            }
            this.splitMain.SplitterDistance = int.Parse(cfg.internalConfig.get("sync_split_at", "210"));
        }

        private void btSyncSrc_Click(object sender, EventArgs e)
        {
            this.dlgSyncSrc.ShowNewFolderButton = true;
            this.dlgSyncSrc.SelectedPath = this.txtSyncSrc.Text;
            if (this.dlgSyncSrc.ShowDialog() == DialogResult.OK)
            {
                this.txtSyncSrc.Text = this.dlgSyncSrc.SelectedPath;
                this.configManager.saveConfigFromControls();
            }
        }

        private void btSyncDest_Click(object sender, EventArgs e)
        {
            this.dlgSyncDest.ShowNewFolderButton = true;
            this.dlgSyncDest.SelectedPath = this.txtSyncDest.Text;
            if (this.dlgSyncDest.ShowDialog() == DialogResult.OK)
            {
                this.txtSyncDest.Text = this.dlgSyncDest.SelectedPath;
                this.configManager.saveConfigFromControls();
            }
        }

        private void btSyncExcludeHelp_Click(object sender, EventArgs e)
        {
            string s = "A pattern for exluding files/folders from syncronization \n";
            s += "Examples:\n";
            s += "  \"4. Data\"\n";
            s += "    - will exclude all files insite the \"4. Data\" folder\n";
            s += "  \"4. Data\" and not \"add page rights.sql\"\n";
            s += "    - will exclude all files inside the \"4. Data\" folder except for the \"add page rights.sql\" file which will be considered\n";

            MessageBox.Show(s);
        }

        private void btSyncPreviw_Click(object sender, EventArgs e)
        {
            runInThread(delegate()
            {
                if (this.txtSyncSrc.Text.Trim() != "" && this.txtSyncDest.Text.Trim() != "")
                {
                    try
                    {
                        this.btSyncRun.Enabled = false;
                        this.btSyncPreviw.Enabled = false;
                        this.btSyncCancel.Enabled = true;

                        if (this.chkMultiSource.Checked)
                        {
                            string[] sources = Directory.GetDirectories(this.txtSyncSrc.Text);
                            foreach (string source in sources)
                            {
                                DoFolderSync(source, this.txtSyncDest.Text, true);
                                this.msgWriter.WriteLine(Color.DarkGreen,
                                                         string.Format("Done doing {0}\n", source), new LogLevel(Level.INFO));
                            }
                        }
                        else
                        {
                            DoFolderSync(this.txtSyncSrc.Text, this.txtSyncDest.Text, true);
                        }
                        this.msgWriter.WriteLine(Color.DarkGreen, "Syncronize Folders finished\n", new LogLevel(Level.INFO));
                    }
                    finally
                    {
                        this.btSyncRun.Enabled = true;
                        this.btSyncPreviw.Enabled = true;
                        this.btSyncCancel.Enabled = false;
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Please enter the path to source and destination folders to be syncronized");
                }
            });
        }

        private void btSyncRun_Click(object sender, EventArgs e)
        {
            runInThread(delegate()
            {
                if (this.txtSyncSrc.Text.Trim() != "" && this.txtSyncDest.Text.Trim() != "")
                {
                    
                    try
                    {
                        this.btSyncRun.Enabled = false;
                        this.btSyncPreviw.Enabled = false;
                        this.btSyncCancel.Enabled = true;

                        if (this.chkMultiSource.Checked)
                        {
                            string[] sources = Directory.GetDirectories(this.txtSyncSrc.Text);
                            foreach (string source in sources)
                            {
                                DoFolderSync(source, this.txtSyncDest.Text, false);
                                this.msgWriter.WriteLine(Color.DarkGreen, string.Format("Done doing {0}\n", source), new LogLevel(Level.INFO));        
                            }
                        }
                        else
                        {
                            DoFolderSync(this.txtSyncSrc.Text, this.txtSyncDest.Text, false);
                        }

                    }
                    catch (Exception er)
                    {
                        this.msgWriter.WriteException(er);
                    }
                    finally
                    {
                        this.msgWriter.WriteLine(Color.DarkGreen, "Syncronize Folders finished\n", new LogLevel(Level.INFO));
                        this.btSyncRun.Enabled = true;
                        this.btSyncPreviw.Enabled = true;
                        this.btSyncCancel.Enabled = false;
                    }
                }
                else
                {
                    MessageBox.Show("Please enter the path to source and destination folders to be syncronized");
                }
            });     
        }

        private void DoFolderSync(string srcPath, string destPath, bool preview)
        {
            FileSystemFolderTree src = new FileSystemFolderTree(srcPath, new OSFileSystem(), this.msgWriter);
            FileSystemFolderTree dest = new FileSystemFolderTree(destPath, new OSFileSystem(), this.msgWriter);

            TreeSync.TreeSync ts = new TreeSync.TreeSync(src, dest, this.txtSyncExclude.Text, this.msgWriter);
            ts.preview = preview;
            this.configManager.saveConfigFromControls();
            var stopper = new OnCancelHandler(() => { ts.cancelAllWorkers(); });
            this.OnCancel += stopper;
            ts.doTreeSync(this.configManager.config["sync_method"]);
            this.OnCancel -= stopper;
        }

        private void splitMain_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btSyncCancel_Click(object sender, EventArgs e)
        {
            if (this.OnCancel != null)
            {
                OnCancel();
            }           
        }



    }
}
