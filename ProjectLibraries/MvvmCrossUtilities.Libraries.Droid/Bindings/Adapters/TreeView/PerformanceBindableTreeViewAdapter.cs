using Android.Content;
using Android.Util;
using Cirrious.MvvmCross.Binding.Droid.BindingContext;
using MvvmCrossUtilities.Libraries.Droid.Bindings.Views.TreeView;
using MvvmCrossUtilities.Libraries.Portable.Models;
using MvvmCrossUtilities.Libraries.Portable.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Adapters.TreeView
{
    public class PerformanceBindableTreeViewAdapter : AbstractTreeViewAdapter<IExpandable>
    {
        #region Fields

        private readonly string TAG = "PerformanceBindableTreeViewAdapter";
        private readonly int LIST_ITEM_CACHE_SIZE = 50;
        private int _lastLoadedIndex = -1;

        #endregion

        #region Properties

        protected TreeBuilder<IExpandable> TreeBuilder
        {
            get { return _treeBuilder; }
        }
        private readonly TreeBuilder<IExpandable> _treeBuilder;

        protected IList<IExpandable> Items
        {
            get { return _items; }
        }
        private IList<IExpandable> _items;

        #endregion

        #region Constructor

        public PerformanceBindableTreeViewAdapter(Context context, IMvxAndroidBindingContext bindingContext,
                                                  ITreeStateManager<IExpandable> treeStateManager,
                                                  IEnumerable<IExpandable> expandableItems,
                                                  int numberOfLevels,
                                                  int itemtemplateId)
            : base(context, bindingContext, treeStateManager, numberOfLevels, itemtemplateId)
        {
            _treeBuilder = new TreeBuilder<IExpandable>(treeStateManager);
            UpdateItemsSource(expandableItems);
        }

        #endregion

        #region Overriden Methods and Properties

        public override int Count
        {
            get
            {
                return Math.Max(Items.SafeCount(), base.Count);
            }
        }

        public override TreeNodeInfo<IExpandable> GetTreeNodeInfo(int position)
        {
            IExpandable item = null;

            try { item = GetTreeId(position); }
            catch
            {
                Log.Debug(TAG, "Item with position " + position + " not present in tree list.");
            }
            if (item == null)
            {
                LoadNextItemsCache();
                TreeStateManager.ResetVisibleList();
                return GetTreeNodeInfo(position);
            }

            return TreeStateManager.GetNodeInfo(item);
        }

        #endregion

        #region Methods

        public void UpdateItemsSource(IEnumerable<IExpandable> newItemsSource)
        {
            Clear();
            _items = newItemsSource != null ? newItemsSource.ToList() : new List<IExpandable>();

            LoadNextItemsCache();
            UpdateTree();
        }

        private void AddChildNodes(TreeBuilder<IExpandable> treeBuilder, IExpandable parent, int level)
        {
            if (parent != null && parent.Children != null && parent.Children.Count > 0)
            {
                foreach (var child in parent.Children)
                {
                    treeBuilder.SequentiallyAddNextNode(child, level);
                    AddChildNodes(treeBuilder, child, level + 1);
                }
            }
        }

        public void Clear()
        {
            _lastLoadedIndex = -1;
            if (TreeBuilder != null)
                TreeBuilder.Clear();
        }

        private void LoadNextItemsCache()
        {
            Log.Debug(TAG, "Cached items #" + (_lastLoadedIndex + 1) + " - Loading " + LIST_ITEM_CACHE_SIZE + " items");
            var index = _lastLoadedIndex + 1;
            var maxIndex = LIST_ITEM_CACHE_SIZE + _lastLoadedIndex;

            for (int i = index; i <= maxIndex; i++)
            {
                if (i >= Items.SafeCount())
                    break;

                var item = Items[i];
                Log.Debug("BindableTreeViewAdapter", "Adding item #" + i);
                TreeBuilder.SequentiallyAddNextNode(item, 0);
                AddChildNodes(TreeBuilder, item, 1);

                _lastLoadedIndex = i;
            }
        }

        #endregion
    }
}