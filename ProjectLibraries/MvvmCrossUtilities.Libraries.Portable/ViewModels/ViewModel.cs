using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Cirrious.CrossCore.WeakSubscription;
using Cirrious.MvvmCross.Localization;
using Cirrious.MvvmCross.Platform;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Views;
using MvvmCrossUtilities.Libraries.Portable.Attributes;
using MvvmCrossUtilities.Libraries.Portable.Extensions;
using MvvmCrossUtilities.Libraries.Portable.Messages.OneWay;
using MvvmCrossUtilities.Libraries.Portable.Messages.TwoWay;
using MvvmCrossUtilities.Libraries.Portable.Models;
using MvvmCrossUtilities.Plugins.Notification;
using MvvmCrossUtilities.Plugins.Notification.Core;
using MvvmCrossUtilities.Plugins.Notification.Core.Async.Subscriptions;
using MvvmCrossUtilities.Plugins.Notification.Messages;
using MvvmCrossUtilities.Plugins.Notification.Messages.Base;
using MvvmCrossUtilities.Plugins.Notification.Messages.OneWay;
using MvvmCrossUtilities.Plugins.Notification.Messages.TwoWay;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvvmCrossUtilities.Libraries.Portable.ViewModels
{
    /// <summary>
    /// Base viewmodel, built on top of MvxViewModel
    /// </summary>
    public abstract class ViewModel : MvxViewModel, IViewModelLifecycle, IDisposable
    {
        #region Fields

        private int _busyCount;

        private volatile MvxNotifyPropertyChangedEventSubscription _propertyChangedSubscription = null;
        private volatile Dictionary<string, MvxNotifyCollectionChangedEventSubscription> _notifiableCollectionsChangedSubscription = new Dictionary<string, MvxNotifyCollectionChangedEventSubscription>();

        private volatile List<NotificationGenericMessage> _initialGenericMessages = new List<NotificationGenericMessage>();
        private volatile IList<SubscriptionToken> _messageTokens = new List<SubscriptionToken>();
        private volatile IList<SubscriptionToken> _longRunningMessageTokens = new List<SubscriptionToken>();

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
            set
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
            protected set { }
        }
        private string _pageTitle = null;

        /// <summary>
        /// Gets this view model specific context.
        /// Can be used to subscribe/publish messages that are only intended for this context
        /// </summary>
        public string ViewModelContext
        {
            get { return _viewModelContext ?? (_viewModelContext = this.GetType().Name); }
        }
        private string _viewModelContext;

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
        /// Publishes the specified async message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task PublishAsync<TMessage>(TMessage message, string context = AsyncSubscription.DefaultContext)
            where TMessage : NotificationMessage
        {
            return _notificationManager.PublishAsync(message, context);
        }

        /// <summary>
        /// Delayed publish of specified async message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="context">The context.</param>
        /// <param name="tryNormalPublish">if set to <c>true</c> tries to do normal publish, before storing the notification for delayed publish.</param>
        /// <returns></returns>
        public Task DelayedPublishAsync<TMessage>(TMessage message, string context = AsyncSubscription.DefaultContext, bool tryNormalPublish = false)
            where TMessage : NotificationMessage
        {
            return _notificationManager.DelayedPublishAsync(message, context, tryNormalPublish);
        }

        /// <summary>
        /// Publishes the specified async message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task<TResult> PublishAsync<TMessage, TResult>(TMessage message, string context = AsyncSubscription.DefaultContext)
            where TMessage : NotificationMessage
            where TResult : NotificationResult
        {
            return _notificationManager.PublishAsync<TResult>(message, context);
        }


        /// <summary>
        /// Publishes an information notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task PublishInfoNotificationAsync(string message, string context = AsyncSubscription.DefaultContext)
        {
            return PublishInfoNotificationAsync(message, NotificationModeEnum.Default, context);
        }

        /// <summary>
        /// Publishes an information notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task PublishInfoNotificationAsync(string message, NotificationModeEnum mode, string context = AsyncSubscription.DefaultContext)
        {
            return PublishAsync(new NotificationGenericMessage(this, message, mode, NotificationSeverityEnum.Info), context);
        }

        /// <summary>
        /// Delayed publish of an information notification with the default mode.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task DelayedPublishInfoNotificationAsync(string message, string context = AsyncSubscription.DefaultContext)
        {
            return DelayedPublishInfoNotificationAsync(message, NotificationModeEnum.Default, context);
        }

        /// <summary>
        /// Delayed publish of an information notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task DelayedPublishInfoNotificationAsync(string message, NotificationModeEnum mode, string context = AsyncSubscription.DefaultContext)
        {
            return DelayedPublishAsync(new NotificationGenericMessage(this, message, mode, NotificationSeverityEnum.Info), context);
        }


        /// <summary>
        /// Publishes a warning notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task PublishWarningNotificationAsync(string message, string context = AsyncSubscription.DefaultContext)
        {
            return PublishWarningNotificationAsync(message, NotificationModeEnum.Default, context);
        }

        /// <summary>
        /// Publishes a warning notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task PublishWarningNotificationAsync(string message, NotificationModeEnum mode, string context = AsyncSubscription.DefaultContext)
        {
            return PublishAsync(new NotificationGenericMessage(this, message, mode, NotificationSeverityEnum.Warning), context);
        }

        /// <summary>
        /// Delayed publish of an warning notification with the default mode.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task DelayedPublishWarningNotificationAsync(string message, string context = AsyncSubscription.DefaultContext)
        {
            return DelayedPublishWarningNotificationAsync(message, NotificationModeEnum.Default, context);
        }

        /// <summary>
        /// Delayed publish of an warning notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task DelayedPublishWarningNotificationAsync(string message, NotificationModeEnum mode, string context = AsyncSubscription.DefaultContext)
        {
            return DelayedPublishAsync(new NotificationGenericMessage(this, message, mode, NotificationSeverityEnum.Warning), context);
        }


        /// <summary>
        /// Publishes an error notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task PublishErrorNotificationAsync(string message, string context = AsyncSubscription.DefaultContext)
        {
            return PublishErrorNotificationAsync(message, NotificationModeEnum.Default, context);
        }

        /// <summary>
        /// Publishes an error notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task PublishErrorNotificationAsync(string message, NotificationModeEnum mode, string context = AsyncSubscription.DefaultContext)
        {
            return PublishAsync(new NotificationGenericMessage(this, message, mode, NotificationSeverityEnum.Error), context);
        }

        /// <summary>
        /// Delayed publish of an error notification with the default mode.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task DelayedPublishErrorNotificationAsync(string message, string context = AsyncSubscription.DefaultContext)
        {
            return DelayedPublishErrorNotificationAsync(message, NotificationModeEnum.Default, context);
        }

        /// <summary>
        /// Delayed publish of an error notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task DelayedPublishErrorNotificationAsync(string message, NotificationModeEnum mode, string context = AsyncSubscription.DefaultContext)
        {
            return DelayedPublishAsync(new NotificationGenericMessage(this, message, mode, NotificationSeverityEnum.Error), context);
        }


        /// <summary>
        /// Publishes a success notification asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task PublishSuccessNotificationAsync(string message, string context = AsyncSubscription.DefaultContext)
        {
            return PublishSuccessNotificationAsync(message, NotificationModeEnum.Default, context);
        }

        /// <summary>
        /// Publishes a success notification asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task PublishSuccessNotificationAsync(string message, NotificationModeEnum mode, string context = AsyncSubscription.DefaultContext)
        {
            return PublishAsync(new NotificationGenericMessage(this, message, mode, NotificationSeverityEnum.Success), context);
        }

        /// <summary>
        /// Delayed publish of a success notification with the default mode.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task DelayedPublishSuccessNotificationAsync(string message, string context = AsyncSubscription.DefaultContext)
        {
            return DelayedPublishSuccessNotificationAsync(message, NotificationModeEnum.Default, context);
        }

        /// <summary>
        /// Delayed publish of a success notification.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task DelayedPublishSuccessNotificationAsync(string message, NotificationModeEnum mode, string context = AsyncSubscription.DefaultContext)
        {
            return DelayedPublishAsync(new NotificationGenericMessage(this, message, mode, NotificationSeverityEnum.Success), context);
        }


        /// <summary>
        /// Publishes a generic question notification asynchronous.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <param name="possibleAnswers">The possible answers.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task<NotificationGenericQuestionResult> PublishGenericQuestionNotificationAsync(string question, NotificationTwoWayAnswersGroupEnum possibleAnswers, string context = AsyncSubscription.DefaultContext)
        {
            return PublishAsync<NotificationGenericQuestionMessage, NotificationGenericQuestionResult>(new NotificationGenericQuestionMessage(this, question, possibleAnswers), context);
        }

        /// <summary>
        /// Publishes a question notification with custom answer asynchronous.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <param name="possibleAnswers">The possible answers.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task<NotificationQuestionCustomAnswerResult> PublishQuestionWithCustomAnswerNotificationAsync(string question, IList<string> possibleAnswers, string context = AsyncSubscription.DefaultContext)
        {
            return PublishAsync<NotificationQuestionWithCustomAnswerMessage, NotificationQuestionCustomAnswerResult>(new NotificationQuestionWithCustomAnswerMessage(this, question, possibleAnswers), context);
        }


        /// <summary>
        /// Publishes a update menu notification.
        /// </summary>
        /// <param name="context">The context.</param>
        protected virtual Task PublishUpdateMenuNotificationAsync(string context = AsyncSubscription.DefaultContext)
        {
            UpdateContextOptions();

            return PublishAsync(new NotificationUpdateMenuMessage(this), context);
        }

        /// <summary>
        /// Publishes a under construction notification.
        /// </summary>
        protected virtual Task PublishUnderConstructionNotificationAsync()
        {
            return PublishInfoNotificationAsync(TextSource.GetText("Message_Info_UnderConstruction"));
        }

        /// <summary>
        /// Publishes a terminate application notification.
        /// </summary>
        protected virtual Task PublishTerminateApplicationNotificationAsync()
        {
            return PublishAsync(new NotificationTerminateApplicationMessage(this));
        }


        /// <summary>
        /// Subscribes event
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        protected void SubscribeEvent<TMessage>(Func<TMessage, Task> asyncDeliveryAction, string context = AsyncSubscription.DefaultContext)
            where TMessage : NotificationMessage
        {
            var token = NotificationManager.Subscribe(asyncDeliveryAction, context);
            _messageTokens.Add(token);
        }

        /// <summary>
        /// Unsubscribes the message events.
        /// </summary>
        protected virtual void UnsubscribeMessageEvents()
        {
            foreach (var item in _messageTokens)
            {
                NotificationManager.Unsubscribe(item);
            }
            _messageTokens.Clear();
        }

        /// <summary>
        /// Subscribes a long running event.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="deliveryAction">The delivery action.</param>
        /// <param name="context">The context.</param>
        protected void SubscribeLongRunningEvent<TMessage>(Func<TMessage, Task> asyncDeliveryAction, string context = AsyncSubscription.DefaultContext) where TMessage : NotificationOneWayMessage
        {
            var token = NotificationManager.Subscribe<TMessage>(asyncDeliveryAction, context);
            _longRunningMessageTokens.Add(token);
        }

        /// <summary>
        /// Subscribes the long running message events.
        /// </summary>
        protected virtual void SubscribeLongRunningMessageEvents()
        {
        }

        /// <summary>
        /// Unsubscribes the long running message events.
        /// </summary>
        protected virtual void UnsubscribeLongRunningMessageEvents()
        {
            foreach (var item in _longRunningMessageTokens)
            {
                NotificationManager.Unsubscribe(item);
            }
            _longRunningMessageTokens.Clear();
        }

        #endregion

        #region ViewModelCommunication Management

        private volatile IDictionary<Type, ViewModelSubscriptionToken> _viewModelCommunicationTokens = new Dictionary<Type, ViewModelSubscriptionToken>();

        /// <summary>
        /// Subscribes a view model communication notification.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="deliveryAction">The delivery action.</param>
        /// <param name="unsubscribeOnMessageArrival">if set to <c>true</c> [unsubscribe on message arrival].</param>
        /// <param name="useDefaultContext">if set to <c>true</c> [use default context].</param>
        public void SubscribeViewModelCommunication<TMessage>(Action<TMessage> deliveryAction, bool unsubscribeOnMessageArrival = false, bool useDefaultContext = false) where TMessage : NotificationViewModelCommunicationMessage
        {
            SubscribeViewModelCommunication(deliveryAction, useDefaultContext ? AsyncSubscription.DefaultContext : ViewModelContext, unsubscribeOnMessageArrival);
        }

        /// <summary>
        /// Subscribes a view model communication notification.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="deliveryAction">The delivery action.</param>
        /// <param name="context">Context of the subscription</param>
        /// <param name="unsubscribeOnMessageArrival">if set to <c>true</c> [unsubscribe on message arrival].</param>
        public void SubscribeViewModelCommunication<TMessage>(Action<TMessage> deliveryAction, string context, bool unsubscribeOnMessageArrival = false) where TMessage : NotificationViewModelCommunicationMessage
        {
            var messageType = typeof(TMessage);

            if (_viewModelCommunicationTokens.ContainsKey(messageType))
            {
                if (_viewModelCommunicationTokens[messageType] != null)
                {
                    _notificationManager.Unsubscribe(_viewModelCommunicationTokens[messageType].Token);
                    _viewModelCommunicationTokens[messageType] = null;
                }
            }
            else
            {
                _viewModelCommunicationTokens.Add(messageType, null);
            }

            var token = _notificationManager.Subscribe<TMessage>(OnViewModelCommunicationAsync, context);

            _viewModelCommunicationTokens[messageType] = new ViewModelSubscriptionToken(token, deliveryAction, unsubscribeOnMessageArrival);
        }

        private Task OnViewModelCommunicationAsync<TMessage>(TMessage deliveryMessage) where TMessage : NotificationViewModelCommunicationMessage
        {
            var messageType = typeof(TMessage);

            if (_viewModelCommunicationTokens.ContainsKey(messageType))
            {
                var deliveryAction = _viewModelCommunicationTokens[messageType].DeliveryAction as Action<TMessage>;
                if (deliveryAction != null)
                    deliveryAction(deliveryMessage);

                if (_viewModelCommunicationTokens[messageType].UnsubscribeOnArrival)
                    _notificationManager.Unsubscribe(_viewModelCommunicationTokens[messageType].Token);
            }

            return Task.FromResult(true);
        }

        #endregion

        #region Dependency Management

        /// <summary>
        /// List of all the properties that have the DependsOn attribute configured
        /// </summary>
        private readonly Dictionary<string, IList<PropertyInfo>> _propertyDependencies = new Dictionary<string, IList<PropertyInfo>>();

        /// <summary>
        /// List of all the notifiable collections properties that have the PropagateCollectionChange attribute configured
        /// </summary>
        private readonly Dictionary<string, INotifyCollectionChanged> _notifiableCollectionsPropertyDependencies = new Dictionary<string, INotifyCollectionChanged>();

        /// <summary>
        /// List of all the methods that have the DependsOn attribute configured
        /// </summary>
        private readonly Dictionary<string, IList<MethodInfo>> _methodDependencies = new Dictionary<string, IList<MethodInfo>>();

        /// <summary>
        /// Gets a value indicating whether this instance should react to property changed events
        /// </summary>
        protected bool HasDependencies
        {
            get
            {
                return _propertyDependencies.Count > 0 || _notifiableCollectionsPropertyDependencies.Count > 0 || _methodDependencies.Count > 0;
            }
        }


        /// <summary>
        /// Maps all the properties that have de DependsOn and/or the PropagateCollectionChange attributes configured
        /// </summary>
        private void InitializePropertyDependencies(Type type)
        {
            foreach (var property in type.GetProperties(true))
            {
                var attributes = property.GetCustomAttributes<DependsOnAttribute>(true);
                if (attributes.SafeCount() > 0)
                {
                    lock (_propertyDependencies)
                    {
                        foreach (var attribute in attributes)
                        {
                            if (!_propertyDependencies.ContainsKey(attribute.Name))
                                _propertyDependencies.Add(attribute.Name, new List<PropertyInfo>());
                            _propertyDependencies[attribute.Name].Add(property);
                        }
                    }
                }

                if (typeof(INotifyCollectionChanged).IsAssignableFrom(property.PropertyType))
                {
                    var attribute = property.GetCustomAttribute<PropagateCollectionChangeAttribute>(true);
                    if (attribute != null)
                    {
                        lock (_notifiableCollectionsPropertyDependencies)
                        {
                            var collection = property.GetValue(this, null) as INotifyCollectionChanged;
                            _notifiableCollectionsPropertyDependencies.Add(property.Name, collection);
                        }
                        lock (_notifiableCollectionsChangedSubscription)
                        {
                            _notifiableCollectionsChangedSubscription.Add(property.Name, null);
                        }
                    }
                }
            }

            if (type != typeof(ViewModel))
            {
                var newType = type.GetTypeInfo().BaseType;
                if (newType != null)
                    InitializePropertyDependencies(newType);
            }
        }

        /// <summary>
        /// Maps all the methods that have de DependsOn attribute configured
        /// </summary>
        private void InitializeMethodDependencies(Type type)
        {
            //foreach (var method in type.GetTypeInfo().DeclaredMethods.Where(m => m.ReturnType.Equals(typeof(void)) && m.GetParameters().Length == 0))
            foreach (var method in type.GetTypeInfo().DeclaredMethods.Where(m => m.GetParameters().Length == 0))
            {
                var attributes = method.GetCustomAttributes<DependsOnAttribute>(true);
                if (attributes.SafeCount() > 0)
                {
                    foreach (var attribute in attributes)
                    {
                        if (!_methodDependencies.ContainsKey(attribute.Name))
                            _methodDependencies.Add(attribute.Name, new List<MethodInfo>());
                        _methodDependencies[attribute.Name].Add(method);
                    }
                }
            }

            if (type != typeof(ViewModel))
            {
                var newType = type.GetTypeInfo().BaseType;
                if (newType != null)
                    InitializeMethodDependencies(newType);
            }
        }

        /// <summary>
        /// Initializes the listeners for property changed events
        /// </summary>
        private void InitializePropertyChanged()
        {
            if (HasDependencies)
            {
                _propertyChangedSubscription = (this as INotifyPropertyChanged).WeakSubscribe(OnPropertyChanged);

                foreach (var item in _notifiableCollectionsPropertyDependencies)
                {
                    if (item.Value != null)
                        _notifiableCollectionsChangedSubscription[item.Key] = item.Value.WeakSubscribe(OnCollectionChanged);
                }
            }
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

            UpdateCollectionPropertyValue(e.PropertyName);
            RaiseDependenciesPropertyChanged(e.PropertyName);
        }

        /// <summary>
        /// Updates the collection property value.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void UpdateCollectionPropertyValue(string propertyName)
        {
            INotifyCollectionChanged collection;
            if (_notifiableCollectionsPropertyDependencies.TryGetValue(propertyName, out collection))
            {
                var senderCollection = this.SafeGetPropertyValue(propertyName) as INotifyCollectionChanged;
                if (!object.ReferenceEquals(collection, senderCollection))
                {
                    //Remove previous subscription
                    if (_notifiableCollectionsChangedSubscription[propertyName] != null)
                    {
                        _notifiableCollectionsChangedSubscription[propertyName].Dispose();
                        _notifiableCollectionsChangedSubscription[propertyName] = null;
                    }

                    //Add new subscription
                    if (senderCollection != null)
                        _notifiableCollectionsChangedSubscription[propertyName] = senderCollection.WeakSubscribe(OnCollectionChanged);

                    _notifiableCollectionsPropertyDependencies[propertyName] = senderCollection;
                }
            }
        }

        /// <summary>
        /// Handles the collection changed events for the notifiable collections marked with the PropagateCollectionChange attribute
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.</param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var collection in _notifiableCollectionsPropertyDependencies.Where(nc => object.ReferenceEquals(sender, nc.Value)).OfType<KeyValuePair<string, INotifyCollectionChanged>?>())
            {
                RaiseDependenciesPropertyChanged(collection.Value.Key);
            }
        }

        /// <summary>
        /// Raises the dependencies property changed.
        /// </summary>
        /// <param name="dependencyName">Name of the dependency.</param>
        public void RaiseDependenciesPropertyChanged(string dependencyName)
        {
            lock (_propertyDependencies)
            {
                IList<PropertyInfo> properties;
                if (_propertyDependencies.TryGetValue(dependencyName, out properties))
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

            lock (_methodDependencies)
            {
                IList<MethodInfo> methods;
                if (_methodDependencies.TryGetValue(dependencyName, out methods))
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

        /// <summary>
        /// Removes the collection changed handlers.
        /// </summary>
        private void RemoveCollectionChangedHandlers()
        {
            foreach (var item in _notifiableCollectionsChangedSubscription)
            {
                if (item.Value == null)
                    continue;
                try
                {
                    item.Value.Dispose();
                }
                catch (InvalidOperationException)
                {
                    // This error might occur during dispose.
                }
            }
            _notifiableCollectionsChangedSubscription.Clear();
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

            if (!Mvx.TryResolve<IMvxLanguageBinder>(out _textSource))
                throw new NullReferenceException("IMvxLanguageBinder");

            _processContextOptionCommand = new MvxCommand<string>(OnProcessContextOptionCommand);
            _backCommand = new MvxCommand(OnBack);

            InitializePropertyDependencies(this.GetType());
            InitializeMethodDependencies(this.GetType());

            InitializePropertyChanged();

            SubscribeLongRunningMessageEvents();
        }

        #endregion

        #region IViewModelLifecycle Members

        /// <summary>
        /// Gets a value indicating whether the correspondent view is visible or hidden.
        /// Controled by the method [IsVisible(bool value)]
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is view visible]; otherwise, <c>false</c>.
        /// </value>
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

        /// <summary>
        /// Used to signal that a view is about to be shown/hidden
        /// </summary>
        /// <param name="value">if set to <c>true</c> the view is about to be shown. Othrewise is about to be hidden</param>
        public void ChangeVisibility(bool value)
        {
            IsViewVisible = value;
        }

        /// <summary>
        /// Used to signal that the view is about to be destroyed
        /// </summary>
        public void KillMe()
        {
            //ensure that the view visibility indicator is set to hidden
            ChangeVisibility(false);
            OnViewKilled();
        }

        /// <summary>
        /// Called when view shown.
        /// </summary>
        protected virtual async void OnViewShown()
        {
            await DoWorkAsync(async () =>
            {
                if (NotificationManager != null)
                {
                    if (_initialGenericMessages.Count > 0)
                    {
                        foreach (var message in _initialGenericMessages)
                        {
                            await PublishAsync(message);
                        }
                        _initialGenericMessages.Clear();
                    }

                    await NotificationManager.PublishPendingAsyncNotificationsAsync(this, ViewModelContext);
                    await NotificationManager.PublishPendingAsyncNotificationsAsync(this, AsyncSubscription.DefaultContext);
                }
            }, isSilent: true);
        }

        /// <summary>
        /// Called when view hidden.
        /// </summary>
        protected virtual void OnViewHidden()
        {
            UnsubscribeMessageEvents();
        }

        /// <summary>
        /// Called when view killed.
        /// </summary>
        protected virtual void OnViewKilled()
        {
            UnsubscribeLongRunningMessageEvents();
        }

        #endregion

        #region ContextOptions Managment

        /// <summary>
        /// Gets the context options.
        /// </summary>
        /// <value>
        /// The context options.
        /// </value>
        public virtual IDictionary<string, ContextOption> ContextOptions
        {
            get
            {
                return _contextOptions.SafeCount() > 0 ? _contextOptions : (_contextOptions = CreateContextOptions());
            }
        }
        private IDictionary<string, ContextOption> _contextOptions;

        /// <summary>
        /// Gets the process context option command.
        /// </summary>
        /// <value>
        /// The process context option command.
        /// </value>
        public ICommand ProcessContextOptionCommand
        {
            get { return _processContextOptionCommand; }
        }
        private readonly ICommand _processContextOptionCommand;


        /// <summary>
        /// Creates the context options.
        /// </summary>
        /// <returns></returns>
        protected virtual IDictionary<string, ContextOption> CreateContextOptions()
        {
            return new Dictionary<string, ContextOption>();
        }

        /// <summary>
        /// Called when a context option is selected .
        /// </summary>
        /// <param name="selectedOption">The selected option.</param>
        private async void OnProcessContextOptionCommand(string selectedOption)
        {
            await DoWorkAsync(() => ProcessContextOptionAsync(selectedOption), TextSource.GetText("Message_Busy_Processing"));
        }

        /// <summary>
        /// Processes the selected context option.
        /// </summary>
        /// <param name="selectedOption">The selected option.</param>
        /// <returns></returns>
        protected virtual Task ProcessContextOptionAsync(string selectedOption)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Updates the context options.
        /// </summary>
        public virtual void UpdateContextOptions()
        {
            ContextOptions.SafeForEach((id, co) => co.IsEnabled = !IsBusy);
        }

        #endregion

        #region Generic Methods

        /// <summary>
        /// Executes work asynchronously.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="workMessage">The work message.</param>
        /// <param name="isSilent">if set to <c>true</c> [is silent].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">action</exception>
        protected async Task DoWorkAsync(Func<Task> action, string workMessage = null, bool isSilent = false)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            StartWork(workMessage, isSilent);

            try
            {
                await action.Invoke();
            }
            catch
            {
                await FinishWorkAsync(isSilent);
                throw;
            }
            finally
            {
                await FinishWorkAsync(isSilent);
            }
        }

        /// <summary>
        /// Signals the IsBusy property
        /// </summary>
        protected void StartWork(bool isSilent = false)
        {
            if (!isSilent)
            {
                Interlocked.Increment(ref _busyCount);
                RaisePropertyChanged(() => IsBusy);

                lock (ContextOptions) { UpdateContextOptions(); }
            }
        }

        /// <summary>
        /// Signals the IsBusy property and sets busy message.
        /// </summary>
        public virtual void StartWork(string message, bool isSilent = false)
        {
            StartWork(isSilent);

            if (!isSilent)
                BusyMessage = message;
        }

        /// <summary>
        /// Signals the the work is finished
        /// </summary>
        public virtual async Task FinishWorkAsync(bool isSilent = false)
        {
            if (!isSilent)
            {
                Interlocked.Decrement(ref _busyCount);
                RaisePropertyChanged(() => IsBusy);

                if (_busyCount <= 0)
                {
                    BusyMessage = null;

                    lock (ContextOptions) { UpdateContextOptions(); }
                }

                await PublishUpdateMenuNotificationAsync();
            }
        }

        /// <summary>
        /// Adds a new message to the initial messages buffer.
        /// </summary>
        /// <param name="newMessage">The new message.</param>
        protected void AddInitialGenericMessage(NotificationGenericMessage newMessage)
        {
            if (!_initialGenericMessages.Contains(newMessage))
                _initialGenericMessages.Add(newMessage);
        }

        /// <summary>
        /// Adds a new info message to the initial messages buffer.
        /// </summary>
        /// <param name="newMessage">The new message.</param>
        protected void AddInitialGenericInfoMessage(string newMessage)
        {
            if (!newMessage.IsNullOrEmpty())
                AddInitialGenericMessage(new NotificationGenericMessage(this, newMessage, NotificationModeEnum.Default, NotificationSeverityEnum.Info));
        }

        /// <summary>
        /// Determines whether there is a registered view for the specified viewmodel.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <returns></returns>
        public static bool HasRegisteredViewFor<TViewModel>() where TViewModel : ViewModel
        {
            return HasRegisteredViewFor(typeof(TViewModel));
        }

        /// <summary>
        /// Determines whether there is a registered view for the specified viewmodel.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <returns></returns>
        public static bool HasRegisteredViewFor(Type viewModelType)
        {
            var viewsContainer = Mvx.Resolve<IMvxViewsContainer>();
            if (viewsContainer != null)
            {
                try
                {
                    var view = viewsContainer.GetViewType(viewModelType);
                    return view != null;
                }
                catch (KeyNotFoundException)
                {
                    return false;
                }
            }

            return false;
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
            RemoveCollectionChangedHandlers();
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

        #region Navigation

        /// <summary>
        /// Shows a view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="parameterValuesObject">The parameter values object.</param>
        /// <param name="presentationBundle">The presentation bundle.</param>
        /// <param name="requestedBy">The requested by.</param>
        /// <param name="removeCurrentViewFromStack">if set to <c>true</c> [remove current view from stack].</param>
        /// <returns></returns>
        protected bool ShowViewModel<TViewModel>(object parameterValuesObject,
                                                 IMvxBundle presentationBundle = null,
                                                 MvxRequestedBy requestedBy = null,
                                                 bool removeCurrentViewFromStack = false)
            where TViewModel : IMvxViewModel
        {
            return ShowViewModel(
                typeof(TViewModel),
                parameterValuesObject.ToSimplePropertyDictionary(),
                presentationBundle,
                requestedBy,
                removeCurrentViewFromStack);
        }

        /// <summary>
        /// Forces navigation to the specified view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="parameterValues">The parameter values.</param>
        /// <param name="presentationBundle">The presentation bundle.</param>
        /// <param name="requestedBy">The requested by.</param>
        /// <param name="removeCurrentViewFromStack">if set to <c>true</c> [remove current view from stack].</param>
        /// <returns></returns>
        protected bool ShowViewModel<TViewModel>(IDictionary<string, string> parameterValues,
                                                 IMvxBundle presentationBundle = null,
                                                 MvxRequestedBy requestedBy = null,
                                                 bool removeCurrentViewFromStack = false)
            where TViewModel : IMvxViewModel
        {
            return ShowViewModel(
                typeof(TViewModel),
                new MvxBundle(parameterValues.ToSimplePropertyDictionary()),
                presentationBundle,
                requestedBy,
                removeCurrentViewFromStack);
        }

        /// <summary>
        /// Forces navigation to the specified view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <param name="parameterBundle">The parameter bundle.</param>
        /// <param name="presentationBundle">The presentation bundle.</param>
        /// <param name="requestedBy">The requested by.</param>
        /// <param name="removeCurrentViewFromStack">if set to <c>true</c> [remove current view from stack].</param>
        /// <returns></returns>
        protected bool ShowViewModel<TViewModel>(IMvxBundle parameterBundle = null,
                                                 IMvxBundle presentationBundle = null,
                                                 MvxRequestedBy requestedBy = null,
                                                 bool removeCurrentViewFromStack = false)
            where TViewModel : IMvxViewModel
        {
            return ShowViewModel(
                typeof(TViewModel),
                parameterBundle,
                presentationBundle,
                requestedBy,
                removeCurrentViewFromStack);
        }


        /// <summary>
        /// Forces navigation to the specified view model.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="parameterValuesObject">The parameter values object.</param>
        /// <param name="presentationBundle">The presentation bundle.</param>
        /// <param name="requestedBy">The requested by.</param>
        /// <param name="removeCurrentViewFromStack">if set to <c>true</c> [remove current view from stack].</param>
        /// <returns></returns>
        protected bool ShowViewModel(Type viewModelType,
                                     object parameterValuesObject,
                                     IMvxBundle presentationBundle = null,
                                     MvxRequestedBy requestedBy = null,
                                     bool removeCurrentViewFromStack = false)
        {
            return ShowViewModel(viewModelType,
                                 new MvxBundle(parameterValuesObject.ToSimplePropertyDictionary()),
                                 presentationBundle,
                                 requestedBy,
                                 removeCurrentViewFromStack);
        }

        /// <summary>
        /// Forces navigation to the specified view model.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <param name="presentationBundle">The presentation bundle.</param>
        /// <param name="requestedBy">The requested by.</param>
        /// <param name="removeCurrentViewFromStack">if set to <c>true</c> [remove current view from stack].</param>
        /// <returns></returns>
        protected bool ShowViewModel(Type viewModelType,
                                     IDictionary<string, string> parameterValues,
                                     IMvxBundle presentationBundle = null,
                                     MvxRequestedBy requestedBy = null,
                                     bool removeCurrentViewFromStack = false)
        {
            return ShowViewModel(viewModelType,
                                 new MvxBundle(parameterValues),
                                 presentationBundle,
                                 requestedBy,
                                 removeCurrentViewFromStack);
        }

        /// <summary>
        /// Forces navigation to the specified view model.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="parameterBundle">The parameter bundle.</param>
        /// <param name="presentationBundle">The presentation bundle.</param>
        /// <param name="requestedBy">The requested by.</param>
        /// <param name="removeCurrentViewFromStack">if set to <c>true</c> [remove current view from stack].</param>
        /// <returns></returns>
        protected bool ShowViewModel(Type viewModelType,
                                     IMvxBundle parameterBundle = null,
                                     IMvxBundle presentationBundle = null,
                                     MvxRequestedBy requestedBy = null,
                                     bool removeCurrentViewFromStack = false)
        {
            return ShowViewModelImpl(viewModelType, parameterBundle, presentationBundle, requestedBy, removeCurrentViewFromStack);
        }


        private bool ShowViewModelImpl(Type viewModelType, IMvxBundle parameterBundle, IMvxBundle presentationBundle,
                                       MvxRequestedBy requestedBy,
                                       bool removeCurrentViewFromStack = false)
        {
            MvxTrace.Trace("Showing ViewModel {0} - RemoveViewFromStack: {1}", viewModelType.Name, removeCurrentViewFromStack);

            var viewDispatcher = ViewDispatcher as IViewDispatcher;
            if (viewDispatcher != null)
            {
                return viewDispatcher.ShowViewModel(new MvxViewModelRequest(
                                                        viewModelType,
                                                        parameterBundle,
                                                        presentationBundle,
                                                        requestedBy),
                                                    removeCurrentViewFromStack);
            }

            MvxTrace.Error("ShowViewModel -> IViewDispatcher not found. Navigation will be aborted");
            return false;
        }

        #endregion

        #region Back Managment

        /// <summary>
        /// The back command.
        /// </summary>
        public ICommand BackCommand
        {
            get { return _backCommand; }
        }
        private readonly ICommand _backCommand;

        /// <summary>
        /// Called when back command is executed.
        /// </summary>
        protected virtual void OnBack() { }

        #endregion
    }
}
