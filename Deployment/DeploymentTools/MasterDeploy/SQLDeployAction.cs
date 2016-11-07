using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeploymentTools;
using EM.Logging;

namespace MasterDeploy
{
    public class SQLDeployAction : DeployAction
    {
        public string exceptions { get; private set; }
        public string server { get; private set; }
        public string strategy { get; private set; }
        public bool winAuth { get; private set; }

        public SQLDeployAction(string rawConfig, string name, bool disable)
            : base(rawConfig, name, disable) 
        {
            
        }

        protected override void loadConfig()
        {
            source = this.config.get("sql_source_folder", "");
            exceptions = this.config.get("sql_exceptions", "");
            server = this.config.get("sql_server_name", "");
            winAuth = this.config.get("sql_use_win_auth", "true").Trim().ToLower() == "true" ? true : false;
            strategy = this.config.get("sql_walk_strategy", "depth");  // breadth / depth                   
        }

        public override bool Deploy(MessageWriter msgWriter)
        {
            return doDeployAction(msgWriter, false); 
        }

        private bool doDeployAction(MessageWriter msgWriter, bool preview)
        {
            var success = false;
            try
            {
                DBContext dbc = new DBContext(server, "", "", winAuth, source);
                SQLDeployment sql = new SQLDeployment(dbc, msgWriter);
                sql.sqlScriptExceptions = exceptions;

                if (strategy == "depth")
                {
                    success = sql.runSriptsDepthFirst(preview);
                }
                else
                {
                    success = sql.runSriptsBreadthFirst(preview);
                }
            }
            catch (Exception er)
            {
                msgWriter.WriteException(er);
            }

            return success;
        }

        public override bool PreviewDeploy(MessageWriter msgWriter)
        {
            return doDeployAction(msgWriter, true); 
        }

    }
}
