using Android.Content;
using Android.Util;
using MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.TreeView;
using MvxExtensions.Libraries.Portable.Core.Extensions;
using MvvmCross.Binding.Droid.BindingContext;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Adapters.TreeView
{
    public class PerformanceTreeViewAdapter : BaseTreeViewAdapter<Portable.Core.Models.ITreeViewItem>
    {
        #region Fields

        private readonly string TAG = "BindablePerformanceTreeViewAdapter";
        private readonly int LIST_ITEM_CACHE_SIZE = 50;
        private int _lastLoadedIndex = -1;

        #endregion

        #region Properties

        protected TreeBuilder<Portable.Core.Models.ITreeViewItem> TreeBuilder
        {
            get { return _treeBuilder; }
        }
        private readonly TreeBuilder<Portable.Core.Models.ITreeViewItem> _treeBuilder;

        protected IList<Portable.Core.Models.ITreeViewItem> Items
        {
            get { return _items; }
        }
        private IList<Portable.Core.Models.ITreeViewItem> _items;

        #endregion

        #region Constructor

        public PerformanceTreeViewAdapter(Context context, IMvxAndroidBindingContext bindingContext,
                                                  ITreeStateManager<Portable.Core.Models.ITreeViewItem> treeStateManager,
                                                  IEnumerable<Portable.Core.Models.ITreeViewItem> expandableItems,
                                                  int numberOfLevels,
                                                  int itemtemplateId)
            : base(context, bindingContext, treeStateManager, numberOfLevels, itemtemplateId)
        {
            _treeBuilder = new TreeBuilder<Portable.Core.Models.ITreeViewItem>(treeStateManager);
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

        public override TreeNodeInfo<Portable.Core.Models.ITreeViewItem> GetTreeNodeInfo(int position)
        {
            Portable.Core.Models.ITreeViewItem item = null;

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

        public void UpdateItemsSource(IEnumerable<Portable.Core.Models.ITreeViewItem> newItemsSource)
        {
            Clear();
            _items = newItemsSource != null ? newItemsSource.ToList() : new List<Portable.Core.Models.ITreeViewItem>();

            LoadNextItemsCache();
            UpdateTree();
        }

        private void AddChildNodes(TreeBuilder<Portable.Core.Models.ITreeViewItem> treeBuilder, Portable.Core.Models.ITreeViewItem parent, int level)
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
                Log.Debug("PerformanceTreeViewAdapter", "Adding item #" + i);
                TreeBuilder.SequentiallyAddNextNode(item, 0);
                AddChildNodes(TreeBuilder, item, 1);

                _lastLoadedIndex = i;
            }
        }

        #endregion
    }
}