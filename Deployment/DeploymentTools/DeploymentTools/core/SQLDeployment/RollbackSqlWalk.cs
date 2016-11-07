using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SqlServer.Management.Smo;
using System.Data.SqlClient;
using System.IO;
using EM.Collections.TreeNode;
using Microsoft.SqlServer.Management.Common;
using EM.DB;
using System.Data;
using EM.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.Text.RegularExpressions;
using EM.Logging;

namespace DeploymentTools
{
    public class DBObject
    {
        public enum DbType { Table = 0, StoredProcedure=1, View=2, UserFunction=3 };
        private EDictionary<string, int> xtypeMap = EDictionary<string, int>.fromStrInt("{'U':0, 'P':1, 'FN': 3, 'V':2}");
        public string name;
        public string schema;
        public int id;
        private DbType _type;
        public DbType type { get { return _type; } }
        public void setType(string xtype)
        {
            _type = (DbType)xtypeMap[xtype.Trim().ToUpper()];
        }
    }

    public class RollbackSqlWalk : TreeWalker
    {
        protected MessageWriter messageWriter;
        protected DBContext dbcontext;
        protected SqlServerDBWorker db;
        protected DataTable dbObjs;
        DirectoryInfo rollbackRootFolder;
        protected EList<DirectoryInfo> rollbackFoldersStack;

        private Regex tableP = new Regex(@"\\\d*\s*\.*\s*tables?\s*", RegexOptions.IgnoreCase);
        private Regex viewP = new Regex(@"\\\d*\s*\.*\s*views?\s*", RegexOptions.IgnoreCase);
        private Regex storeProcP = new Regex(@"\\\d*\s*\.*\s*stored?\s*proc(edure|edures)?\s*", RegexOptions.IgnoreCase);
        private Regex funcP = new Regex(@"\\\d*\s*\.*\s*(user)?\s*func(tion|tions)?\s*", RegexOptions.IgnoreCase);
        //private Regex scriptableP;  //tableP, viewP, storeProcP, funcP

        //separate on any white space and on some punctuation !@#$%^&*-+=|:;'<,>.?/
        private Regex filenameP = new Regex(@"(\s+|[!@#\$%\^&\*-+=|:;'<,>\.\?/]+)");
        

        public RollbackSqlWalk(SqlFolderTree root, MessageWriter writer, DirectoryInfo rollbackFolder, DBContext dbcontext)
            : base(root)
        {
            this.messageWriter = writer;
            this.dbcontext = dbcontext;
            this.rollbackRootFolder = rollbackFolder;
            //this.scriptableP =  new Regex(String.Format("({0}|{1}|{2}|{3})?", tableP, viewP, storeProcP, funcP), RegexOptions.IgnoreCase);
        }

        protected DBObject getIDEndsWith(FileInfo file)
        {
            string name = file.Name.Replace(file.Extension, "").ToLower().Trim();
            EDictionary<string, DataRow> matches = new EDictionary<string, DataRow>();
            foreach (DataRow r in this.dbObjs.Rows)
            {
                string dbObjName = r["name"].ToString().ToLower().Trim();
                if (name.EndsWith(dbObjName))
                {
                    matches[dbObjName] = r; // (int)r["id"];
                    //return (int)r["id"];
                }
            }
            if (matches.Keys.Count > 0)
            {
                string maxMatchName = "";
                foreach (string dbObjName in matches.Keys)
                {

                    if (dbObjName.Length > maxMatchName.Length)
                    {
                        maxMatchName = dbObjName;
                    }

                }
                if (maxMatchName != "")
                {
                    DBObject dbo = new DBObject();
                    dbo.name = maxMatchName;
                    dbo.id = (int)matches[maxMatchName]["id"];
                    dbo.schema = (string)matches[maxMatchName]["schema_name"];
                    return dbo;
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        protected string getTargetDBName(FileInfo file)
        {
            string[] splitted = this.filenameP.Split(file.Name.Replace(file.Extension, ""));
            string name = splitted[splitted.Length - 1].Trim();
            return name;
        }
        protected DBObject getID(FileInfo file)
        {
            string name = getTargetDBName(file).ToLower();
            
            foreach (DataRow r in this.dbObjs.Rows)
            {
                string dbObjName = r["name"].ToString().ToLower().Trim();
                if (name == dbObjName)
                {
                    DBObject dbo = new DBObject();
                    dbo.name = r["name"].ToString();
                    dbo.id = (int)r["id"];
                    dbo.setType(r["xtype"].ToString());
                    dbo.schema = r["schema_name"].ToString();
                    return dbo;
                }
                else
                {
                    if (dbObjName.Trim().StartsWith("dbo.") && !name.Trim().StartsWith("dbo."))
                    {
                        if (name.Trim() == dbObjName.Trim().Substring(4))
                        {
                            DBObject dbo = new DBObject();
                            dbo.name = r["name"].ToString();
                            dbo.id = (int)r["id"];
                            dbo.setType(r["xtype"].ToString());
                            dbo.schema = r["schema_name"].ToString();
                            return dbo;
                        }
                    }
                }
            }
            return null;    
        }
            
        public StringCollection doSMOScript(Database sdb, DBObject dbObj)
        {
            //messageWriter.WriteLine(Color.BlueViolet, "SMO doSMOScript for " + dbObj.name);
            //string name = file.FullName.ToUpper().Trim();

            StringCollection scol = null;
            IScriptable scriptableDbObj = null;
            ScriptingOptions so = new ScriptingOptions();
            so.ContinueScriptingOnError = true;
            so.IncludeHeaders = true;
            so.IncludeDatabaseContext = true;

            if ( dbObj.type == DBObject.DbType.Table)    //( tableP.IsMatch(name) )         //if (name.Contains("TABLE"))
            {
                sdb.Tables.Refresh();
                scriptableDbObj = sdb.Tables[dbObj.name, dbObj.schema];     //sdb.Tables.ItemById(dbObj.id);
                so.ClusteredIndexes = true;
                so.DriAll = true;               //so.Triggers = true;     //so.Permissions = true;                                
            }
            else if (dbObj.type == DBObject.DbType.View)   //( viewP.IsMatch(name) )     //else if (name.Contains("VIEW"))
            {
                sdb.Views.Refresh();
                scriptableDbObj = sdb.Views[dbObj.name, dbObj.schema];                
            }
            else if (dbObj.type == DBObject.DbType.StoredProcedure)   //( storeProcP.IsMatch(name) )    //else if (name.Contains("STORE"))
            {
                sdb.StoredProcedures.Refresh();
                scriptableDbObj = sdb.StoredProcedures[dbObj.name, dbObj.schema];                
            }
            else if (dbObj.type == DBObject.DbType.UserFunction)   //( funcP.IsMatch(name) )         //else if (name.Contains("FUNC"))
            {
                sdb.UserDefinedFunctions.Refresh();
                scriptableDbObj = sdb.UserDefinedFunctions[dbObj.name, dbObj.schema];                
            }
            if (scriptableDbObj != null)
            {
                scol = scriptableDbObj.Script(so);                
            }
            return scol;
        }

        public override void doItem(EM.Collections.TreeNode.TreeNode item)
        {
            FileInfo file = new FileInfo((string)item.node);

            DBObject dbObj = this.getID(file);
            if (dbObj == null)
            {
                messageWriter.WriteLine(Color.Brown, "Object was not found in database for {0}      - ( {1} )", new LogLevel(Level.WARN), getTargetDBName(file), file.FullName);
                return;
            }

            DirectoryInfo destRollbackFolder = this.rollbackFoldersStack[this.rollbackFoldersStack.Count - 1];

            //DBParams p = new DBParams();
            //p.Add("@id", dbObj.id);
            //DataTable res = this.db.getDataTable("SELECT text FROM syscomments WHERE id = @id", p);
            //if (res.Rows.Count > 0)
            //{
            //    string objDef = (string)res.Rows[0]["text"];
            //    objDef = string.Format("/***** Script Generated on {1} ******/\r\nUSE [{0}]\r\nGO\r\n\r\n", db.connection.Database, DateTime.Now) + objDef;
            //    SqlFolderTree.writeFile(file.Name, objDef, destRollbackFolder);
            //    messageWriter.WriteLine("Created rollback for {0}      - ( {1} )", dbObj.name, file.FullName);
            //}
            //else
            //{
                try
                {
                    Server server = this.dbcontext.getSMOServerInstance(db);
                    //messageWriter.WriteLine(Color.BlueViolet, "SMO looging to " + db.connection.Database);
                    Database sdb = new Database(server, db.connection.Database);
                    StringCollection scol = this.doSMOScript(sdb, dbObj);
                    if (scol != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (object o in scol)
                        {
                            sb.AppendLine((string)o);
                        }
                        SqlFolderTree.writeFile(file.Name, sb.ToString(), destRollbackFolder);
                        messageWriter.WriteLine("Created rollback for {0}      - ( {1} )", new LogLevel(Level.DEBUG), dbObj.name, file.FullName);
                    }
                    else
                    {
                        messageWriter.WriteLine(Color.Brown, "No database script could be generated for {0}      - ( {1} )", new LogLevel(Level.WARN), dbObj.name, file.FullName);                       
                    }

                }
                catch(Exception er)
                {
                    messageWriter.WriteLine(Color.Red, "Error generating script for {0}      - ( {1} )", new LogLevel(Level.ERROR), dbObj.name, file.FullName);                       
                    messageWriter.WriteException(er);                    
                }
                
            //}
            
                        
        }

        public override bool ignore(TreeNode tree)
        {
            string treePath = ((string)tree.node).ToUpper();
            
            //if ( treePath.Contains("ROLLBACK") )
            //{
            //    this.messageWriter.WriteLine(Color.Gray, "Ignoring " + (string)tree.node);
            //    return true;  //ignore
            //}

            if ( tree.isLeaf() && ((SqlFolderTree)this.root).isDoExclude(treePath) )
            {   // exclude scripts specified in txtSqlExceptions
                this.messageWriter.WriteLine(Color.Gray, "EXCLUDE " + (string)tree.node, new LogLevel(Level.DEBUG));
                return true;
            }

            if (this.depth >= 2)
            {
                //if (treePath.Contains("TABLE") || treePath.Contains("FUNC") || treePath.Contains("VIEW") || (treePath.Contains("STORE") && treePath.Contains("PROC"))  )
                if (tableP.IsMatch(treePath) || viewP.IsMatch(treePath) || storeProcP.IsMatch(treePath) || funcP.IsMatch(treePath) )
                {
                    return false;           //consider 
                }
                if (this.depth == 3)
                {
                    this.messageWriter.WriteLine(Color.Gray, "Ignoring " + (string)tree.node, new LogLevel(Level.DEBUG));                    
                }
                return true;                //ignore everything else (ex data, agents, jobs etc)
            }
            else
            {
                return false;               //cosider all databases
            }

        }

        

        public override void enterNode(EM.Collections.TreeNode.TreeNode tree)
        {
            if (this.depth == 2)
            {
                DirectoryInfo f = new DirectoryInfo((string)tree.node);
                this.messageWriter.WriteLine(Color.Blue, "FROM DATABASE " + f.FullName, new LogLevel(Level.INFO));
                
                string conStr = this.dbcontext.makeConnectionString(f.Name);   
                if ( this.db != null ) 
                {
                    this.db.Dispose();
                }
                //this.messageWriter.WriteLine(Color.BlueViolet, "Logging to " + conStr);                    
                this.db = new SqlServerDBWorker(conStr);
                //this.messageWriter.WriteLine(Color.BlueViolet, "Logged in DONE, loading data from sysobjects");
                this.dbObjs = db.getDataTable(@"SELECT s.name, s.id, s.xtype, schema_name(so.schema_id) schema_name 
                                                FROM sysobjects s join sys.objects so on s.id=so.object_id
                                                WHERE s.xtype IN ('P', 'U', 'V', 'FN')");  //proc, table, view, func
                //EM.DB.Index.DictIndex idx = new EM.DB.Index.DictIndex(dbObjs, "name");

            }

            DirectoryInfo destRollbackFolder = new DirectoryInfo(((string)tree.node).Replace((string)this.root.node, this.rollbackRootFolder.FullName));
            if (!destRollbackFolder.Exists)
            {
                destRollbackFolder.Create();
            }
            if ( this.rollbackFoldersStack == null)
            {
                this.rollbackFoldersStack = new EList<DirectoryInfo>();
            }
            this.rollbackFoldersStack.Add(destRollbackFolder);
        }

        public override void exitNode(TreeNode tree)
        {
            if (this.depth == 2)
            {
                if (this.db != null)
                {
                    this.db.Dispose();
                    this.db = null;
                }
                if (this.dbObjs != null)
                {
                    this.dbObjs.Dispose();
                    this.dbObjs = null;
                }                
            }

            DirectoryInfo destRollbackFolder = this.rollbackFoldersStack.pop();
            SqlFolderTree destTree = new SqlFolderTree(destRollbackFolder.FullName);
            if ( destTree.content.Count == 0 && destRollbackFolder.FullName != this.rollbackRootFolder.FullName ) 
            {
                destRollbackFolder.Delete(true);
            }
            

        }


    }
}
