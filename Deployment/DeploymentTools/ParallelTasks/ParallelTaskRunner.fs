#light

namespace DeploymentTools.Parallel

open DeploymentTools
open TreeSync
open System.Drawing
open System.Threading

type ParallelTasksRunner = 
    
    ///run WorkOnServer in parallel for a given list of servers
    static member runParallel (servers:RemoteServers, sworker:WorkOnServer ) =
        let invokeAsync ss = Async.FromBeginEnd(ss, sworker.BeginInvoke, sworker.EndInvoke)
        [ for srv in servers.Values -> srv ]
        |> List.map (fun s -> async { 
                                        let! res = invokeAsync s
                                        return 1
                                    })
        |> Async.Parallel 
        |> Async.RunSynchronously 
        |> ignore
            
            
    ///like the runParallel method but spelling out the steps to do on the RemoteServers (namely: copy files)        
//    static member runCopyInParallel (servers:RemoteServers, fd:#ISourceContainer, cpMethod, preview ) =
//        [ for srv in servers.Values -> srv ]
//        |> List.map (fun s -> async { 
//                                        fd.msgWriter.WriteLine(Color.Blue, "Thread: " + Thread.CurrentThread.ManagedThreadId.ToString() + (if fd.preview then " Preview - " else " ") + "Copying files for {0}... ", s.computerName);
//                                        let serverPath = s.remotePath;
//                                        let dest = new FileSystemFolderTree(serverPath, new OSFileSystem(), fd.msgWriter);
//                                        let ts = new TreeSync(fd.source, dest, null, fd.msgWriter);
//                                        ts.preview <- preview
//                                        ts.doTreeSync(cpMethod);    
//                                    })
//        |> Async.Parallel 
//        |> Async.RunSynchronously 
//        |> ignore   



        