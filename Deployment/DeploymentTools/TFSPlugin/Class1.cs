using System;
using System.Collections.Generic;
using System.Text;


using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.Proxy;
 
using System.Security.Principal;
using System.Net;
using EM.Collections;
using System.Xml;

namespace TFSPlugin
{

    public class CredProvider : ICredentialsProvider
    {

        #region ICredentialsProvider Members

        public ICredentials GetCredentials(Uri uri, ICredentials failedCredentials)
        {
            throw new NotImplementedException();
        }

        public void NotifyCredentialsAuthenticated(Uri uri)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class Class1
    {
        public Class1() { }

        public void doo() 
        {
            //WindowsIdentity wi = WindowsIdentity.GetCurrent(TokenAccessLevels.Impersonate); 
            //var imp = wi.Impersonate();
            //NetworkCredential cr = CredentialCache.DefaultNetworkCredentials;
            
            
            ICredentialsProvider credUi = new UICredentialsProvider();
            TeamFoundationServer srv = TeamFoundationServerFactory.GetServer("devsource", credUi);
            
            ICommonStructureService cs = (ICommonStructureService)srv.GetService(typeof(ICommonStructureService));
			var projects = cs.ListAllProjects();
            var spprj = cs.GetProjectFromName("SurePayroll");            
            NodeInfo[] niList = cs.ListStructures(spprj.Uri);
            NodeInfo area = EList<NodeInfo>.fromAray(niList).Find(ni => ni.Name.ToLower() == "area");
            XmlElement el = cs.GetNodesXml(new string[] { area.Uri }, true);


            VersionControlServer vcs = (VersionControlServer)srv.GetService(typeof(VersionControlServer));
            TeamProject[] prjLst = vcs.GetAllTeamProjects(true);
            Workspace ws = vcs.GetWorkspace("Source");
            
        }
    }
}
