using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace EM.Logging.Config
{
    public class LoggerElement : ConfigurationElement
    {
        
        [ConfigurationProperty("id", IsRequired = true)]
        public string id
        {
            get { return (string)this["id"]; }
            set { this["id"] = value; }
        }

        [ConfigurationProperty("className", IsRequired = true)]
        public string className
        {
            get { return (string)this["className"]; }
            set { this["className"] = value; }
        }

        /// <summary>
        /// defaults to error
        /// </summary>
        [ConfigurationProperty("level", IsRequired = false)]
        public string level
        {
            get { return (string)this["level"]; }
            set { this["level"] = value; }
        }

        [ConfigurationProperty("rollingType", IsRequired = false)]
        public string rollingType
        {
            get { return (string)this["rollingType"]; }
            set { this["rollingType"] = value; }
        }

        [ConfigurationProperty("rollingTypeRemove", IsRequired = false)]
        public string rollingTypeRemove
        {
            get { return (string)this["rollingTypeRemove"]; }
            set { this["rollingTypeRemove"] = value; }
        }

        [ConfigurationProperty("logFileName", IsRequired = false)]
        public string logFileName
        {
            get { return (string)this["logFileName"]; }
            set { this["logFileName"] = value; }
        }

        [ConfigurationProperty("emailAddress", IsRequired = false)]
        public string emailAddress
        {
            get { return (string)this["emailAddress"]; }
            set { this["emailAddress"] = value; }
        }

        [ConfigurationProperty("senderEmailAddress", IsRequired = false)]
        public string senderEmailAddress
        {
            get { return (string)this["senderEmailAddress"]; }
            set { this["senderEmailAddress"] = value; }
        }

        [ConfigurationProperty("mailServerAddress", IsRequired = false)]
        public string mailServerAddress
        {
            get { return (string)this["mailServerAddress"]; }
            set { this["mailServerAddress"] = value; }
        }

        [ConfigurationProperty("batchProvider", IsRequired = false)]
        public string batchProvider
        {
            get { return (string)this["batchProvider"]; }
            set { this["batchProvider"] = value; }
        }

        [ConfigurationProperty("batchThreshold", IsRequired = false)]
        public string batchThreshold
        {
            get { return (string)this["batchThreshold"]; }
            set { this["batchThreshold"] = value; }
        }

        [ConfigurationProperty("user", IsRequired = false)]
        public string user
        {
            get { return (string)this["user"]; }
            set { this["user"] = value; }
        }

        [ConfigurationProperty("password", IsRequired = false)]
        public string password
        {
            get { return (string)this["password"]; }
            set { this["password"] = value; }
        }

    }
}
