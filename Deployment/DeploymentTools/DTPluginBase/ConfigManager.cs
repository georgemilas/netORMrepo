using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using EM.Util;
using System.Diagnostics;
using System.Reflection;


namespace DeploymentTools
{
    public class ConfigManager
    {
        public SimpleConfigParser internalConfig;
        public SimpleConfigParser config;
        public delegate void ConfigEventHandler(ConfigManager cfg);
        public event ConfigEventHandler OnSave;
        public event ConfigEventHandler OnInitControls;
        public event ConfigEventHandler OnAfterLoadConfig;

        public DirectoryInfo folder;

        public ConfigManager()
        {
            this.folder = new DirectoryInfo(Assembly.GetExecutingAssembly().Location);
            //this.loadConfig();

        }
        
        /// <summary>
        /// parses and loads the config file(s) 
        /// </summary>
        public void loadConfig()
        {
            FileInfo cfgFile = new FileInfo(this.folder.Parent.FullName + "\\cfgloader.txt");  //Environment.CurrentDirectory + "\\config.cfg"
            if (!cfgFile.Exists) { cfgFile.Create().Close(); }
            this.internalConfig = SimpleConfigParser.parse(cfgFile.FullName, true, true);
            this.internalConfig.writeFileHeader = true;
            string cp = this.internalConfig.get("config_file", this.defaultConfigFilePath);
            if (cp.Trim() == "" || (cp != this.defaultConfigFilePath && !File.Exists(cp)))  //key exists in the file but is not set
            {
                cp = this.defaultConfigFilePath;
                this.internalConfig["config_file"] = cp;
                this.internalConfig.save();
            }
            this.configFilePath = cp;

            this.config.writeFileHeader = true;

            if (this.OnAfterLoadConfig != null)
            {
                this.OnAfterLoadConfig(this);
            }
        }

        public string defaultConfigFilePath
        {
            get { return this.folder.Parent.FullName + "\\config.cfg"; }
        }

        /// <summary>
        /// overrites config values with what it reads from the GUI controls
        /// and then saves the config files 
        /// </summary>
        public void saveConfigFromControls()
        {
            if (this.OnSave != null)
            {
                this.OnSave(this);
            }

            
            this.internalConfig.save();
            this.config.save();
        }

        /// <summary>
        /// sets values to controls on the GUI based on what it reads from the config files
        /// </summary>
        public void initControlsFromConfig()
        {
            if (this.OnInitControls != null)
            {
                this.OnInitControls(this);
            }
        }

        private string _configFilePath;
        public string configFilePath
        {
            get  { return this._configFilePath; }
            set
            {
                this._configFilePath = value;
                FileInfo cfgFile = new FileInfo(this._configFilePath);
                if (!cfgFile.Exists) { cfgFile.Create().Close(); }

                this.config = SimpleConfigParser.parse(cfgFile.FullName, true, true);
                //if (this.config != null)
                //{
                //    if (this.config.filePath.ToLower().Trim() != this._configFilePath.ToLower().Trim())
                //    {
                //        this.config = SimpleConfigParser.parse(cfgFile.FullName, true, true);                        
                //    }
                //else do nothing
                //}
                //else
                //{
                //    this.config = SimpleConfigParser.parse(cfgFile.FullName, true, true);                    
                //}                
            }
        }

        
        /// <summary>
        /// start notepad to edit the main configuration file
        /// </summary>
        public void editConfig()
        {
            if (this.config == null)
            {
                throw new InvalidDataException("Configuration file was not supplied");
            }
            else
            {
                //save what we customized so far
                this.saveConfigFromControls();
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

    }
}
