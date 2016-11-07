using System;
using System.ServiceProcess;

namespace EM.Util.TickerService
{
    public abstract class TickerStartManager
    {

        public abstract GenericTickCollectionService getWindowsServiceStarter();

        /// <summary>
        /// will run the WCF service T either as a windows service or as a console app based on WCFStartSettings.runType
        /// </summary>
        public void runService(ServiceStartSettings config)
        {
            var services = getWindowsServiceStarter();
            if (config.runType == ApplicationStartType.TestConsoleApp)
            {
                config.log.debug("Core", "Service Server running as a console app");
                if (config.onStart != null) { config.onStart(); }
                foreach (var serv in services.GetServicesToRun())
                {
                    serv.Start();
                }


                Console.ReadLine();
                Console.WriteLine("Services Server Closing");
                foreach (var serv in services.GetServicesToRun())
                {
                    serv.Stop();
                }
                if (config.onStop != null) { config.onStop(); }
                Console.WriteLine("Services Server Stoped");
            }
            else
            {
                config.log.debug("Core", "Services Server running as windows service");
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
		        { 
			        getWindowsServiceStarter() 
		        };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
