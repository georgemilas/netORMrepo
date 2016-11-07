using System;
using System.Collections.Generic;
using System.Text;

namespace EM.Collections.TreeNode
{
    
    /// <summary>
    ///   - Abstract Tree:    
    ///        -you must subclass and implement fetchContent and makeTreeNode
    ///               
    ///    - A tree is a node that has kids that may be themselfs trees
    ///    - the kids of a tree are the content returned by fetchContent
    ///    - Ex. an OS folder structure is a Tree, look at FileSystem.FolderTree.cs for an implementation
    /// </summary>  
    public abstract class TreeNode 
    {
        public TreeNode parent;
        public object node;
        public bool enableEmptyContentAsTree;
        
        protected EList<TreeNode> _content;

        ///////////////////////////////////////////////////////////////////////////////////////////////////////
        public TreeNode(object node) 
            : this(node, null)
        {}
        public TreeNode(object node, TreeNode parent) 
        {
            this.enableEmptyContentAsTree = false; //if the node has no content but we still want it to be a tree (empty tree) than make this True
            this.node = node;
            this.parent = parent;
        }
        
        /// <summary>
        ///   - must return list of Tree's  
        ///   - if it's an empty collection but must be seen as tree then you must set this.enableEmptyContentAsTree = true
        ///     (for example and empty OS folder is still a tree (folders are trees and files are items))
        /// </summary>
        public abstract IEnumerable<TreeNode> fetchContent();
        /// <summary>
        ///    - a virtual tree node factory 
        ///         - ex. 1) FolderTree: TreeNode
        ///                     implements makeTreeNode and fetchContent (using makeTreeNode to return IEnumerable&lt;FolderTree>
        ///               2) ASPTree: FolderTree
        ///                     - implements filter and makeTreeNode 
        ///                     - even if we don't implement fetchContent again, the FolderTree.fechContent will return IEnumerable&lt;ASPFolder>
        ///               
        /// </summary>
        public abstract TreeNode makeTreeNode(object node);   //something like return new TreeNode(node, this);

        /// <summary>
        /// we may subclass and reimplement "filter method" 
        /// </summary>   
        public virtual bool filter(object item)
        {
            return false;
        }
        
        /// <summary>
        ///   - return content of node (it's kids) as returned by fetchContent
        /// </summary>
        public EList<TreeNode> content 
        { 
            get 
            { 
                if (this._content == null) 
                {
                    this._content = new EList<TreeNode>();
                    this._content.AddRange(this.fetchContent());
                }
                return this._content;
            }
        }

        /// <summary>
        ///  - is a Tree if has any 'content' (kids)
        /// </summary>
        public bool isTree()
        {
            if (this.content.Count>0 || this.enableEmptyContentAsTree) {
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// it's leaf if it's not a tree (has no kids)
        /// </summary>
        public bool isLeaf() 
        {
            return this.content.Count==0; 
        }

        /// <summary>
        ///  - it's a terminal tree if none of it's kids is a Tree
        /// </summary>
        public bool isTerminal() 
        {
            if (this.content.Count==0) {return true;}
            foreach(TreeNode node in this.content) 
            {
                if (node.isTree()) 
                { 
                    return false; 
                }
            }
            return true;
        }
        
        /// <summary>
        /// the 'content' collection is an ordered collection so we can say first kid, second kid ...
        /// </summary>
        public int getPosition()
        {
            if (this.parent != null) 
            {
                return this.parent.content.IndexOf(this);
            }
            return 0;
        }
    
        /// <summary>
        /// is getPosition() == 0 ?
        /// </summary>
        public bool isFirst 
        {
            get 
            {
                if ( this.getPosition() == 0 )
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// is getPosition() == content.length-1  ?  
        /// </summary>
        public bool isLast
        {
            get
            {
                if (this.parent != null)
                {
                    if (this.getPosition() == this.parent.content.Count - 1)
                    {
                        return true;
                    }
                    return false;
                }
                return true;
            }
        }


    }



}
