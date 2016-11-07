using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace EM.Logging.Config
{
    public class LoggerSection : ConfigurationSection
    {
        LoggerElement element;
        public LoggerSection()
        {
            element = new LoggerElement();
        }

        [ConfigurationProperty("loggers", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(LoggersCollection), AddItemName = "logger")]
        public LoggersCollection loggers
        {
            get
            {
                return (LoggersCollection)base["loggers"];
            }
        }
    }
}
