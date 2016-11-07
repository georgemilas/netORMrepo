using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;


namespace EM.Logging
{
    public class ConsoleLogger : BaseLogger
    {
        public ConsoleLogger(string appId, Level level): this(appId, new LogLevel(level)) { }
        public ConsoleLogger(string appId, ILogLevel level)
        {
            this.appId = appId;
            this.level = level;
        }

        /// <summary>
        /// support old log write method
        /// </summary> 
        public void write(string msg)
        {
            Console.WriteLine(msg);
        }

        /// <summary>
        /// generic log write method
        /// </summary>
        public override void write(string appId, string logId, ILogLevel logLevel, string msg, Exception e, string moreDetails, object context)
        {
            if (logLevel != null && this.level != null && logLevel.priority < this.level.priority) { return; }

            string s = "";
            if ( (moreDetails != null && moreDetails.Trim() != "") || e != null)
            {
                s = "-----------------------------------------------------------------------------" + StringUtil.CRLF;
                s += logLevel.ToString() + ": " + logId + ": -> " + msg;
                s += StringUtil.CRLF + getDetails(e, moreDetails) + StringUtil.CRLF;
                s += "-----------------------------------------------------------------------------";
            }
            else
            {
                s = logLevel.ToString() + ": " + logId + ": -> " + msg ;
            }

            Console.WriteLine(s);            

        }

    }
}
