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
using System.Threading.Tasks;

namespace MasterDeploy
{
    public class COMDeployAction : DeployAction
    {
        public string exceptions { get; private set; }
        public string destination { get; private set; }
        public bool runParallel { get; private set; }
        public RemoteServers servers { get; private set; }

        private MessageWriter msgWriter;

        public COMDeployAction(string rawConfig, string name, bool disable)
            : base(rawConfig, name, disable) 
        {
            
        }

        protected override void loadConfig()
        {
            source = this.config.get("com_source_folder", "");
            exceptions = this.config.get("com_exceptions", "");
            destination = this.config.get("com_destination_folder", "");
            runParallel = this.config.get("com_run_parallel", "true").Trim().ToLower() == "true" ? true : false;
            var cservers = this.config.get("com_servers", "");
            servers = new RemoteServers(cservers);            
        }

        public override bool Deploy(MessageWriter msgWriter)
        {
            return DoCOMDeployment(msgWriter, false);
        }

        private bool DoCOMDeployment(MessageWriter msgWriter, bool preview)
        {
            this.msgWriter = msgWriter;
            string msgStart = preview ? "Preview - " : "";
            bool success = true;
            try
            {
                if (this.servers.ignoredServers.Count > 0)
                {
                    msgWriter.WriteLine(Color.Brown, String.Format("{0}There are duplicated COM servers in the config file, only one instance of them will be used. The instances that are ignored are:\r\n{1}", msgStart, this.servers.ignoredServersConfig), new LogLevel(Level.WARN));
                }

                if (runParallel)
                {
                    msgWriter.WriteLine(Color.Blue, String.Format("\n{0}Start new Components Registration on remote servers in parallel", msgStart), new LogLevel(Level.INFO));
                    TimeSpan s = TimeTracker.trackTimeForAction(() =>
                    {
                        ParallelOptions po = new ParallelOptions() { MaxDegreeOfParallelism = this.servers.Keys.Count };
                        if (preview) 
                        {
                            //ParallelTasksRunner.runParallel(this.servers, new WorkOnServer(PreviewRegisterOnServer));
                            Parallel.ForEach(this.servers.Values, po, PreviewRegisterOnServer);
                        }
                        else
                        {
                            
                            Parallel.ForEach(this.servers.Values, po, RegisterOnServer);
                            //ParallelTasksRunner.runParallel(this.servers, new WorkOnServer(RegisterOnServer));
                        }
                    });
                    msgWriter.WriteLine(Color.Green, String.Format("{0}Done, took {1} seconds", msgStart, s.TotalSeconds), new LogLevel(Level.INFO));
                }
                else
                {
                    msgWriter.WriteLine(Color.Blue, String.Format("\n{0}Start new Components Registration on remote servers", msgStart), new LogLevel(Level.INFO));
                    TimeSpan s = TimeTracker.trackTimeForAction(() =>
                    {
                        foreach (RemoteServer server in this.servers.Values)
                        {
                            doRegisterOnServer(server, preview);
                        }
                    });
                    this.msgWriter.WriteLine(Color.Green, String.Format("{0}Done, took {1} seconds", msgStart, s.TotalSeconds), new LogLevel(Level.INFO));
                }
                this.msgWriter.WriteLine(Color.DarkGreen, String.Format("\n{0}Components Registration is Done for all remote servers\n", msgStart), new LogLevel(Level.INFO));
            }
            catch (System.Security.SecurityException ser)
            {
                msgWriter.WriteException(ser);
                msgWriter.WriteLine(Color.Brown, msgStart + "You should run the program from a local drive (C:\\)\n", new LogLevel(Level.ERROR));
                success = false;
            }
            catch (Exception er)
            {
                msgWriter.WriteException(er);
                success = false;
            }

            return success;
        }


        public override bool PreviewDeploy(MessageWriter msgWriter)
        {
            return DoCOMDeployment(msgWriter, true);            
        }

        public void RegisterOnServer(RemoteServer server) { doRegisterOnServer(server, false); }
        public void PreviewRegisterOnServer(RemoteServer server) { doRegisterOnServer(server, true); }

        public void doRegisterOnServer(RemoteServer server, bool preview)
        {
            string msgStart = preview ? "Preview - Thread: " : "Thread: ";
            ComponentsRegistration com = new ComponentsRegistration(msgWriter);
            com.registrationExceptions = exceptions;
            msgWriter.WriteLine(Color.Blue, String.Format("\n{0}{1} Start Registration for: {2}", msgStart, Thread.CurrentThread.ManagedThreadId, server.name), new LogLevel(Level.INFO));
            com.register(source, new COMDestination(server), preview);
            msgWriter.WriteLine(Color.Blue, String.Format("{0}{1} Registration is Done for: {2}", msgStart, Thread.CurrentThread.ManagedThreadId, server.name), new LogLevel(Level.INFO));
        }

    }
}
