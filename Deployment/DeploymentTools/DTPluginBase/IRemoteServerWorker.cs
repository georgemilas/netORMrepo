using System;
using System.Collections.Generic;
using System.Text;

namespace DeploymentTools
{
    public interface IRemoteServerWorker
    {
        void workOnServer(RemoteServer server);
    }
}
