using MvvmCross;
using MvvmCross.Base;
using MvvmCross.Commands;
using MvvmCross.Localization;
using MvvmCross.Logging;
using MvvmCross.Navigation;
using MvvmCross.Plugin.JsonLocalization;
using MvvmCross.ViewModels;
using MvxExtensions.Core.Extensions;
using MvxExtensions.Core.ViewModels;
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
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvxExtensions.ViewModels
{
    /// <summary>
    /// Base viewmodel, built on top of <see cref="MvxViewModel"/>
    /// </summary>
    public abstract class ViewModel : CoreViewModel
    {
        #region Fields

        private volatile List<NotificationGenericMessage> _initialGenericMessages = new List<NotificationGenericMessage>();

        private volatile IList<SubscriptionToken> _messageTokens = new List<SubscriptionToken>();
        private volatile IList<LongRunningSubscriptionToken> _longRunningMessageTokens = new List<LongRunningSubscriptionToken>();

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

        #endregion

        #region Properties

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
        protected ViewModel(
            IMvxLanguageBinder textSource,
            IMvxLanguageBinder textSourceCommon,
            IMvxJsonConverter jsonConverter,
            INotificationService notificationManager,
            IMvxLogProvider logProvider,
            IMvxNavigationService navigationService) : base(logProvider)
        {
            TextSource = textSource.ThrowIfIoComponentIsNull(nameof(textSource));
            TextSourceCommon = textSourceCommon.ThrowIfIoComponentIsNull(nameof(textSourceCommon));
            JsonConverter = jsonConverter.ThrowIfIoComponentIsNull(nameof(jsonConverter));
            NotificationManager = notificationManager.ThrowIfIoComponentIsNull(nameof(notificationManager));
            NavigationService = navigationService;

            BackCommand = new MvxCommand(OnBack);
        }

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
            Task CastedAsyncDeliveryAction(INotificationOneWayMessage msg) => asyncDeliveryAction((TMessage)msg);
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
                {
                    await asyncDeliveryAction.Invoke(msg as INotificationOneWayMessage).ConfigureAwait(false);
                }

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
                var textBuilder = Mvx.IoCProvider.Resolve<IMvxTextProviderBuilder>();

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

        #region ViewModel Lifecycle

        public override void ViewCreated()
        {
            SubscribeLongRunningMessageEvents();

            base.ViewCreated();
        }

        public override async void ViewAppeared()
        {
            SubscribeLongRunningMessageEvents();

            base.ViewAppeared();

#pragma warning disable 4014
            await DoWorkAsync(OnViewShownAsync, isSilent: true).ConfigureAwait(false);
#pragma warning restore 4014
        }

        protected virtual async Task OnViewShownAsync()
        {
            if (_initialGenericMessages.Count > 0)
            {
                foreach (var message in _initialGenericMessages)
                {
                    await NotificationManager.PublishAsync(message).ConfigureAwait(false);
                }
                _initialGenericMessages.Clear();
            }

            if (NotificationManager != null)
            {
                await NotificationManager.PublishPendingNotificationsAsync(this, ViewModelContext).ConfigureAwait(false);
                await NotificationManager.PublishPendingNotificationsAsync(this).ConfigureAwait(false);
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

        #endregion

        #region Navigation

        protected IMvxNavigationService NavigationService { get; }

        /// <summary>
        /// The back command.
        /// </summary>
        public ICommand BackCommand { get; }

        /// <summary>
        /// Called when back command is executed.
        /// </summary>
        protected virtual void OnBack()
        {
            NavigationService.Close(this);
        }

        #endregion
    }
}