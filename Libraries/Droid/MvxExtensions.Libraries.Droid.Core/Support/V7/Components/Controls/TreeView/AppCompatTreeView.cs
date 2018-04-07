using Android.Content;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Adapters.TreeView;
using MvvmCross.Binding;
using MvvmCross.Binding.Attributes;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Binding.Droid.ResourceHelpers;
using MvvmCross.Binding.Droid.Views;
using MvvmCross.Platform;
using MvvmCross.Platform.Platform;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.TreeView
{
    public class AppCompatTreeView : ListViewCompat
    {
        #region Fields

        private readonly int DEFAULT_COLLAPSED_RESOURCE = Resource.Drawable.collapsed;
        private readonly int DEFAULT_EXPANDED_RESOURCE = Resource.Drawable.expanded;
        private readonly int DEFAULT_INDENT = 4;
        private readonly GravityFlags DEFAULT_GRAVITY = GravityFlags.Left | GravityFlags.CenterVertical;

        #endregion

        #region Properties

        public new BaseTreeViewAdapter<Portable.Core.Models.ITreeViewItem> Adapter
        {
            get { return base.Adapter as BaseTreeViewAdapter<Portable.Core.Models.ITreeViewItem>; }
            set
            {
                var existing = Adapter;
                if (existing == value)
                    return;

                if (!(value is BaseTreeViewAdapter<Portable.Core.Models.ITreeViewItem>))
                {
                    throw new TreeConfigurationException("The adapter is not of BaseTreeViewAdapter<IExpandable> type");
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

        public AppCompatTreeView(Context context)
            : this(context, null)
        {
        }

        public AppCompatTreeView(Context context, IAttributeSet attrs)
            : this(context, attrs, Resource.Style.treeViewListStyle)
        {
        }

        public AppCompatTreeView(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            ParseAttributes(context, attrs);

            Adapter = new PerformanceTreeViewAdapter(Context, MvxAndroidBindingContextHelpers.Current(), new InMemoryTreeStateManager<Portable.Core.Models.ITreeViewItem>(), null, 1, ItemTemplateId);

            SyncAdapter();
        }

        #endregion

        #region Methods

        private void ParseAttributes(Context context, IAttributeSet attrs)
        {
            ItemTemplateId = MvxAttributeHelpers.ReadListItemTemplateId(context, attrs);

            var finder = Mvx.Resolve<IMvxAppResourceTypeFinder>();
            var resourceType = finder.Find();
            var styleable = resourceType.GetNestedType("Styleable");

            var treeListViewAttrsId = (int[])SafeGetFieldValue(styleable, "AppCompatTreeView", new int[0]);
            TypedArray a = context.ObtainStyledAttributes(attrs, treeListViewAttrsId, 0, 0);

            _expandedDrawable = a.GetDrawable((int)SafeGetFieldValue(styleable, "AppCompatTreeView_src_expanded", 0));
            if (_expandedDrawable == null)
                _expandedDrawable = ContextCompat.GetDrawable(context, DEFAULT_EXPANDED_RESOURCE);

            _collapsedDrawable = a.GetDrawable((int)SafeGetFieldValue(styleable, "AppCompatTreeView_src_collapsed", 0));
            if (_collapsedDrawable == null)
                _collapsedDrawable = ContextCompat.GetDrawable(context, DEFAULT_COLLAPSED_RESOURCE);

            _indentWidth = a.GetDimensionPixelSize((int)SafeGetFieldValue(styleable, "AppCompatTreeView_indent_width", 0), DEFAULT_INDENT);
            _indicatorGravity = (GravityFlags)a.GetInteger((int)SafeGetFieldValue(styleable, "AppCompatTreeView_indicator_gravity", 0), (int)DEFAULT_GRAVITY);
            _indicatorBackgroundDrawable = a.GetDrawable((int)SafeGetFieldValue(styleable, "AppCompatTreeView_indicator_background", 0));
            _rowBackgroundDrawable = a.GetDrawable((int)SafeGetFieldValue(styleable, "AppCompatTreeView_row_background", 0));
            _collapsible = a.GetBoolean((int)SafeGetFieldValue(styleable, "AppCompatTreeView_collapsible", 0), true);
            _selectionEnabled = a.GetBoolean((int)SafeGetFieldValue(styleable, "AppCompatTreeView_selection_enabled", 1), true);
        }

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
            if (Adapter == null)
            {
                MvxTrace.Warning("AppCompatTreeView.Adapter is null");
                return;
            }

            _itemsSource = value;
            if (value == null)
            {
                MvxTrace.Warning("AppCompatTreeView.ItemsSource is null");
                return;
            }
            var expandableItems = value as IEnumerable<Portable.Core.Models.ITreeViewItem>;
            if (expandableItems == null)
            {
                MvxTrace.Warning("AppCompatTreeView.ItemsSource is not of type IList<IExpandable>");
                return;
            }

            (Adapter as PerformanceTreeViewAdapter).UpdateItemsSource(expandableItems);
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

        [Obsolete("The number of levels should allways be 1 (one)", true)]
        private int GetMaximumNumberOfLevels(IEnumerable<Portable.Core.Models.ITreeViewItem> expandableItems)
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
        private int GetItemMaximumNumberOfLevels(Portable.Core.Models.ITreeViewItem item, int currentLevel = 1)
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