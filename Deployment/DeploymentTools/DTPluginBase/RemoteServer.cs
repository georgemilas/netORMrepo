using System;
using System.Collections.Generic;
using System.Text;

namespace DeploymentTools
{
    public delegate void WorkOnServer(RemoteServer srv);

    public class RemoteServer
    {
        public string serverConfigEntry; 

        public string name;
        public string remotePath;           //  \\web-dev\EDrive\IIS
        public string remoteLocalPath = null;      //  E:\IIS

        public RemoteServer(string serverConfig)
        {
            this.serverConfigEntry = serverConfig.Trim();
            string[] srn = serverConfig.Trim().Split(new char[] { ',' });
            int edge = srn[1].Trim().IndexOf('\\', 2);
            this.name = srn[0].Trim();
            this.computerUri = srn[1].Trim().Substring(0, edge);
            this.remotePath = srn[1].Trim();
            if (srn.Length > 2)
            {
                this.remoteLocalPath = srn[2].Trim();
            }
        }

        private string _computerUri;          //  \\web-dev
        public string computerUri
        {
            get { return _computerUri; }
            set 
            { 
                _computerUri = value;
                _computerName = value.Replace(@"\\", "");
            }
        }

        private string _computerName;         //  web-dev
        public string computerName
        {
            get { return _computerName; }         
        }


        public override string ToString()
        {
            string lb = "- " + this.name;  // +"\n";
            if (this.remoteLocalPath != null)
            {
                lb += ", " + this.remoteLocalPath;
            }
            else
            {
                lb += ", " + this.remotePath;
            }
            return lb;
        }


    }
}
