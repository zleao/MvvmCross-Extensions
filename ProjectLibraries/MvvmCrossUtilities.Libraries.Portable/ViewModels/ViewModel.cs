using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows.Input;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Core;
using Cirrious.CrossCore.Platform;
using Cirrious.CrossCore.WeakSubscription;
using Cirrious.MvvmCross.Localization;
using Cirrious.MvvmCross.ViewModels;
using MvvmCrossUtilities.Libraries.Portable.Attributes;
using MvvmCrossUtilities.Libraries.Portable.Extensions;
using MvvmCrossUtilities.Libraries.Portable.LanguageBinder;
using MvvmCrossUtilities.Libraries.Portable.Messages;
using MvvmCrossUtilities.Libraries.Portable.Messages.TwoWay;
using MvvmCrossUtilities.Plugins.Notification;
using MvvmCrossUtilities.Plugins.Notification.Exceptions;
using MvvmCrossUtilities.Plugins.Notification.Messages;
using MvvmCrossUtilities.Plugins.Notification.Messages.Base;
using MvvmCrossUtilities.Plugins.Notification.Messages.OneWay;
using MvvmCrossUtilities.Plugins.Notification.Messages.TwoWay;
using MvvmCrossUtilities.Plugins.Notification.Subscriptions;

namespace MvvmCrossUtilities.Libraries.Portable.ViewModels
{
    public abstract class ViewModel : MvxViewModel, IViewModelLifecycle, IDisposable
    {
        #region Fields

        private int _busyCount;
        private MvxNotifyPropertyChangedEventSubscription _propertyChangedSubscription = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is busy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsBusy
        {
            get { return _busyCount > 0; }
        }

        /// <summary>
        /// Gets or sets the busy message.
        /// </summary>
        /// <value>The busy message.</value>
        public string BusyMessage
        {
            get { return _busyMessage; }
            protected set
            {
                if (_busyMessage != value)
                {
                    _busyMessage = value;
                    RaisePropertyChanged(() => BusyMessage);
                }
            }
        }
        private string _busyMessage;

        /// <summary>
        /// Gets a value indicating whether this instance is child.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is child; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsChild
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the page title.
        /// </summary>
        /// <value>
        /// The page title.
        /// </value>
        public virtual string PageTitle
        {
            get
            {
                if (_pageTitle == null)
                {
                    var ownType = this.GetType();

                    if (ownType.GetTypeInfo().IsAbstract || ownType.IsNested)
                    {
                        _pageTitle = string.Empty;
                    }
                    else
                    {
                        var ownName = this.GetType().Name;
                        var strippedName = ownName.Remove(ownName.IndexOf("ViewModel"));

                        if (TextSource != null)
                        {
                            _pageTitle = TextSource.GetText("Label_Page_Title_" + strippedName);
                        }
                    }
                }

                return _pageTitle;
            }
        }
        private string _pageTitle = null;

        /// <summary>
        /// Gets the long running message context.
        /// </summary>
        /// <value>
        /// The long running message context.
        /// </value>
        public virtual string LongRunningMessageContext
        {
            get { return string.Empty; }
        }

        #endregion

        #region Notification Management

        /// <summary>
        /// Gets the notification manager.
        /// </summary>
        /// <value>
        /// The notification manager.
        /// </value>
        protected INotificationService NotificationManager
        {
            get { return _notificationManager; }
        }
        private readonly INotificationService _notificationManager;

        /// <summary>
        /// Publishes the specified message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message.</param>
        public void Publish<TMessage>(TMessage message, string context = Subscription.DefaultContext) where TMessage : NotificationOneWayMessage
        {
            _notificationManager.Publish<TMessage>(message, context);
        }

        /// <summary>
        /// Publishes the specified message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="onResultCallback">The on result callback.</param>
        /// <param name="context">The context.</param>
        public void Publish<TMessage, TResult>(TMessage message, Action<TResult> onResultCallback, string context = Subscription.DefaultContext)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult
        {
            Publish<TMessage, TResult>(message, onResultCallback, OnTwoWayNotificationError, context);
        }

        /// <summary>
        /// Publishes the specified message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="onResultCallback">The on result callback.</param>
        /// <param name="onErrorCallback">The on error callback.</param>
        /// <param name="context">The context.</param>
        public void Publish<TMessage, TResult>(TMessage message, Action<TResult> onResultCallback, Action<NotificationErrorException> onErrorCallback, string context = Subscription.DefaultContext)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult
        {
            _notificationManager.Publish<TMessage, TResult>(message, onResultCallback, onErrorCallback, context);
        }

        /// <summary>
        /// Called when two way notification error.
        /// </summary>
        /// <param name="obj">The object.</param>
        protected virtual void OnTwoWayNotificationError(NotificationErrorException obj)
        {
            MvxTrace.Trace(MvxTraceLevel.Error, "Notification error occurred - " + obj.Message);
        }


        /// <summary>
        /// Publishes the info notification.
        /// Default mode will be used
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="source">The source.</param>
        public void PublishInfoNotification(string message, string context = Subscription.DefaultContext)
        {
            PublishInfoNotification(message, NotificationModeEnum.Default, context);
        }

        /// <summary>
        /// Publishes the information notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        public void PublishInfoNotification(string message, NotificationModeEnum mode, string context = Subscription.DefaultContext)
        {
            Publish(new NotificationGenericMessage(this, message, mode, NotificationSeverityEnum.Info), context);
        }

        /// <summary>
        /// Publishes the error notification.
        /// Default mode will be used
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="source">The source.</param>
        public void PublishErrorNotification(string message, string context = Subscription.DefaultContext)
        {
            PublishErrorNotification(message, NotificationModeEnum.Default, context);
        }

        /// <summary>
        /// Publishes the error notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        public void PublishErrorNotification(string message, NotificationModeEnum mode, string context = Subscription.DefaultContext)
        {
            Publish(new NotificationGenericMessage(this, message, mode, NotificationSeverityEnum.Error), context);
        }

        /// <summary>
        /// Publishes the success notification.
        /// Default mode will be used
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="source">The source.</param>
        public void PublishSuccessNotification(string message, string context = Subscription.DefaultContext)
        {
            PublishSuccessNotification(message, NotificationModeEnum.Default, context);
        }

        /// <summary>
        /// Publishes the success notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        public void PublishSuccessNotification(string message, NotificationModeEnum mode, string context = Subscription.DefaultContext)
        {
            Publish(new NotificationGenericMessage(this, message, mode, NotificationSeverityEnum.Success), context);
        }


        /// <summary>
        /// Publishes a generic blocking notification.
        /// </summary>
        /// <param name="severity">The severity.</param>
        /// <param name="message">The message.</param>
        /// <param name="onResultCallback">The on result callback.</param>
        /// <param name="context">The context.</param>
        public void PublishGenericBlockingNotification(NotificationSeverityEnum severity, string message, Action<NotificationResult> onResultCallback, string context = Subscription.DefaultContext)
        {
            Publish<NotificationGenericBlockingMessage, NotificationResult>(new NotificationGenericBlockingMessage(this, severity, message), onResultCallback, context);
        }

        /// <summary>
        /// Publishes a generic question notification.
        /// </summary>
        /// <param name="possibleAnswers">The possible answers.</param>
        /// <param name="question">The question.</param>
        /// <param name="onResultCallback">The on result callback.</param>
        /// <param name="context">The context.</param>
        public void PublishGenericQuestionNotification(string question, NotificationTwoWayAnswersGroupEnum possibleAnswers, Action<NotificationGenericQuestionResult> onResultCallback, string context = Subscription.DefaultContext)
        {
            Publish<NotificationGenericQuestionMessage, NotificationGenericQuestionResult>(new NotificationGenericQuestionMessage(this, question, possibleAnswers), onResultCallback, context);
        }

        /// <summary>
        /// Publishes a question notification with custom answer.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <param name="possibleAnswers">The possible answers.</param>
        /// <param name="onResultCallback">The on result callback.</param>
        /// <param name="context">The context.</param>
        public void PublishQuestionWithCustomAnswerNotification(string question, IList<string> possibleAnswers, Action<NotificationQuestionCustomAnswerResult> onResultCallback, string context = Subscription.DefaultContext)
        {
            Publish<NotificationQuestionWithCustomAnswerMessage, NotificationQuestionCustomAnswerResult>(new NotificationQuestionWithCustomAnswerMessage(this, question, possibleAnswers), onResultCallback, context);
        }


        /// <summary>
        /// Publishes the update menu notification.
        /// </summary>
        /// <param name="context">The context.</param>
        protected virtual void PublishUpdateMenuNotification(string context = Subscription.DefaultContext)
        {
            Publish(new NotificationUpdateMenuMessage(this), context);
        }

        #endregion

        #region Dependency Management

        /// <summary>
        /// Gets the property dependencies.
        /// </summary>
        private IDictionary<string, IList<PropertyInfo>> PropertyDependencies
        {
            get { return _dependencies; }
        }
        private readonly IDictionary<string, IList<PropertyInfo>> _dependencies;

        /// <summary>
        /// Gets the method dependencies.
        /// </summary>
        private IDictionary<string, IList<MethodInfo>> MethodDependencies
        {
            get { return _methodDependencies; }
        }
        private readonly IDictionary<string, IList<MethodInfo>> _methodDependencies;

        /// <summary>
        /// Gets a value indicating whether this instance properties or methods, have dependencies defined
        /// </summary>
        /// <value>
        ///   <c>true</c> if has dependencies; otherwise, <c>false</c>.
        /// </value>
        protected bool HasDependencies
        {
            get { return (PropertyDependencies != null && PropertyDependencies.Count > 0) || (MethodDependencies != null && MethodDependencies.Count > 0); }
        }

        /// <summary>
        /// Initializes the property dependencies.
        /// </summary>
        private IDictionary<string, IList<PropertyInfo>> InitializePropertyDependencies()
        {
            var dependencies = new Dictionary<string, IList<PropertyInfo>>();
            lock (dependencies)
            {
                foreach (var property in GetType().GetProperties(true))
                {
                    var attributes = property.GetCustomAttributes<DependsOnAttribute>(true);
                    foreach (var attribute in attributes)
                    {
                        if (!dependencies.ContainsKey(attribute.Name))
                            dependencies.Add(attribute.Name, new List<PropertyInfo>());
                        dependencies[attribute.Name].Add(property);
                    }
                }
            }
            return dependencies;
        }

        /// <summary>
        /// Initializes the method dependencies.
        /// </summary>
        private IDictionary<string, IList<MethodInfo>> InitializeMethodDependencies()
        {
            var methodDependencies = new Dictionary<string, IList<MethodInfo>>();

            lock (methodDependencies)
            {
                foreach (var method in this.GetType().GetMethods())
                {
                    if (!method.ReturnType.Equals(typeof(void)))
                        continue;
                    if (method.GetParameters().Length > 0)
                        continue;

                    var attributes = method.GetCustomAttributes<DependsOnAttribute>(true);
                    foreach (var attribute in attributes)
                    {
                        if (!methodDependencies.ContainsKey(attribute.Name))
                            methodDependencies.Add(attribute.Name, new List<MethodInfo>());
                        methodDependencies[attribute.Name].Add(method);
                    }
                }
            }

            return methodDependencies;
        }

        /// <summary>
        /// Initializes the property changed.
        /// </summary>
        private void InitializePropertyChanged()
        {
            if (HasDependencies)
                _propertyChangedSubscription = (this as INotifyPropertyChanged).WeakSubscribe(OnPropertyChanged);
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e == null)
                return;

            RaiseDependenciesPropertyChanged(e.PropertyName);
        }

        /// <summary>
        /// Raises the dependencies property changed.
        /// </summary>
        /// <param name="dependencyName">Name of the dependency.</param>
        public void RaiseDependenciesPropertyChanged(string dependencyName)
        {
            lock (PropertyDependencies)
            {
                IList<PropertyInfo> properties;
                if (PropertyDependencies.TryGetValue(dependencyName, out properties))
                {
                    foreach (var property in properties)
                    {
                        if (!typeof(ICommand).IsAssignableFrom(property.PropertyType))
                        {
                            RaisePropertyChanged(property.Name);
                        }
                        else
                        {
                            var command = property.GetValue(this, null) as MvxCommand;
                            if (command != null)
                                command.RaiseCanExecuteChanged();
                        }
                    }
                }
            }

            lock (MethodDependencies)
            {
                IList<MethodInfo> methods;
                if (MethodDependencies.TryGetValue(dependencyName, out methods))
                {
                    foreach (var method in methods)
                    {
                        method.Invoke(this, null);
                    }
                }
            }
        }

        /// <summary>
        /// Removes the property changed handlers.
        /// </summary>
        private void RemovePropertyChangedHandlers()
        {
            if (_propertyChangedSubscription != null)
            {
                _propertyChangedSubscription.Dispose();
                _propertyChangedSubscription = null;
            }
        }

        #endregion

        #region Text Source

        /// <summary>
        /// Gets the text source.
        /// </summary>
        /// <value>
        /// The text source.
        /// </value>
        public IMvxLanguageBinder TextSource
        {
            get { return _textSource; }
        }
        private readonly IMvxLanguageBinder _textSource;

        /// <summary>
        /// Changes the text source language.
        /// </summary>
        /// <param name="newLanguage">The new language.</param>
        /// <returns></returns>
        protected abstract bool ChangeTextSourceLanguage(string newLanguage);

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel"/> class.
        /// </summary>
        public ViewModel()
        {
            if (!Mvx.TryResolve<INotificationService>(out _notificationManager))
                throw new NullReferenceException("INotificationService");

            if (!Mvx.TryResolve<ITextLanguageBinder>(out _textSource))
                throw new NullReferenceException("ITextLanguageBinder");

            _dependencies = InitializePropertyDependencies();
            _methodDependencies = InitializeMethodDependencies();

            InitializePropertyChanged();
        }

        #endregion

        #region IViewModelLifecycle Members

        public bool IsViewVisible
        {
            get { return _isViewVisible; }
            private set
            {
                if (_isViewVisible != value)
                {
                    _isViewVisible = value;
                    if (value)
                        OnViewShown();
                    else
                        OnViewHidden();
                }
            }
        }
        private bool _isViewVisible;

        public void ChangeVisibility(bool value)
        {
            IsViewVisible = value;
        }

        public void KillMe()
        {
            //ensure that the view visibility indicator is set to hidden
            ChangeVisibility(false);
            OnViewKilled();
        }

        protected virtual void OnViewShown()
        {
        }

        protected virtual void OnViewHidden()
        {
        }

        protected virtual void OnViewKilled()
        {
        }

        #endregion

        #region Generic Methods

        /// <summary>
        /// Does the work in background.
        /// </summary>
        /// <param name="action">The action.</param>
        protected void DoWorkInBackground(Action action, string workMessage = null)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            MvxAsyncDispatcher.BeginAsync(() => BackgroundWorker(action, workMessage));
        }

        /// <summary>
        /// Backgrounds the worker.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="workMessage">The work message.</param>
        private void BackgroundWorker(Action action, string workMessage)
        {
            StartWork(workMessage);

            try
            {
                action.Invoke();
            }
            catch
            {
                FinishedWork();
                throw;
            }
            finally
            {
                FinishedWork();
            }
        }

        /// <summary>
        /// Signals the IsBusy property
        /// </summary>
        protected void StartWork()
        {
            Interlocked.Increment(ref _busyCount);
            RaisePropertyChanged(() => IsBusy);
        }

        /// <summary>
        /// Signals the IsBusy property and sets busy message.
        /// </summary>
        public virtual void StartWork(string message)
        {
            StartWork();

            BusyMessage = message;
        }

        /// <summary>
        /// Finished doing work.
        /// </summary>
        public virtual void FinishedWork()
        {
            Interlocked.Decrement(ref _busyCount);
            RaisePropertyChanged(() => IsBusy);

            if (_busyCount <= 0)
                BusyMessage = null;
        }

        #endregion

        #region IDisposable Members

        private bool _disposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!_disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    DisposeManagedResources();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                DisposeUnmanagedResources();

                // Note disposing has been done.
                _disposed = true;
            }
        }

        /// <summary>
        /// Disposes the managed resources.
        /// </summary>
        protected virtual void DisposeManagedResources()
        {
            RemovePropertyChangedHandlers();
        }

        /// <summary>
        /// Disposes the unmanaged resources.
        /// </summary>
        protected virtual void DisposeUnmanagedResources()
        {
        }

        #endregion

        #region Finalizers

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="Model"/> is reclaimed by garbage collection.
        /// </summary>
        ~ViewModel()
        {
            Dispose(false);
        }

        #endregion
    }
}
