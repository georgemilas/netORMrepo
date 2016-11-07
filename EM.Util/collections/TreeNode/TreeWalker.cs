using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace EM.Collections.TreeNode
{
    /// <summary>
    /// - A tree walker for TreeNode objects
    /// - even if not marked as abstract it MUST be subclassed and override one of 
    ///         "enterNode, exitNode, doItem" to actualy do something
    /// - "ignore" and "expand" may also be overwriten in order to filter TreeNode objects out of the walk process
    /// - one usage example is in building SAX like parsers ("enterNode, exitNode, doItem" are not events but by subclassing) 
    /// </summary>
    public class TreeWalker
    {  
        public bool cancel;
        public int depth;
        protected TreeNode _root;

        public TreeWalker()
            : this(null)
        { }

        public TreeWalker(TreeNode root)
        {
            this.cancel = false;
            this.depth = 0;
            this.root = root;
        }

        public TreeNode root
        {
            get { return this._root; }
            set { this._root = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////
        ////   Overwrite this methods to do the work you need
        ////////////////////////////////////////////////////////////////////////////////////////////    
    

        /// <summary>
        ///   - whether to pretend (at walk time) that this node does not exists
        ///   - we do not enter it (we don't even see it), is like it's not there at all
        /// </summary>
        public virtual bool ignore(TreeNode tree) { return false; }

        /// <summary>
        /// whether to walk this node or not - we enter it (we see it) but chose if to expand or not
        /// </summary>
        public virtual bool expand(TreeNode tree) { return true; }

        /// <summary>
        ///     -recursive walk may benefit from this (ex: default walk - depthFirst)
        ///         - enter .....do stuff with sub tree .....exit
        /// </summary>
        public virtual void enterNode(TreeNode tree) { }
        
        /// <summary>
        ///     -recursive walk may benefit from this (ex: default walk - depthFirst)
        ///         - enter .....do stuff with sub tree .....exit
        /// </summary>
        public virtual void exitNode(TreeNode tree) { }

        
        public virtual void doItem(TreeNode item) { }
        ////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Walks a tree, calling a enter/exit methods for each node/item it encounters (it's a depthFirstWalk)
        /// </summary>
        public void walk()
        {
            this.walk(this.root);
        }

        protected virtual void walk(TreeNode tree)
        {
            if (tree != null)
            {
                this.depthFirstWalk(tree);
            }
        }
        
        /// <summary>
        /// recursive walk 
        /// </summary>
        public void depthFirstWalk(TreeNode tree) 
        {
            if (!this.cancel) 
            {
                this.depth += 1;
                this.enterNode(tree);
                if (this.expand(tree))
                {
                    foreach (TreeNode subTree in tree.content)
                    {
                        if (this.cancel) { break; }

                        if (!this.ignore(subTree))
                        {
                            if (subTree.isTree())
                            {
                                this.depthFirstWalk(subTree);
                            }
                            else
                            {
                                this.doItem(subTree);
                            }
                        }
                    }
                }
                this.exitNode(tree);
                this.depth -= 1;
            }

        }
        
        /// <summary>
        /// walks all items from a level then goes to the next level and walks all items there etc.
        ///     - root (level 1)- call enter and add for level 2
        ///     - level 2 - call enter on all items that are tree themselfs and not ignore-able and add for level 3
        ///               - call doItem() on all items that are leafs and not ignore-able
        ///     - level 3 .....
        ///     .....
        ///     - when done call exit on all items where enter was called but in reverse order
        ///             last one to call enter is the first one to call exit
        /// </summary>
        public void breadthFirstWalk(TreeNode tree) { this.breadthFirstWalk(tree, null); } 
        public void breadthFirstWalk(TreeNode tree, Comparison<TreeNode> contentSorter) 
        {
            int depthStart = this.depth;

            EList<TreeNode> exitList = new EList<TreeNode>();     //LIFO: last in first to call exitNote() 

            this.enterNode(tree);
            exitList.Add(tree);
                        
            EList<TreeNode>content = tree.content;
            
            while (content.Count > 0)
            {
                if (this.cancel) { break; }
                this.depth+=1;
                EList<TreeNode> nextLevel = new EList<TreeNode>();

                foreach(TreeNode subTree in content)
                {
                    if (this.cancel) { break; }
                    if (!this.ignore(subTree))
                    {
                        if (subTree.isTree()) 
                        {
                            this.enterNode(subTree);
                            exitList.Add(subTree);
                            nextLevel.AddRange(subTree.content);
                        }
                        else 
                        {
                            this.doItem(subTree);
                        }
                    }
                }

                content = nextLevel;
                if (contentSorter != null)
                {
                    content.Sort(contentSorter);
                }
            }

            exitList.Reverse();
            foreach (TreeNode t in exitList)      
            {
                this.exitNode(t);
            }
            
            this.depth = depthStart;    //no decrements but all the way back

        }
    
   


    }



}
