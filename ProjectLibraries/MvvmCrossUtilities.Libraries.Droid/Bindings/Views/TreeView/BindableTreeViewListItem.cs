using System;
using Android.Content;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Droid.BindingContext;
using Cirrious.MvvmCross.Binding.Droid.Views;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views.TreeView
{
    public class BindableTreeViewListItem 
        : LinearLayout
        , ITreeViewListItem
        , IMvxBindingContextOwner
    {
        #region Fields

        private object _cachedDataContext;
        private bool _isAttachedToWindow;

        #endregion

        #region Properties

        protected IMvxAndroidBindingContext AndroidBindingContext
        {
            get { return _bindingContext; }
        }

        protected View Content { get; set; }

        public object DataContext
        {
            get { return _bindingContext.DataContext; }
            set
            {
                if (_isAttachedToWindow)
                {
                    _bindingContext.DataContext = value;
                }
                else
                {
                    _cachedDataContext = value;
                    if (_bindingContext.DataContext != null)
                    {
                        _bindingContext.DataContext = null;
                    }
                }
            }
        }

        protected virtual View FirstChild
        {
            get
            {
                if (ChildCount == 0)
                    return null;
                var firstChild = this.GetChildAt(0);
                return firstChild;
            }
        }

        protected virtual ICheckable ContentCheckable
        {
            get
            {
                var firstChild = FirstChild;
                return firstChild as ICheckable;
            }
        }

        #endregion

        #region Constructor

        public BindableTreeViewListItem(Context context, IMvxLayoutInflater layoutInflater, object dataContext, int templateId)
            : base(context)
        {
            _bindingContext = new MvxAndroidBindingContext(context, layoutInflater, dataContext);

            _templateId = templateId;
            Content = AndroidBindingContext.BindingInflate(templateId, this);
        }
        
        #endregion

        #region Methods

        protected override void OnAttachedToWindow()
        {
            base.OnAttachedToWindow();
            _isAttachedToWindow = true;
            if (_cachedDataContext != null
                && DataContext == null)
            {
                DataContext = _cachedDataContext;
            }
        }

        protected override void OnDetachedFromWindow()
        {
            _cachedDataContext = DataContext;
            DataContext = null;
            base.OnDetachedFromWindow();
            _isAttachedToWindow = false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.ClearAllBindings();
                _cachedDataContext = null;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region IMvxBindingContextOwner Members

        public IMvxBindingContext BindingContext
        {
            get { return _bindingContext; }
            set { throw new NotImplementedException("BindingContext is readonly in the treeview list item"); }
        }
        private readonly IMvxAndroidBindingContext _bindingContext;

        #endregion

        #region ICheckable Members

        public virtual bool Checked
        {
            get
            {
                var contentCheckable = ContentCheckable;
                if (contentCheckable == null)
                    return _checked;

                return contentCheckable.Checked;
            }
            set
            {
                var contentCheckable = ContentCheckable;
                if (contentCheckable == null)
                {
                    _checked = value;
                }
                else
                {
                    contentCheckable.Checked = value;
                }
            }
        }
        private bool _checked;

        public virtual void Toggle()
        {
            var contentCheckable = ContentCheckable;
            if (contentCheckable == null)
            {
                _checked = !_checked;
                return;
            }

            contentCheckable.Toggle();
        }

        #endregion

        #region ITreeViewListItem Members

        public int TemplateId
        {
            get { return _templateId; }
        }
        private readonly int _templateId;

        #endregion
    }
}