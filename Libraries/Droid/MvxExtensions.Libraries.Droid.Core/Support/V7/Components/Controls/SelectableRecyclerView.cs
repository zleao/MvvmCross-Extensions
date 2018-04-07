using Android.Content;
using Android.Runtime;
using Android.Util;
using MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Adapters;
using MvvmCross.Droid.Support.V7.RecyclerView;
using System;
using System.Collections;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls
{
    public class SelectableRecyclerView : MvxRecyclerView
    {
        #region Properties

        protected SelectableRecyclerViewAdapter TypedAdapter
        {
            get { return Adapter as SelectableRecyclerViewAdapter; }
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

        #endregion

        #region Constructors

        protected SelectableRecyclerView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer) { }
        public SelectableRecyclerView(Context context, IAttributeSet attrs) : this(context, attrs, 0) { }
        public SelectableRecyclerView(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle, new SelectableRecyclerViewAdapter()) { }

        #endregion
    }
}