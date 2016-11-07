using System;
using System.Configuration;

namespace EM.Util.Config
{
    public class ServiceSettingsElement : ConfigurationElement
    {
        [ConfigurationProperty("serviceName", IsRequired = true)]
        public string serviceName
        {
            get
            {
                return (string)this["serviceName"];
            }
            set
            {
                this["serviceName"] = value;
            }
        }
    }
}
