using System;

namespace EM.Logging
{
    public interface ILogMessage
    {
        string logId { get; set; }
        ILogLevel logLevel { get; set; }
        string msg { get; set; }
        Exception e { get; set; }
        string moreDetails { get; set; }
        object context { get; set; }
    }

    //public interface IWarnLogMessage : ILogMessage
    //{
    //    ILogLevel logLevel { get; }
    //}

    //public interface IErrorLogMessage : ILogMessage
    //{
    //    ILogLevel logLevel { get; }
    //}

    //public interface IDebugLogMessage : ILogMessage
    //{
    //    ILogLevel logLevel { get; }
    //}

    //public interface IInfoLogMessage : ILogMessage
    //{
    //    ILogLevel logLevel { get; }
    //}

    //public interface ITraceLogMessage : ILogMessage
    //{
    //    ILogLevel logLevel { get; }
    //}

    //public interface IFatalLogMessage : ILogMessage
    //{
    //    ILogLevel logLevel { get; }
    //}
}
