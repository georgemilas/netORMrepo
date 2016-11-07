using System.ServiceProcess;
using EM.Logging;

namespace EM.Util.WCF
{
    public class ServiceBaseStarter<T> : ServiceBase
    {
        private ServiceStartSettings config;

        public ServiceBaseStarter()
        {
            config = new ServiceStartSettings();
            config.log = new Logger();
            config.runType = "service";
        }

        public ServiceBaseStarter(ServiceStartSettings config)
        {
            this.config = config;
            if (!string.IsNullOrWhiteSpace(config.serviceName))
            {
                this.ServiceName = config.serviceName;
            }
        }

        private WCFServiceStarter<T> server;
        protected override void OnStart(string[] args)
        {
            this.server = new WCFServiceStarter<T>(config);

            this.server.startServer();
        }

        protected override void OnStop()
        {
            config.log.debug("Core", "Executing method Service_OnStop()");
            this.server.stopServer();
            this.server = null;
        }

        protected override void OnPause()
        {
            if (this.server != null)
            {
                config.log.debug("Core", "Executing method Service_OnPause()");
                this.server.stopServer();
            }
        }

        protected override void OnContinue()
        {
            config.log.debug("Core", "Executing method Service_OnContinue()");
            if (this.server == null)
            {
                this.server = new WCFServiceStarter<T>(config);
            }
            this.server.startServer();
        }

    }
}