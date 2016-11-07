using System.ServiceModel;
using System.Security.Cryptography.X509Certificates;

namespace EM.Util.WCF
{

    /// <summary>
    /// start a WCF listener for the service T 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WCFServiceStarter<T>
    {
        private ServiceHost host;
        private ServiceStartSettings config;

        public WCFServiceStarter(ServiceStartSettings config)
        {
            this.config = config;
        }
        
        public void startServer()
        {            
            this.host = new ServiceHost(typeof(T));
            config.log.info("Core", "Kick start internal service " + config.internalServerDesription);
            if (config.customizeServer != null)
            {
                config.log.info("Core", "Running Service customization");
                config.customizeServer(host);
            } 

            this.host.Open();

            config.log.info("Core", "Service is ready at " + config.internalServerAddress);

            if (config.onStart != null)
            {
                config.onStart();
            }
        }

        public void stopServer()
        {
            if (this.host != null &&
                (this.host.State != CommunicationState.Closed &&
                 this.host.State != CommunicationState.Closing)
                )
            {
                config.log.info("Core", "Internal service is closing ");
                this.host.Close();
                this.host = null;
                if (config.onStop != null)
                {
                    config.onStop();
                }
                config.log.debug("Core", "Internal service is closed ");
            }
        }
    }
}