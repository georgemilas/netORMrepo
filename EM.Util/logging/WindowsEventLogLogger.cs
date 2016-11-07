using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using EM.Collections;

using System.Diagnostics;

//using EM.DB;

namespace EM.Logging
{
    public class WindowsEventLogLogger : BaseLogger
    {
        
        public WindowsEventLogLogger(string appId, Level level) : this(appId, new LogLevel(level)) { }
        public WindowsEventLogLogger(string appId, ILogLevel level)
        {
            this.appId = appId;
            this.level = level;
        }

        /// <summary>
        /// generic log write method
        /// </summary>
        public override void write(string appId, string logId, ILogLevel logLevel, string msg, Exception e, string moreDetails, object context)
        {
            if (logLevel != null && this.level != null && logLevel.priority < this.level.priority) { return; }

            EventLogEntryType lgType = EventLogEntryType.Information;
            if (logLevel.level == Level.WARN)
            {
                lgType = EventLogEntryType.Warning;
            }
            if (logLevel.level == Level.ERROR || logLevel.level == Level.FATAL)
            {
                lgType = EventLogEntryType.Error;
            }
            EventLog.WriteEntry(
                appId + " " + logId, 
                msg + StringUtil.CRLF + getDetails(e, moreDetails), 
                lgType);
        }

    }

}

