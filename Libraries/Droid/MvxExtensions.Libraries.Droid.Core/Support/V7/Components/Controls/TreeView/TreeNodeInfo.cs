using System;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.TreeView
{
    public class TreeNodeInfo<T> where T : Portable.Core.Models.ITreeViewItem
    {
        #region Fields

        private readonly string STRING_FORMAT = "TreeNodeInfo [id={0}, level={1}, withChildren={2}, visible={3}, expanded={4}]";
        private T id;
        private int level;
        private bool withChildren;
        private bool visible;
        private bool expanded;

        #endregion

        #region Constructor

        public TreeNodeInfo(T id, int level, bool withChildren, bool visible, bool expanded)
        {
            this.id = id;
            this.level = level;
            this.withChildren = withChildren;
            this.visible = visible;
            this.expanded = expanded;
        }

        #endregion

        #region Methods

        public T GetId()
        {
            return id;
        }

        public bool IsWithChildren()
        {
            return withChildren || (id != null && id.HasChildren);
        }

        public bool IsVisible()
        {
            return visible;
        }

        public bool IsExpanded()
        {
            return expanded;
        }

        public int GetLevel()
        {
            return level;
        }

        public override string ToString()
        {
            return String.Format(STRING_FORMAT, id, level, withChildren, visible, expanded);
        }
        
        #endregion
    }
}
