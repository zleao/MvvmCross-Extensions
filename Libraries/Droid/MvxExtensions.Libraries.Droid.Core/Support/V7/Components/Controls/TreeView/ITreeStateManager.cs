using Android.Database;
using System.Collections.Generic;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.TreeView
{
    public interface ITreeStateManager<T> where T : Portable.Core.Models.ITreeViewItem
    {
        /**
         * Returns array of integers showing the location of the node in hierarchy.
         * It corresponds to heading numbering. {0,0,0} in 3 level node is the first
         * node {0,0,1} is second leaf (assuming that there are two leaves in first
         * subnode of the first node).
         * 
         * @param id
         *            id of the node
         * @return textual description of the hierarchy in tree for the node.
         */
        int[] GetHierarchyDescription(T id);

        /**
         * Returns level of the node.
         * 
         * @param id
         *            id of the node
         * @return level in the tree
         */
        int GetLevel(T id);

        /**
         * Returns information about the node.
         * 
         * @param id
         *            node id
         * @return node info
         */
        TreeNodeInfo<T> GetNodeInfo(T id);

        /**
         * Returns children of the node.
         * 
         * @param id
         *            id of the node or null if asking for top nodes
         * @return children of the node
         */
        List<T> GetChildren(T id);

        /**
         * Returns parent of the node.
         * 
         * @param id
         *            id of the node
         * @return parent id or null if no parent
         */
        T GetParent(T id);

        /**
         * Adds the node before child or at the beginning.
         * 
         * @param parent
         *            id of the parent node. If null - adds at the top level
         * @param newChild
         *            new child to add if null - adds at the beginning.
         * @param beforeChild
         *            child before which to add the new child
         */
        void AddBeforeChild(T parent, T newChild, T beforeChild);

        /**
         * Adds the node after child or at the end.
         * 
         * @param parent
         *            id of the parent node. If null - adds at the top level.
         * @param newChild
         *            new child to add. If null - adds at the end.
         * @param afterChild
         *            child after which to add the new child
         */
        void AddAfterChild(T parent, T newChild, T afterChild);

        /**
         * Removes the node and all children from the tree.
         * 
         * @param id
         *            id of the node to remove or null if all nodes are to be
         *            removed.
         */
        void RemoveNodeRecursively(T id);

        /**
         * Expands all children of the node.
         * 
         * @param id
         *            node which children should be expanded. cannot be null (top
         *            nodes are always expanded!).
         */
        void ExpandDirectChildren(T id);

        /**
         * Expands everything below the node specified. Might be null - then expands
         * all.
         * 
         * @param id
         *            node which children should be expanded or null if all nodes
         *            are to be expanded.
         */
        void ExpandEverythingBelow(T id);

        /**
         * Collapse children.
         * 
         * @param id
         *            id collapses everything below node specified. If null,
         *            collapses everything but top-level nodes.
         */
        void CollapseChildren(T id);

        /**
         * Returns next sibling of the node (or null if no further sibling).
         * 
         * @param id
         *            node id
         * @return the sibling (or null if no next)
         */
        T GetNextSibling(T id);

        /**
         * Returns previous sibling of the node (or null if no previous sibling).
         * 
         * @param id
         *            node id
         * @return the sibling (or null if no previous)
         */
        T GetPreviousSibling(T id);

        /**
         * Checks if given node is already in tree.
         * 
         * @param id
         *            id of the node
         * @return true if node is already in tree.
         */
        bool IsInTree(T id);

        /**
         * Count visible elements.
         * 
         * @return number of currently visible elements.
         */
        int GetVisibleCount();

        /**
         * Returns visible node list.
         * 
         * @return return the list of all visible nodes in the right sequence
         */
        List<T> GetVisibleList();

        /**
         * Registers observers with the manager.
         * 
         * @param observer
         *            observer
         */
        void RegisterDataSetObserver(DataSetObserver observer);

        /**
         * Unregisters observers with the manager.
         * 
         * @param observer
         *            observer
         */
        void UnregisterDataSetObserver(DataSetObserver observer);

        /**
         * Cleans tree stored in manager. After this operation the tree is empty.
         * 
         */
        void Clear();

        /**
         * Refreshes views connected to the manager.
         */
        void Refresh();

        /**
         * Resets the visible list of the manager
         */
        void ResetVisibleList();
    }
}