using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using System.Data.SqlClient;
using System.IO;
using EM.Collections.TreeNode;
using Microsoft.SqlServer.Management.Common;
using System.Drawing;
using EM.Logging;
using EM.Collections;
using System.Data;
using System.Text.RegularExpressions;


namespace DeploymentTools
{
    public class DepthFirstSqlWalk : TreeWalker
    {
        protected MessageWriter messageWriter;
        protected string conStr;
        protected Server server;

        public DepthFirstSqlWalk(SqlFolderTree root, MessageWriter writer, string conStr)
            : base(root)
        {
            this.messageWriter = writer;
            this.conStr = conStr;
            this.doTraceOnly = false;
        }

        protected override void walk(TreeNode tree)
        {
            this.depthFirstWalk(tree);
        }

        private bool _doTraceOnly;
        public bool doTraceOnly
        {
            get { return _doTraceOnly; }
            set { _doTraceOnly = value; }
        }

        public bool run()
        {
            SqlConnection con = null;
            try
            {
                con = new SqlConnection(this.conStr);
                con.Open();
            }
            catch (Exception err)
            {
                this.messageWriter.WriteLine(Color.Red, String.Format("Could not open a connection to the database server ({0})", this.conStr), new LogLevel(Level.ERROR));
                this.messageWriter.WriteException(err);
                return false;
            }



            if (this.doTraceOnly)
            {
                this.walk();
                if (this.hasValidationProblems)
                {
                    this.messageWriter.WriteLine(Color.Brown, "There might be problems. Scroll up to see file details.\n", new LogLevel(Level.WARN));
                }
                else
                {
                    this.messageWriter.WriteLine(Color.DarkGreen, "Scripts validated successfully.\n", new LogLevel(Level.INFO));
                }
                return true;
            }

            

            bool success = false;
            try
            {
                //'GO' is not part of T-SQL so we can't simply use ADO.NET 
                //however is parsed and resolved by the Smo 
                //http://weblogs.asp.net/jgalloway/archive/2006/11/07/Handling-_2200_GO_2200_-Separators-in-SQL-Scripts-_2D00_-the-easy-way.aspx
                this.server = new Server(new ServerConnection(con));
                this.server.ConnectionContext.BeginTransaction();

                this.walk();

                DataSet ds = this.server.ConnectionContext.ExecuteWithResults("select @@TRANCOUNT");
                int trancount = (int) ds.Tables[0].Rows[0][0];
                if (trancount != 1)
                {
                    this.server.ConnectionContext.RollBackTransaction();
                    this.messageWriter.WriteLine(Color.Brown, string.Format("@@Trancount was {0}, please review your scripts and try again", trancount), new LogLevel(Level.ERROR));
                    this.messageWriter.WriteLine(Color.Brown, "BATCH WAS ROLLED BACK", new LogLevel(Level.ERROR));
                }
                else
                {
                    this.server.ConnectionContext.CommitTransaction();
                    this.messageWriter.WriteLine(Color.DarkGreen, "\nSCRIPTS WERE SUCCESSFULY RAN\n", new LogLevel(Level.INFO));
                    success = true;
                }
                
                
            }     
            catch (SmoBatchException smoer)
            {
                //logged and rolledback already
            }
            catch (Exception er)
            {
                try
                {
                    this.server.ConnectionContext.RollBackTransaction();
                    this.messageWriter.WriteLine(Color.Brown, "BATCH WAS ROLLED BACK", new LogLevel(Level.ERROR));
                }
                catch { }
                this.messageWriter.WriteException(er);                
            }
            finally
            {
                con.Close();
                con.Dispose();
            }


            return success;

        }

        public override bool ignore(TreeNode tree)
        {
            string treePath = ((string)tree.node).ToUpper();
            if ( tree.isLeaf() && ((SqlFolderTree)this.root).isDoExclude(treePath))
            {   // exclude scripts specified in txtSqlExceptions
                this.messageWriter.WriteLine(Color.Gray, "EXCLUDE " + (string)tree.node, new LogLevel(Level.DEBUG));                
                return true;
            }
            return false;
        }

        public bool hasValidationProblems { get; set; }

        public override void doItem(EM.Collections.TreeNode.TreeNode item)
        {
            FileInfo file = new FileInfo((string)item.node);
            

            string fileContent = StringUtil.getTextFileContent(file.FullName);
            
            try
            {
                this.messageWriter.WriteLine((this.doTraceOnly ? "Preview: " : "Running: ") + file.FullName, new LogLevel(Level.DEBUG));
                
                if ( !fileContent.Trim().ToUpper().StartsWith("USE ") )
                {
                    //maybe is unicode and was not detected as such
                    fileContent = StringUtil.getTextFileContent(file.FullName, Encoding.Unicode);
                    
                    if (!fileContent.Trim().ToUpper().StartsWith("USE "))
                    {
                        if (this.doTraceOnly)
                        {
                            this.messageWriter.WriteLine(Color.Brown, string.Format("{0} does not start with a USE statement", file.Name), new LogLevel(Level.ERROR));
                            hasValidationProblems = true;
                        }
                        else
                        {
                            throw new Exception("The SCRIPT MUST START WITH a USE statement");
                        }
                    }
                }

                Regex rxBT = new Regex("Begin +Tran(saction)?", RegexOptions.IgnoreCase);
                Regex rxCT = new Regex("Commit( +Tran(saction)?)?", RegexOptions.IgnoreCase);
                Regex rxC = new Regex("Committed", RegexOptions.IgnoreCase);
                MatchCollection matchBT = rxBT.Matches(fileContent);
                MatchCollection matchCT = rxCT.Matches(fileContent);
                MatchCollection matchC = rxC.Matches(fileContent);
                if (matchBT.Count != (matchCT.Count-matchC.Count))
                {
                    this.messageWriter.WriteLine(Color.Brown, string.Format("{2} Contains {0} BEGIN TRAN and {1} COMMIT TRAN", matchBT.Count, matchCT.Count - matchC.Count, file.Name), new LogLevel(Level.ERROR));
                    hasValidationProblems = true;
                }
                

                if (!this.doTraceOnly)
                {
                    this.server.ConnectionContext.ExecuteNonQuery(fileContent);
                }
            }
            catch (Exception er)
            {
                this.server.ConnectionContext.RollBackTransaction();
                this.messageWriter.WriteLine("\n----------------------------------------------", new LogLevel(Level.ERROR));
                this.messageWriter.color = Color.Brown;
                this.messageWriter.WriteLine("ERROR IN FILE " + file.FullName, new LogLevel(Level.ERROR));
                if (er.InnerException != null && er.InnerException.Message.Contains("is not a recognized"))
                {
                    this.messageWriter.WriteLine("POSIBLE PROBLEM IS THAT YOU MIGHT NEED TO FULLY QUALIFY FUNCTION (etc) NAMES WHEN CREATING AND REFERENCING ", new LogLevel(Level.ERROR));                    
                }
                this.messageWriter.WriteLine("ENTIRE BATCH WAS ROLLED BACK", new LogLevel(Level.ERROR));
                this.messageWriter.WriteException(er);
                this.messageWriter.restorePreviousColor();
                throw new SmoBatchException();
            }

        }

        public override void enterNode(EM.Collections.TreeNode.TreeNode tree)
        {
            if (this.depth == 2)
            {
                DirectoryInfo f = new DirectoryInfo((string)tree.node);
                messageWriter.WriteLine(Color.Blue, "FROM DATABASE " + f.FullName, new LogLevel(Level.INFO));                
            }
        }


    }
}
