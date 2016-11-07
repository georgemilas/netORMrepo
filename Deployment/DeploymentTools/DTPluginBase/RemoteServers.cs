using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;
using System.Windows.Forms;

namespace DeploymentTools
{
    public class RemoteServers: EDictionary<string, RemoteServer>
    {
        //servers that are duplicated in the config will only have one in the 
        //collection of remote servers, the other one will go to ignored list
        public EList<RemoteServer> ignoredServers;   

        public RemoteServers(string serversConfig): base()
        {
            this.ignoredServers = new EList<RemoteServer>();
            if (serversConfig != null && serversConfig.Trim() != "")
            {
                string[] sr = serversConfig.Split(new char[] { '\n' });
                foreach (string s in sr)
                {

                    RemoteServer rs = new RemoteServer(s);
                    if (!this.ContainsKey(rs.computerName))
                    {
                        this.Add(rs.computerName, rs);
                    }
                    else
                    {
                        this.ignoredServers.Add(rs);
                        //throw new ArgumentException("Configuration file contains 2 or more entries of the same remote server " + rs.computerName);
                    }
                }
            }            
        }

        public override string ToString()
        {
            string lb = "";
            foreach (RemoteServer server in this.Values)
            {
                lb += server.ToString() + "\n";
            }
            return lb;
        }

        public string ignoredServersConfig
        {
            get
            {
                string txt = "";
                foreach (RemoteServer s in this.ignoredServers)
                {
                    txt += "- " + s.serverConfigEntry + "\r\n";
                }                    
                return txt;
            }
        }

        
    }
}
