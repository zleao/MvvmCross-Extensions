using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.Attributes;
using Cirrious.MvvmCross.Binding.Droid.BindingContext;
using Cirrious.MvvmCross.Binding.Droid.ResourceHelpers;
using Cirrious.MvvmCross.Binding.Droid.Views;
using MvvmCrossUtilities.Libraries.Droid.Bindings.Adapters.TreeView;
using MvvmCrossUtilities.Libraries.Portable.Models;
using System;
using System.Collections;
using System.Collections.Generic;

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

        public new AbstractTreeViewAdapter<IExpandable> Adapter
        {
            get { return base.Adapter as AbstractTreeViewAdapter<IExpandable>; }
            set
            {
                var existing = Adapter;
                if (existing == value)
                    return;

                if (!(value is AbstractTreeViewAdapter<IExpandable>))
                {
                    throw new TreeConfigurationException("The adapter is not of AbstractTreeViewAdapter<IExpandable> type");
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

        public bool SelectionEnabled
        {
            get { return _selectionEnabled; }
            set
            {
                if (_selectionEnabled != value)
                {
                    _selectionEnabled = value;
                    OnAttributeChange();
                }
            }
        }
        private bool _selectionEnabled;

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

            Adapter = new PerformanceBindableTreeViewAdapter(Context, MvxAndroidBindingContextHelpers.Current(), new InMemoryTreeStateManager<IExpandable>(), null, 1, ItemTemplateId);

            SyncAdapter();
        }

        #endregion

        #region Methods

        private object SafeGetFieldValue(Type styleable, string fieldName)
        {
            return SafeGetFieldValue(styleable, fieldName, 0);
        }

        private object SafeGetFieldValue(Type styleable, string fieldName, object defaultValue)
        {
            var field = styleable.GetField(fieldName);
            if (field == null)
            {
                MvxBindingTrace.Trace(MvxTraceLevel.Error, "Missing stylable field {0}", fieldName);
                return defaultValue;
            }

            return field.GetValue(null);
        }

        private void ParseAttributes(Context context, IAttributeSet attrs)
        {
            ItemTemplateId = MvxAttributeHelpers.ReadListItemTemplateId(context, attrs);

            var finder = Mvx.Resolve<IMvxAppResourceTypeFinder>();
            var resourceType = finder.Find();
            var styleable = resourceType.GetNestedType("Styleable");

            var treeListViewAttrsId = (int[])SafeGetFieldValue(styleable, "BindableTreeViewList", new int[0]);
            TypedArray a = context.ObtainStyledAttributes(attrs, treeListViewAttrsId, 0, 0);

            _expandedDrawable = a.GetDrawable((int)SafeGetFieldValue(styleable, "BindableTreeViewList_src_expanded", 0));
            if (_expandedDrawable == null)
                _expandedDrawable = context.Resources.GetDrawable(DEFAULT_EXPANDED_RESOURCE);

            _collapsedDrawable = a.GetDrawable((int)SafeGetFieldValue(styleable, "BindableTreeViewList_src_collapsed", 0));
            if (_collapsedDrawable == null)
                _collapsedDrawable = context.Resources.GetDrawable(DEFAULT_COLLAPSED_RESOURCE);

            _indentWidth = a.GetDimensionPixelSize((int)SafeGetFieldValue(styleable, "BindableTreeViewList_indent_width", 0), DEFAULT_INDENT);
            _indicatorGravity = (GravityFlags)a.GetInteger((int)SafeGetFieldValue(styleable, "BindableTreeViewList_indicator_gravity", 0), (int)DEFAULT_GRAVITY);
            _indicatorBackgroundDrawable = a.GetDrawable((int)SafeGetFieldValue(styleable, "BindableTreeViewList_indicator_background", 0));
            _rowBackgroundDrawable = a.GetDrawable((int)SafeGetFieldValue(styleable, "BindableTreeViewList_row_background", 0));
            _collapsible = a.GetBoolean((int)SafeGetFieldValue(styleable, "BindableTreeViewList_collapsible", 0), true);
            _selectionEnabled = a.GetBoolean((int)SafeGetFieldValue(styleable, "BindableTreeViewList_selection_enabled", 1), true);
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
            Adapter.SetSelectionEnabled(SelectionEnabled);
        }

        private void OnAttributeChange()
        {
            SyncAdapter();
            Adapter.Refresh();
        }

        private void SetItemsSource(IEnumerable value)
        {
            if(Adapter == null)
            {
                MvxTrace.Warning("Adapter is null");
                return;
            }

            _itemsSource = value;
            if(value == null)
            {
                MvxTrace.Warning("TreeviewList.ItemsSource is null");
                return;
            }
            var expandableItems = value as IEnumerable<IExpandable>;
            if (expandableItems == null)
            {
                MvxTrace.Warning("TreeviewList.ItemsSource is not of type IList<IExpandable>");
                return;
            }

            (Adapter as PerformanceBindableTreeViewAdapter).UpdateItemsSource(expandableItems);
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