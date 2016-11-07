using System;
using System.Collections.Generic;
using System.Text;
using EM.DB;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System.Data.SqlClient;

namespace DeploymentTools
{
    public class DBContext
    {

        public string serverAddress;
        public string userName;
        public string password;
        public bool useWindowsAuthentication;
        
        public string sqlScriptsPath;

        public DBContext(string serverAddress, string userName, string password, bool useWindowsAuthentication, string sqlScriptsPath)
        {
            this.serverAddress = serverAddress;
            this.userName = userName;
            this.password = password;
            this.useWindowsAuthentication = useWindowsAuthentication;
            this.sqlScriptsPath = sqlScriptsPath;
        }


        public string makeConnectionString(string databaseName)
        {
            if (this.useWindowsAuthentication)
            {
                return SqlServerDBWorker.MakeConnectionString(this.serverAddress, databaseName);
            }
            else
            {
                return SqlServerDBWorker.MakeConnectionString(this.serverAddress, databaseName, this.userName, this.password);
            }                        
        }

        public Server getSMOServerInstance(SqlServerDBWorker db)
        {
            if (this.useWindowsAuthentication)
            {
                return new Server(new ServerConnection((SqlConnection)db.connection));
            }
            else
            {
                //must regive user and password because it can't find it from an existing connection 
                SqlConnectionInfo ci = new SqlConnectionInfo(this.serverAddress, this.userName, this.password);
                return new Server(new ServerConnection(ci));
            }
        }

    }
}
