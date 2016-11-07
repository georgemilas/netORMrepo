using System;
using System.Collections.Generic;
using System.Text;
using EM.Batch;

namespace EM.Logging.Config
{
    public class TypedLoggerElement
    {        
        public ILogLevel level { get; set; }
        public RollingType rollingType { get; set; }
        public RollingTypeRemove rollingTypeRemove { get; set; }
        public string logFileName { get; set; }
        public string senderEmailAddress { get; set; }
        public string mailServerAddress { get; set; }
        public string emailAddress { get; set; }
        public string user { get; set; }
        public string password { get; set; }
        public BatchProvider batchProvider { get; set; }
        public int batchThreshold { get; set; }
    }
}

