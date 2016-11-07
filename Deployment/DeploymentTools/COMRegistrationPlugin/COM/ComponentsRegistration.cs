using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using MTSAdmin;
using System.Drawing;
using EM.parser.keywords;
using EM.Logging;
using System.Threading;


namespace DeploymentTools
{

    public class ComponentsRegistration
    {
        private MessageWriter messageWriter;
        
        public ComponentsRegistration(MessageWriter writer)
        {
            this.messageWriter = writer;            
        }

        private string _registrationExceptions;
        private KeywordsExpressionParser registrationExceptionsParser;
        public virtual string registrationExceptions
        {
            get { return _registrationExceptions; }
            set 
            { 
                _registrationExceptions = value;
                if (!string.IsNullOrEmpty(value) && value.Trim() != "")
                {
                    registrationExceptionsParser = new KeywordsExpressionParser(value.Trim().ToLower());
                }
            }
        }

        public bool isDoExclude(string dllPath)
        {
            if (registrationExceptionsParser != null)
            {
                return (bool)registrationExceptionsParser.evaluate(dllPath.Trim().ToLower());
            }
            return false;
        }

        protected bool contains(FileInfo[] files, string dllPath)
        {
            string dllName = new FileInfo(dllPath).Name;
            foreach (FileInfo f in files)
            {
                if (f.Name == dllName)
                {
                    return true;
                }
            }
            return false;
        }

        //public void test()
        //{
        //    this.register("C:\\WebSites\\SurePayrollWeb\\SurePayrollWeb\\Objects", "C:\\Objects");
        //}

        public void doNonMTS(DirectoryInfo src, DirectoryInfo dest, COMDestination destination, bool preview)
        {
            string msgStart = preview ? "Preview - Thread: " : "Thread: ";

            DirectoryInfo[] subdirs = src.GetDirectories();
            foreach (DirectoryInfo subdir in subdirs)
            {
                if (subdir.Name.ToLower() == "nonmts")
                {
                    FileInfo[] srcFiles = subdir.GetFiles("*.dll");
                    if (srcFiles.Length > 0)
                    {
                        string destpath  = dest.FullName + "\\" + subdir.Name;
                        if ( !Directory.Exists(destpath) )
                        {
                            messageWriter.WriteLine(Color.Brown, String.Format("{0}{1} Destination folder does not exist, needs to be created {2}", msgStart, Thread.CurrentThread.ManagedThreadId, destpath), new LogLevel(Level.WARN));
                            if (!preview)
                            {
                                Directory.CreateDirectory(destpath);
                            }
                        }
                        CopyAndRegisterFiles(new DirectoryInfo(destpath), srcFiles, null, destination, preview);
                    }
                    break;
                }
            }
        }

        public void register(string srcFolderPath, COMDestination destination, bool preview)
        {
            //http://technet.microsoft.com/en-us/library/cc750041.aspx
            try
            {
                string msgStart = preview ? "Preview - Thread: " : "Thread: ";
                string computer = destination.isRemoteComputer ? destination.remoteComputer : "Local - " + destination.copyPath;

                DirectoryInfo src = new DirectoryInfo(srcFolderPath);
                DirectoryInfo dest = new DirectoryInfo(destination.copyPath);

                doNonMTS(src, dest, destination, preview);

                FileInfo[] srcFiles = src.GetFiles("*.dll");
                if (srcFiles.Length <= 0)
                {
                    messageWriter.WriteLine(Color.Brown, String.Format("{0}{1} No DLLs found in source folder, nothing to register.", msgStart, Thread.CurrentThread.ManagedThreadId), new LogLevel(Level.WARN));
                    return;     //no com compontes to register
                }

                if (!dest.Exists)
                {
                    messageWriter.WriteLine(Color.Brown, String.Format("{0}{1} Destination folder does not exist, needs to be created {2}", msgStart, Thread.CurrentThread.ManagedThreadId, dest.FullName), new LogLevel(Level.WARN));
                    if (!preview)
                    {
                        dest.Create();
                    }
                }

                CatalogClass c = new CatalogClass();
                if ( destination.isRemoteComputer )
                {
                    c.Connect(destination.remoteComputer);
                }
                
                ICatalogCollection packages = (ICatalogCollection)c.GetCollection("Packages");
                packages.Populate();

                ICatalogObject surepayroll = null;
                foreach (ICatalogObject p in packages)
                {
                    if (p.Name.ToString() == "SurePayroll")
                    {
                        surepayroll = p;
                        IPackageUtil packageUtil = (IPackageUtil)packages.GetUtilInterface();
                        if (!preview)
                        {
                            packageUtil.ShutdownPackage(surepayroll.Key.ToString());
                        }
                        messageWriter.WriteLine(Color.Blue, String.Format("{0}{1} {2} SurePayroll package was shut down", msgStart, Thread.CurrentThread.ManagedThreadId, computer), new LogLevel(Level.INFO));
                        break;
                    }
                }

                if (surepayroll == null)
                {
                    throw new NotSupportedException(String.Format("Could not find SurePayroll in the COM+ applications in {0}. Create it if necessary and make sure the name is exactly SurePayroll", computer));
                }

                ICatalogCollection components = (ICatalogCollection)packages.GetCollection("ComponentsInPackage", surepayroll.Key);
                components.Populate();

                int i = 0;
                int j = 0;
                foreach (ICatalogObject component in components)
                {
                    string dllPath = component.get_Value("DLL").ToString();
                    if (contains(srcFiles, dllPath))
                    {
                        if (this.isDoExclude(dllPath))
                        {
                            messageWriter.WriteLine(Color.DimGray, String.Format("{0}{1} {2} SKIP un-regisration for {{0}}.   {{1}}   - {{2}}", msgStart, Thread.CurrentThread.ManagedThreadId, computer), new LogLevel(Level.DEBUG), i, component.Name, component.get_Value("DLL"));
                            j++;   //skip it
                        }
                        else
                        {
                            messageWriter.WriteLine(String.Format("{0}{1} {2} UN-REGISTER {{0}}.   {{1}}   - {{2}}", msgStart, Thread.CurrentThread.ManagedThreadId, computer), new LogLevel(Level.DEBUG), i, component.Name, component.get_Value("DLL"));
                            if (!preview)
                            {
                                components.Remove(j);
                            }
                        }
                    }
                    else
                    {
                        j++;
                    }
                    i++;
                }
                if (!preview)
                {
                    components.SaveChanges();
                }

                IComponentUtil componentUtil = (IComponentUtil)components.GetUtilInterface();
                CopyAndRegisterFiles(dest, srcFiles, componentUtil, destination, preview);

            }
            catch (Exception e)
            {
                this.messageWriter.WriteException(e);
                if (e.Message.ToLower().Contains("component or application containing the component has been disabled"))
                {
                    string todo = @"You mighnt need enable network COM+ access on the remote computer:
                            1.	Click Start, point to Control Panel, and then click Add or Remove Programs.
                            2.	Click Add/Remove Windows Components.
                            3.	Select Application Server, and then click Details.
                            4.	Click Enable network COM+ access, and then click OK.
                            5.	Click Next, and then click Finish.
                            6.	Optionally Restart the computer. ";
                    this.messageWriter.WriteLine(Color.Brown, todo, new LogLevel(Level.ERROR));                     
                }
                
            }
        }

        
        /// <summary>
        /// copies files from srcFiles into dest and if componentUtil != null then also registeres the files
        /// </summary>
        private void CopyAndRegisterFiles(DirectoryInfo dest, FileInfo[] srcFiles, IComponentUtil componentUtil, COMDestination origDestination, bool preview)
        {
            string msgStart = preview ? "Preview - Thread: " : "Thread: ";
            string computer = origDestination.isRemoteComputer ? origDestination.remoteComputer : "Local - " + origDestination.copyPath;

            foreach (FileInfo file in srcFiles)
            {
                string destFilePath = String.Format("{0}\\{1}", dest.FullName, file.Name);

                if (this.isDoExclude(destFilePath))
                {
                    messageWriter.WriteLine(Color.DimGray, String.Format("{0}{1} {2} SKIP REGISTRATION FOR {{0}}", msgStart, Thread.CurrentThread.ManagedThreadId, computer), new LogLevel(Level.DEBUG), destFilePath);
                }
                else
                {
                    try
                    {
                        //Remove Read-Only attribute
                        if ((file.Attributes & FileAttributes.ReadOnly) != 0)
                        {
                            if (!preview) { file.Attributes = FileAttributes.Normal; }
                            messageWriter.WriteLine(Color.Blue, String.Format("{0}{1} {2} Remove Read-Only Attribute {{0}}", msgStart, Thread.CurrentThread.ManagedThreadId, computer), new LogLevel(Level.INFO), file.Name);                            
                        }
                        if (File.Exists(destFilePath) && ((File.GetAttributes(destFilePath) & FileAttributes.ReadOnly) != 0))
                        {
                            if (!preview) { File.SetAttributes(destFilePath, FileAttributes.Normal); }
                            messageWriter.WriteLine(Color.Blue, String.Format("{0}{1} {2} Remove Read-Only Attribute {{0}}", msgStart, Thread.CurrentThread.ManagedThreadId, computer), new LogLevel(Level.INFO), destFilePath);                            
                        }

                        if (!preview)
                        {
                            file.CopyTo(destFilePath, true);
                        }
                        messageWriter.WriteLine(String.Format("{0}{1} {2} COPY {{0}} - {{1}}", msgStart, Thread.CurrentThread.ManagedThreadId, computer), new LogLevel(Level.DEBUG), file.Name, destFilePath);
                        if (componentUtil != null)
                        {
                            string regPath = destFilePath;
                            if (origDestination != null)
                            {
                                regPath = regPath.Replace(origDestination.copyPath, origDestination.registrationPath);
                            }
                            if (!preview)
                            {
                                componentUtil.InstallComponent(regPath, "", "");
                            }
                            messageWriter.WriteLine(Color.Blue, String.Format("{0}{1} {2} REGISTER {{0}}", msgStart, Thread.CurrentThread.ManagedThreadId, computer), new LogLevel(Level.INFO), regPath);                            
                        }

                    }
                    catch (Exception e)
                    {
                        messageWriter.WriteLine(Color.Brown, String.Format("{0}{1} {2} FAILED to copy/register {{0}}", msgStart, Thread.CurrentThread.ManagedThreadId, computer), new LogLevel(Level.ERROR), file.Name);
                        messageWriter.WriteException(e);                        
                    }
                }


            }
        }


    }
}
