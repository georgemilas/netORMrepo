using System;
using System.Collections.Generic;
using System.Text;
using EM.Collections.TreeNode;
using EM.Collections;
using System.IO;
using EM.parser.keywords;
using EM.Logging;
using System.Drawing;
using EM.Util;
using ProjectDeployPackage;
using System.ServiceProcess;

namespace TreeSync
{

    //public class ProjectDeployFolder : FileSystemFolderTree
    //{

    //    public ProjectDeployFolder(string node, IFileSystem fileSystem, ILogger logger, DeployScriptConfig cfg) : this(node, null, fileSystem, logger, cfg) { }
    //    public ProjectDeployFolder(string node, TreeNode parent, IFileSystem fileSystem, ILogger logger, DeployScriptConfig cfg)
    //        : base(node, parent, fileSystem, logger)
    //    {
    //        this.cfg = cfg;            
    //    }

    //    private DeployScriptConfig cfg { get; set; }

    //    public override IEnumerable<TreeNode> fetchContent()
    //    {
    //        List<TreeNode> res = (List<TreeNode>)base.fetchContent();
    //        List<TreeNode> filtered = new List<TreeNode>();
    //        foreach (TreeNode tr in res)
    //        {
    //            string path = tr.node.ToString();
    //            if (File.Exists(path))
    //            {
    //                if (Path.GetExtension(path).ToLower() == ".deploy")
    //                {
    //                    continue;
    //                }
    //                FileInfo fi = new FileInfo(path);
    //                string file = fi.Name;
    //                string dbObj = fi.Directory.Name;
    //                string database = fi.Directory.Parent.Name;
    //                if (cfg.databases.ContainsKey(database))
    //                {
    //                    if (cfg.databases[database].contains(dbObj, file))
    //                    {
    //                        filtered.Add(tr);
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                filtered.Add(tr);   //add all folders
    //            }
    //        }

    //        string entrypath = (string)this.node;
    //        if (this.fileSystem.isFolder(entrypath) && filtered.Count <= 0)
    //        {
    //            //an empty folder is still a tree
    //            this.enableEmptyContentAsTree = true;
    //        }
    //        return filtered;

    //    }

    //    public override TreeNode makeTreeNode(object node)
    //    {
    //        return new ProjectDeployFolder((string)node, this, this.fileSystem, this.logger, this.cfg);
    //    }



    //}



}
