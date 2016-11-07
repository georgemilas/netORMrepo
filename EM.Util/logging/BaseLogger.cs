using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;
using EM.Util;


namespace EM.Logging
{
    

    public abstract class BaseLogger : ILogger
    {
        private string _appId = "GenericLogger";
        private ILogLevel _level = new LogLevel(Level.WARN);
        
        public virtual string appId
        {
            get { return this._appId; } 
            set { this._appId = value; }
        }

        public virtual ILogLevel level
        {
            get { return this._level; }
            set { this._level = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////// Main Write methods
        // generic log write method 
        // for maitenability is preferably to use error, debug, warn or info methods
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        
        //will apply only if parameter logLevel's priority is bigger than the instance priority
        public virtual void write(string logId, ILogLevel logLevel, string msg) { write(logId, logLevel, msg, null, ""); }
        public virtual void write(string logId, ILogLevel logLevel, string msg, Exception e) { write(logId, logLevel, msg, e, ""); }
        public virtual void write(string logId, ILogLevel logLevel, string msg, Exception e, string moreDetails)
        {
            if (logLevel == null || this.level == null)
            {
                this.write(this.appId, logId, logLevel, msg, e, moreDetails);
                return;
            }
            if (logLevel.priority >= this.level.priority)
            {
                this.write(this.appId, logId, logLevel, msg, e, moreDetails);
            }
        }
        
        public virtual void write(string logId, ILogLevel logLevel, string msg, string moreDetails)
        {
            if (logLevel == null || this.level == null)
            {
                this.write(this.appId, logId, logLevel, msg, moreDetails);
                return;
            }
            if (logLevel.priority >= this.level.priority)
            {
                this.write(this.appId, logId, logLevel, msg, moreDetails);
            }
        }
        public virtual void write(ILogMessage message)
        {
            if (message.logLevel == null || this.level == null)
            {
                this.write(this.appId, message.logId, message.logLevel, message.msg, message.e, message.moreDetails, message.context);
                return;
            }
            if (message.logLevel.priority >= this.level.priority)
            {
                this.write(this.appId, message.logId, message.logLevel, message.msg, message.e, message.moreDetails, message.context);
            }
        }
        
        ///////////////  using AppID parameter
        public virtual void write(string appId, string logId, ILogLevel logLevel, string msg, Exception e) { write(appId, logId, logLevel, msg, e, "", null); }
        public virtual void write(string appId, string logId, ILogLevel logLevel, string msg, string moreDetails) { write(appId, logId, logLevel, msg, null, moreDetails, null); }
        public virtual void write(string appId, string logId, ILogLevel logLevel, string msg, Exception e, string moreDetails) { write(appId, logId, logLevel, msg, e, moreDetails, null); }
        public abstract void write(string appId, string logId, ILogLevel logLevel, string msg, Exception e, string moreDetails, object context);
        

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////  Other Stuff
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual string getDetails(Exception e, string moreDetails)
        {
            StringBuilder res = new StringBuilder( moreDetails != null ? moreDetails : "" );
            if (e != null)
            {
                if (String.IsNullOrEmpty(moreDetails))
                {
                    res.Append(StringUtil.CRLF);
                }
                res.Append(this.getExceptionDetails(e));
            }
            return res.ToString();
        }

        public virtual string getExceptionDetails(Exception e) 
        {
            return getExceptionDetails(e, "Exception");
        }        
        public virtual string getExceptionDetails(Exception e, string exceptionTitle)
        {
            if (e == null) { return ""; }

            StringBuilder res = new StringBuilder();
            res.Append(exceptionTitle+": " + e.GetType().ToString() + " -> " + e.Message);
            if (e.StackTrace != null)
            {
                res.Append(StringUtil.CRLF + "Stack Trace:" + StringUtil.CRLF);
                res.Append(e.StackTrace);
                res.Append(StringUtil.CRLF);
            }
            if (e.InnerException != null)
            {
                res.Append(StringUtil.CRLF);
                res.Append(getExceptionDetails(e.InnerException, "InnerException"));                
            }
            return res.ToString();
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /////  Main client interface 
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public virtual void error(string log_id, string msg) { write(log_id, new LogLevel(Level.ERROR), msg, null, ""); }
        public virtual void error(string log_id, string msg, string moreDetails) { write(log_id, new LogLevel(Level.ERROR), msg, moreDetails); }
        public virtual void error(string log_id, string msg, Exception e) { write(log_id, new LogLevel(Level.ERROR), msg, e); }
        public virtual void error(string log_id, string msg, Exception e, string moreDetails) { write(log_id, new LogLevel(Level.ERROR), msg, e, moreDetails); }
        public virtual void error(ILogMessage message) { message.logLevel = new LogLevel(Level.ERROR); write(message); }

        public virtual void warn(string log_id, string msg) { write(log_id, new LogLevel(Level.WARN), msg, null, ""); }
        public virtual void warn(string log_id, string msg, string moreDetails) { write(log_id, new LogLevel(Level.WARN), msg, moreDetails); }
        public virtual void warn(string log_id, string msg, Exception e) { write(log_id, new LogLevel(Level.WARN), msg, e); }
        public virtual void warn(string log_id, string msg, Exception e, string moreDetails) { write(log_id, new LogLevel(Level.WARN), msg, e, moreDetails); }
        public virtual void warn(ILogMessage message) { message.logLevel = new LogLevel(Level.WARN); write(message); }

        public virtual void info(string log_id, string msg) { write(log_id, new LogLevel(Level.INFO), msg, null, ""); }
        public virtual void info(string log_id, string msg, string moreDetails) { write(log_id, new LogLevel(Level.INFO), msg, moreDetails); }
        public virtual void info(string log_id, string msg, Exception e) { write(log_id, new LogLevel(Level.INFO), msg, e); }
        public virtual void info(string log_id, string msg, Exception e, string moreDetails) { write(log_id, new LogLevel(Level.INFO), msg, e, moreDetails); }
        public virtual void info(ILogMessage message) { message.logLevel = new LogLevel(Level.INFO); write(message); }

        public virtual void debug(string log_id, string msg) { write(log_id, new LogLevel(Level.DEBUG), msg, null, ""); }
        public virtual void debug(string log_id, string msg, string moreDetails) { write(log_id, new LogLevel(Level.DEBUG), msg, moreDetails); }
        public virtual void debug(string log_id, string msg, Exception e) { write(log_id, new LogLevel(Level.DEBUG), msg, e); }
        public virtual void debug(string log_id, string msg, Exception e, string moreDetails) { write(log_id, new LogLevel(Level.DEBUG), msg, e, moreDetails); }
        public virtual void debug(ILogMessage message) { message.logLevel = new LogLevel(Level.DEBUG); write(message); }

        public virtual void trace(string log_id, string msg) { write(log_id, new LogLevel(Level.TRACE), msg, null, ""); }
        public virtual void trace(string log_id, string msg, string moreDetails) { write(log_id, new LogLevel(Level.TRACE), msg, moreDetails); }
        public virtual void trace(string log_id, string msg, Exception e) { write(log_id, new LogLevel(Level.TRACE), msg, e); }
        public virtual void trace(string log_id, string msg, Exception e, string moreDetails) { write(log_id, new LogLevel(Level.TRACE), msg, e, moreDetails); }
        public virtual void trace(ILogMessage message) { message.logLevel = new LogLevel(Level.TRACE); write(message); }

        public virtual void fatal(string log_id, string msg) { write(log_id, new LogLevel(Level.FATAL), msg, null, ""); }
        public virtual void fatal(string log_id, string msg, string moreDetails) { write(log_id, new LogLevel(Level.FATAL), msg, moreDetails); }
        public virtual void fatal(string log_id, string msg, Exception e) { write(log_id, new LogLevel(Level.FATAL), msg, e); }
        public virtual void fatal(string log_id, string msg, Exception e, string moreDetails) { write(log_id, new LogLevel(Level.FATAL), msg, e, moreDetails); }
        public virtual void fatal(ILogMessage message) { message.logLevel = new LogLevel(Level.FATAL); write(message); }
    }
}
