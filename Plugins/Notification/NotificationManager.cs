using MvxExtensions.Plugins.Notification.Core;
using MvxExtensions.Plugins.Notification.Core.Async.Subscriptions;
using MvxExtensions.Plugins.Notification.Core.Async.Subscriptions.OneWay;
using MvxExtensions.Plugins.Notification.Core.Async.Subscriptions.TwoWay;
using MvxExtensions.Plugins.Notification.Core.Async.ThreadRunners;
using MvxExtensions.Plugins.Notification.Messages;
using MvxExtensions.Plugins.Notification.Messages.OneWay;
using MvxExtensions.Plugins.Notification.Messages.TwoWay.Question;
using MvxExtensions.Plugins.Notification.Messages.TwoWay.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MvvmCross.Logging;

namespace MvxExtensions.Plugins.Notification
{
    /// <summary>
    /// Notification service plugin
    /// </summary>
    public class NotificationManager : INotificationService
    {
        #region Fields

        private SemaphoreSlim _purgeSemaphore = new SemaphoreSlim(1, 1);

        private volatile Dictionary<Type, List<ISubscription>> _oneWaySubscriptions = new Dictionary<Type, List<ISubscription>>();
        private volatile Dictionary<Type, Dictionary<string, ISubscription>> _twoWaySubscriptions = new Dictionary<Type, Dictionary<string, ISubscription>>();

        private volatile Dictionary<string, List<INotificationMessage>> _pendingNotifications = new Dictionary<string, List<INotificationMessage>>();

        #endregion

        #region Subscribe/Unsubscribe

        #region OneWay Subscription

        /// <summary>
        /// Subscribes an one-way notification with the default context and with a weak reference
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <returns></returns>
        public SubscriptionToken Subscribe<TMessage>(Func<TMessage, Task> asyncDeliveryAction)
            where TMessage : INotificationMessage
        {
            return Subscribe(asyncDeliveryAction, AsyncSubscription.DefaultContext);
        }

        /// <summary>
        /// Subscribes an one-way notification with a weak reference
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public SubscriptionToken Subscribe<TMessage>(Func<TMessage, Task> asyncDeliveryAction, string context)
            where TMessage : INotificationMessage
        {
            return Subscribe(asyncDeliveryAction, context, SubscriptionReferenceTypeEnum.Weak);
        }

        /// <summary>
        /// Subscribes an one-way notification
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        /// <param name="subscriptionReferenceType">Type of the subscription reference.</param>
        /// <returns></returns>
        public SubscriptionToken Subscribe<TMessage>(Func<TMessage, Task> asyncDeliveryAction,
                                                     string context,
                                                     SubscriptionReferenceTypeEnum subscriptionReferenceType)
            where TMessage : INotificationMessage
        {
            return CommonSubscribe(asyncDeliveryAction, new AsyncActionRunner(), context, subscriptionReferenceType);
        }

        private SubscriptionToken CommonSubscribe<TMessage>(Func<TMessage, Task> asyncDeliveryAction,
                                                            IAsyncActionRunner asyncActionRunner,
                                                            string context,
                                                            SubscriptionReferenceTypeEnum subscriptionReferenceType)
           where TMessage : INotificationMessage
        {
            if (asyncDeliveryAction == null)
            {
                throw new ArgumentNullException("asyncDeliveryAction");
            }

            AsyncSubscription asyncSubscription;

            switch (subscriptionReferenceType)
            {
                case SubscriptionReferenceTypeEnum.Strong:
                    asyncSubscription = new OneWayAsyncStrongSubscription<TMessage>(asyncActionRunner, asyncDeliveryAction, context);
                    break;
                case SubscriptionReferenceTypeEnum.Weak:
                    asyncSubscription = new OneWayAsyncWeakSubscription<TMessage>(asyncActionRunner, asyncDeliveryAction, context);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(subscriptionReferenceType), "Subscription Reference Type unexpected: " + subscriptionReferenceType);
            }

            var messageType = typeof(TMessage);

            lock (this)
            {
                if (_oneWaySubscriptions.ContainsKey(messageType))
                {
                    MvxPluginLog.Instance.Trace($"Adding oneway subscription (MessageType: '{messageType.Name}' Context: '{asyncSubscription.Context}' SubscriptionId: '{asyncSubscription.Id}')");
                    _oneWaySubscriptions[messageType].Add(asyncSubscription);
                }
                else
                {
                    MvxPluginLog.Instance.Trace($"Adding new message type with oneway subscription (MessageType: {messageType.Name} Context: {asyncSubscription.Context} SubscriptionId: {asyncSubscription.Id})");
                    _oneWaySubscriptions.Add(messageType, new List<ISubscription>() { asyncSubscription });
                }
            }

            return new SubscriptionToken(messageType, context, asyncSubscription.Id, asyncDeliveryAction);
        }



        /// <summary>
        /// Subscribes an one-way notification with the default context and with a weak reference
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <returns></returns>
        public SubscriptionToken Subscribe(Type messageType, Func<INotificationMessage, Task> asyncDeliveryAction)
        {
            return Subscribe(messageType, asyncDeliveryAction, AsyncSubscription.DefaultContext);
        }

        /// <summary>
        /// Subscribes an one-way notification with a weak reference
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public SubscriptionToken Subscribe(Type messageType, Func<INotificationMessage, Task> asyncDeliveryAction, string context)
        {
            return Subscribe(messageType, asyncDeliveryAction, context, SubscriptionReferenceTypeEnum.Weak);
        }

        /// <summary>
        /// Subscribes an one-way notification
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        /// <param name="subscriptionReferenceType">Type of the subscription reference.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public SubscriptionToken Subscribe(Type messageType, Func<INotificationMessage, Task> asyncDeliveryAction, string context, SubscriptionReferenceTypeEnum subscriptionReferenceType)
        {
            return CommonSubscribe(messageType, asyncDeliveryAction, new AsyncActionRunner(), context, subscriptionReferenceType);
        }

        private SubscriptionToken CommonSubscribe(Type messageType,
                                                  Func<INotificationMessage, Task> asyncDeliveryAction,
                                                  IAsyncActionRunner asyncActionRunner,
                                                  string context,
                                                  SubscriptionReferenceTypeEnum subscriptionReferenceType)
        {
            if (asyncDeliveryAction == null)
                throw new ArgumentNullException("asyncDeliveryAction");

            AsyncSubscription asyncSubscription;

            switch (subscriptionReferenceType)
            {
                case SubscriptionReferenceTypeEnum.Strong:
                    asyncSubscription = new OneWayAsyncStrongSubscription(asyncActionRunner, asyncDeliveryAction, context);
                    break;
                case SubscriptionReferenceTypeEnum.Weak:
                    asyncSubscription = new OneWayAsyncWeakSubscription(asyncActionRunner, asyncDeliveryAction, context);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(subscriptionReferenceType), "Subscription Reference Type unexpected: " + subscriptionReferenceType);
            }

            lock (this)
            {
                if (_oneWaySubscriptions.ContainsKey(messageType))
                {
                    MvxPluginLog.Instance.Trace($"Adding oneway subscription (MessageType: {messageType.Name} Context: {asyncSubscription.Context} SubscriptionId: {asyncSubscription.Id})");
                    _oneWaySubscriptions[messageType].Add(asyncSubscription);
                }
                else
                {
                    MvxPluginLog.Instance.Trace($"Adding new message type with oneway subscription (MessageType: {messageType.Name} Context: {asyncSubscription.Context} SubscriptionId: {asyncSubscription.Id})");
                    _oneWaySubscriptions.Add(messageType, new List<ISubscription>() { asyncSubscription });
                }
            }

            return new SubscriptionToken(messageType, context, asyncSubscription.Id, asyncDeliveryAction);
        }

        #endregion

        #region TwoWay Subscription

        /// <summary>
        /// Subscribes a two-way notification with the default context and with a weak reference
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <returns></returns>
        public SubscriptionToken Subscribe<TMessage, TResult>(Func<TMessage, Task<TResult>> asyncDeliveryAction)
            where TMessage : INotificationMessage
            where TResult : INotificationResult
        {
            return Subscribe<TMessage, TResult>(asyncDeliveryAction, AsyncSubscription.DefaultContext);
        }

        /// <summary>
        /// Subscribes a two-way notification with a weak reference
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public SubscriptionToken Subscribe<TMessage, TResult>(Func<TMessage, Task<TResult>> asyncDeliveryAction, string context)
            where TMessage : INotificationMessage
            where TResult : INotificationResult
        {
            return Subscribe<TMessage, TResult>(asyncDeliveryAction, context, SubscriptionReferenceTypeEnum.Weak);
        }

        /// <summary>
        /// Subscribes a two-way notification
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        /// <param name="subscriptionReferenceType">Type of the subscription reference.</param>
        /// <returns></returns>
        public SubscriptionToken Subscribe<TMessage, TResult>(Func<TMessage, Task<TResult>> asyncDeliveryAction, string context, SubscriptionReferenceTypeEnum subscriptionReferenceType)
            where TMessage : INotificationMessage
            where TResult : INotificationResult
        {
            return CommonSubscribe<TMessage, TResult>(asyncDeliveryAction, new AsyncActionRunner<TResult>(), subscriptionReferenceType, context);
        }

        /// <summary>
        /// Commons the subscribe.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="asyncActionRunner">The asynchronous action runner.</param>
        /// <param name="subscriptionReferenceType">Type of the subscription reference.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">asyncDeliveryAction</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">reference;SubscriptionTypeEnum type unexpected  + reference</exception>
        private SubscriptionToken CommonSubscribe<TMessage, TResult>(Func<TMessage, Task<TResult>> asyncDeliveryAction, IAsyncActionRunner<TResult> asyncActionRunner, SubscriptionReferenceTypeEnum subscriptionReferenceType, string context)
            where TMessage : INotificationMessage
            where TResult : INotificationResult
        {
            if (asyncDeliveryAction == null)
            {
                throw new ArgumentNullException("asyncDeliveryAction");
            }

            AsyncSubscription<TResult> asyncSubscription;

            switch (subscriptionReferenceType)
            {
                case SubscriptionReferenceTypeEnum.Strong:
                    asyncSubscription = new TwoWayAsyncStrongSubscription<TMessage, TResult>(asyncActionRunner, asyncDeliveryAction, context);
                    break;
                case SubscriptionReferenceTypeEnum.Weak:
                    asyncSubscription = new TwoWayAsyncWeakSubscription<TMessage, TResult>(asyncActionRunner, asyncDeliveryAction, context);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(subscriptionReferenceType), "Subscription Reference Type unexpected " + subscriptionReferenceType);
            }

            var messageType = typeof(TMessage);

            lock (this)
            {
                if (_twoWaySubscriptions.ContainsKey(messageType))
                {
                    if (_twoWaySubscriptions[messageType].ContainsKey(context))
                    {
                        MvxPluginLog.Instance.Trace($"Replacing twoway subscription for specified context (MessageType: {messageType.Name} Context: {asyncSubscription.Context} SubscriptionId: {asyncSubscription.Id})");
                        _twoWaySubscriptions[messageType][context] = asyncSubscription;
                    }
                    else
                    {
                        MvxPluginLog.Instance.Trace($"Adding twoway subscription for specified context (MessageType: {messageType.Name} Context: {asyncSubscription.Context} SubscriptionId: {asyncSubscription.Id})");
                        _twoWaySubscriptions[messageType].Add(context, asyncSubscription);
                    }
                }
                else
                {
                    MvxPluginLog.Instance.Trace($"Adding new message type with twoway subscription for specified context (MessageType: {messageType.Name} Context: {asyncSubscription.Context} SubscriptionId: {asyncSubscription.Id})");
                    _twoWaySubscriptions.Add(messageType, new Dictionary<string, ISubscription>());
                    _twoWaySubscriptions[messageType].Add(context, asyncSubscription);
                }
            }

            return new SubscriptionToken(messageType, context, asyncSubscription.Id, asyncDeliveryAction);
        }

        #endregion

        #region Unsubscription

        /// <summary>
        /// Unsubscribesthe message with the specified token
        /// </summary>
        /// <param name="subscriptionToken">The subscription token.</param>
        public void Unsubscribe(SubscriptionToken subscriptionToken)
        {
            if (subscriptionToken != null)
            {
                lock (this)
                {
                    List<ISubscription> oneWaySubscriptions;
                    if (_oneWaySubscriptions.TryGetValue(subscriptionToken.MessageType, out oneWaySubscriptions))
                    {
                        var subscriptionToRemove = oneWaySubscriptions.FirstOrDefault(s => s.Id.Equals(subscriptionToken.Id));
                        if (subscriptionToRemove != null)
                        {
                            MvxPluginLog.Instance.Trace($"Removing oneway subscription (MessageType: {subscriptionToken.MessageType} Context: {subscriptionToken.Context} SubscriptionId: {subscriptionToken.Id})");
                            _oneWaySubscriptions[subscriptionToken.MessageType].Remove(subscriptionToRemove);
                        }
                    }


                    Dictionary<string, ISubscription> twoWaySubscriptions;
                    if (_twoWaySubscriptions.TryGetValue(subscriptionToken.MessageType, out twoWaySubscriptions))
                    {
                        if (twoWaySubscriptions != null &&
                            twoWaySubscriptions.ContainsKey(subscriptionToken.Context) &&
                            twoWaySubscriptions[subscriptionToken.Context] != null &&
                            twoWaySubscriptions[subscriptionToken.Context].Id == subscriptionToken.Id)
                        {
                            MvxPluginLog.Instance.Trace($"Removing twoway subscription (MessageType: {subscriptionToken.MessageType} Context: {subscriptionToken.Context} SubscriptionId: {subscriptionToken.Id})");
                            _twoWaySubscriptions[subscriptionToken.MessageType].Remove(subscriptionToken.Context);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Unsubscribe all messages from a particular message type.
        /// </summary>
        /// <typeparam name="TMessage">Type of message</typeparam>
        public void Unsubscribe<TMessage>() where TMessage : INotificationMessage
        {
            Unsubscribe(typeof(TMessage));
        }

        /// <summary>
        /// Unsubscribe all messages from a particular message type
        /// </summary>
        /// <param name="messageType">Type of message.</param>
        public void Unsubscribe(Type messageType)
        {
            lock (this)
            {
                List<ISubscription> oneWaySubscriptions;
                if (_oneWaySubscriptions.TryGetValue(messageType, out oneWaySubscriptions))
                {
                    foreach (var subscription in oneWaySubscriptions)
                    {
                        MvxPluginLog.Instance.Trace($"Removing oneway subscription (MessageType: {messageType} Context: {subscription.Context} SubscriptionId: {subscription.Id})");
                        oneWaySubscriptions.Remove(subscription);
                    }
                }


                if (_twoWaySubscriptions.ContainsKey(messageType))
                {
                    MvxPluginLog.Instance.Trace($"Removing all twoway subscriptions for type '{messageType}'");
                    _twoWaySubscriptions.Remove(messageType);
                }
            }
        }

        #endregion

        #endregion

        #region Publish

        #region OneWay publish

        /// <summary>
        /// Publish an one-way message
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="context">The context of the message.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">message</exception>
        public async Task PublishAsync(INotificationMessage message, string context = AsyncSubscription.DefaultContext)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            var messageType = message.GetType();
            if (messageType == typeof(NotificationMessage))
            {
                MvxPluginLog.Instance.Warn("NotificationMessage publishing not allowed - this normally suggests non-specific generic used in calling code");
                return;
            }

            var oneWaySubscriptions = GetSubscriptionsFor(messageType, context, SubscriptionDirectionEnum.OneWay);
            if (oneWaySubscriptions == null)
            {
                MvxPluginLog.Instance.Trace($"Nothing registered for one-way messages of type '{messageType.Name}' with context '{context}'");
                return;
            }

            var allSucceeded = true;

            foreach (var subscription in oneWaySubscriptions)
            {
                var voidAsyncSubscription = subscription as OneWayAsyncSubscription;
                if (voidAsyncSubscription == null)
                {
                    MvxPluginLog.Instance.Warn($"Found one-way subscription that is not a OneWayAsyncSubscription! ({messageType.Name})");
                }
                else
                    allSucceeded &= await voidAsyncSubscription.InvokeAsync(message);
            }

            if (!allSucceeded)
            {
                MvxPluginLog.Instance.Trace("One or more oneway listeners failed - purge scheduled");
                await SchedulePurgeAsync(SubscriptionDirectionEnum.OneWay, messageType);
            }
        }

        /// <summary>
        /// Adds a message to the pending async notification queue
        /// </summary>
        /// <param name="message">The message to store.</param>
        /// <param name="context">The context of the message.</param>
        /// <param name="tryNormalPublish">if set to <c>true</c> tries to do normal publish, before storing the notification for delayed publish.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">message</exception>
        public async Task DelayedPublishAsync(INotificationMessage message, string context = AsyncSubscription.DefaultContext, bool tryNormalPublish = false)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            var messageType = message.GetType();
            if (messageType == typeof(NotificationMessage))
            {
                MvxPluginLog.Instance.Warn("NotificationMessage publishing not allowed - this normally suggests non-specific generic used in calling code");
                return;
            }

            if (tryNormalPublish)
            {
                if (HasSubscriptionsForContext(messageType, context, SubscriptionDirectionEnum.OneWay))
                {
                    await PublishAsync(message, context);
                    return;
                }
                else
                    MvxPluginLog.Instance.Warn(string.Format("Failed to normal publish the one-way '{0}' message for context '{1}'. Adding it to delayed publish...", messageType.FullName, context));
            }

            AddToPendingNotificationList(message, context);
        }

        /// <summary>
        /// Publishes the pending notifications for a particular context
        /// and makes sure that they're not published to the sender thata added them to the pending list
        /// </summary>
        /// <param name="currentPublisher">The current publisher that is requesting to publish de pending messages.</param>
        /// <param name="context">The context of the pending messages.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public async Task PublishPendingNotificationsAsync(object currentPublisher, string context = AsyncSubscription.DefaultContext)
        {
            if (string.IsNullOrEmpty(context))
            {
                MvxPluginLog.Instance.Warn("PublishPendingNotificationsFor - Context is null or empty. Assuming default");
                context = AsyncSubscription.DefaultContext;
            }

            if (_pendingNotifications.ContainsKey(context) && _pendingNotifications[context].Count() > 0)
            {
                var msgsCount = _pendingNotifications[context].Where(m => m.Sender != currentPublisher).Count();
                if (msgsCount > 0)
                {
                    MvxPluginLog.Instance.Trace($"Found {msgsCount} pending one-way message(s) with context '{context}' available for current publisher ({currentPublisher})");

                    foreach (var msg in _pendingNotifications[context].ToArray())
                    {
                        if (HasSubscriptionsForContext(msg.GetType(), context, SubscriptionDirectionEnum.OneWay))
                        {
                            await PublishAsync(msg, context);
                            _pendingNotifications[context].Remove(msg);
                        }
                    }
                }
                else
                {
                    MvxPluginLog.Instance.Trace($"Found {_pendingNotifications[context].Count()} one-way pending message(s) with context '{context}' but NONE available for current publisher ({currentPublisher})");
                }
            }
            else
            {
                MvxPluginLog.Instance.Trace($"No pending one-way messages found with context {context}");
                return;
            }
        }


        #region Info Notification

        /// <summary>
        /// Publishes an one-way information message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task PublishInfoNotificationAsync(string message, NotificationModeEnum mode = NotificationModeEnum.Default, string context = AsyncSubscription.DefaultContext)
        {
            return PublishAsync(new NotificationGenericMessage(this, message, mode, NotificationSeverityEnum.Info), context);
        }

        /// <summary>
        /// Adds an one-way information message to the pending notification queue
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task DelayedPublishInfoNotificationAsync(string message, NotificationModeEnum mode = NotificationModeEnum.Default, string context = AsyncSubscription.DefaultContext)
        {
            return DelayedPublishAsync(new NotificationGenericMessage(this, message, mode, NotificationSeverityEnum.Info), context);
        }

        #endregion

        #region Warning Notifications

        /// <summary>
        /// Publishes an one-way warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task PublishWarningNotificationAsync(string message, NotificationModeEnum mode = NotificationModeEnum.Default, string context = AsyncSubscription.DefaultContext)
        {
            return PublishAsync(new NotificationGenericMessage(this, message, mode, NotificationSeverityEnum.Warning), context);
        }

        /// <summary>
        /// Adds an one-way warning message to the pending notification queue
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task DelayedPublishWarningNotificationAsync(string message, NotificationModeEnum mode = NotificationModeEnum.Default, string context = AsyncSubscription.DefaultContext)
        {
            return DelayedPublishAsync(new NotificationGenericMessage(this, message, mode, NotificationSeverityEnum.Warning), context);
        }

        #endregion

        #region Error Notifications

        /// <summary>
        /// Publishes an one-way error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task PublishErrorNotificationAsync(string message, NotificationModeEnum mode = NotificationModeEnum.Default, string context = AsyncSubscription.DefaultContext)
        {
            return PublishAsync(new NotificationGenericMessage(this, message, mode, NotificationSeverityEnum.Error), context);
        }

        /// <summary>
        /// Adds an one-way error message to the pending notification queue
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task DelayedPublishErrorNotificationAsync(string message, NotificationModeEnum mode = NotificationModeEnum.Default, string context = AsyncSubscription.DefaultContext)
        {
            return DelayedPublishAsync(new NotificationGenericMessage(this, message, mode, NotificationSeverityEnum.Error), context);
        }

        #endregion

        #region Success Notifications

        /// <summary>
        /// Publishes an one-way success message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task PublishSuccessNotificationAsync(string message, NotificationModeEnum mode = NotificationModeEnum.Default, string context = AsyncSubscription.DefaultContext)
        {
            return PublishAsync(new NotificationGenericMessage(this, message, mode, NotificationSeverityEnum.Success), context);
        }

        /// <summary>
        /// Adds an one-way error message to the pending notification queue
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task DelayedPublishSuccessNotificationAsync(string message, NotificationModeEnum mode = NotificationModeEnum.Default, string context = AsyncSubscription.DefaultContext)
        {
            return DelayedPublishAsync(new NotificationGenericMessage(this, message, mode, NotificationSeverityEnum.Success), context);
        }

        #endregion

        #endregion

        #region TwoWay publish

        /// <summary>
        /// Publishes a two-way message
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">message</exception>
        public async Task<TResult> PublishAsync<TResult>(INotificationMessage message, string context = AsyncSubscription.DefaultContext)
            where TResult : INotificationResult
        {
            if (message == null)
                throw new ArgumentNullException("message");

            var messageType = message.GetType();
            if (messageType == typeof(NotificationMessage))
            {
                MvxPluginLog.Instance.Warn("NotificationMessage publishing not allowed - this normally suggests non-specific generic used in calling code");
                return default(TResult);
            }

            var twoWaySubscriptions = GetSubscriptionsFor(messageType, context, SubscriptionDirectionEnum.TwoWay);
            if (twoWaySubscriptions == null)
            {
                MvxPluginLog.Instance.Trace($"Nothing registered for two-way async messages of type '{messageType.Name}' with context '{context}'");
                return default(TResult);
            }

            if (twoWaySubscriptions.Count() > 1)
                MvxPluginLog.Instance.Trace($"Found {twoWaySubscriptions.Count()} two-way messages for type '{messageType}' and context '{context}'. This should not happen. Going to use the first one...");

            var twoWaySubscription = twoWaySubscriptions.FirstOrDefault() as TwoWayAsyncSubscription<TResult>;
            if (twoWaySubscription == null)
            {
                MvxPluginLog.Instance.Warn($"Found a two-way subscription that is not a TwoWayAsyncSubscription! (MessageType: {messageType.Name} Context: {context})");
                return default(TResult);
            }

            if (twoWaySubscription.IsAlive)
                return await twoWaySubscription.InvokeAsync(message);
            else
            {
                MvxPluginLog.Instance.Warn($"Two-way subscription is dead. Going to pruge it (MessageType: '{messageType.Name}' Context: '{context}' SubscriptionId: '{twoWaySubscription.Id}')");
                await RequestPurgeAsync(messageType, SubscriptionDirectionEnum.TwoWay);
                return default(TResult);
            }
        }


        /// <summary>
        /// Publishes a two-way generic question message.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <param name="possibleAnswers">The possible answers.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<NotificationGenericQuestionResult> PublishGenericQuestionNotificationAsync(string question, NotificationTwoWayAnswersGroupEnum possibleAnswers, string context = AsyncSubscription.DefaultContext)
        {
            return PublishAsync<NotificationGenericQuestionResult>(new NotificationGenericQuestionMessage(this, question, possibleAnswers), context);
        }

        /// <summary>
        /// Publishes a two-way question with custom answer message.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <param name="possibleAnswers">The possible answers.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Task<NotificationQuestionCustomAnswerResult> PublishQuestionWithCustomAnswerNotificationAsync(string question, IList<string> possibleAnswers, string context = AsyncSubscription.DefaultContext)
        {
            return PublishAsync<NotificationQuestionCustomAnswerResult>(new NotificationQuestionWithCustomAnswerMessage(this, question, possibleAnswers), context);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Adds a message to the pending notification list, for the specified context.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="context">The context.</param>
        private void AddToPendingNotificationList(INotificationMessage message, string context)
        {
            var messageType = message.GetType();
            if (string.IsNullOrEmpty(context))
            {
                MvxPluginLog.Instance.Warn($"AddToPendingNotificationList - Context of message type '{messageType.Name}' is null or empty. Assuming default");
                context = AsyncSubscription.DefaultContext;
            }

            lock (this)
            {
                if (!_pendingNotifications.ContainsKey(context))
                    _pendingNotifications.Add(context, new List<INotificationMessage>());

                MvxPluginLog.Instance.Trace($"AddToPendingAsyncNotificationList - Adding one-way message of type '{messageType.Name}' with context '{context}' to pending async notification list");

                _pendingNotifications[context].Add(message);
            }
        }

        /// <summary>
        /// Gets the subscriptions for the specified messageType with the specified context
        /// for the specified direction.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="context">The context.</param>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns>
        /// List of subscriptions
        /// </returns>
        private IEnumerable<ISubscription> GetSubscriptionsFor(Type messageType, string context, SubscriptionDirectionEnum subscriptionDirection)
        {
            IEnumerable<ISubscription> messageSubscriptions = null;

            switch (subscriptionDirection)
            {
                case SubscriptionDirectionEnum.OneWay:
                    lock (this)
                    {
                        List<ISubscription> oneWayTypedSubscriptions;
                        if (_oneWaySubscriptions.TryGetValue(messageType, out oneWayTypedSubscriptions))
                            messageSubscriptions = oneWayTypedSubscriptions.Where(s => s.Context == context).ToArray();
                    }
                    break;

                case SubscriptionDirectionEnum.TwoWay:
                    lock (this)
                    {
                        Dictionary<string, ISubscription> twoWayTypedSubscriptions;
                        if (_twoWaySubscriptions.TryGetValue(messageType, out twoWayTypedSubscriptions))
                            if (twoWayTypedSubscriptions.ContainsKey(context) && twoWayTypedSubscriptions[context] != null)
                                messageSubscriptions = new List<ISubscription>() { twoWayTypedSubscriptions[context] }.ToArray();
                    }
                    break;
            }

            return messageSubscriptions;
        }



        /// <summary>
        /// Checks if there are subscriptions for TMessage,
        /// for a specific subscription direction
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool HasSubscriptionsFor<TMessage>(SubscriptionDirectionEnum subscriptionDirection) where TMessage : INotificationMessage
        {
            return HasSubscriptionsFor(typeof(TMessage), subscriptionDirection);
        }

        /// <summary>
        /// Checks if there are subscriptions for messageType,
        /// for a specific subscription direction
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool HasSubscriptionsFor(Type messageType, SubscriptionDirectionEnum subscriptionDirection)
        {
            lock (this)
            {
                switch (subscriptionDirection)
                {
                    case SubscriptionDirectionEnum.OneWay:
                        List<ISubscription> oneWayMessageSubscriptions = null;
                        if (_oneWaySubscriptions.TryGetValue(messageType, out oneWayMessageSubscriptions))
                            return oneWayMessageSubscriptions != null && oneWayMessageSubscriptions.Any();
                        break;

                    case SubscriptionDirectionEnum.TwoWay:
                        Dictionary<string, ISubscription> twoWayMessageSubscriptions = null;
                        if (_twoWaySubscriptions.TryGetValue(messageType, out twoWayMessageSubscriptions))
                            return twoWayMessageSubscriptions != null && twoWayMessageSubscriptions.Any();
                        break;
                }

                return false;
            }
        }

        /// <summary>
        /// Counts the number of subscriptions for TMessage
        /// for a specific subscription direction
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public int CountSubscriptionsFor<TMessage>(SubscriptionDirectionEnum subscriptionDirection) where TMessage : INotificationMessage
        {
            return CountSubscriptionsFor(typeof(TMessage), subscriptionDirection);
        }

        /// <summary>
        /// Counts the number of subscriptions for messageType,
        /// for a specific subscription direction
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public int CountSubscriptionsFor(Type messageType, SubscriptionDirectionEnum subscriptionDirection)
        {
            lock (this)
            {
                switch (subscriptionDirection)
                {
                    case SubscriptionDirectionEnum.OneWay:
                        List<ISubscription> oneWayMessageSubscriptions = null;
                        if (_oneWaySubscriptions.TryGetValue(messageType, out oneWayMessageSubscriptions) && oneWayMessageSubscriptions != null)
                            return oneWayMessageSubscriptions.Count;
                        break;

                    case SubscriptionDirectionEnum.TwoWay:
                        Dictionary<string, ISubscription> twoWayMessageSubscriptions = null;
                        if (_twoWaySubscriptions.TryGetValue(messageType, out twoWayMessageSubscriptions) && twoWayMessageSubscriptions != null)
                            return twoWayMessageSubscriptions.Count;
                        break;
                }

                return 0;
            }
        }

        /// <summary>
        /// Check if there're subscriptions of TMessage for the specified context,
        /// for a specific subscription direction
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="context">Context of the subscription</param>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool HasSubscriptionsForContext<TMessage>(string context, SubscriptionDirectionEnum subscriptionDirection) where TMessage : INotificationMessage
        {
            return HasSubscriptionsForContext(typeof(TMessage), context, subscriptionDirection);
        }

        /// <summary>
        /// Check if there're subscriptions of messageType for the specified context,
        /// for a specific subscription direction
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="context">The context.</param>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool HasSubscriptionsForContext(Type messageType, string context, SubscriptionDirectionEnum subscriptionDirection)
        {
            lock (this)
            {
                switch (subscriptionDirection)
                {
                    case SubscriptionDirectionEnum.OneWay:
                        List<ISubscription> oneWayMessageSubscriptions = null;
                        if (_oneWaySubscriptions.TryGetValue(messageType, out oneWayMessageSubscriptions))
                            return oneWayMessageSubscriptions != null && oneWayMessageSubscriptions.Any(m => m.Context == context);
                        break;

                    case SubscriptionDirectionEnum.TwoWay:
                        Dictionary<string, ISubscription> twoWayMessageSubscriptions = null;
                        if (_twoWaySubscriptions.TryGetValue(messageType, out twoWayMessageSubscriptions))
                            return twoWayMessageSubscriptions != null && twoWayMessageSubscriptions.Any(m => m.Key == context);
                        break;
                }

                return false;
            }
        }

        /// <summary>
        /// Count the number of subscriptions of TMessage for the specified context,
        /// for a specific subscription direction
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns>
        /// Count of subscriptions
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public int CountSubscriptionsForContext<TMessage>(string context, SubscriptionDirectionEnum subscriptionDirection) where TMessage : INotificationMessage
        {
            return CountSubscriptionsForContext(typeof(TMessage), context, subscriptionDirection);
        }

        /// <summary>
        /// Counts the subscriptions of messageType for the specified context,
        /// for a specific subscription direction
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="context">The context.</param>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public int CountSubscriptionsForContext(Type messageType, string context, SubscriptionDirectionEnum subscriptionDirection)
        {
            lock (this)
            {
                switch (subscriptionDirection)
                {
                    case SubscriptionDirectionEnum.OneWay:
                        List<ISubscription> oneWayMessageSubscriptions = null;
                        if (_oneWaySubscriptions.TryGetValue(messageType, out oneWayMessageSubscriptions) && oneWayMessageSubscriptions != null)
                            return oneWayMessageSubscriptions.Count(m => m.Context == context);
                        break;

                    case SubscriptionDirectionEnum.TwoWay:
                        Dictionary<string, ISubscription> twoWayMessageSubscriptions = null;
                        if (_twoWaySubscriptions.TryGetValue(messageType, out twoWayMessageSubscriptions) && twoWayMessageSubscriptions != null)
                            return twoWayMessageSubscriptions.Count(m => m.Key == context);
                        break;
                }

                return 0;
            }
        }

        /// <summary>
        /// Get all the distinct context of subscriptions for TMessage,
        /// for a specific subscription direction
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerable<string> GetSubscriptionsContextFor<TMessage>(SubscriptionDirectionEnum subscriptionDirection) where TMessage : INotificationMessage
        {
            return GetSubscriptionsContextFor(typeof(TMessage), subscriptionDirection);
        }

        /// <summary>
        /// Get all the distinct context of subscriptions for messageType,
        /// for a specific subscription direction
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IEnumerable<string> GetSubscriptionsContextFor(Type messageType, SubscriptionDirectionEnum subscriptionDirection)
        {
            lock (this)
            {
                switch (subscriptionDirection)
                {
                    case SubscriptionDirectionEnum.OneWay:
                        List<ISubscription> oneWayMessageSubscriptions = null;
                        if (_oneWaySubscriptions.TryGetValue(messageType, out oneWayMessageSubscriptions) && oneWayMessageSubscriptions != null)
                            return oneWayMessageSubscriptions.Select(x => x.Context).Distinct().ToArray();
                        break;

                    case SubscriptionDirectionEnum.TwoWay:
                        Dictionary<string, ISubscription> twoWayMessageSubscriptions = null;
                        if (_twoWaySubscriptions.TryGetValue(messageType, out twoWayMessageSubscriptions) && twoWayMessageSubscriptions != null)
                            return twoWayMessageSubscriptions.Select(x => x.Key).Distinct().ToArray();
                        break;
                }

                return new List<string>(0);
            }
        }

        #endregion

        #region Message purging

        /// <summary>
        /// Schedules a check on all subscribers for the specified messageType,
        /// for a specific subscription direction.
        /// The dead ones will be removed
        /// </summary>
        /// <param name="messageType">The type of the message to check</param>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns></returns>
        public Task RequestPurgeAsync(Type messageType, SubscriptionDirectionEnum subscriptionDirection)
        {
            return SchedulePurgeAsync(subscriptionDirection, messageType);
        }

        /// <summary>
        /// Schedules a check on all subscribers for all messageType for the specified subscriber direction.
        /// The dead ones will be removed
        /// </summary>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns></returns>
        public Task RequestPurgeAllAsync(SubscriptionDirectionEnum subscriptionDirection)
        {
            Type[] subscriptionTypes = null;

            switch (subscriptionDirection)
            {
                case SubscriptionDirectionEnum.OneWay:
                    subscriptionTypes = _oneWaySubscriptions.Keys.ToArray();
                    break;

                case SubscriptionDirectionEnum.TwoWay:
                    subscriptionTypes = _twoWaySubscriptions.Keys.ToArray();
                    break;
            }

            return SchedulePurgeAsync(subscriptionDirection, subscriptionTypes);
        }

        private async Task SchedulePurgeAsync(SubscriptionDirectionEnum subscriptionDirection, params Type[] messageTypes)
        {
            if (messageTypes == null || messageTypes.Length == 0)
                return;

            await Task.Run(() =>
            {
                switch (subscriptionDirection)
                {
                    case SubscriptionDirectionEnum.OneWay:
                        lock (this)
                        {
                            foreach (var type in messageTypes)
                            {
                                List<ISubscription> oneWaySubscriptionsForType;
                                if (_oneWaySubscriptions.TryGetValue(type, out oneWaySubscriptionsForType) && oneWaySubscriptionsForType != null)
                                {
                                    foreach (var subscription in oneWaySubscriptionsForType.ToArray())
                                    {
                                        if (!subscription.IsAlive)
                                        {
                                            MvxPluginLog.Instance.Trace($"Purging one-way subscription (MessageType: {type} Context: { subscription.Context} SubscriptionId: {subscription.Id})");
                                            _oneWaySubscriptions[type].Remove(subscription);
                                        }
                                    }
                                }

                            }

                        }
                        break;

                    case SubscriptionDirectionEnum.TwoWay:
                        lock (this)
                        {
                            foreach (var type in messageTypes)
                            {
                                Dictionary<string, ISubscription> twoWaySubscriptionsForType;
                                if (_twoWaySubscriptions.TryGetValue(type, out twoWaySubscriptionsForType) && twoWaySubscriptionsForType != null)
                                {
                                    foreach (var context in twoWaySubscriptionsForType.Keys.ToArray())
                                    {
                                        if (twoWaySubscriptionsForType[context] != null && !twoWaySubscriptionsForType[context].IsAlive)
                                        {
                                            MvxPluginLog.Instance.Trace($"Purging two-way subscription (MessageType: {type} Context: {context} SubscriptionId: {twoWaySubscriptionsForType[context].Id})");
                                            _twoWaySubscriptions[type].Remove(context);
                                        }
                                    }
                                }

                            }
                        }
                        break;
                }
            });
        }

        #endregion
    }
}
