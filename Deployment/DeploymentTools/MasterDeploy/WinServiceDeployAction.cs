using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeploymentTools;
using EM.Logging;
using System.Drawing;
using EM.Util;
using DeploymentTools.Parallel;
using System.Threading;

namespace MasterDeploy
{
    public class WinServiceDeployAction : DeployAction
    {
        public bool stop { get; private set; }
        public bool start { get; private set; }
        public bool copy { get; private set; }
        public bool runParallel { get; private set; }
        public RemoteServers servers { get; private set; }

        public WinServiceDeployAction(string rawConfig, string name, bool disable)
            : base(rawConfig, name, disable) 
        {
            
        }

        protected override void loadConfig()
        {
            source = this.config.get("service_source_folder", "");
            stop = this.config.get("service_stop", "true").Trim().ToLower() == "true" ? true : false;
            copy = this.config.get("service_copy", "true").Trim().ToLower() == "true" ? true : false;
            start = this.config.get("service_start", "true").Trim().ToLower() == "true" ? true : false;
            runParallel = this.config.get("service_run_parallel", "true").Trim().ToLower() == "true" ? true : false;

            var cservers = this.config.get("service_servers", "");
            servers = new RemoteServers(cservers);            
        }


        public override bool Deploy(MessageWriter msgWriter)
        {
            return doServiceDeploy(msgWriter, false);
        }

        public override bool PreviewDeploy(MessageWriter msgWriter)
        {
            return doServiceDeploy(msgWriter, true);
        }

        private bool doServiceDeploy(MessageWriter msgWriter, bool preview)
        {
            bool success = false;
            try
            {
                ServiceDeployment dep = new ServiceDeployment(msgWriter, source, servers, config);
                dep.preview = preview;
                dep.runInParallel = runParallel;
                success = dep.deploy(stop, copy, start);
            }
            catch (Exception er)
            {
                msgWriter.WriteException(er);
            }
            return success;
        }        
    }
}
