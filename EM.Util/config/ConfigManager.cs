using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Timers;

namespace EM.Util
{
    public class ConfigManager: IDisposable
    {
        public SimpleConfigParser config;

        public delegate void ConfigEventHandler(ConfigManager cfg);
        private Timer timerTick = new Timer();
        public event ConfigEventHandler OnBeforeLoadConfig;
        public event ConfigEventHandler OnAfterLoadConfig;

        //forward the event to SimpleConfigParser so it fires when someone actualy calls the save method on the SimpleConfigParser instance
        private Dictionary<ConfigEventHandler, SimpleConfigParser.ConfigEventHandler> _h = new Dictionary<ConfigEventHandler, SimpleConfigParser.ConfigEventHandler>();
        //private ConfigEventHandler _OnBeforeSavingConfig;
        public virtual event ConfigEventHandler OnBeforeSavingConfig
        {
            add 
            {
                _h[value] = new SimpleConfigParser.ConfigEventHandler(delegate(SimpleConfigParser cfg)
                {
                    value(this);
                });
                this.config.OnSave += _h[value];
                //_OnBeforeSavingConfig += value;
            }
            remove 
            { 
                this.config.OnSave -= _h[value];
                //_OnBeforeSavingConfig -= value;
            }            
        }

        public string configFile;

        public ConfigManager()
            : this((new DirectoryInfo(Assembly.GetExecutingAssembly().Location)).Parent.FullName + "\\config.cfg")
        { }

        public ConfigManager(string filePath)
        {
            this.configFile = filePath;
            this.config = new SimpleConfigParser();
            this.config.filePath = filePath;
            this.config.writeFileHeader = true;
            timerTick.Elapsed += new ElapsedEventHandler(timerTick_Elapsed);
            timerTick.Enabled = false;            
        }

        public void startLoadingLoop(int secondsInterval)
        {            
            timerTick.Enabled = true;
            timerTick.Interval = secondsInterval * 1000;  
        }

        public void stopLoadingLoop()
        {
            timerTick.Enabled = false;
        }

        private void timerTick_Elapsed(object sender, ElapsedEventArgs e)
        {
            this.loadConfig();
        }

        /// <summary>
        /// parses and loads the config file(s) 
        /// </summary>
        public void loadConfig()
        {
            if (this.OnBeforeLoadConfig != null)
            {
                this.OnBeforeLoadConfig(this);
            }

            FileInfo cfgFile = new FileInfo(this.configFile);
            if (!cfgFile.Exists) { cfgFile.Create().Close(); }

            this.config.filePath = cfgFile.FullName;
            this.config.construct(true);

            if (this.OnAfterLoadConfig != null)
            {
                this.OnAfterLoadConfig(this);
            }
        }

        public void setFormSizeAndPosition(System.Windows.Forms.Form frm)
        {
            setFormSizeAndPosition(frm, this.config);
        }
        public void setFormSizeAndPosition(System.Windows.Window frm)
        {
            setFormSizeAndPosition(frm, this.config);
        }
        public static void setFormSizeAndPosition(System.Windows.Forms.Form frm, SimpleConfigParser cfg)
        {
            string frmName = frm.Name + "_" + frm.Text;
            if (frm.Top > 5)  //not maximized
            {
                cfg[frmName + "_win_left"] = frm.Left.ToString();
                cfg[frmName + "_win_top"] = frm.Top.ToString();
                cfg[frmName + "_win_width"] = frm.Width.ToString();
                cfg[frmName + "_win_height"] = frm.Height.ToString();
            }
        }
        public static void setFormSizeAndPosition(System.Windows.Window frm, SimpleConfigParser cfg)
        {
            string frmName = frm.Name + "_" + frm.Title;
            if (frm.Top > 5)  //not maximized
            {
                cfg[frmName + "_win_left"] = frm.Left.ToString();
                cfg[frmName + "_win_top"] = frm.Top.ToString();
                cfg[frmName + "_win_width"] = frm.Width.ToString();
                cfg[frmName + "_win_height"] = frm.Height.ToString();
            }
        }
        public void getFormSizeAndPosition(System.Windows.Forms.Form frm)
        {
            getFormSizeAndPosition(frm, this.config);
        }
        public void getFormSizeAndPosition(System.Windows.Window frm)
        {
            getFormSizeAndPosition(frm, this.config);
        }
        public static void getFormSizeAndPosition(System.Windows.Forms.Form frm, SimpleConfigParser cfg)
        {
            string frmName = frm.Name + "_" + frm.Text;
            frm.Left = int.Parse(cfg.get(frmName + "_win_left", frm.Left.ToString()));
            frm.Top = int.Parse(cfg.get(frmName + "_win_top", frm.Top.ToString()));
            frm.Width = int.Parse(cfg.get(frmName + "_win_width", frm.Width.ToString()));
            frm.Height = int.Parse(cfg.get(frmName + "_win_height", frm.Height.ToString()));
            frm.StartPosition = System.Windows.Forms.FormStartPosition.Manual;            
        }
        public static void getFormSizeAndPosition(System.Windows.Window frm, SimpleConfigParser cfg)
        {
            try
            {
                string frmName = frm.Name + "_" + frm.Title;
                frm.Left = int.Parse(cfg.get(frmName + "_win_left", frm.Left.ToString()));
                frm.Top = int.Parse(cfg.get(frmName + "_win_top", frm.Top.ToString()));
                frm.Width = int.Parse(cfg.get(frmName + "_win_width", frm.Width.ToString()));
                frm.Height = int.Parse(cfg.get(frmName + "_win_height", frm.Height.ToString()));
            }
            catch { }
        }



        /// <summary>
        /// start your default editor for *.cfg files or notepad to edit the configuration file
        /// </summary>
        public void openConfigInExternalEditor()
        {
            if (this.config == null)
            {
                throw new InvalidDataException("Configuration file was not supplied");
            }
            
            Process proc = new Process();
            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.FileName = this.config.filePath; // "notepad";
            procInfo.UseShellExecute = true;
            proc.StartInfo = procInfo;
            try
            {
                proc.Start();
            }
            catch
            {
                //no program was associated with this file type, so use notepad
                procInfo.FileName = "notepad";
                procInfo.Arguments = this.config.filePath;
                proc.StartInfo = procInfo;
                proc.Start();
            }
        }


        #region IDisposable Members

        public void Dispose()
        {
            this.timerTick.Enabled = false;
            this.timerTick = null;
            if ( this.config != null )
            {
                try { this.config.save(); }
                catch { }
            }
        }

        #endregion
    }
}
