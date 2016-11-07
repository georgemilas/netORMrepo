using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace EM.Util
{
    /// <summary>
    /// Example of turning a PDF files into a PostScript using an external tool:
    /// 
    /// ShellProcess proc = new ShellProcess();
    /// proc.process = @"C:\tools\xpdf-3.01-win32\pdftops.exe";
    /// FileInfo[] PSs = proc.runForFiles(PDFs, 
    ///         delegate(FileInfo file, ProcessStartInfo pinfo) {
    ///             return file.FullName.Split(new char[] { '.' })[0] + ".ps";
    ///         },
    ///         delegate(FileInfo pdf, string currentArgs) {
    ///             return " \"" + pdf.FullName + "\"";
    ///         }
    ///     );  
    /// 
    /// Example of compressing files using rar:
    /// 
    /// proc.process = @"C:\ProgramFiles\WinRar\rar.exe";
    /// proc.processOneByOne = false;   //process all files by only one instance of the executable
    /// proc.sleepForEnd = true;        //allow for some rar processes to startup before checking if it finished
    /// FileInfo[] rar = proc.runForFiles(files, 
    ///         delegate(FileInfo file, ProcessStartInfo pinfo) {
    ///             //return first file name as resulting file name
    ///             Regex ptt = new Regex(@"\".+\.rar\"");
    ///             Match mptt = ptt.Match(pinfo.Arguments));
    ///             return mptt.Value.Substring(1, mptt.Value.Length-3);
    ///         },
    ///         delegate(FileInfo file, string currentArgs) {
    ///             if (currentArgs != null && currentArgs != "")
    ///             {
    ///                 return currentArgs + " \"" +  file.FullName + "\"";
    ///             }
    ///             else 
    ///             {
    ///                 return String.Format(" a -ep \"{0}.rar\" \"{1}\"", file.FullName.Split(new char[] {'.'})[0], file.FullName);
    ///             }
    ///         }
    ///     );
    /// </summary>

    public class ShellProcess
    {
        private string _processPath;
        private string _processName;
        public bool processOneByOne;
        public bool sleepForEnd;
        public bool printTrace = true;

        public ShellProcess() 
            : this(null) 
        { }

        public ShellProcess(string pathToExecutable)
        {
            this.processOneByOne = true;
            this.sleepForEnd = false;
            if (pathToExecutable != null)
            {
                this.process = pathToExecutable;
            }
        }

        /// <summary>
        /// path to an execulable file (ex: C:\tools\WinZip.exe)
        /// </summary>
        public string process
        {
            get
            {
                if (this._processPath != null && this._processPath.Trim() == "")
                {
                    return null;
                }
                return this._processPath;
            }
            set
            {
                this._processPath = value;
                string[] pathsplit = this._processPath.Split(new char[] { '\\', '.' });
                this._processName = pathsplit[pathsplit.Length - 2];
            }
        }
        
        public string processName
        {
            get
            {
                return this._processName;
            }
        }

        /// <summary>
        /// resulting file extension (ex: ".rar" or ".pdf" etc)
        /// </summary>

        public delegate string GetArguments(FileInfo file, string currentArgs);
        public delegate string GetExpectedResultFile(FileInfo file, ProcessStartInfo pinfo);

        public void run(string args) { run(args, false); }
        public void run(string args, bool inShell)
        {
            Process proc = new Process();
            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.FileName = this.process;
            procInfo.UseShellExecute = inShell;
            if (args != null)
            {
                procInfo.Arguments = args;                
            }
            proc.StartInfo = procInfo;
            proc.Start();

            //wait to finish all the jobs
            if (this.sleepForEnd)
            {   //some processes won't start up instantly so wait for one of them to start 
                Thread.Sleep(5000);
            }

            Process[] allprocesses = Process.GetProcesses();
            foreach (Process p in allprocesses)
            {
                if (p.ProcessName == this.processName)
                {
                    if (this.printTrace) Console.WriteLine("^^^^^^^^^^WAIT FOR ALL {0} to exit^^^^^^^^^^^", this.processName);
                    p.WaitForExit();
                    if (this.printTrace) Console.WriteLine("**********ALL {0} DONE***********", this.processName);
                }
            }
            
            if (this.sleepForEnd)
            {   //some processes won't start up instantly so wait for one of them to start 
                Thread.Sleep(5000);
            }
        }

        public FileInfo[] runForFiles(FileInfo[] files, GetExpectedResultFile resGetter, GetArguments argGetter)
        {   
            if (files.Length == 0 || this.process==null)
            {
                return new FileInfo[0];
            }

            List<string> resExpect = new List<string>();  //list of paths to expect as a result

            Process proc = new Process();
            ProcessStartInfo procInfo = new ProcessStartInfo();
            procInfo.FileName = this.process;
            procInfo.UseShellExecute = false;
            
            foreach (FileInfo file in files)
            {
                procInfo.Arguments = argGetter(file, procInfo.Arguments);
                if (this.processOneByOne)
                {
                    proc.StartInfo = procInfo;
                    resExpect.Add(resGetter(file, procInfo));
                    proc.Start();
                    if (this.printTrace) Console.WriteLine("+++++++++++++++DO 1 {0}++++++++++++++++", this.processName);

                    proc = new Process();
                    procInfo = new ProcessStartInfo();
                    procInfo.FileName = this.process;
                    procInfo.UseShellExecute = false;

                }
            }
            if (!this.processOneByOne)
            {
                proc.StartInfo = procInfo;
                resExpect.Add(resGetter(null, procInfo));
                proc.Start();
                if (this.printTrace) Console.WriteLine("+++++++++++++++DO ALL at once {0}++++++++++++++++", this.processName);
            }

            //wait to finish all the jobs
            if (this.sleepForEnd)
            {   //some processes won't start up instantly so wait for one of them to start 
                Thread.Sleep(5000);
            }

            Process[] allprocesses = Process.GetProcesses();
            foreach (Process p in allprocesses)
            {
                if (p.ProcessName == this.processName)
                {
                    if (this.printTrace) Console.WriteLine("^^^^^^^^^^WAIT FOR ALL {0} to exit^^^^^^^^^^^", this.processName);
                    p.WaitForExit();
                    if (this.printTrace) Console.WriteLine("**********ALL {0} DONE***********", this.processName);
                }
            }
            if (this.sleepForEnd)
            {   //some processes won't start up instantly so wait for one of them to start 
                Thread.Sleep(5000);
            }

            //verify wich files were succesfuly created 
            List<FileInfo> res = new List<FileInfo>();
            foreach (string path in resExpect)
            {
                if (File.Exists(path))
                {   //TODO: what if it creates a file but is an empty file because some errors and did not delete the incomplete file?
                    res.Add(new FileInfo(path));
                }
            }

            return res.ToArray();
        }
    }




}
