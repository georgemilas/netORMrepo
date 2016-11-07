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
using DeploymentTools.Parallel;

namespace DeploymentTools
{

    public class FilesDeployment : ISourceContainer
    {
        public delegate void OnCancelHandler();
        public event OnCancelHandler OnCancel;

        public MessageWriter msgWriter { get; set; }
        public string sourceFolder;
        public bool preview { get; set; } 
        public bool runInParallel = true;
        private bool cancel = false;
        
        public RemoteServers servers;
        public SimpleConfigParser config;

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

        public FilesDeployment(MessageWriter msgWriter, 
            string sourceFolder, 
            RemoteServers servers, 
            SimpleConfigParser config)
        {

            this.msgWriter = msgWriter;
            this.sourceFolder = sourceFolder;
            this.servers = servers;
            this.config = config;
            this.preview = false;

            this.cancel = false;
        }

        public void deployTakeOutOfTeam(RemoteServer server)
        {
            if (!cancel)
            {
                string f = String.Format("{0}\\{1}", server.computerUri, this.config["prod_team_file"]);
                FileInfo fi = new FileInfo(f);
                string tname = String.Format("{0}\\{1}", fi.DirectoryName, fi.Name.Replace(fi.Extension, "_OUT_" + fi.Extension));
                if (!this.preview)
                {
                    fi.MoveTo(tname);
                }
                this.msgWriter.WriteLine(String.Format("{0}Renamed {1}", (this.preview ? "Preview - " : ""), tname), new LogLevel(Level.DEBUG));
            }
        }

        public bool doPutIntoTeam(RemoteServer server)
        {
            if (!cancel)
            {
                try
                {
                    string f = server.computerUri + "\\" + this.config["prod_team_file"];
                    FileInfo fi = new FileInfo(f);
                    string tname = fi.DirectoryName + "\\" + fi.Name.Replace(fi.Extension, "_OUT_" + fi.Extension);
                    FileInfo ti = new FileInfo(tname);
                    if (!this.preview)
                    {
                        ti.MoveTo(fi.FullName);
                    }
                    this.msgWriter.WriteLine((this.preview ? "Preview - " : "") + "Renamed {0} to {1} ", new LogLevel(Level.DEBUG), ti.Name, fi.FullName);
                    return true;
                }
                catch (IOException ex)
                {
                    return false;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                return false;
            }
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
        
        public void deployCopyFiles(RemoteServer server)
        {
            if (!cancel)
            {
                this.msgWriter.WriteLine(Color.Blue, String.Format("Thread: {0}{1}Copying files for {{0}}... ", Thread.CurrentThread.ManagedThreadId, (this.preview ? " Preview - " : " ")), new LogLevel(Level.INFO), server.computerName);
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

                TreeSync.TreeSync ts = new TreeSync.TreeSync(this.source, dest, null, this.msgWriter);
                ts.preview = this.preview;
                string cpMethod = this.config.setdefault("prod_copy_method", "copy");
                var stopper = new OnCancelHandler(() => { ts.cancelAllWorkers(); });
                this.OnCancel += stopper;
                ts.doTreeSync(cpMethod);
                this.OnCancel -= stopper;
            }
        }


        public void doResetIIS(RemoteServer server)
        {
            if (!cancel)
            {
                this.msgWriter.WriteLine(Color.Blue, "Reset IIS on server {0}... ", new LogLevel(Level.INFO), server.computerName);
                if (!preview)
                {
                    Process proc = new Process();
                    ProcessStartInfo procInfo = new ProcessStartInfo();
                    procInfo.FileName = "iisreset";
                    procInfo.UseShellExecute = true;
                    procInfo.Arguments = server.computerUri;
                    proc.StartInfo = procInfo;
                    proc.Start();
                }
                else
                {
                    this.msgWriter.WriteLine("Preview: iisreset " + server.computerUri, new LogLevel(Level.DEBUG));
                }
            }
        }

        public void doRestartServer(RemoteServer server)
        {
            if (!cancel)
            {
                //remote shutdown  -- http://technet.microsoft.com/en-us/library/cc780360.aspx#BKMK_winui
                //shutdown -r -f -m \\Tsnode2-dev -t 10 -c "deployment restart" -d up:4:2   

                this.msgWriter.WriteLine(Color.Blue, "Restart server {0}... ", new LogLevel(Level.INFO), server.computerName);
                if (!preview)
                {
                    Process proc = new Process();
                    ProcessStartInfo procInfo = new ProcessStartInfo();
                    procInfo.FileName = "shutdown";
                    procInfo.UseShellExecute = true;
                    string restartArgs = this.config.setdefault("prod_restart_params", "-r -f \\\\computername -t 10 -c \"deployment restart\" -d up:4:2");
                    procInfo.Arguments = restartArgs.Replace(@"\\computername", server.computerUri);
                    proc.StartInfo = procInfo;
                    proc.Start();
                }
                else
                {
                    this.msgWriter.WriteLine("Preview: shutdown /r /m " + server.computerUri + "/d[p:]4:2", new LogLevel(Level.DEBUG));
                }
            }
        }

        //private void runCopyInParallel(RemoteServers servers)
        //{
        //    var m = new ManualResetEvent(false);
        //    int todo = servers.Values.Count;
        //    string cpMethod = this.config.setdefault("prod_copy_method", "copy");                        
        //    object o = new object();
        //    foreach (var s in servers.Values)
        //    {
        //        ThreadPool.QueueUserWorkItem(new WaitCallback((obj) =>
        //        {
        //            //this.deployCopyFiles((RemoteServer)obj);  //does not work

        //            TreeSync.TreeSync ts = null;
        //            lock (o)
        //            {
        //                RemoteServer server = (RemoteServer)obj;
        //                this.msgWriter.WriteLine(Color.Blue, "Thread: " + Thread.CurrentThread.ManagedThreadId.ToString() + (this.preview ? " Preview - " : " ") + "Copying files for {0}... ", server.computerName);
        //                string serverPath = server.remotePath;
        //                FileSystemFolderTree dest = new FileSystemFolderTree(serverPath, new OSFileSystem(), this.msgWriter);
        //                ts = new TreeSync.TreeSync(this.source, dest, null, this.msgWriter);
        //                ts.preview = this.preview;
        //            }
        //            ts.doTreeSync(cpMethod);  //will "ts" be the same after switching threads?

        //            Interlocked.Decrement(ref todo);
        //            if (todo == 0)
        //            {
        //                m.Set();
        //            }
        //        }), s);
        //    }
        //    m.WaitOne();    
        //}


        public void deploy(bool takeOutOfTeam, bool copyFiles, bool restartServers, bool putBackIntoTeam, bool resetIIS)
        {
            this.cancel = false;
            this.msgWriter.WriteLine(Color.Blue, "Start Deploy " + (this.preview ? "Preview" : ""), new LogLevel(Level.INFO));
            deployTakeOutOfTeam(takeOutOfTeam);
            deployCopyFiles(copyFiles);
            deployResetIIS(resetIIS);
            deployRestartServers(restartServers);
            deployPutBackIntoTeam(putBackIntoTeam);
            this.msgWriter.WriteLine(Color.DarkGreen, "Deploy was successfull", new LogLevel(Level.INFO));
        }

        private void deployCopyFiles(bool copyFiles)
        {
            if (!cancel)
            {
                if (copyFiles)
                {
                    if (this.runInParallel) //  && !preview)
                    {
                        this.msgWriter.WriteLine(Color.Blue, "Copy Files In Parralel", new LogLevel(Level.INFO));
                        TimeSpan s = TimeTracker.trackTimeForAction(() =>
                            {
                                ParallelTasksRunner.runParallel(servers, new WorkOnServer(this.deployCopyFiles));
                                //this.runCopyInParallel(servers);
                                //ParallelTasksRunner.runCopyInParallel(servers, this, this.config.setdefault("prod_copy_method", "copy"), this.preview);
                            });
                        this.msgWriter.WriteLine(Color.Green, String.Format("Done, took {0} seconds", s.TotalSeconds), new LogLevel(Level.INFO));
                    }
                    else
                    {
                        this.msgWriter.WriteLine(Color.Blue, "Copy Files Synchronously", new LogLevel(Level.INFO));
                        TimeSpan s = TimeTracker.trackTimeForAction(() =>
                        {
                            foreach (RemoteServer server in this.servers.Values)
                            {
                                if (cancel) { break; }
                                this.deployCopyFiles(server);
                                this.msgWriter.WriteLine(" ", new LogLevel(Level.DEBUG));
                            }
                        });
                        this.msgWriter.WriteLine(Color.Green, String.Format("Done, took {0} seconds", s.TotalSeconds), new LogLevel(Level.INFO));
                    }
                }
                else
                {
                    this.msgWriter.WriteLine(Color.DarkGray, "Copy files is disabled", new LogLevel(Level.INFO));
                }
            }
        }

        private void deployTakeOutOfTeam(bool takeOutOfTeam)
        {
            if (!cancel)
            {
                if (takeOutOfTeam)
                {

                    this.msgWriter.WriteLine(Color.Blue, "Take out of team", new LogLevel(Level.INFO));
                    foreach (RemoteServer server in this.servers.Values)
                    {
                        if (cancel) { break; }
                        this.deployTakeOutOfTeam(server);
                    }

                    int secwait = int.Parse(this.config.setdefault("prod_team_out_seconds", "45"));
                    this.msgWriter.WriteLine(Color.Blue, string.Format("Wait {0} seconds... ", secwait), new LogLevel(Level.INFO));
                    if (!preview && !cancel) { Thread.Sleep(TimeSpan.FromSeconds(secwait)); }

                }
                else
                {
                    this.msgWriter.WriteLine(Color.DarkGray, "Take out of team is disabled", new LogLevel(Level.INFO));
                }
            }
        }

        private void deployPutBackIntoTeam(bool putBackIntoTeam)
        {
            if (!cancel)
            {
                if (putBackIntoTeam)
                {
                    this.msgWriter.WriteLine(Color.Blue, "Put Back into Team", new LogLevel(Level.INFO));
                    DateTime now = DateTime.Now;
                    DateTime start = now;
                    EList<string> serversToDo = EList<string>.fromEnumarable(this.servers.Keys);
                    int retryTime = int.Parse(this.config.setdefault("prod_team_in_minutes", "5"));  //by default retry for no more then 5 minutes 
                    while (serversToDo.Count > 0 || ((TimeSpan)(now - start)).Minutes >= retryTime)
                    {
                        if (cancel) { break; }

                        EList<string> serversDone = new EList<string>();
                        foreach (string server in serversToDo)
                        {
                            if (cancel) { break; }
                            bool sok = this.doPutIntoTeam(this.servers[server]);
                            if (!sok)
                            {
                                this.msgWriter.WriteLine(Color.Brown, "Failed to put back into team server {0}, retry. ", new LogLevel(Level.ERROR), server);
                            }
                            else
                            {
                                serversDone.Add(server);
                            }
                        }

                        if (serversDone.Count != serversToDo.Count)
                        {
                            this.msgWriter.WriteLine(Color.Brown, "Could not put all servers back into team, waiting for 30 seconds to retry... ", new LogLevel(Level.ERROR));
                            if (!preview) { Thread.Sleep(TimeSpan.FromSeconds(30)); }
                        }
                        serversToDo = serversToDo.left_diference(serversDone);
                    }
                }
                else
                {
                    this.msgWriter.WriteLine(Color.DarkGray, "Put servers back into team is disabled", new LogLevel(Level.INFO));
                }
            }
        }

        private void deployResetIIS(bool resetIIS)
        {
            if (!cancel)
            {
                if (resetIIS)
                {
                    this.msgWriter.WriteLine(Color.Blue, "Reset IIS", new LogLevel(Level.INFO));
                    foreach (RemoteServer server in this.servers.Values)
                    {
                        if (cancel) { break; }
                        this.doResetIIS(server);
                    }                    
                }
                else
                {
                    this.msgWriter.WriteLine(Color.DarkGray, "Reset IIS is disabled", new LogLevel(Level.INFO));
                }
            }
        }

        private void deployRestartServers(bool restartServers)
        {
            if (!cancel)
            {
                if (restartServers)
                {
                    this.msgWriter.WriteLine(Color.Blue, "Restart Servers", new LogLevel(Level.INFO));
                    foreach (RemoteServer server in this.servers.Values)
                    {
                        if (cancel) { break; }
                        this.doRestartServer(server);
                    }
                    int minwait = int.Parse(this.config.setdefault("prod_restart_minutes", "3"));
                    this.msgWriter.WriteLine(Color.Blue, string.Format("Wait {0} minutes... ", minwait), new LogLevel(Level.INFO));
                    if (!preview && !cancel) { Thread.Sleep(TimeSpan.FromMinutes(minwait)); }
                }
                else
                {
                    this.msgWriter.WriteLine(Color.DarkGray, "Restart server is disabled", new LogLevel(Level.INFO));
                }
            }
        }

        public void doCancel()
        {
            this.msgWriter.WriteLine(Color.Blue, "User Cancel Request", new LogLevel(Level.INFO));
            this.cancel = true;
            if (this.OnCancel != null)
            {
                this.OnCancel();
            }
        }


        


    }


    




}
