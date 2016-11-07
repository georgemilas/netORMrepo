using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections.TreeNode;
using EM.Collections;
using System.IO;
using EM.parser.keywords;

namespace DeploymentTools
{
    public class SqlFolderTree : TreeNode
    {
        public SqlFolderTree(string node) : this(node, null) { }
        public SqlFolderTree(string node, TreeNode parent) : base(node, parent) { }
        public override IEnumerable<TreeNode> fetchContent()
        {
            EList<TreeNode> res = new EList<TreeNode>();

            if (Directory.Exists((string)this.node))
            {
                EList<string> folders = EList<string>.fromAray(Directory.GetDirectories((string)this.node));
                folders.Sort();

                EList<string> files = EList<string>.fromAray(Directory.GetFiles((string)this.node, "*.sql"));
                files.Sort();
                
                foreach (string folder in folders)
                {
                    //if (!folder.ToUpper().Contains("ROLLBACK"))
                    //{
                        res.Add(this.makeTreeNode(folder));
                    //}
                }

                foreach (string file in files)
                {
                    res.Add(this.makeTreeNode(file));
                }

                if (res.Count <= 0)   //an empty folder is still a tree
                {
                    this.enableEmptyContentAsTree = true;
                }
            }

            return res;

        }

        public override TreeNode makeTreeNode(object node)
        {
            return new SqlFolderTree((string)node, this);
        }

        public static void writeFile(string name, string content, DirectoryInfo destFolder)
        {
            StringUtil.writeToFile(destFolder, name, content);            
        }

        private string _sqlScriptExceptions;
        private KeywordsExpressionParser sqlScriptExceptionsParser;
        public virtual string sqlScriptExceptions
        {
            get { return _sqlScriptExceptions; }
            set
            {
                _sqlScriptExceptions = value;
                if (!string.IsNullOrEmpty(value) && value.Trim() != "")
                {
                    sqlScriptExceptionsParser = new KeywordsExpressionParser(value.Trim().ToLower());
                }
            }
        }

        public bool isDoExclude(string sqlScriptPath)
        {
            if (sqlScriptExceptionsParser != null)
            {
                bool excluded = (bool)sqlScriptExceptionsParser.evaluate(sqlScriptPath.Trim().ToLower());
                return excluded;
            }
            return false;
        }


    }

}
