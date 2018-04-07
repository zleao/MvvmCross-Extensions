using MvvmCross.Localization;
using MvvmCross.Platform.Platform;
using MvvmCross.Platform.WeakSubscription;
using MvxExtensions.Plugins.Notification;
using System;
using System.ComponentModel;

namespace MvxExtensions.Libraries.Portable.Core.ViewModels.SimpleMenu
{
    /// <summary>
    /// View model normally used to represent a view inside a larger view. 
    /// </summary>
    /// <typeparam name="TParentViewModel">The type of the parent view model.</typeparam>
    public abstract class SimpleMenuChildViewModel<TParentViewModel> : SimpleMenuViewModel
        where TParentViewModel : SimpleMenuViewModel
    {
        #region Fields

        private MvxNotifyPropertyChangedEventSubscription _parentPropertyChangedSubscription = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is child.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is child; otherwise, <c>false</c>.
        /// </value>
        public override bool IsChild
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public virtual TParentViewModel Parent
        {
            get { return _parent; }
        }
        private readonly TParentViewModel _parent;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleMenuChildViewModel{TParentViewModel}" /> class.
        /// </summary>
        /// <param name="textSource">The text source.</param>
        /// <param name="jsonConverter">The json converter.</param>
        /// <param name="notificationManager">The notification manager.</param>
        /// <param name="parent">The parent.</param>
        /// <exception cref="System.ArgumentNullException">parent</exception>
        public SimpleMenuChildViewModel(IMvxLanguageBinder textSource,
                                        IMvxJsonConverter jsonConverter,
                                        INotificationService notificationManager,
                                        TParentViewModel parent)
            : base(textSource, jsonConverter, notificationManager)
        {
            if (parent == null)
                throw new ArgumentNullException("parent");

            _parent = parent;

            InitializeParentDependency();
        }

        #endregion

        #region Parent notification management

        /// <summary>
        /// Initializes the parent dependency.
        /// </summary>
        private void InitializeParentDependency()
        {
            if (HasDependencies)
            {
                var notifiableVM = (Parent as INotifyPropertyChanged);
                if (notifiableVM != null)
                    _parentPropertyChangedSubscription = notifiableVM.WeakSubscribe(OnParentPropertyChanged);
            }
        }

        /// <summary>
        /// Called when parent property changed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        void OnParentPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var propertyName = "Parent." + e.PropertyName;
            RaiseDependenciesPropertyChanged(propertyName);
        }

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected override void DisposeManagedResources()
        {
            try
            {
                if (_parentPropertyChangedSubscription != null)
                {
                    _parentPropertyChangedSubscription.Dispose();
                    _parentPropertyChangedSubscription = null;
                }
            }
            catch (InvalidOperationException)
            {
                // This error might occur during dispose.
            }

            base.DisposeManagedResources();
        }

        #endregion

        #region Busy notification propagation

        /// <summary>
        /// Starts doing work.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isSilent"></param>
        public override void StartWork(string message, bool isSilent = false)
        {
            base.StartWork(message, isSilent);

            Parent.StartWork(message, isSilent);
        }

        /// <summary>
        /// Signals the IsBusy to indicate that work is finished.
        /// </summary>
        /// <param name="isSilent">if set to <c>true</c> the IsBusy will no be signaled.</param>
        public override void FinishWork(bool isSilent = false)
        {
            base.FinishWork(isSilent);

            Parent.FinishWork(isSilent);
        }

        #endregion
    }
}
