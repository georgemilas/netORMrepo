using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using EM.Logging;
using EM.Util;

namespace DeploymentTools.Controls
{
    public partial class BaseControl : UserControl, IBaseControl 
    {      

        public BaseControl()
        {
            InitializeComponent();            
        }

        public virtual string labelName { get { throw new NotImplementedException(); } }

        private ConfigManager _configManager;
        public virtual ConfigManager configManager
        {
            get { return _configManager; }
            set 
            {

                if (_configManager == null && value != null)
                {
                    _configManager = value;
                    _configManager.OnSave += this.configManager_OnSave;
                    _configManager.OnInitControls += this.configManager_OnInitControls;

                    if (this.log != null)
                    {
                        _configManager.OnAfterLoadConfig += _configManager_OnAfterLoadConfig;                                                
                    }
                }
                else
                {
                    if (_configManager != null) { throw new InvalidOperationException("Configuration manager may only be set one time"); }
                }
            }
        }

        void _configManager_OnAfterLoadConfig(ConfigManager cfg)
        {
            if (this.log != null)
            {
                string d = cfg.config.setdefault("rolling_log_file_delete", "month");
                switch (d)
                {
                    case "day":
                        this.log.rollingTypeRemove = RollingTypeRemove.DayOld;
                        break;
                    case "week":
                        this.log.rollingTypeRemove = RollingTypeRemove.WeekOld;
                        break;
                    case "month":
                        this.log.rollingTypeRemove = RollingTypeRemove.MonthOld;
                        break;
                    case "three_months":
                        this.log.rollingTypeRemove = RollingTypeRemove.ThreeMonthsOld;
                        break;
                    case "six_month":
                        this.log.rollingTypeRemove = RollingTypeRemove.SixMonthOld;
                        break;
                    case "year":
                        this.log.rollingTypeRemove = RollingTypeRemove.YearOld;
                        break;

                }
            }
        }

        public virtual void configManager_OnInitControls(ConfigManager cfg) { throw new NotImplementedException(); }
        public virtual void configManager_OnSave(ConfigManager cfg) { throw new NotImplementedException(); }
        public virtual void cleanMessageBox() { throw new NotImplementedException(); }
        

        private MessageWriter _msgWriter;
        public MessageWriter msgWriter
        {
            get { return _msgWriter; }
            set { _msgWriter = value; }
        }


        private RollingFileLogger _log;
        protected RollingFileLogger log
        {
            get { return _log; }
            set 
            { 
                _log = value;
                _log.headerTemplate = this.LogHeader;
                _log.detailTemplate = this.LogDetail;
            }
        }

        private string LogHeader()
        {
            return " date :: msg ";
        }
        private string LogDetail(string appId, string logId, ILogLevel logLevel, string msg, string stackTrace)
        {
            return DateTime.Now.ToString() + " :: " + msg ;
        }


        public virtual void runInThread(ThreadStart func)
        {
            Thread t = new Thread(func);
            t.IsBackground = true;
            t.Start();            
        }

    }

}
