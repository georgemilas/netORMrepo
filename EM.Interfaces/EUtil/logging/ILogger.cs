using System;
using System.Collections.Generic;
using System.Text;

namespace EM.Logging
{
    public interface ILogger
    {
        ILogLevel level { get; set; }

        /// <summary>
        /// The name of current logging session. 
        /// In case the same storage is used by diferent logging sessions, this can identify them
        /// </summary>
        string appId { get; set; }

        void trace(string log_id, string msg);
        void trace(string log_id, string msg, Exception e);
        void trace(string log_id, string msg, Exception e, string moreDetails);
        void trace(string log_id, string msg, string moreDetails);
        void trace(ILogMessage message);

        void debug(string log_id, string msg, Exception e);
        void debug(string log_id, string msg, Exception e, string moreDetails);
        void debug(string log_id, string msg);
        void debug(string log_id, string msg, string moreDetails);
        void debug(ILogMessage message);

        void info(string log_id, string msg, Exception e);
        void info(string log_id, string msg, Exception e, string moreDetails);
        void info(string log_id, string msg);
        void info(string log_id, string msg, string moreDetails);
        void info(ILogMessage message);

        void warn(string log_id, string msg, string moreDetails);
        void warn(string log_id, string msg, Exception e);
        void warn(string log_id, string msg, Exception e, string moreDetails);
        void warn(string log_id, string msg);
        void warn(ILogMessage message);

        void error(string log_id, string msg, Exception e, string moreDetails);
        void error(string log_id, string msg, Exception e);
        void error(string log_id, string msg);
        void error(string log_id, string msg, string moreDetails);
        void error(ILogMessage message);

        void fatal(string log_id, string msg, Exception e, string moreDetails);
        void fatal(string log_id, string msg, string moreDetails);
        void fatal(string log_id, string msg);
        void fatal(string log_id, string msg, Exception e);
        void fatal(ILogMessage message);

        /// <summary>
        /// Use logId to identify current logging message 
        /// </summary>
        void write(string logId, ILogLevel logLevel, string msg);
        void write(string logId, ILogLevel logLevel, string msg, string moreDetails);
        void write(string logId, ILogLevel logLevel, string msg, Exception e);
        void write(string logId, ILogLevel logLevel, string msg, Exception e, string moreDetails);
        
        /// <summary>
        /// A collection looger may pass it's appID to its registered sub-loggers for consistent appID acros loggers 
        /// </summary>
        void write(string appId, string logId, ILogLevel logLevel, string msg, string moreDetails);
        void write(string appId, string logId, ILogLevel logLevel, string msg, Exception e);
        void write(string appId, string logId, ILogLevel logLevel, string msg, Exception e, string moreDetails);
        void write(string appId, string logId, ILogLevel logLevel, string msg, Exception e, string moreDetails, object context);
        void write(ILogMessage message);

        string getDetails(Exception e, string moreDetails);
        string getExceptionDetails(Exception e); 
    }
}
