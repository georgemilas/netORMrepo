using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EM.Util;
using System.IO;
using EM.Logging;

namespace MasterDeploy
{
    public abstract class DeployAction
    {

        //public string DeployType { get; set; }
        public string source { get; protected set; }
        public string name { get; protected set; }
        public bool skipDeploy { get; set; }
        public bool disable { get; set; }

        public SimpleConfigParser config { get; private set; }
        private string rawConfig { get; set; }

        public DeployAction(string rawConfig, string name, bool disable)
        {
            this.name = name;
            this.skipDeploy = disable;
            this.disable = disable;
            this.PrepareConfig(rawConfig);
        }

        public void PrepareConfig(string rawConfig)
        {
            this.rawConfig = rawConfig;
            MemoryStream ms = new MemoryStream(Encoding.ASCII.GetBytes(rawConfig));
            StreamReader sr = new StreamReader(ms);
            this.config = SimpleConfigParser.parse(sr, true);
            sr.Close();
            ms.Close();
            this.loadConfig();
        }

        /// <summary>
        /// return true/ false whether successfull or not
        /// </summary>
        /// <param name="msgWriter"></param>
        /// <returns></returns>
        public abstract bool Deploy(MessageWriter msgWriter);
        public abstract bool PreviewDeploy(MessageWriter msgWriter);
        protected abstract void loadConfig();

        
    }
}
