using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace EM.Util.Config
{
    public class ServiceLoaderElement : ConfigurationElement
    {
        
        [ConfigurationProperty("name", IsRequired = true)]
        public string name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string type
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("order", IsRequired = false)]
        public string order
        {
            get { return (string)this["order"]; }
            set { this["order"] = value; }
        }
    }
}