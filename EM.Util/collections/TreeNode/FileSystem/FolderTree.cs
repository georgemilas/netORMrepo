using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace EM.Collections.TreeNode.FileSystem
{
    //look at tests.TreeNodeTest.cs to see how to subclass this and add filtering of only *.asp files for example
    public class FolderTree : TreeNode  
    {
        public FolderTree(string node) : this(node, null) { }
        public FolderTree(string node, TreeNode parent) : base(node, parent) { }
        public override IEnumerable<TreeNode> fetchContent()
        {
            EList<TreeNode> res = new EList<TreeNode>();
            
            //only folders are trees, files are not
            if (Directory.Exists((string)this.node))
            {
                string[] folders = Directory.GetDirectories((string)this.node);
                string[] files = Directory.GetFiles((string)this.node);
                
                foreach (string folder in folders)
                {
                    if (!this.filter(folder))
                    {
                        res.Add(this.makeTreeNode(folder));
                    }
                }

                foreach (string file in files)
                {
                    if (!this.filter(file))
                    {
                        res.Add(this.makeTreeNode(file));
                    }
                }

                if (res.Count<=0)   //an empty folder is still a tree
                {
                    this.enableEmptyContentAsTree = true;
                }

            }

            return res;


        }

        public override TreeNode makeTreeNode(object node)
        {
            return new FolderTree((string)node, this);
        }
    }

    
    //a dummy walker to see how it works
    public class PrintFolderTree : TreeWalker
    {
        public PrintFolderTree(FolderTree root) : base(root) { }
        private string spaces()
        {
            string s = "   ";
            StringBuilder res = new StringBuilder(s);
            for (int i = 0; i < this.depth; i++)
            {
                res.Append(s);
            }
            return res.ToString();
        }
        public override void doItem(TreeNode item)
        {
            FileInfo f = new FileInfo((string)item.node);
            Console.WriteLine(spaces() + "    doing " + f.Name);
        }
        public override void enterNode(TreeNode tree)
        {
            DirectoryInfo f = new DirectoryInfo((string)tree.node);
            Console.WriteLine(spaces() + "+entering " + f.Name);
        }
        public override void exitNode(TreeNode tree)
        {
            DirectoryInfo f = new DirectoryInfo((string)tree.node);
            Console.WriteLine(spaces() + "-exiting " + f.Name);
        }        


    }

    
}
