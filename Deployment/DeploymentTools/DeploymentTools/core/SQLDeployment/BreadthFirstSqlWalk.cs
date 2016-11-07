using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections.TreeNode;
using System.IO;
using System.Drawing;
using EM.Logging;

namespace DeploymentTools
{
    public class BreadthFirstSqlWalk : DepthFirstSqlWalk
    {
        public BreadthFirstSqlWalk(SqlFolderTree root, MessageWriter writer, string conStr)
            : base(root, writer, conStr) { }

        protected override void walk(TreeNode tree)
        {
            this.breadthFirstWalk(tree, 
                delegate(TreeNode src, TreeNode dest)
                {
                    FileInfo s = new FileInfo((string)src.node);
                    FileInfo d = new FileInfo((string)dest.node);
                    if (this.depth == 2)  //is a script
                    {
                        //sort on script name and containing folder ex.  "\1. Tables\02 Create dbo.tblTest.sql"
                        //so that it puts all tables under one run order and all stored procedures onder one run order 
                        //for all databases it's doing the breath first lookup
                        string cs = s.Directory.Name + "\\" + s.Name;
                        string cd = d.Directory.Name + "\\" + d.Name;
                        return cs.CompareTo(cd);
                    }
                    else    // is a folder like database name or like "1. Table"  etc
                    {
                        return s.Name.CompareTo(d.Name);
                    }
                });
        }

        public override void enterNode(EM.Collections.TreeNode.TreeNode tree)
        {
            if (this.depth == 1)
            {
                DirectoryInfo f = new DirectoryInfo((string)tree.node);
                this.messageWriter.WriteLine(Color.Blue, "FROM DATABASE " + f.FullName, new LogLevel(Level.INFO));                
            }
        }

    }

}
