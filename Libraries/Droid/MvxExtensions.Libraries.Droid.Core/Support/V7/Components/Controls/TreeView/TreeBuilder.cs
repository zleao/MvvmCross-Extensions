using Android.Util;
using System;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.TreeView
{
    public class TreeBuilder<T> where T : Portable.Core.Models.ITreeViewItem
    {
        #region Properties

        private readonly String TAG = "TreeBuilder";

        private ITreeStateManager<T> manager;

        private T lastAddedId = default(T);
        private int lastLevel = -1;

        #endregion

        #region Constructor

        public TreeBuilder(ITreeStateManager<T> manager)
        {
            this.manager = manager;
        }

        #endregion

        #region Methods

        public void Clear()
        {
            manager.Clear();
            lastAddedId = default(T);
            lastLevel = -1;
        }

        /// <summary>
        /// Adds new relation to existing tree. Child is set as the last child of the
        /// parent node. Parent has to already exist in the tree, child cannot yet
        /// exist. This method is mostly useful in case you add entries layer by
        /// layer - i.e. first top level entries, then children for all parents, then
        /// grand-children and so on.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="child">The child.</param>
        public void AddRelation(T parent, T child)
        {
            Log.Debug(TAG, "Adding relation parent:" + parent + " -> child: " + child);
            manager.AddAfterChild(parent, child, default(T));
            lastAddedId = child;
            lastLevel = manager.GetLevel(child);
        }

        /// <summary>
        /// Adds sequentially new node. Using this method is the simplest way of
        /// building tree - if you have all the elements in the sequence as they
        /// should be displayed in fully-expanded tree. You can combine it with add
        /// relation - for example you can add information about few levels using
        /// {@link addRelation} and then after the right level is added as parent,
        /// you can continue adding them using sequential operation.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="level">The level.</param>
        public void SequentiallyAddNextNode(T id, int level)
        {
            Log.Debug(TAG, "Adding sequential node " + id + " at level " + level);
            if (lastAddedId == null)
            {
                AddNodeToParentOneLevelDown(default(T), id, level);
            }
            else
            {
                if (level <= lastLevel)
                {
                    T parent = FindParentAtLevel(lastAddedId, level - 1);
                    AddNodeToParentOneLevelDown(parent, id, level);
                }
                else
                {
                    AddNodeToParentOneLevelDown(lastAddedId, id, level);
                }
            }
        }

        /// <summary>
        /// Find parent of the node at the level specified.
        /// </summary>
        /// <param name="node">The node from which we start.</param>
        /// <param name="levelToFind">The level which we are looking for.</param>
        /// <returns>the node found (null if it is topmost node)</returns>
        private T FindParentAtLevel(T node, int levelToFind)
        {
            T parent = manager.GetParent(node);
            while (parent != null)
            {
                if (manager.GetLevel(parent) == levelToFind)
                {
                    break;
                }
                parent = manager.GetParent(parent);
            }
            return parent;
        }

        /// <summary>
        /// Adds note to parent at the level specified. But it verifies that the
        /// level is one level down than the parent!
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="id">The identifier.</param>
        /// <param name="level">The level (should always be parent's level + 1).</param>
        /// <exception cref="TreeConfigurationException">
        /// </exception>
        private void AddNodeToParentOneLevelDown(T parent, T id, int level)
        {
            if (parent == null && level != 0)
            {
                throw new TreeConfigurationException("Trying to add new id " + id
                        + " to top level with level != 0 (" + level + ")");
            }
            if (parent != null && manager.GetLevel(parent) != level - 1)
            {
                throw new TreeConfigurationException("Trying to add new id " + id
                        + " <" + level + "> to " + parent + " <"
                        + manager.GetLevel(parent)
                        + ">. The difference in levels up is bigger than 1.");
            }
            manager.AddAfterChild(parent, id, default(T));
            SetLastAdded(id, level);
        }

        private void SetLastAdded(T id, int level)
        {
            lastAddedId = id;
            lastLevel = level;
        }
        
        #endregion
    }

}