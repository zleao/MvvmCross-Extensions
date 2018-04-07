using System;
using System.Collections.Generic;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.TreeView
{
    public class InMemoryTreeNode<T> where T : Portable.Core.Models.ITreeViewItem
    {
        #region Fields

        private readonly string STRING_FORMAT = "InMemoryTreeNode [id={0}, parent={1}, level={2}, visible={3}, children={4}, childIdListCache={5}]";
        //private static readonly long serialVersionUID = 1L;
        private readonly List<InMemoryTreeNode<T>> _children = new List<InMemoryTreeNode<T>>();
        private List<T> _childIdListCache = null;
        
        #endregion

        #region Properties

        public T Id
        {
            get { return _id; }
        }
        private readonly T _id;

        public T Parent
        {
            get { return _parent; }
        }
        private readonly T _parent;

        public int Level
        {
            get { return _level; }
        }
        private readonly int _level;

        public bool IsVisible { get; set; }
        
        #endregion

        #region Constructor

        public InMemoryTreeNode(T id, T parent, int level, bool isVisible)
        {
            _id = id;
            _parent = parent;
            _level = level;
            IsVisible = isVisible;
        }
        
        #endregion

        #region Methods

        public int IndexOf(T id)
        {
            return GetChildIdList().IndexOf(id);
        }

        public List<T> GetChildIdList()
        {
            if (_childIdListCache == null)
            {
                _childIdListCache = new List<T>();
                foreach (var item in _children)
                {
                    _childIdListCache.Add(item.Id);
                }
            }

            return _childIdListCache;
        }

        public int GetChildrenListSize()
        {
            return _children.Count;
        }

        public InMemoryTreeNode<T> Add(int index, T child, bool visible)
        {
            _childIdListCache = null;

            // Note! top levell children are always visible (!)
            InMemoryTreeNode<T> newNode = new InMemoryTreeNode<T>(child, Id, Level + 1, Id == null ? true : visible);
            _children.Insert(index, newNode);
            return newNode;
        }

        public List<InMemoryTreeNode<T>> GetChildren()
        {
            return _children;
        }

        public void ClearChildren()
        {
            _children.Clear();
            _childIdListCache = null;
        }

        public void RemoveChild(T child)
        {
            int childIndex = IndexOf(child);
            if (childIndex != -1)
            {
                _children.RemoveAt(childIndex);
                _childIdListCache = null;
            }
        }

        public override string ToString()
        {
            return String.Format(STRING_FORMAT, Id, Parent, Level, IsVisible, _childIdListCache);
        }

        #endregion
    }
}
