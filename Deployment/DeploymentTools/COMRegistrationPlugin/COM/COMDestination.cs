using System;
using System.Collections.Generic;
using System.Text;

namespace DeploymentTools
{
    /// <summary>
    /// abstraction to encapsulate the logic if the registration of DLL's is to be done localy 
    /// or on a remote server
    /// </summary>
    public class COMDestination
    {
        private RemoteServer server = null;
        private string localPath = null;

        public COMDestination(string localPath)
        {
            this.localPath = localPath;
        }
        public COMDestination(RemoteServer server)
        {
            this.server = server;
        }

        public bool isRemoteComputer
        {
            get { return this.server != null; }
        }

        public string remoteComputer
        {
            get { return this.server.computerName; }
        }

        public string copyPath
        {
            get
            {
                if (this.localPath != null)
                {
                    return this.localPath;
                }
                else
                {
                    return this.server.remotePath;
                }
            }
        }

        public string registrationPath
        {
            get
            {
                if (this.localPath != null)
                {
                    return this.localPath;
                }
                else
                {
                    return this.server.remoteLocalPath;
                }
            }
        }


    }
}
