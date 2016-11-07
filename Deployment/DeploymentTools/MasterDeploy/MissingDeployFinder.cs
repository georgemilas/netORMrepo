using System;
using System.Collections.Generic;
using System.Linq;
using EM.Collections.TreeNode;
using TreeSync;


namespace MasterDeploy
{
    public class MissingDeployFinder : TreeWalker
    {

        public IEnumerable<DeployAction> actions { get; set; }
        public List<string> missing { get; set; }

        public MissingDeployFinder(FileSystemFolderTree root, IEnumerable<DeployAction> actions)
            : base(root) 
        {
            this.actions = actions;
        }

        protected override void walk(TreeNode tree)
        {
            missing = new List<string>();
            this.depthFirstWalk(tree);
        }

        public override bool ignore(TreeNode tree)
        {
            try
            {
                if (tree.isLeaf()) { return true; }

                if (tree.isTerminal())
                {
                    string treePath = (string)tree.node;
                    var res = from ac in actions where treePath.ToUpper().Contains(ac.source.ToUpper()) select ac;
                    if (res.Count() == 0)
                    {
                        missing.Add(treePath);
                    }
                    return true;
                }
            }
            catch (Exception) { }
            
            return false;
        }
        
        //public override void doItem(EM.Collections.TreeNode.TreeNode item) {  }
        //public override void enterNode(EM.Collections.TreeNode.TreeNode tree) { }

    }
}
