using System;
using System.ServiceProcess;

namespace EM.Util.WCF
{
    public class WCFStartManager<T>
    {

        public virtual ServiceBaseStarter<T> getWindowsServiceStarter(ServiceStartSettings config)
        {
            return new ServiceBaseStarter<T>(config);
        }

        /// <summary>
        /// will run the WCF service T either as a windows service or as a console app based on WCFStartSettings.runType
        /// </summary>
        public void runService(ServiceStartSettings config) 
        {
            if (config.runType == "test")
            {
                config.log.debug("Core", "Service Server running as a console app");
                var server = new WCFServiceStarter<T>(config);
                server.startServer();

                Console.ReadLine();
                Console.WriteLine("Service Server Closing");
                server.stopServer();
                Console.WriteLine("Service Server Stoped");
            }
            else
            {
                config.log.debug("Core", "Service Server running as windows service");
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
		        { 
			        getWindowsServiceStarter(config) 
		        };
                ServiceBase.Run(ServicesToRun);
            }
        }        
    }
}
