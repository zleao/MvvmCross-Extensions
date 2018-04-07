using Android.Database;
using Android.Util;
using Java.Util;
using MvxExtensions.Libraries.Portable.Core.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.TreeView
{
    public class InMemoryTreeStateManager<T> : ITreeStateManager<T>
        where T : class, Portable.Core.Models.ITreeViewItem
    {
        private static readonly string TAG = "InMemoryTreeStateManager";
        //private static readonly long serialVersionUID = 1L;

        private IDictionary<T, InMemoryTreeNode<T>> _allNodes = new Dictionary<T, InMemoryTreeNode<T>>();
        private InMemoryTreeNode<T> _topSentinel = new InMemoryTreeNode<T>(null, null, -1, true);
        private List<T> _visibleListCache = null; // lazy initialised
        private List<T> _unmodifiableVisibleList = null;
        private bool _visibleByDefault = false;
        private HashSet<DataSetObserver> _observers = new HashSet<DataSetObserver>();

        public void ResetVisibleList()
        {
            _visibleListCache = null;
            _unmodifiableVisibleList = null;
        }

        private void InternalDataSetChanged()
        {
            _visibleListCache = null;
            _unmodifiableVisibleList = null;

            foreach (var item in _observers)
            {
                item.OnChanged();
            }
        }

        public void SetVisibleByDefault(bool visibleByDefault)
        {
            _visibleByDefault = visibleByDefault;
        }

        private InMemoryTreeNode<T> GetNodeFromTreeOrThrow(T id)
        {
            if (id == null)
                throw new NodeNotInTreeException("(null)");

            InMemoryTreeNode<T> node = null;
            if (!_allNodes.TryGetValue(id, out  node))
                throw new NodeNotInTreeException(id.ToString());

            return node;
        }

        private InMemoryTreeNode<T> GetNodeFromTreeOrThrowAllowRoot(T id)
        {
            if (id == null)
                return _topSentinel;

            return GetNodeFromTreeOrThrow(id);
        }

        private void ExpectNodeNotInTreeYet(T id)
        {
            InMemoryTreeNode<T> node = null;

            if (_allNodes.TryGetValue(id, out node))
                throw new NodeAlreadyInTreeException(id.ToString(), node.ToString());
        }

        public TreeNodeInfo<T> GetNodeInfo(T id)
        {
            InMemoryTreeNode<T> node = GetNodeFromTreeOrThrow(id);
            List<InMemoryTreeNode<T>> children = node.GetChildren();
            bool expanded = false;
            if (!children.IsNullOrEmpty() && children.ElementAt(0).IsVisible)
            {
                expanded = true;
            }

            return new TreeNodeInfo<T>(id, node.Level, !children.IsNullOrEmpty(), node.IsVisible, expanded);
        }

        public List<T> GetChildren(T id)
        {
            InMemoryTreeNode<T> node = GetNodeFromTreeOrThrowAllowRoot(id);
            return node.GetChildIdList();
        }

        public T GetParent(T id)
        {
            InMemoryTreeNode<T> node = GetNodeFromTreeOrThrowAllowRoot(id);
            return node.Parent;
        }

        private bool GetChildrenVisibility(InMemoryTreeNode<T> node)
        {
            bool visibility;

            List<InMemoryTreeNode<T>> children = node.GetChildren();
            if (children.IsNullOrEmpty())
            {
                visibility = _visibleByDefault;
            }
            else
            {
                visibility = children.ElementAt(0).IsVisible;
            }

            return visibility;
        }

        public void AddBeforeChild(T parent, T newChild, T beforeChild)
        {
            ExpectNodeNotInTreeYet(newChild);

            InMemoryTreeNode<T> node = GetNodeFromTreeOrThrowAllowRoot(parent);
            bool visibility = GetChildrenVisibility(node);

            // top nodes are always expanded.
            if (beforeChild == null)
            {
                InMemoryTreeNode<T> added = node.Add(0, newChild, visibility);
                _allNodes.Add(newChild, added);
            }
            else
            {
                int index = node.IndexOf(beforeChild);
                InMemoryTreeNode<T> added = node.Add(index == -1 ? 0 : index, newChild, visibility);
                _allNodes.Add(newChild, added);
            }

            if (visibility)
            {
                InternalDataSetChanged();
            }
        }

        public void AddAfterChild(T parent, T newChild, T afterChild)
        {
            ExpectNodeNotInTreeYet(newChild);

            InMemoryTreeNode<T> node = GetNodeFromTreeOrThrowAllowRoot(parent);
            bool visibility = GetChildrenVisibility(node);

            if (afterChild == null)
            {
                InMemoryTreeNode<T> added = node.Add(node.GetChildrenListSize(), newChild, visibility);
                _allNodes.Add(newChild, added);
            }
            else
            {
                int index = node.IndexOf(afterChild);
                InMemoryTreeNode<T> added = node.Add(index == -1 ? node.GetChildrenListSize() : index + 1, newChild, visibility);
                _allNodes.Add(newChild, added);
            }

            if (visibility)
            {
                InternalDataSetChanged();
            }
        }

        public void RemoveNodeRecursively(T id)
        {
            InMemoryTreeNode<T> node = GetNodeFromTreeOrThrowAllowRoot(id);

            bool visibleNodeChanged = RemoveNodeRecursively(node);
            T parent = node.Parent;

            InMemoryTreeNode<T> parentNode = GetNodeFromTreeOrThrowAllowRoot(parent);
            parentNode.RemoveChild(id);

            if (visibleNodeChanged)
            {
                InternalDataSetChanged();
            }
        }

        private bool RemoveNodeRecursively(InMemoryTreeNode<T> node)
        {
            bool visibleNodeChanged = false;

            node.GetChildren().ForEach((c) =>
            {
                if (RemoveNodeRecursively(c))
                    visibleNodeChanged = true;
            });

            node.ClearChildren();

            if (node.Id != null)
            {
                _allNodes.Remove(node.Id);

                if (node.IsVisible)
                    visibleNodeChanged = true;
            }

            return visibleNodeChanged;
        }

        private void SetChildrenVisibility(InMemoryTreeNode<T> node, bool visible, bool recursive)
        {
            node.GetChildren().ForEach((c) =>
            {
                c.IsVisible = visible;

                if (recursive)
                    SetChildrenVisibility(c, visible, true);
            });
        }

        public void ExpandDirectChildren(T id)
        {
            Log.Debug(TAG, "Expanding direct children of " + id);

            InMemoryTreeNode<T> node = GetNodeFromTreeOrThrowAllowRoot(id);
            SetChildrenVisibility(node, true, false);
            InternalDataSetChanged();
        }

        public void ExpandEverythingBelow(T id)
        {
            Log.Debug(TAG, "Expanding all children below " + id);
            InMemoryTreeNode<T> node = GetNodeFromTreeOrThrowAllowRoot(id);
            SetChildrenVisibility(node, true, true);
            InternalDataSetChanged();
        }

        public void CollapseChildren(T id)
        {
            InMemoryTreeNode<T> node = GetNodeFromTreeOrThrowAllowRoot(id);
            if (node == _topSentinel)
            {
                _topSentinel.GetChildren().ForEach(c => SetChildrenVisibility(c, false, true));
            }
            else
            {
                SetChildrenVisibility(node, false, true);
            }

            InternalDataSetChanged();
        }

        public T GetNextSibling(T id)
        {
            bool returnNext = false;

            T parent = GetParent(id);
            InMemoryTreeNode<T> parentNode = GetNodeFromTreeOrThrowAllowRoot(parent);

            foreach (var child in parentNode.GetChildren())
            {
                if (returnNext)
                    return child.Id;

                if (child.Id.Equals(id))
                    returnNext = true;
            }

            return null;
        }

        public T GetPreviousSibling(T id)
        {
            T previousSibling = null;

            T parent = GetParent(id);
            InMemoryTreeNode<T> parentNode = GetNodeFromTreeOrThrowAllowRoot(parent);

            foreach (var child in parentNode.GetChildren())
            {
                if (child.Id.Equals(id))
                    return previousSibling;

                previousSibling = child.Id;
            }

            return null;
        }

        public bool IsInTree(T id)
        {
            return _allNodes.ContainsKey(id);
        }

        public int GetVisibleCount()
        {
            return GetVisibleList().Count;
        }

        public List<T> GetVisibleList()
        {
            T currentId = null;
            if (_visibleListCache == null)
            {
                _visibleListCache = new List<T>(_allNodes.Count);
                do
                {
                    currentId = GetNextVisible(currentId);
                    if (currentId == null)
                    {
                        break;
                    }
                    else
                    {
                        _visibleListCache.Add(currentId);
                    }
                } while (true);
            }

            if (_unmodifiableVisibleList == null)
            {
                _unmodifiableVisibleList = new List<T>(_visibleListCache);
            }

            return _unmodifiableVisibleList;
        }

        public T GetNextVisible(T id)
        {
            InMemoryTreeNode<T> node = GetNodeFromTreeOrThrowAllowRoot(id);
            if (!node.IsVisible)
            {
                return null;
            }

            List<InMemoryTreeNode<T>> children = node.GetChildren();
            if (!children.IsNullOrEmpty())
            {
                InMemoryTreeNode<T> firstChild = children.ElementAt(0);
                if (firstChild.IsVisible)
                {
                    return firstChild.Id;
                }
            }

            T sibl = GetNextSibling(id);
            if (sibl != null)
            {
                return sibl;
            }

            T parent = node.Parent;
            do
            {
                if (parent == null)
                {
                    return null;
                }
                T parentSibling = GetNextSibling(parent);
                if (parentSibling != null)
                {
                    return parentSibling;
                }
                parent = GetNodeFromTreeOrThrow(parent).Parent;
            } while (true);
        }

        public void RegisterDataSetObserver(DataSetObserver observer)
        {
            _observers.Add(observer);
        }

        public void UnregisterDataSetObserver(DataSetObserver observer)
        {
            _observers.Remove(observer);
        }

        public int GetLevel(T id)
        {
            return GetNodeFromTreeOrThrow(id).Level;
        }

        public int[] GetHierarchyDescription(T id)
        {
            int level = GetLevel(id);
            int[] hierarchy = new int[level + 1];
            int currentLevel = level;
            T currentId = id;
            T parent = GetParent(currentId);

            while (currentLevel >= 0)
            {
                hierarchy[currentLevel--] = GetChildren(parent).IndexOf(currentId);
                currentId = parent;
                parent = GetParent(parent);
            }
            return hierarchy;
        }

        private void AppendToSb(StringBuilder sb, T id)
        {
            if (id != null)
            {
                TreeNodeInfo<T> node = GetNodeInfo(id);
                int indent = node.GetLevel() * 4;
                char[] indentString = new char[indent];
                Arrays.Fill(indentString, ' ');
                sb.Append(indentString);
                sb.Append(node.ToString());
                sb.Append(Arrays.AsList(GetHierarchyDescription(id)).ToString());
                sb.Append("\n");
            }

            GetChildren(id).ForEach(c => AppendToSb(sb, c));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            AppendToSb(sb, null);
            return sb.ToString();
        }

        public void Clear()
        {
            _allNodes.Clear();
            _topSentinel.ClearChildren();
            InternalDataSetChanged();
        }

        public void Refresh()
        {
            InternalDataSetChanged();
        }
    }
}