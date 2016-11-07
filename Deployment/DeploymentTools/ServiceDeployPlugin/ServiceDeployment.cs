using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections;
using EM.Logging;
using System.Drawing;
using System.IO;
using System.Threading;
using TreeSync;
using System.Diagnostics;
using EM.Util;
using System.ServiceProcess;
using DeploymentTools.Parallel;
using System.Collections;

namespace DeploymentTools
{
    
    public class ServiceDeployment
    {
        public MessageWriter msgWriter;
        public string sourceFolder;
        public bool preview = false;
        public bool runInParallel = true;

        private const int WAIT_MINUTES = 1;

        public RemoteServers servers;
        public SimpleConfigParser config;

        public string serviceName;

        public FileSystemFolderTree source 
        {
            get 
            {
                string src = this.sourceFolder;
                if (src.EndsWith("/") || src.EndsWith("\\"))
                {
                    src = src.slice(null, -1);
                }
                return new FileSystemFolderTree(src, new OSFileSystem(), this.msgWriter);
            }
        }


        public ServiceDeployment(MessageWriter msgWriter, 
            string sourceFolder, 
            RemoteServers servers, 
            SimpleConfigParser config)
        {

            this.msgWriter = msgWriter;
            this.sourceFolder = sourceFolder;
            this.servers = servers;
            this.config = config;

            this.serviceName = config["service_name"];
            
            //source = new FileSystemFolderTree(this.sourceFolder, new OSFileSystem(), this.msgWriter);
        }

        public bool doStopService(RemoteServer server)
        {
            string msgStart = preview ? "Preview - Thread: " : "Thread: ";
            this.msgWriter.WriteLine(Color.Blue, String.Format("{0}{1} Stop service {2} on {3}", msgStart, Thread.CurrentThread.ManagedThreadId, serviceName, server.computerUri), new LogLevel(Level.INFO));            
            bool ok = true;

            try
            {
                ServiceController sc = new ServiceController(this.serviceName, server.computerName);
                
                if (sc.Status.Equals(ServiceControllerStatus.Running))
                {
                    if (!preview)
                    {
                        sc.Stop();
                        try
                        {
                            sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMinutes(WAIT_MINUTES));
                        }
                        catch (System.ServiceProcess.TimeoutException err)
                        {
                            this.msgWriter.WriteLine(Color.Brown, String.Format("{0}{1} Continuing after waiting {{1}} minute(s) for the service to stop unsuccessfully {{0}} ... ", msgStart, Thread.CurrentThread.ManagedThreadId), new LogLevel(Level.ERROR), server.computerName, WAIT_MINUTES);
                            ok = false;
                        }
                    }
                }
            }
            catch(Exception err)
            {
                this.msgWriter.WriteLine(Color.Red, String.Format("{0}{1} Error connecting to the service {2} on computer {3}", msgStart, Thread.CurrentThread.ManagedThreadId, serviceName, server.computerName), new LogLevel(Level.ERROR));
                this.msgWriter.WriteException(err);
                ok = false;
            }
            
            return ok;
        }



        public virtual void runInThread(ThreadStart func)
        {
            Thread t = new Thread(delegate() 
            {
                func();                
            });
            t.IsBackground = true;
            t.Start();
        }
        
        public void doCopyFiles(RemoteServer server)
        {
            string msgStart = preview ? "Preview - Thread: " : "Thread: ";
            this.msgWriter.WriteLine(Color.Blue, String.Format("{0}{1} Copying files for {{0}}... ", msgStart, Thread.CurrentThread.ManagedThreadId), new LogLevel(Level.INFO), server.computerName);
            string serverPath = server.remotePath;
            if (serverPath.EndsWith("/") || serverPath.EndsWith("\\"))
            {
                serverPath = serverPath.slice(null, -1);
            }
            FileSystemFolderTree dest = new FileSystemFolderTree(serverPath, new OSFileSystem(), this.msgWriter);
            if (preview && !dest.fileSystem.exists(serverPath))
            {
                this.msgWriter.WriteLine(Color.Red, String.Format("Thread: {0}{1} Server Path was not found {{0}}... ", Thread.CurrentThread.ManagedThreadId, (this.preview ? " Preview - " : " ")), new LogLevel(Level.ERROR), serverPath);
                return;
            }
            TreeSync.TreeSync ts = new TreeSync.TreeSync(this.source, dest, config["service_copy_exclude"], this.msgWriter);            
            ts.preview = this.preview;
            ts.copy();
        }

        public bool doStartService(RemoteServer server)
        {
            string msgStart = preview ? "Preview - Thread: " : "Thread: ";
            this.msgWriter.WriteLine(Color.Blue, String.Format("{0}{1} Start service {2} on {3}", msgStart, Thread.CurrentThread.ManagedThreadId, serviceName, server.computerUri), new LogLevel(Level.INFO));

            bool ok = true;
            try
            {
                ServiceController sc = new ServiceController(this.serviceName, server.computerName);
                if (sc.Status.Equals(ServiceControllerStatus.Stopped))
                {
                    if (!preview)
                    {
                        sc.Start();
                        try
                        {
                            sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMinutes(WAIT_MINUTES));
                        }
                        catch (System.ServiceProcess.TimeoutException err)
                        {
                            this.msgWriter.WriteLine(Color.Brown, String.Format("{0}{1} Continuing after waiting {{1}} minute(s) for the service to start unsuccessfully {{0}} ... ", msgStart, Thread.CurrentThread.ManagedThreadId), new LogLevel(Level.WARN), server.computerName, WAIT_MINUTES);
                            ok = false;
                        }
                    }
                }
            }
            catch (Exception err)
            {
                this.msgWriter.WriteLine(Color.Red, String.Format("{0}{1} Error connecting to the service {2} on computer {3}", msgStart, Thread.CurrentThread.ManagedThreadId, serviceName, server.computerName), new LogLevel(Level.ERROR));
                this.msgWriter.WriteException(err);
                ok = false;
            }


            return ok;
        }


        public bool deploy(bool stopService, bool copyFiles, bool startService)
        {
            this.msgWriter.WriteLine(Color.Blue, "Start Deploy " + (this.preview ? "Preview" : "") + (this.runInParallel ? " In Parallel" : ""), new LogLevel(Level.INFO));
            bool ok = true;
            TimeSpan s = TimeTracker.trackTimeForAction(() =>
                {
                    ok = deployStopService(stopService);
                    deployCopyFiles(copyFiles);
                    ok = deployStartService(startService) && ok;
                }
            );
            this.msgWriter.WriteLine(Color.Green, "Done, took " + s.TotalSeconds + " seconds", new LogLevel(Level.INFO));
            if (ok)
            {
                this.msgWriter.WriteLine(Color.DarkGreen, "Deploy was successfull", new LogLevel(Level.INFO));
            }
            else
            {
                this.msgWriter.WriteLine(Color.Red, "Deploy finished with issues", new LogLevel(Level.ERROR));
            }
            return ok;
        }

        private void deployCopyFiles(bool copyFiles)
        {
            if (copyFiles)
            {
                this.msgWriter.WriteLine(Color.Blue, "Copy Files", new LogLevel(Level.INFO));
                if (runInParallel)
                {
                    ParallelTasksRunner.runParallel(this.servers, new WorkOnServer(this.doCopyFiles));
                }
                else
                {
                    foreach (RemoteServer server in this.servers.Values)
                    {
                        this.doCopyFiles(server);
                        this.msgWriter.WriteLine(" ", new LogLevel(Level.DEBUG));
                    }
                }
            }
            else
            {
                this.msgWriter.WriteLine(Color.DarkGray, "Copy files is disabled", new LogLevel(Level.INFO));
            }
        }

        private object _lockObject = new object();
        private bool deployStopService(bool stopService)
        {
            bool ok = true;
            if (stopService)
            {

                this.msgWriter.WriteLine(Color.Blue, "Stop Service", new LogLevel(Level.INFO));
                if (runInParallel)
                {
                    ParallelTasksRunner.runParallel(this.servers, new WorkOnServer((srv) => 
                        {
                            var oks = this.doStopService(srv);
                            lock (_lockObject)
                            {
                                ok = oks && ok;
                            }
                        }));
                }
                else
                {
                    foreach (RemoteServer server in this.servers.Values)
                    {
                        try
                        {
                            ok = this.doStopService(server) && ok;
                        }
                        catch (Exception err)
                        {
                            this.msgWriter.WriteException(err);
                        }
                    }
                }           
            }
            else
            {
                this.msgWriter.WriteLine(Color.DarkGray, "Stop Service is disabled", new LogLevel(Level.INFO));
            }

            return ok;
        }

    

        private bool deployStartService(bool restartServers)
        {
            bool ok = true;
            if (restartServers)
            {
                this.msgWriter.WriteLine(Color.Blue, "Start Service", new LogLevel(Level.INFO));
                if (runInParallel)
                {
                    ParallelTasksRunner.runParallel(this.servers, new WorkOnServer((srv) => 
                        {
                            var oks = this.doStartService(srv);
                            lock (_lockObject)
                            {
                                ok = oks && ok;
                            }
                        }));
                }
                else
                {
                    foreach (RemoteServer server in this.servers.Values)
                    {
                        try
                        {
                            ok = this.doStartService(server) && ok;
                        }
                        catch (Exception err)
                        {
                            this.msgWriter.WriteException(err);
                        }
                    }
                }
            }
            else
            {
                this.msgWriter.WriteLine(Color.DarkGray, "Start Service is disabled", new LogLevel(Level.INFO));
            }
            return ok;
        }
        

    }


    




}
