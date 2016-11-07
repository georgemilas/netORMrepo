using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using EM.Logging;
using System.IO;
using System.Reflection;
using System.Globalization;
using EM.Logging.Config;


namespace EM.Util.TickerService
{
    /// <summary>
    /// - logs to a file
    /// - reads  LOG_LEVEL (debug,info,warninf,error) and LOG_ENABLED(true/false) from app.config
    /// </summary>
    public class ServiceLogger: LoggerAsync
    {

        public TickerConfig config { get; set; }

        protected bool isEnabled 
        { 
            get 
            { 
                return config.IsLoggingEnabled; 
            } 
        }

        protected ServiceLogger() : this(new TickerConfig(), new ConfigLoggerFactory("TickerAsync")) { }
        protected ServiceLogger(TickerConfig config, ConfigLoggerFactory cl): base()
        {
            this.config = config;
            this.appId = cl.appId;
            this.level = config.LoggingType;

            if (this.isEnabled) 
            {
                try
                {
                    var clogger = cl.processConfigSection<Logger>("loggersSection");
                    foreach (var logger in clogger.loggers)
                    {
                        this.register(logger);
                    }
                    clogger.loggers.Clear();
                }
                catch (Exception err)
                {
                    this.register(new RollingFileLogger(this.appId, this.level, RollingType.Daily, RollingTypeRemove.MonthOld));
                    this.error(new LogMessage
                    {
                        e = err,
                        logId = "Core",
                        msg = "There was an error in the loggers section of the configuration file. Now using default loggers."
                    });
                }

                
            }
            else
            {
                this.consoleLogger = null;
            }            
        }

        public override void register(ILogger logger)
        {
            if (this.isEnabled)
            {
                base.register(logger);
            }
        }

        private static volatile ServiceLogger _instance;    //volatile -> make sure an assignement has happend before a read can be made
        private static object _locker = new Object();
        public static ServiceLogger instance
        {
            get
            {
                if (_instance == null)  //don't create a lock if instance exist
                {
                    lock ( _locker )    
                    {
                        if (_instance == null)
                        {
                            _instance = new ServiceLogger();
                        }                        
                    }
                }
                return _instance;
            }
        }

    }

}
