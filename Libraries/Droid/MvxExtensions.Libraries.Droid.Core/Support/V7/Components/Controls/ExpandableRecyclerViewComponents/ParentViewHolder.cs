using Android.Views;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V7.RecyclerView;
using MvxExtensions.Libraries.Portable.Core.Models;
using System;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.ExpandableRecyclerViewComponents
{
    public class ParentViewHolder : Android.Support.V7.Widget.RecyclerView.ViewHolder, IMvxRecyclerViewHolder, IMvxBindingContextOwner, View.IOnClickListener
    {
        #region Properties

        public IMvxBindingContext BindingContext
        {
            get { return _bindingContext; }
            set { throw new NotImplementedException("BindingContext is readonly in the list item"); }
        }
        private readonly IMvxBindingContext _bindingContext;

        public object DataContext
        {
            get { return _bindingContext.DataContext; }
            set { _bindingContext.DataContext = value; }
        }

        /// <summary>
        /// Used to determine whether a click in the entire parent <see cref="View"/>
        /// should trigger row expansion.
        /// If you return false, you can call <see cref="ExpandView"/> to trigger an
        /// expansion in response to a another event or <see cref="CollapseView"/> to
        /// trigger a collapse.
        /// </summary>
        /// <value>
        /// <c>true</c> to set an <see cref="View.IOnClickListener"/> on the item view
        /// </value>
        public virtual bool ItemViewClickTogglesExpansion
        {
            get { return true; }
        }

        public IParentExpandCollapseListener ExpandCollapseListener { get; private set; }

        public bool IsExpanded
        {
            get
            {
                return (DataContext as IGroupItem).IsExpanded;
            }
            private set
            {
                (DataContext as IGroupItem).IsExpanded = value;
            }
        }

        #endregion

        #region Constructor

        public ParentViewHolder(View itemView, IMvxAndroidBindingContext context)
            : base(itemView)
        {
            this._bindingContext = context;
        }

        #endregion

        #region Methods

        public void SetIsExpanded(bool value)
        {
            IsExpanded = value;
            OnExpansionToggled(!value);
        }

        /// <summary>
        /// Sets the expand/collapse listener.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public void SetExpandCollapseListener(IParentExpandCollapseListener listener)
        {
            ExpandCollapseListener = listener;
        }

        /// <summary>
        /// Sets a <see cref="View.IOnClickListener"/> on the entire parent
        /// view to trigger expansion.
        /// </summary>
        public void SetMainItemClickToExpand()
        {
            ItemView.SetOnClickListener(this);
        }

        /// <summary>
        /// <see cref="View.IOnClickListener"/> to listen for click events on
        /// the entire parent <see cref="View"/>.
        /// Only registered if <see cref="ItemViewClickTogglesExpansion"/> is true.
        /// </summary>
        /// <param name="v">The <see cref="View"/> that is the trigger for expansion</param>
        public void OnClick(View v)
        {
            if (IsExpanded)
                CollapseView();
            else
                ExpandView();
        }

        /// <summary>
        /// Triggers expansion of the parent.
        /// </summary>
        protected void ExpandView()
        {
            if (ExpandCollapseListener != null)
            {
                ExpandCollapseListener.OnParentExpand(this);
                OnExpansionToggled(false);
            }
        }

        /// <summary>
        /// Triggers collapse of the parent.
        /// </summary>
        protected void CollapseView()
        {
            if (ExpandCollapseListener != null)
            {
                ExpandCollapseListener.OnParentCollapse(this);
                OnExpansionToggled(true);
            }
        }

        /// <summary>
        /// Callback triggered when expansion state is changed, but not during
        /// initialization.
        /// Useful for implementing animations on expansion.
        /// </summary>
        /// <param name="expanded"><c>true</c> if view is expanded before expansion is toggled, otherwise <c>false</c>.</param>
        public virtual void OnExpansionToggled(bool expanded)
        {

        }

        public void OnAttachedToWindow()
        {
        }

        public void OnDetachedFromWindow()
        {
            DataContext = null;
        }

        public void OnViewRecycled()
        {
            DataContext = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _bindingContext.ClearAllBindings();
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}