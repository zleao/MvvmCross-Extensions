using Android.Content;
using Android.Runtime;
using Android.Util;
using MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Adapters.ExpandableRecyclerView;
using MvvmCross.Binding.Droid.Views;
using MvvmCross.Droid.Support.V7.RecyclerView;
using System;
using System.Collections;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls
{
    public class ExpandableRecyclerView : MvxRecyclerView
    {
        #region Properties

        protected ExpandableRecyclerViewAdapter TypedAdapter
        {
            get { return Adapter as ExpandableRecyclerViewAdapter; }
        }

        public IList SelectedItems
        {
            get { return TypedAdapter.SelectedItems; }
            set { TypedAdapter.SelectedItems = value; }
        }

        public bool SingleSelection
        {
            get { return TypedAdapter.SingleSelection; }
            set { TypedAdapter.SingleSelection = value; }
        }

        public bool SelectionEnabled
        {
            get { return TypedAdapter.SelectionEnabled; }
            set { TypedAdapter.SelectionEnabled = value; }
        }

        public bool SingleExpansion
        {
            get { return TypedAdapter.SingleExpansion; }
            set { TypedAdapter.SingleExpansion = value; }
        }

        #endregion

        #region Constructors

        protected ExpandableRecyclerView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }
        public ExpandableRecyclerView(Context context, IAttributeSet attrs) : this(context, attrs, 0) { }
        public ExpandableRecyclerView(Context context, IAttributeSet attrs, int defStyle) 
            : base(context, attrs, defStyle, new ExpandableRecyclerViewAdapter())
        {
            TypedAdapter.ChildTemplateId = MvxAttributeHelpers.ReadAttributeValue(context, 
                                                                                  attrs,
                                                                                  AppCompatAndroidBindingResource.Instance.ExpandableRecyclerViewStylableGroupId,
                                                                                  AppCompatAndroidBindingResource.Instance.ExpandableRecyclerViewChildItemTemplateId);

        }

        #endregion
    }
}