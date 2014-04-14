using System;
using System.Collections;
using System.Collections.Generic;
using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Binding.Attributes;
using Cirrious.MvvmCross.Binding.Droid.Views;
using MvvmCrossUtilities.Libraries.Droid.Bindings.Adapters.TreeView;
using MvvmCrossUtilities.Libraries.Portable.Models;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views.TreeView
{
    public class BindableTreeViewList : ListView
    {
        #region Fields

        private readonly int DEFAULT_COLLAPSED_RESOURCE = Resource.Drawable.collapsed;
        private readonly int DEFAULT_EXPANDED_RESOURCE = Resource.Drawable.expanded;
        private readonly int DEFAULT_INDENT = 4;
        private readonly GravityFlags DEFAULT_GRAVITY = GravityFlags.Left | GravityFlags.CenterVertical;

        #endregion

        #region Properties

        public new BindableTreeViewAdapter Adapter
        {
            get { return base.Adapter as BindableTreeViewAdapter; }
            set
            {
                var existing = Adapter;
                if (existing == value)
                    return;

                if (!(value is AbstractTreeViewAdapter<IExpandable>))
                {
                    throw new TreeConfigurationException("The adapter is not of TreeViewAdapter type");
                }

                base.Adapter = value;

                SyncAdapter();
            }
        }

        [MvxSetToNullAfterBinding]
        public IEnumerable ItemsSource
        {
            get { return _itemsSource; }
            set { SetItemsSource(value); }
        }
        private IEnumerable _itemsSource;

        public int ItemTemplateId { get; private set; }

        public bool SingleSelection
        {
            get { return Adapter.SingleSelection; }
            set { Adapter.SingleSelection = value; }
        }

        public bool PropagateSelection
        {
            get { return Adapter.PropagateSelection; }
            set { Adapter.PropagateSelection = value; }
        }

        public bool ExpandChildWhenSelected
        {
            get { return Adapter.ExpandChildWhenSelected; }
            set { Adapter.ExpandChildWhenSelected = value; }
        }

        public bool AllowParentSelection
        {
            get { return Adapter.AllowParentSelection; }
            set { Adapter.AllowParentSelection = value; }
        }

        public IList SelectedItems
        {
            get { return Adapter.SelectedItems; }
            set { Adapter.SelectedItems = value; }
        }

        public int IndentWidth
        {
            get { return _indentWidth; }
            set
            {
                if (_indentWidth != value)
                {
                    _indentWidth = value;
                    OnAttributeChange();
                }
            }
        }
        private int _indentWidth;

        public GravityFlags IndicatorGravity
        {
            get { return _indicatorGravity; }
            set
            {
                if (_indicatorGravity != value)
                {
                    _indicatorGravity = value;
                    OnAttributeChange();
                }
            }
        }
        private GravityFlags _indicatorGravity;

        public Drawable IndicatorBackgroundDrawable
        {
            get { return _indicatorBackgroundDrawable; }
            set
            {
                if (_indicatorBackgroundDrawable != value)
                {
                    _indicatorBackgroundDrawable = value;
                    OnAttributeChange();
                }
            }
        }
        private Drawable _indicatorBackgroundDrawable;

        public Drawable RowBackgroundDrawable
        {
            get { return _rowBackgroundDrawable; }
            set
            {
                if (_rowBackgroundDrawable != value)
                {
                    _rowBackgroundDrawable = value;
                    OnAttributeChange();
                }
            }
        }
        private Drawable _rowBackgroundDrawable;

        public Drawable ExpandedDrawable
        {
            get { return _expandedDrawable; }
            set
            {
                if (_expandedDrawable != value)
                {
                    _expandedDrawable = value;
                    OnAttributeChange();
                }
            }
        }
        private Drawable _expandedDrawable;

        public Drawable CollapsedDrawable
        {
            get { return _collapsedDrawable; }
            set
            {
                if (_collapsedDrawable != value)
                {
                    _collapsedDrawable = value;
                    OnAttributeChange();
                }
            }
        }
        private Drawable _collapsedDrawable;

        public bool Collapsible
        {
            get { return _collapsible; }
            set
            {
                if (_collapsible != value)
                {
                    _collapsible = value;
                    OnAttributeChange();
                }
            }
        }
        private bool _collapsible;

        #endregion

        #region Constructors

        public BindableTreeViewList(Context context)
            : this(context, null)
        {
        }

        public BindableTreeViewList(Context context, IAttributeSet attrs)
            : this(context, attrs, Resource.Style.treeViewListStyle)
        {
        }

        public BindableTreeViewList(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            ParseAttributes(context, attrs);
        }

        #endregion

        #region Methods

        private void ParseAttributes(Context context, IAttributeSet attrs)
        {
            ItemTemplateId = MvxAttributeHelpers.ReadListItemTemplateId(context, attrs);

            TypedArray a = context.ObtainStyledAttributes(attrs, Resource.Styleable.TreeViewList);

            _expandedDrawable = a.GetDrawable(Resource.Styleable.TreeViewList_src_expanded);
            if (_expandedDrawable == null)
                _expandedDrawable = context.Resources.GetDrawable(DEFAULT_EXPANDED_RESOURCE);

            _collapsedDrawable = a.GetDrawable(Resource.Styleable.TreeViewList_src_collapsed);
            if (_collapsedDrawable == null)
                _collapsedDrawable = context.Resources.GetDrawable(DEFAULT_COLLAPSED_RESOURCE);

            _indentWidth = a.GetDimensionPixelSize(Resource.Styleable.TreeViewList_indent_width, DEFAULT_INDENT);
            _indicatorGravity = (GravityFlags)a.GetInteger(Resource.Styleable.TreeViewList_indicator_gravity, (int)DEFAULT_GRAVITY);
            _indicatorBackgroundDrawable = a.GetDrawable(Resource.Styleable.TreeViewList_indicator_background);
            _rowBackgroundDrawable = a.GetDrawable(Resource.Styleable.TreeViewList_row_background);
            _collapsible = a.GetBoolean(Resource.Styleable.TreeViewList_collapsible, true);
        }

        private void SyncAdapter()
        {
            Adapter.SetCollapsedDrawable(CollapsedDrawable);
            Adapter.SetExpandedDrawable(ExpandedDrawable);
            Adapter.SetIndicatorGravity(IndicatorGravity);
            Adapter.SetIndentWidth(IndentWidth);
            Adapter.SetIndicatorBackgroundDrawable(IndicatorBackgroundDrawable);
            Adapter.SetRowBackgroundDrawable(RowBackgroundDrawable);
            Adapter.SetCollapsible(Collapsible);
        }

        private void OnAttributeChange()
        {
            SyncAdapter();
            Adapter.Refresh();
        }

        private void SetItemsSource(IEnumerable value)
        {
            _itemsSource = value;
            var expandableItems = value as IEnumerable<IExpandable>;
            if (expandableItems == null)
            {
                MvxTrace.Warning("TreeviewList.ItemsSource are null or not of type IList<IExpandable>");
                return;
            }

            //var maximumNumberOfLevels = GetMaximumNumberOfLevels(expandableItems);
            var maximumNumberOfLevels = 1;
            var manager = new InMemoryTreeStateManager<IExpandable>();
            var treeBuilder = new TreeBuilder<IExpandable>(manager);

            foreach (var item in expandableItems)
            {
                treeBuilder.SequentiallyAddNextNode(item, 0);
                AddChildNodes(treeBuilder, item, 1);
            }

            Adapter = new BindableTreeViewAdapter(Context, manager, maximumNumberOfLevels, ItemTemplateId);
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

        [Obsolete("The number of levels should allways be 1 (one)", true)]
        private int GetMaximumNumberOfLevels(IEnumerable<IExpandable> expandableItems)
        {
            var numberOfLevels = 0;

            foreach (var item in expandableItems)
            {
                //var currentItemLevels = GetItemMaximumNumberOfLevels(item);
                //if (currentItemLevels > numberOfLevels)
                //    numberOfLevels = currentItemLevels;
            }

            return numberOfLevels;
        }

        [Obsolete("The number of levels should allways be 1 (one)", true)]
        private int GetItemMaximumNumberOfLevels(IExpandable item, int currentLevel = 1)
        {
            var maxLevel = currentLevel;

            if (item != null && item.Children != null && item.Children.Count > 0)
            {
                maxLevel++;
                foreach (var child in item.Children)
                {
                    var currNumber = GetItemMaximumNumberOfLevels(child, maxLevel);
                    if (currNumber > maxLevel)
                        maxLevel = currNumber;
                }
            }

            return maxLevel;
        }

        #endregion
    }
}