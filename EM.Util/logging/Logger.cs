using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using EM.Collections; 

namespace EM.Logging
{
    /// <summary>
    /// - a logger (ILogger) and also a collection of loggers
    ///     - by itself dosen't do anything other then dispaching logging to is collection members
    ///     - if it's collection is empty, then directs loging to console
    /// to log: 
    ///        log = new Logger();
    ///        log.register(new MailLogger(LogLevel.Level.WARN)); 
    ///        log.register(new FileLogger(LogLevel.Level.INFO)); 
    /// 
    ///        log.debug(log_id, msg, stackTrace);  - will not write anywhere
    ///        log.info(log_id, msg, stackTrace);   - will write to File but not Mail
    ///        log.error(log_id, msg, stackTrace);  - will write to both File and Mail
    ///        
    /// </summary>
    public class Logger: BaseLogger, IDisposable
    {

        public ESet<ILogger> loggers;
        public ConsoleLogger consoleLogger;
        
        public Logger()
        {
            this.loggers = new ESet<ILogger>();
            this.level = new LogLevel(Level.DEBUG);
        }
        public Logger(string logName) : this()
        {
            this.appId = logName;  //fld_app_id for logging in tbl_application_log 
            this.consoleLogger = new ConsoleLogger(logName, Level.DEBUG);  //print everything            
        }

        public override string appId
        {
            get
            {
                return base.appId;
            }
            set
            {
                base.appId = value;
                this.consoleLogger = new ConsoleLogger(value, Level.DEBUG);  //print everything            
            }
        }

        public virtual void register(ILogger logger) 
        {
            this.loggers.Add(logger);
        }
        public virtual void unregister(ILogger logger)
        {
            this.loggers.Remove(logger);
        }
        public virtual void clearAllLoggers()
        {
            this.loggers.Clear();
        }

        /// <summary>
        /// will let individual registered loggers to hadle priority insteaf of
        /// applying only if parameter logLevel's priority is bigger than the container's instance priority
        /// </summary>
        public override void write(string logId, ILogLevel logLevel, string msg, Exception e, string moreDetails)
        {
            //let registrars handle priority
            if (this.loggers.Count > 0)
            {
                foreach (ILogger logger in this.loggers)
                {
                    try
                    {
                        logger.write(logId, logLevel, msg, e, moreDetails);
                    }
                    catch(Exception)
                    {
                        logToConsole(logId, logLevel, "Could not use " + logger.GetType().Name + ", writing to console", logger);
                        logToConsole(logId, logLevel, msg, logger);
                    }
                }
            }
            else
            {
                base.write(logId, logLevel, msg, e, moreDetails);
            }
        }

        /// <summary>
        /// will let individual registered loggers to hadle priority insteaf of
        /// applying only if parameter logLevel's priority is bigger than the container's instance priority
        /// </summary>
        public override void write(string logId, ILogLevel logLevel, string msg, string moreDetails)
        {
            //let registrars handle priority
            if (this.loggers.Count > 0)
            {
                foreach (ILogger logger in this.loggers)
                {
                    try
                    {
                        logger.write(logId, logLevel, msg, moreDetails);
                    }
                    catch
                    {
                        logToConsole(logId, logLevel, msg, logger);
                    }
                }
            }
            else
            {
                base.write(logId, logLevel, msg, moreDetails);
            }
        }

        /// <summary>
        /// will let individual registered loggers to hadle priority insteaf of
        /// applying only if parameter logLevel's priority is bigger than the container's instance priority
        /// </summary>
        public override void write(ILogMessage message)
        {
            //let registrars handle priority
            if (this.loggers.Count > 0)
            {
                foreach (ILogger logger in this.loggers)
                {
                    try
                    {
                        logger.write(message);
                    }
                    catch
                    {
                        logToConsole(message.logId, message.logLevel, message.msg, logger);
                    }
                }
            }
            else
            {
                base.write(message);
            }
        }

        /// <summary>
        /// the last method and in the chain of calls 
        /// </summary>
        public override void write(string appId, string logId, ILogLevel logLevel, string msg, Exception e, string moreDetails, object context)
        {
            if (this.loggers.Count > 0)
            {
                foreach (ILogger logger in this.loggers)
                {
                    try
                    {
                        logger.write(appId, logId, logLevel, msg, e, moreDetails, context);                        
                    }
                    catch
                    {
                        logToConsole(logId, logLevel, msg, logger);
                    }
                }
            }
            else
            {
                if (this.consoleLogger != null)
                {
                    this.consoleLogger.write(appId, logId, logLevel, msg, e, moreDetails, context);                    
                }
            }
            
            
        }

        protected virtual void logToConsole(string logId, ILogLevel logLevel, string msg, ILogger logger)
        {
            if (this.consoleLogger != null)
            {
                string s = "-----------------------------------------------------------" + StringUtil.CRLF;
                s += "failed to log to " + logger.GetType().ToString() + ":" + StringUtil.CRLF +
                     logLevel.ToString() + ": " + logId + ": " + msg;
                this.consoleLogger.write(s);
            }
        }

        
        public static string getErrorDetails(Exception e)
        {
            Logger l = new Logger();
            return l.getExceptionDetails(e);
        }

        #region IDisposable Members

        public virtual void Dispose()
        {            
            while (this.loggers.Count > 0)
            {
                ILogger l = this.loggers[0];
                this.unregister(l);
                if (l is IDisposable)
                {
                    ((IDisposable)l).Dispose();
                }
            }            
        }

        #endregion
    }
}
