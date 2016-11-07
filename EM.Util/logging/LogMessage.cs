using System;
using System.Collections.Generic;
using System.Text;

namespace EM.Logging
{
    public class LogMessage : ILogMessage
    {
        public string logId {get; set;}
        public virtual ILogLevel logLevel {get; set;}
        public string msg {get; set;}
        public Exception e {get; set;}
        public string moreDetails {get; set;}
        public object context { get; set; }
    }
 

    //public class WarnLogMessage : IWarnLogMessage
    //{
    //    private ILogLevel level = new LogLevel(Level.WARN);
    //    public ILogLevel logLevel { get { return level; } set {} }        
    //    public string logId { get; set; }
    //    public string msg { get; set; }
    //    public Exception e { get; set; }
    //    public string moreDetails { get; set; }
    //    public object context { get; set; }
    //}

    //public class DebugLogMessage : IDebugLogMessage
    //{
    //    private ILogLevel level = new LogLevel(Level.DEBUG);
    //    public ILogLevel logLevel { get { return level; } set {} }
    //    public string logId { get; set; }
    //    public string msg { get; set; }
    //    public Exception e { get; set; }
    //    public string moreDetails { get; set; }
    //    public object context { get; set; }
    //}
    //public class ErrorLogMessage : IErrorLogMessage
    //{
    //    private ILogLevel level = new LogLevel(Level.ERROR);
    //    public ILogLevel logLevel { get { return level; } set {} }
    //    public string logId { get; set; }
    //    public string msg { get; set; }
    //    public Exception e { get; set; }
    //    public string moreDetails { get; set; }
    //    public object context { get; set; }
    //}
    //public class InfoLogMessage : IInfoLogMessage
    //{
    //    private ILogLevel level = new LogLevel(Level.INFO);
    //    public ILogLevel logLevel { get { return level; } set {} }
    //    public string logId { get; set; }
    //    public string msg { get; set; }
    //    public Exception e { get; set; }
    //    public string moreDetails { get; set; }
    //    public object context { get; set; }
    //}
    //public class TraceLogMessage : ITraceLogMessage
    //{
    //    private ILogLevel level = new LogLevel(Level.TRACE);
    //    public ILogLevel logLevel { get { return level; } set {} }
    //    public string logId { get; set; }
    //    public string msg { get; set; }
    //    public Exception e { get; set; }
    //    public string moreDetails { get; set; }
    //    public object context { get; set; }
    //}
    //public class FatalLogMessage : IFatalLogMessage
    //{
    //    private ILogLevel level = new LogLevel(Level.FATAL);
    //    public ILogLevel logLevel { get { return level; } set {} }
    //    public string logId { get; set; }
    //    public string msg { get; set; }
    //    public Exception e { get; set; }
    //    public string moreDetails { get; set; }
    //    public object context { get; set; }
    //}
}
