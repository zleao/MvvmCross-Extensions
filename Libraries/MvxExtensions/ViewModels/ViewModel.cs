using MvvmCross.Localization;
using MvxExtensions.Attributes;
using MvxExtensions.Extensions;
using MvxExtensions.Models;
using MvxExtensions.Plugins.Notification;
using MvxExtensions.Plugins.Notification.Core;
using MvxExtensions.Plugins.Notification.Core.Async.Subscriptions;
using MvxExtensions.Plugins.Notification.Messages;
using MvxExtensions.Plugins.Notification.Messages.Base;
using MvxExtensions.Plugins.Notification.Messages.OneWay;
using MvxExtensions.Plugins.Notification.Messages.TwoWay;
using MvxExtensions.Plugins.Notification.Messages.TwoWay.Result;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Commands;
using MvvmCross.Logging;
using MvvmCross.Plugin.JsonLocalization;
using MvvmCross.ViewModels;
using MvvmCross.Views;
using MvvmCross.WeakSubscription;

namespace MvxExtensions.ViewModels
{
    /// <summary>
    /// Base viewmodel, built on top of <see cref="MvxViewModel"/>
    /// </summary>
    public abstract class ViewModel : MvxViewModel, IDisposable
    {
        #region Fields

        private int _busyCount;

        private volatile Dictionary<string, int> _dependsOnConditionalCount = new Dictionary<string, int>();

        private MvxNotifyPropertyChangedEventSubscription _propertyChangedSubscription = null;
        private volatile Dictionary<string, MvxNotifyCollectionChangedEventSubscription> _notifiableCollectionsChangedSubscription = new Dictionary<string, MvxNotifyCollectionChangedEventSubscription>();

        private volatile List<NotificationGenericMessage> _initialGenericMessages = new List<NotificationGenericMessage>();

        private volatile IList<SubscriptionToken> _messageTokens = new List<SubscriptionToken>();
        private volatile IList<LongRunningSubscriptionToken> _longRunningMessageTokens = new List<LongRunningSubscriptionToken>();

        private IMvxLog _log;

        #endregion

        #region Plugins

        /// <summary>
        /// Plugin for json manipulation
        /// </summary>
        protected IMvxJsonConverter JsonConverter { get; }

        /// <summary>
        /// Plugin used for notifications propagation.
        /// </summary>
        protected INotificationService NotificationManager { get; }

        /// <summary>
        /// The log provider.
        /// </summary>
        protected IMvxLogProvider LogProvider { get; }

        /// <summary>
        /// Log clas instance.
        /// </summary>
        protected IMvxLog Log => GetLog();

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether this instance is child.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is child; otherwise, <c>false</c>.
        /// </value>
        public virtual bool IsChild => false;

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
                        var strippedName = ownName.Remove(ownName.IndexOf("ViewModel", StringComparison.Ordinal));

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
        private string _pageTitle;

        /// <summary>
        /// Gets this view model specific context.
        /// Can be used to subscribe/publish messages that are only intended for this context
        /// </summary>
        public string ViewModelContext => _viewModelContext ?? (_viewModelContext = GetType().Name);

        private string _viewModelContext;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is hosted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is hosted; otherwise, <c>false</c>.
        /// </value>
        protected bool IsHosted { get; set; }

        #endregion

        #region Notification Management

        #region Subscription/Unsubscription

        /// <summary>
        /// Subscribes an one-way event.
        /// This subscription will at best be valid until the 'OnViewHidden' is called
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        protected void SubscribeEvent<TMessage>(Func<TMessage, Task> asyncDeliveryAction, string context = AsyncSubscription.DefaultContext)
            where TMessage : INotificationMessage
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
        /// Subscribes the long running message events.
        /// This Method is called in the viewmodel base constructor
        /// </summary>
        protected virtual void SubscribeLongRunningMessageEvents() { }

        /// <summary>
        /// Subscribes an one-way long running event.
        /// This subscription will at best be valid until the destructor of this class is called
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="asyncDeliveryActionName">Name of the asynchronous delivery action.</param>
        /// <param name="unsubscribeOnMessageArrival">if set to <c>true</c> [unsubscribe on message arrival].</param>
        /// <param name="context">The context.</param>
        protected void SubscribeLongRunningEvent<TMessage>(Func<TMessage, Task> asyncDeliveryAction, string asyncDeliveryActionName, bool unsubscribeOnMessageArrival = false, string context = AsyncSubscription.DefaultContext)
            where TMessage : NotificationOneWayMessage
        {
            var messageType = typeof(TMessage);
            Task CastedAsyncDeliveryAction(INotificationOneWayMessage msg) => asyncDeliveryAction((TMessage) msg);
            SubscribeLongRunningEvent(messageType, CastedAsyncDeliveryAction, asyncDeliveryActionName, unsubscribeOnMessageArrival, context);
        }

        /// <summary>
        /// Subscribes an one-way long running event.
        /// This subscription will at best be valid until the destructor of this class is called
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="asyncDeliveryActionName">Name of the asynchronous delivery action.</param>
        /// <param name="unsubscribeOnMessageArrival">if set to <c>true</c> [unsubscribe on message arrival].</param>
        /// <param name="context">The context.</param>
        protected void SubscribeLongRunningEvent(Type messageType, Func<INotificationOneWayMessage, Task> asyncDeliveryAction, string asyncDeliveryActionName, bool unsubscribeOnMessageArrival = false, string context = AsyncSubscription.DefaultContext)
        {
            var equalLongSubscription = _longRunningMessageTokens.SafeFirstOrDefault(l => l.Token.MessageType == messageType);
            if (equalLongSubscription != null)
            {
                NotificationManager.Unsubscribe(equalLongSubscription.Token);
                _longRunningMessageTokens.Remove(equalLongSubscription);
            }

            var token = NotificationManager.Subscribe(messageType, OnLongRunningNotificationAsync, context);

            _longRunningMessageTokens.Add(new LongRunningSubscriptionToken(token, asyncDeliveryAction, asyncDeliveryActionName, unsubscribeOnMessageArrival));
        }

        private async Task OnLongRunningNotificationAsync(INotificationMessage msg)
        {
            var messageType = msg.GetType();

            var longSubscription = _longRunningMessageTokens.SafeFirstOrDefault(l => l.Token.MessageType == messageType);
            if (longSubscription != null)
            {
                if (longSubscription.AsyncDeliveryAction is Func<INotificationOneWayMessage, Task> asyncDeliveryAction)
                    await asyncDeliveryAction.Invoke(msg as INotificationOneWayMessage);

                if (longSubscription.UnsubscribeOnArrival)
                {
                    NotificationManager.Unsubscribe(longSubscription.Token);
                    _longRunningMessageTokens.Remove(longSubscription);
                }
            }
        }


        /// <summary>
        /// Unsubscribes the long running message events.
        /// </summary>
        protected virtual void UnsubscribeLongRunningMessageEvents()
        {
            foreach (var item in _longRunningMessageTokens)
            {
                NotificationManager.Unsubscribe(item.Token);
            }
            _longRunningMessageTokens.Clear();
        }

        #endregion

        #region Publish

        /// <summary>
        /// Publishes an one-way under construction message.
        /// The assumes that the text resource 'Message_Info_UnderConstruction' is correctly defined for the current language
        /// </summary>
        protected virtual Task PublishUnderConstructionNotificationAsync()
        {
            return NotificationManager.PublishInfoNotificationAsync(TextSourceCommon.GetText("Message_Info_UnderConstruction"));
        }

        /// <summary>
        /// Publishes a two way input dialog notification. Used to get a string input from the user
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        protected virtual Task<NotificationInputDialogResult> PublishInputDialogNotificationAsync(string message)
        {
            return NotificationManager.PublishAsync<NotificationInputDialogResult>(new NotificationInputDialogMessage(this, message));
        }

        #endregion

        #endregion

        #region Dependency Management

        /// <summary>
        /// List of all the properties that have the DependsOn attribute configured
        /// </summary>
        private readonly Dictionary<string, IList<DependencyInfo>> _propertyDependencies = new Dictionary<string, IList<DependencyInfo>>();

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
        protected bool HasDependencies => _propertyDependencies != null && 
                                          (_propertyDependencies.Count > 0 ||
                                           _notifiableCollectionsPropertyDependencies.Count > 0 ||
                                           _methodDependencies.Count > 0);


        /// <summary>
        /// Maps all the properties that have de DependsOn and/or the PropagateCollectionChange attributes configured
        /// </summary>
        private void InitializePropertyDependencies(Type type)
        {
            foreach (var property in type.GetProperties(true))
            {
                var attributes = property.GetCustomAttributes<DependsOnAttribute>(true);
                var dependsOnAttributes = attributes as DependsOnAttribute[] ?? attributes.ToArray();
                if (dependsOnAttributes.SafeCount() > 0)
                {
                    lock (_propertyDependencies)
                    {
                        foreach (var attribute in dependsOnAttributes)
                        {
                            if (!_propertyDependencies.ContainsKey(attribute.Name))
                                _propertyDependencies.Add(attribute.Name, new List<DependencyInfo>());
                            _propertyDependencies[attribute.Name].Add(new DependencyInfo(property, attribute.IsConditional));
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
                var dependsOnAttributes = attributes as DependsOnAttribute[] ?? attributes.ToArray();
                if (dependsOnAttributes.SafeCount() > 0)
                {
                    foreach (var attribute in dependsOnAttributes)
                    {
                        lock (_methodDependencies)
                        {
                            if (!_methodDependencies.ContainsKey(attribute.Name))
                                _methodDependencies.Add(attribute.Name, new List<MethodInfo>());
                            _methodDependencies[attribute.Name].Add(method);
                        }
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
                _propertyChangedSubscription = this.WeakSubscribe(OnPropertyChanged);

                foreach (var item in _notifiableCollectionsPropertyDependencies)
                {
                    if (item.Value != null)
                        _notifiableCollectionsChangedSubscription[item.Key] = item.Value.WeakSubscribe(OnCollectionChanged);
                }
            }
        }

        /// <summary>
        /// Called when a property raises the <see cref="PropertyChangedEventHandler"/>
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
        /// Updates the collection property dependency subscription.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void UpdateCollectionPropertyValue(string propertyName)
        {
            if (_notifiableCollectionsPropertyDependencies.TryGetValue(propertyName, out var collection))
            {
                var senderCollection = this.SafeGetPropertyValue(propertyName) as INotifyCollectionChanged;
                if (!ReferenceEquals(collection, senderCollection))
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
            foreach (var collection in _notifiableCollectionsPropertyDependencies.Where(nc => ReferenceEquals(sender, nc.Value)).OfType<KeyValuePair<string, INotifyCollectionChanged>?>())
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
            //Prevents the conditional DependsOn from firing, if the execution was made
            //to prevent propagation (ExecuteWithoutConditionalDependsOn)
            if (_dependsOnConditionalCount.ContainsKey(dependencyName) &&
                _dependsOnConditionalCount[dependencyName] > 0)
            {
                return;
            }

            lock (_propertyDependencies)
            {
                if (_propertyDependencies.TryGetValue(dependencyName, out var properties))
                {
                    foreach (var property in properties)
                    {
                        if (!typeof(ICommand).IsAssignableFrom(property.Info.PropertyType))
                        {
                            RaisePropertyChanged(property.Info.Name);
                        }
                        else
                        {
                            var command = property.Info.GetValue(this, null) as MvxCommand;
                            command?.RaiseCanExecuteChanged();
                        }
                    }
                }
            }

            lock (_methodDependencies)
            {
                if (_methodDependencies.TryGetValue(dependencyName, out var methods))
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

        /// <summary>
        /// Executes the action, preventing the propagation of DependsOn
        /// that are marked with the 'IsConditional' flag for the specified property
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="action">The action.</param>
        protected void ExecuteWithoutConditionalDependsOn(string propertyName, Action action)
        {
            InvokeOnMainThread(() =>
            {
                try
                {
                    if (!_dependsOnConditionalCount.ContainsKey(propertyName))
                        _dependsOnConditionalCount.Add(propertyName, 1);
                    else
                        _dependsOnConditionalCount[propertyName]++;

                    action.Invoke();

                    _dependsOnConditionalCount[propertyName] = Math.Max(0, _dependsOnConditionalCount[propertyName] - 1);
                }
                catch
                {
                    _dependsOnConditionalCount.Remove(propertyName);
                }
            });
        }

        #endregion

        #region Text Source

        /// <summary>
        /// Text source for text resources translation
        /// </summary>
        public IMvxLanguageBinder TextSource { get; }

        public IMvxLanguageBinder TextSourceCommon { get; }

        /// <summary>
        /// Changes the text source language.
        /// </summary>
        /// <param name="newLanguage">The new language.</param>
        /// <returns></returns>
        protected virtual bool ChangeTextSourceLanguage(string newLanguage)
        {
            try
            {
                var textBuilder = Mvx.Resolve<IMvxTextProviderBuilder>();

                textBuilder.LoadResources(newLanguage);

                return true;
            }
            catch (Exception ex)
            {
                Log.Trace(ex, "ChangeTextSourceLanguage could not update text provider.");
                return false;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel" /> class.
        /// </summary>
        /// <param name="textSource">The text source.</param>
        /// <param name="textSourceCommon">The text source common.</param>
        /// <param name="jsonConverter">The json converter.</param>
        /// <param name="notificationManager">The notification manager.</param>
        /// <param name="logProvider">The log provider.</param>
        /// <exception cref="System.NullReferenceException">IMvxJsonConverter
        /// or
        /// INotificationService
        /// or
        /// IMvxLanguageBinder</exception>
        protected ViewModel(IMvxLanguageBinder textSource,
                         IMvxLanguageBinder textSourceCommon,
                         IMvxJsonConverter jsonConverter,
                         INotificationService notificationManager,
                         IMvxLogProvider logProvider)
        {
            TextSource = textSource.ThrowIfIoComponentIsNull(nameof(textSource));
            TextSourceCommon = textSourceCommon.ThrowIfIoComponentIsNull(nameof(textSourceCommon));
            JsonConverter = jsonConverter.ThrowIfIoComponentIsNull(nameof(jsonConverter));
            NotificationManager = notificationManager.ThrowIfIoComponentIsNull(nameof(notificationManager));
            LogProvider = logProvider.ThrowIfIoComponentIsNull(nameof(logProvider));

            BackCommand = new MvxCommand(OnBack);

            InitializePropertyDependencies(GetType());

            InitializeMethodDependencies(GetType());

            InitializePropertyChanged();
        }

        #endregion

        #region ViewModel Lifecycle

        public override void ViewCreated()
        {
            SubscribeLongRunningMessageEvents();
         
            base.ViewCreated();
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();

            #pragma warning disable 4014
            DoWorkAsync(OnViewShownAsync, isSilent: true);
            #pragma warning restore 4014
        }
       
        protected virtual async Task OnViewShownAsync()
        {
            if (_initialGenericMessages.Count > 0)
            {
                foreach (var message in _initialGenericMessages)
                {
                    await NotificationManager.PublishAsync(message);
                }
                _initialGenericMessages.Clear();
            }

            if (NotificationManager != null)
            {
                await NotificationManager.PublishPendingNotificationsAsync(this, ViewModelContext);
                await NotificationManager.PublishPendingNotificationsAsync(this);
            }
        }

        public override void ViewDisappeared()
        {
            UnsubscribeMessageEvents();

            base.ViewDisappeared();
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            UnsubscribeLongRunningMessageEvents();

            base.ViewDestroy(viewFinishing);
        }

        private readonly string _longRunningNotificationTokenBaseName = typeof(LongRunningSubscriptionToken).Name;

        /// <summary>
        /// Saves the state to bundle.
        /// </summary>
        /// <param name="bundle">The bundle.</param>
        protected override void SaveStateToBundle(IMvxBundle bundle)
        {
            if (_longRunningMessageTokens.SafeCount() > 0)
            {
                foreach (var token in _longRunningMessageTokens)
                {
                    var serializedInfo = JsonConverter.SerializeObject(new LongRunnigNotificationSaveBundle()
                    {
                        Context = token.Token.Context,
                        MessageType = token.Token.MessageType,
                        MethodName = token.AsyncDeliveryActionName,
                        UnsubscribeOnArrival = token.UnsubscribeOnArrival
                    });
                    bundle.Data.Add(_longRunningNotificationTokenBaseName + "_" + token.Token.Id.ToString(), serializedInfo);
                }
            }

            base.SaveStateToBundle(bundle);
        }

        /// <summary>
        /// Reloads state from bundle.
        /// </summary>
        /// <param name="state">The state.</param>
        protected override void ReloadFromBundle(IMvxBundle state)
        {
            foreach (var item in state.Data)
            {
                if (item.Key.StartsWith(_longRunningNotificationTokenBaseName))
                {
                    var savedInfo = JsonConverter.DeserializeObject<LongRunnigNotificationSaveBundle>(item.Value);
                    var method = GetType().GetRuntimeMethods().SafeFirstOrDefault(m => m.Name == savedInfo.MethodName);
                    Task MethodHandler(INotificationOneWayMessage arg) => (Task)method.Invoke(this, new object[] { arg });

                    SubscribeLongRunningEvent(savedInfo.MessageType,
                                              MethodHandler,
                                              savedInfo.MethodName,
                                              savedInfo.UnsubscribeOnArrival,
                                              savedInfo.Context);
                }
            }

            base.ReloadFromBundle(state);
        }

        #endregion

        #region Generic Methods

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

        /// <summary>
        /// Gets the log instance.
        /// </summary>
        /// <returns></returns>
        protected virtual IMvxLog GetLog()
        {
            return _log ?? (_log = LogProvider.GetLogFor(GetType().Name));
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

        //TODO: Refactor navigation with MvxNavigationService
        #region Navigation

        ///// <summary>
        ///// Shows a view model.
        ///// </summary>
        ///// <typeparam name="TViewModel">The type of the view model.</typeparam>
        ///// <param name="parameterValuesObject">The parameter values object.</param>
        ///// <param name="presentationBundle">The presentation bundle.</param>
        ///// <param name="requestedBy">The requested by.</param>
        ///// <param name="removeCurrentViewFromStack">if set to <c>true</c> removes the current view from application stack.</param>
        ///// <param name="clearBackStack">if set to <c>true</c> clears the entire back stack of the application.</param>
        ///// <returns></returns>
        //protected bool ShowViewModel<TViewModel>(object parameterValuesObject,
        //                                         IMvxBundle presentationBundle = null,
        //                                         MvxRequestedBy requestedBy = null,
        //                                         bool removeCurrentViewFromStack = false,
        //                                         bool clearBackStack = false)
        //    where TViewModel : IMvxViewModel
        //{
        //    return ShowViewModel(
        //        typeof(TViewModel),
        //        parameterValuesObject.ToSimplePropertyDictionary(),
        //        presentationBundle,
        //        requestedBy,
        //        removeCurrentViewFromStack,
        //        clearBackStack);
        //}

        ///// <summary>
        ///// Forces navigation to the specified view model.
        ///// </summary>
        ///// <typeparam name="TViewModel">The type of the view model.</typeparam>
        ///// <param name="parameterValues">The parameter values.</param>
        ///// <param name="presentationBundle">The presentation bundle.</param>
        ///// <param name="requestedBy">The requested by.</param>
        ///// <param name="removeCurrentViewFromStack">if set to <c>true</c> removes the current view from application stack.</param>
        ///// <param name="clearBackStack">if set to <c>true</c> clears the entire back stack of the application.</param>
        ///// <returns></returns>
        //protected bool ShowViewModel<TViewModel>(IDictionary<string, string> parameterValues,
        //                                         IMvxBundle presentationBundle = null,
        //                                         MvxRequestedBy requestedBy = null,
        //                                         bool removeCurrentViewFromStack = false,
        //                                         bool clearBackStack = false)
        //    where TViewModel : IMvxViewModel
        //{
        //    return ShowViewModel(
        //        typeof(TViewModel),
        //        new MvxBundle(parameterValues.ToSimplePropertyDictionary()),
        //        presentationBundle,
        //        requestedBy,
        //        removeCurrentViewFromStack,
        //        clearBackStack);
        //}

        ///// <summary>
        ///// Forces navigation to the specified view model.
        ///// </summary>
        ///// <typeparam name="TViewModel">The type of the view model.</typeparam>
        ///// <param name="parameterBundle">The parameter bundle.</param>
        ///// <param name="presentationBundle">The presentation bundle.</param>
        ///// <param name="requestedBy">The requested by.</param>
        ///// <param name="removeCurrentViewFromStack">if set to <c>true</c> removes the current view from application stack.</param>
        ///// <param name="clearBackStack">if set to <c>true</c> clears the entire back stack of the application.</param>
        ///// <returns></returns>
        //protected bool ShowViewModel<TViewModel>(IMvxBundle parameterBundle = null,
        //                                         IMvxBundle presentationBundle = null,
        //                                         MvxRequestedBy requestedBy = null,
        //                                         bool removeCurrentViewFromStack = false,
        //                                         bool clearBackStack = false)
        //    where TViewModel : IMvxViewModel
        //{
        //    return ShowViewModel(
        //        typeof(TViewModel),
        //        parameterBundle,
        //        presentationBundle,
        //        requestedBy,
        //        removeCurrentViewFromStack,
        //        clearBackStack);
        //}


        ///// <summary>
        ///// Forces navigation to the specified view model.
        ///// </summary>
        ///// <param name="viewModelType">Type of the view model.</param>
        ///// <param name="parameterValuesObject">The parameter values object.</param>
        ///// <param name="presentationBundle">The presentation bundle.</param>
        ///// <param name="requestedBy">The requested by.</param>
        ///// <param name="removeCurrentViewFromStack">if set to <c>true</c> removes the current view from application stack.</param>
        ///// <param name="clearBackStack">if set to <c>true</c> clears the entire back stack of the application.</param>
        ///// <returns></returns>
        //protected bool ShowViewModel(Type viewModelType,
        //                             object parameterValuesObject,
        //                             IMvxBundle presentationBundle = null,
        //                             MvxRequestedBy requestedBy = null,
        //                             bool removeCurrentViewFromStack = false,
        //                             bool clearBackStack = false)
        //{
        //    return ShowViewModel(viewModelType,
        //                         new MvxBundle(parameterValuesObject.ToSimplePropertyDictionary()),
        //                         presentationBundle,
        //                         requestedBy,
        //                         removeCurrentViewFromStack,
        //                         clearBackStack);
        //}

        ///// <summary>
        ///// Forces navigation to the specified view model.
        ///// </summary>
        ///// <param name="viewModelType">Type of the view model.</param>
        ///// <param name="parameterValues">The parameter values.</param>
        ///// <param name="presentationBundle">The presentation bundle.</param>
        ///// <param name="requestedBy">The requested by.</param>
        ///// <param name="removeCurrentViewFromStack">if set to <c>true</c> removes the current view from application stack.</param>
        ///// <param name="clearBackStack">if set to <c>true</c> clears the entire back stack of the application.</param>
        ///// <returns></returns>
        //protected bool ShowViewModel(Type viewModelType,
        //                             IDictionary<string, string> parameterValues,
        //                             IMvxBundle presentationBundle = null,
        //                             MvxRequestedBy requestedBy = null,
        //                             bool removeCurrentViewFromStack = false,
        //                             bool clearBackStack = false)
        //{
        //    return ShowViewModel(viewModelType,
        //                         new MvxBundle(parameterValues),
        //                         presentationBundle,
        //                         requestedBy,
        //                         removeCurrentViewFromStack,
        //                         clearBackStack);
        //}

        ///// <summary>
        ///// Forces navigation to the specified view model.
        ///// </summary>
        ///// <param name="viewModelType">Type of the view model.</param>
        ///// <param name="parameterBundle">The parameter bundle.</param>
        ///// <param name="presentationBundle">The presentation bundle.</param>
        ///// <param name="requestedBy">The requested by.</param>
        ///// <param name="removeCurrentViewFromStack">if set to <c>true</c> removes the current view from application stack.</param>
        ///// <param name="clearBackStack">if set to <c>true</c> clears the entire back stack of the application.</param>
        ///// <returns></returns>
        //protected bool ShowViewModel(Type viewModelType,
        //                             IMvxBundle parameterBundle = null,
        //                             IMvxBundle presentationBundle = null,
        //                             MvxRequestedBy requestedBy = null,
        //                             bool removeCurrentViewFromStack = false,
        //                             bool clearBackStack = false)
        //{
        //    return ShowViewModelImpl(viewModelType, parameterBundle, presentationBundle, requestedBy, removeCurrentViewFromStack, clearBackStack);
        //}


        //internal virtual bool ShowViewModelImpl(Type viewModelType, IMvxBundle parameterBundle, IMvxBundle presentationBundle,
        //                                         MvxRequestedBy requestedBy,
        //                                         bool removeCurrentViewFromStack,
        //                                         bool clearBackStack)
        //{
        //    MvxTrace.Trace("Showing ViewModel {0} - RemoveViewFromStack: {1}", viewModelType.Name, removeCurrentViewFromStack);

        //    var viewDispatcher = ViewDispatcher as IViewDispatcher;
        //    if (viewDispatcher != null)
        //    {
        //        return viewDispatcher.ShowViewModel(new MvxViewModelRequest(
        //                                                viewModelType,
        //                                                parameterBundle,
        //                                                presentationBundle,
        //                                                requestedBy),
        //                                            removeCurrentViewFromStack,
        //                                            clearBackStack);
        //    }

        //    MvxTrace.Error("ShowViewModel -> IViewDispatcher not found. Navigation will be aborted");
        //    return false;
        //}

        #endregion

        #region Back Managment

        /// <summary>
        /// The back command.
        /// </summary>
        public ICommand BackCommand { get; }

        /// <summary>
        /// Called when back command is executed.
        /// </summary>
        protected virtual void OnBack()
        {
            //TODO: USe thew new navigation service
            //Close(this);
        }

        #endregion

        #region Busy Notification Management

        /// <summary>
        /// Indicates if there's work in progress. 
        /// Tipically controled by the 'DoWorkAsync' method
        /// </summary>
        public virtual bool IsBusy => _busyCount > 0;

        /// <summary>
        /// Message to be shown in the busy indicator
        /// </summary>
        public string BusyMessage
        {
            get => _busyMessage;
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
        /// Executes work asynchronously.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="workMessage">The work message.</param>
        /// <param name="isSilent">if set to <c>true</c> [is silent].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">action</exception>
        protected virtual async Task DoWorkAsync(Func<Task> action, string workMessage = null, bool isSilent = false)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));

            StartWork(workMessage, isSilent);

            try
            {
                await action.Invoke();
            }
            catch
            {
                FinishWork(isSilent);
                throw;
            }

            FinishWork(isSilent);
        }

        /// <summary>
        /// Signals the IsBusy to indicate that a new work has started
        /// </summary>
        /// <param name="isSilent">if set to <c>true</c> the IsBusy will no be signaled.</param>
        protected virtual void StartWork(bool isSilent = false)
        {
            if (!isSilent)
            {
                Interlocked.Increment(ref _busyCount);
                RaisePropertyChanged(() => IsBusy);
            }
        }

        /// <summary>
        /// Signals the IsBusy to indicate that a new work has started and sets busy message.
        /// </summary>
        /// <param name="message">The busy message.</param>
        /// <param name="isSilent">if set to <c>true</c> the IsBusy will no be signaled.</param>
        public virtual void StartWork(string message, bool isSilent = false)
        {
            StartWork(isSilent);

            if (!isSilent)
                BusyMessage = message;
        }

        /// <summary>
        /// Signals the IsBusy to indicate that work is finished.
        /// </summary>
        /// <param name="isSilent">if set to <c>true</c> the IsBusy will no be signaled.</param>
        public virtual void FinishWork(bool isSilent = false)
        {
            if (!isSilent)
            {
                Interlocked.Decrement(ref _busyCount);
                RaisePropertyChanged(() => IsBusy);

                if (_busyCount <= 0)
                    BusyMessage = null;

            }
        }

        #endregion
    }

    public abstract class ViewModel<TParameter> : ViewModel, IMvxViewModel<TParameter>
    {
        protected ViewModel(IMvxLanguageBinder textSource, 
                            IMvxLanguageBinder textSourceCommon, 
                            IMvxJsonConverter jsonConverter, 
                            INotificationService notificationManager, 
                            IMvxLogProvider logProvider) 
            : base(textSource, textSourceCommon, jsonConverter, notificationManager, logProvider)
        {
        }

        public abstract void Prepare(TParameter parameter);
    }
}