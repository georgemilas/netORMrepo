using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.Odbc;
using EM.Collections;

namespace EM.Logging
{
    /// <summary>
    /// - a logger (ILogger) and also a collection of loggers
    /// - the registered Loggers will inherit the level set here and disregard whatever level they were created with   
    /// to log: 
    ///        log = new LevelLogger(LogLevel.Level.DEBUG);
    ///        log.register(new MailLogger(LogLevel.Level.WARN)); 
    ///        log.register(new FileLogger(LogLevel.Level.INFO)); 
    /// 
    ///        log.debug(log_id, msg, stackTrace);  - will write to both File and Mail because their level is now DEBUG
    /// </summary>
    /// 
    public class LevelLogger : Logger
    {
        private Dictionary<ILogger, ILogLevel> originalLevels;
        public LevelLogger(string logName, Level level) : this(logName, new LogLevel(level)) { }
        public LevelLogger(string logName, ILogLevel level)
        {
            this.appId = logName;
            this.originalLevels = new Dictionary<ILogger, ILogLevel>();
        }

        public override void register(ILogger logger)
        {
            this.loggers.Add(logger);
            this.originalLevels[logger] = logger.level;
            logger.level = this.level;
        }

        /// <summary>
        ///  - Setting the level will force the registered Loggers to inherit this new level
        ///    and disregard whatever level they were created with
        ///  - Setting the level to NULL will set the registered Loggers levels back to their original value
        /// </summary>
        public override ILogLevel level
        {
            get { return base.level; }
            set
            {
                base.level = value;
                if (value != null)
                {
                    foreach (ILogger logger in this.loggers)
                    {
                        logger.level = value;
                    }
                }
                else
                {
                    foreach (ILogger logger in this.loggers)
                    {
                        logger.level = this.originalLevels[logger];
                    }
                }
            }
        }

    }
}
