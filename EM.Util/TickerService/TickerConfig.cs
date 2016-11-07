using System;
using System.Collections.Generic;
//using System.Linq;
using System.Text;
using EM.Util;
using EM.Logging;
using EM.Collections;
//using System.ServiceModel.Configuration;
using System.Configuration;

namespace EM.Util.TickerService
{
    public class TickerConfig
    {
        public TickerConfig()
        { 
        }

        /// <summary>
        /// one second
        /// </summary>
        public double DefaultTickerIntervalSeconds { get { return 1; } }        //one second

        public bool IsLoggingEnabled
        {
            get
            {
                string cfg = ConfigurationManager.AppSettings["LOG_ENABLED"];
                if (cfg == null || cfg.Trim() == "") cfg = "true";
                return cfg.ToLower().Trim() == "true" ? true : false;
            }
        }

        public LogLevel LoggingType
        {
            get
            {
                string cfg = ConfigurationManager.AppSettings["LOG_LEVEL"];
                if (cfg == null || cfg.Trim() == "") cfg = "debug";
                return new LogLevel(cfg);
            }
        }

        /// <summary>
        /// Defaults to service if nothing was specified
        /// </summary>
        public ApplicationStartType ApplicationStartType
        {
            get
            {
                string val = ConfigurationManager.AppSettings["START_TYPE"];
                if (val == null || val.Trim() == "") val = "service";
                return val.Trim().ToLower() == "service" ? ApplicationStartType.WindowsService : ApplicationStartType.TestConsoleApp;
            }
        }

    }
}
