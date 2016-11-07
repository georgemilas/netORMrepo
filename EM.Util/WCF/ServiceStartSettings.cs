using System;
using EM.Logging;
using System.ServiceModel.Configuration;
using System.Configuration;
using System.ServiceModel;

namespace EM.Util.WCF
{
    public class ServiceStartSettings
    {
        /// <summary>
        /// either "test" or "service"
        /// </summary>
        public string runType { get; set; } 

        /// <summary>
        /// name of windows service
        /// </summary>
        public string serviceName { get; set; }
        public ILogger log { get; set; }

        //public string ThumbPrint { get; set; }
        public Action<ServiceHost> customizeServer { get; set; } 

        public string internalServerDesription { get; set; }

        private string _internalServerAddress;
        public string internalServerAddress
        {
            get
            {
                if (_internalServerAddress == null)
                {
                    return defaultWCFConfigServiceUri;
                }
                return _internalServerAddress;
            }
            set { _internalServerAddress = value; }
        }
        
        public Action onStart { get; set; }
        public Action onStop { get; set; }

        /// <summary>
        /// Get the URI of the first service from config file section system.serviceModel/services
        /// </summary>
        public static string defaultWCFConfigServiceUri 
        {
            get
            {
                ServicesSection services = (ServicesSection)ConfigurationManager.GetSection("system.serviceModel/services");
                return services.Services[0].Endpoints[0].Address.AbsoluteUri;
            }
        }

    }
}