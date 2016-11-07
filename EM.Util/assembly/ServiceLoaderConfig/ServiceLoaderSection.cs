using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace EM.Util.Config
{
    public class ServiceLoaderSection : ConfigurationSection
    {
        ServiceLoaderElement element;
        public ServiceLoaderSection()
        {
            element = new ServiceLoaderElement();
        }
        
        [ConfigurationProperty("services", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ServiceLoaderCollection), AddItemName = "service")]
        public ServiceLoaderCollection services
        {
            get
            {
                return (ServiceLoaderCollection)base["services"];
            }
        }

        [ConfigurationProperty("settings")]
        public ServiceSettingsElement settings
        {
            get
            {
                return (ServiceSettingsElement)base["settings"];
            }
        }
    }
}
