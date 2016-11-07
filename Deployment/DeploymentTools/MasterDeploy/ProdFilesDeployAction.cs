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
    public class ProdFilesDeployAction : DeployAction
    {
        public bool teamOut { get; private set; }
        public bool teamIn { get; private set; }
        public bool copy { get; private set; }
        public bool restart { get; private set; }
        public bool resetIIS { get; private set; }
        public string copyMethod { get; private set; }
        public bool runParallel { get; private set; }
        public RemoteServers servers { get; private set; }

        private MessageWriter msgWriter;

        public ProdFilesDeployAction(string rawConfig, string name, bool disable)
            : base(rawConfig, name, disable) 
        {
            
        }

        protected override void loadConfig()
        {
            source = this.config.get("prod_source_folder", "");
            teamOut = this.config.get("prod_team_out", "false").Trim().ToLower() == "true" ? true : false;
            copy = this.config.get("prod_copy", "true").Trim().ToLower() == "true" ? true : false;
            restart = this.config.get("prod_restart", "false").Trim().ToLower() == "true" ? true : false;
            resetIIS = this.config.get("prod_reset_iis", "true").Trim().ToLower() == "true" ? true : false;
            teamIn = this.config.get("prod_team_in", "false").Trim().ToLower() == "true" ? true : false;
            runParallel = this.config.get("prod_run_parallel", "true").Trim().ToLower() == "true" ? true : false;

            var cservers = this.config.get("prod_servers", "");
            servers = new RemoteServers(cservers);            
        }

        public override bool Deploy(MessageWriter msgWriter)
        {
            return doProdFilesDeploy(msgWriter, false);
        }

        public override bool PreviewDeploy(MessageWriter msgWriter)
        {
            return doProdFilesDeploy(msgWriter, true);
        }

        private bool doProdFilesDeploy(MessageWriter msgWriter, bool preview)
        {
            bool success = false;
            try
            {
                FilesDeployment dep = new FilesDeployment(msgWriter, source, servers, config);
                dep.preview = preview;
                dep.runInParallel = runParallel;
                //var stopper = new OnCancelHandler(() => { dep.doCancel(); });
                dep.deploy(teamOut, copy, restart, teamIn, resetIIS);
                success = true;
            }
            catch (Exception er)
            {
                msgWriter.WriteException(er);
            }
            return success;
        }

        
    }
}
