using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using EM.DB;
using System.Data.Common;
using EM.Collections;
using System.Data;
using EM.DB.Index;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Data.SqlClient;
using EM.Collections.TreeNode.FileSystem;
using EM.Collections.TreeNode;
using System.Drawing;
using EM.Logging;

namespace DeploymentTools
{

    public class SmoBatchException : Exception
    {
        public SmoBatchException(): base(){ }
        public SmoBatchException(String message): base(message) { }
        public SmoBatchException(String message, Exception innerException) : base(message, innerException) { }
    }

    public class SQLDeployment
    {
        
        public DBContext dbcontext;
        
        private MessageWriter messageWriter;
        private DirectoryInfo root;

        public SQLDeployment(DBContext dbcontext, MessageWriter writer)
        {
            this.dbcontext = dbcontext;
            this.messageWriter = writer;

            root = new DirectoryInfo(this.dbcontext.sqlScriptsPath);
            
        }

        private string _sqlScriptExceptions;
        public virtual string sqlScriptExceptions
        {
            get { return _sqlScriptExceptions; }
            set { _sqlScriptExceptions = value; }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////   ROLLBACK
        /////////////////////////////////////////////////////////////////////////////////////////////////////

        public void generateRollbackSripts(DirectoryInfo rollbackFolder)
        {
            if (!rollbackFolder.Exists)
            {
                rollbackFolder.Create();
            }

            messageWriter.WriteLine("----------------------------------------------", new LogLevel(Level.DEBUG));
            messageWriter.WriteLine("\nStart running rollback scripts generation", new LogLevel(Level.INFO));

            SqlFolderTree ft = new SqlFolderTree(this.root.FullName);
            ft.sqlScriptExceptions = this.sqlScriptExceptions;
            RollbackSqlWalk walk = new RollbackSqlWalk(ft, this.messageWriter, rollbackFolder, this.dbcontext);

            try
            {
                walk.walk();
                messageWriter.WriteLine(Color.DarkGreen, "\nRollback scripts generation is DONE", new LogLevel(Level.INFO));                
            }
            catch (Exception er)
            {

                this.messageWriter.WriteException(er);
            }
            messageWriter.WriteLine("----------------------------------------------", new LogLevel(Level.DEBUG));

            
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////
        /////////////////   RUN Scripts
        /////////////////////////////////////////////////////////////////////////////////////////////////////
        
        public bool runSriptsDepthFirst(bool preview)
        {
            if (!this.root.Exists)
            {
                messageWriter.WriteLine("Nothing was found in the folder provided. Nothing to do.", new LogLevel(Level.DEBUG));
                return true;
            }

            messageWriter.WriteLine("----------------------------------------------", new LogLevel(Level.DEBUG));
            messageWriter.WriteLine("\nStart running scripts", new LogLevel(Level.DEBUG));

            string conStr = this.dbcontext.makeConnectionString("tempdb");   
            SqlFolderTree ft = new SqlFolderTree(this.root.FullName);
            ft.sqlScriptExceptions = this.sqlScriptExceptions;
            DepthFirstSqlWalk walk = new DepthFirstSqlWalk(ft, this.messageWriter, conStr);
            walk.doTraceOnly = preview;
            bool success = walk.run();

            messageWriter.WriteLine("----------------------------------------------", new LogLevel(Level.DEBUG));

            return success;
                    
        }

        public bool runSriptsBreadthFirst(bool preview)
        {
            if (!this.root.Exists)
            {
                messageWriter.WriteLine("Nothing was found in the folder provided. Nothing to do.", new LogLevel(Level.DEBUG));
                return true;
            }

            messageWriter.WriteLine("----------------------------------------------", new LogLevel(Level.DEBUG));
            messageWriter.WriteLine("\nStart running scripts", new LogLevel(Level.DEBUG));

            string conStr = this.dbcontext.makeConnectionString("tempdb");   
            SqlFolderTree ft = new SqlFolderTree(this.root.FullName);
            ft.sqlScriptExceptions = this.sqlScriptExceptions;
            BreadthFirstSqlWalk walk = new BreadthFirstSqlWalk(ft, this.messageWriter, conStr);
            walk.doTraceOnly = preview;
            bool success = walk.run();

            messageWriter.WriteLine("----------------------------------------------", new LogLevel(Level.DEBUG));

            return success;
        }

    }


    

    



}
