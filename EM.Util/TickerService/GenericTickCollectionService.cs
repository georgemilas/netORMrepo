using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceProcess;

namespace EM.Util.TickerService
{
    public class GenericTickCollectionService : ServiceBase
    {    
        protected List<ITickService> services = new List<ITickService>();
        public IEnumerable<ITickService> GetServicesToRun()
        {
            return services;
        }
        
        protected override void OnStart(string[] args)
        {
            foreach(ITickService s in services)
            {
                s.Start();
            }
        }

        protected override void OnStop()
        {
            foreach (ITickService s in services)
            {
                s.Stop();
            }
        }

        protected override void OnPause()
        {
            foreach (ITickService s in services)
            {
                s.Pause();
            }
        }

        protected override void OnContinue()
        {
            foreach (ITickService s in services)
            {
                s.Continue();
            }
        }

    }
}
