using Cirrious.CrossCore.Core;
using Cirrious.CrossCore.Platform;
using MvvmCrossUtilities.Plugins.Notification.Core;
using MvvmCrossUtilities.Plugins.Notification.Core.Async.Subscriptions;
using MvvmCrossUtilities.Plugins.Notification.Core.Async.Subscriptions.Single;
using MvvmCrossUtilities.Plugins.Notification.Core.Async.Subscriptions.Void;
using MvvmCrossUtilities.Plugins.Notification.Core.Async.ThreadRunners;
using MvvmCrossUtilities.Plugins.Notification.Exceptions;
using MvvmCrossUtilities.Plugins.Notification.Messages;
using MvvmCrossUtilities.Plugins.Notification.Messages.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MvvmCrossUtilities.Plugins.Notification
{
    /// <summary>
    /// Notification service plugin
    /// </summary>
    public class NotificationManager : INotificationService
    {
        #region Fields

        private SemaphoreSlim _purgeSemaphore = new SemaphoreSlim(1, 1);

        private volatile Dictionary<Type, ISubscription> _subscriptions = new Dictionary<Type, ISubscription>();
        private volatile Dictionary<string, List<INotificationMessage>> _pendingNotifications = new Dictionary<string, List<INotificationMessage>>();

        #endregion

        #region Subscribe/Unsubscribe message

        #region OneWay Subscription

        /// <summary>
        /// Subscribes an one-way notification with the default context and with a weak reference
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <returns></returns>
        public SubscriptionToken Subscribe<TMessage>(Func<TMessage, Task> asyncDeliveryAction)
            where TMessage : NotificationMessage
        {
            return Subscribe<TMessage>(asyncDeliveryAction, AsyncSubscription.DefaultContext);
        }

        /// <summary>
        /// Subscribes an one-way notification with a weak reference
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public SubscriptionToken Subscribe<TMessage>(Func<TMessage, Task> asyncDeliveryAction, string context)
            where TMessage : NotificationMessage
        {
            return Subscribe<TMessage>(asyncDeliveryAction, context, SubscriptionTypeEnum.Weak);
        }

        /// <summary>
        /// Subscribes an one-way notification
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        /// <param name="reference">The reference.</param>
        /// <returns></returns>
        public SubscriptionToken Subscribe<TMessage>(Func<TMessage, Task> asyncDeliveryAction, string context, SubscriptionTypeEnum reference)
            where TMessage : NotificationMessage
        {
            return CommonSubscribe<TMessage>(asyncDeliveryAction, new SimpleAsyncActionRunner(), reference, context);
        }

        private SubscriptionToken CommonSubscribe<TMessage>(Func<TMessage, Task> asyncDeliveryAction, IAsyncActionRunner asyncActionRunner, SubscriptionTypeEnum reference, string context)
            where TMessage : NotificationMessage
        {
            if (asyncDeliveryAction == null)
            {
                throw new ArgumentNullException("asyncDeliveryAction");
            }

            AsyncSubscription asyncSubscription;

            switch (reference)
            {
                case SubscriptionTypeEnum.Strong:
                    asyncSubscription = new VoidAsyncStrongSubscription<TMessage>(asyncActionRunner, asyncDeliveryAction, context);
                    break;
                case SubscriptionTypeEnum.Weak:
                    asyncSubscription = new VoidAsyncWeakSubscription<TMessage>(asyncActionRunner, asyncDeliveryAction, context);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("reference", "reference type unexpected " + reference);
            }

            var messageType = typeof(TMessage);

            if (_subscriptions.ContainsKey(messageType))
            {
                MvxTrace.Trace("Replacing subscription {0} with context {1} for {2}", asyncSubscription.Id, asyncSubscription.Context, messageType.Name);
                _subscriptions[messageType] = asyncSubscription;
            }
            else
            {
                MvxTrace.Trace("Adding subscription {0} with context {1} for {2}", asyncSubscription.Id, asyncSubscription.Context, messageType.Name);
                _subscriptions.Add(messageType, asyncSubscription);
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
            where TMessage : NotificationMessage
            where TResult : NotificationResult
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
            where TMessage : NotificationMessage
            where TResult : NotificationResult
        {
            return Subscribe<TMessage, TResult>(asyncDeliveryAction, context, SubscriptionTypeEnum.Weak);
        }

        /// <summary>
        /// Subscribes a two-way notification
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        /// <param name="reference">The reference.</param>
        /// <returns></returns>
        public SubscriptionToken Subscribe<TMessage, TResult>(Func<TMessage, Task<TResult>> asyncDeliveryAction, string context, SubscriptionTypeEnum reference)
            where TMessage : NotificationMessage
            where TResult : NotificationResult
        {
            return CommonSubscribe<TMessage, TResult>(asyncDeliveryAction, new SimpleAsyncActionRunner<TResult>(), reference, context);
        }

        private SubscriptionToken CommonSubscribe<TMessage, TResult>(Func<TMessage, Task<TResult>> asyncDeliveryAction, IAsyncActionRunner<TResult> asyncActionRunner, SubscriptionTypeEnum reference, string context)
            where TMessage : NotificationMessage
            where TResult : NotificationResult
        {
            if (asyncDeliveryAction == null)
            {
                throw new ArgumentNullException("asyncDeliveryAction");
            }

            AsyncSubscription<TResult> asyncSubscription;

            switch (reference)
            {
                case SubscriptionTypeEnum.Strong:
                    asyncSubscription = new SingleAsyncStrongSubscription<TMessage, TResult>(asyncActionRunner, asyncDeliveryAction, context);
                    break;
                case SubscriptionTypeEnum.Weak:
                    asyncSubscription = new SingleAsyncWeakSubscription<TMessage, TResult>(asyncActionRunner, asyncDeliveryAction, context);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("reference", "reference type unexpected " + reference);
            }

            var messageType = typeof(TMessage);

            if (_subscriptions.ContainsKey(messageType))
            {
                MvxTrace.Trace("Replacing subscription {0} with context {1} for {2}", asyncSubscription.Id, asyncSubscription.Context, messageType.Name);
                _subscriptions[messageType] = asyncSubscription;
            }
            else
            {
                MvxTrace.Trace("Adding subscription {0} with context {1} for {2}", asyncSubscription.Id, asyncSubscription.Context, messageType.Name);
                _subscriptions.Add(messageType, asyncSubscription);
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
                ISubscription asyncSubscription;
                if (_subscriptions.TryGetValue(subscriptionToken.MessageType, out asyncSubscription))
                {
                    if (asyncSubscription != null && asyncSubscription.Id == subscriptionToken.Id)
                    {
                        MvxTrace.Trace("Removing subscription {0}", subscriptionToken.Id);

                        _subscriptions.Remove(subscriptionToken.MessageType);
                    }
                }
            }
        }

        /// <summary>
        /// Unsubscribe all messages from a particular message type.
        /// </summary>
        /// <typeparam name="TMessage">Type of message</typeparam>
        public void Unsubscribe<TMessage>() where TMessage : NotificationMessage
        {
            Unsubscribe(typeof(TMessage));
        }

        /// <summary>
        /// Unsubscribe all messages from a particular message type
        /// </summary>
        /// <param name="messageType">Type of message.</param>
        public void Unsubscribe(Type messageType)
        {
            ISubscription asyncSubscription;
            if (_subscriptions.TryGetValue(messageType, out asyncSubscription))
            {
                MvxTrace.Trace("Removing subscription {0}", asyncSubscription.Id);

                _subscriptions.Remove(messageType);
            }
        }

        #endregion

        #endregion

        #region Publish message

        #region OneWay publish

        /// <summary>
        /// Publish a message, using the async/await pattern
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns></returns>
        public Task PublishAsync(INotificationMessage message)
        {
            return PublishAsync(message, AsyncSubscription.DefaultContext);
        }

        /// <summary>
        /// Publish a message, using the async/await pattern
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="context">The context of the message.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">message</exception>
        public async Task PublishAsync(INotificationMessage message, string context)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            var messageType = message.GetType();
            if (messageType == typeof(NotificationMessage))
            {
                MvxTrace.Warning("NotificationMessage publishing not allowed - this normally suggests non-specific generic used in calling code");
                return;
            }

            var toNotify = GetSingleSubscriptionFor(messageType, context);
            if (toNotify == null)
            {
                MvxTrace.Trace("Nothing registered for messages of type {0} with context {1}", messageType.Name, context);
                return;
            }

            var asyncSubscription = toNotify as VoidAsyncSubscription;
            if (asyncSubscription == null)
            {
                MvxTrace.Trace("Nothing registered for messages of type {0}", messageType.Name);
                return;
            }

            await asyncSubscription.InvokeAsync(message);
        }

        /// <summary>
        /// Adds a message to the pending async notification queue
        /// </summary>
        /// <param name="message">The message to store.</param>
        /// <param name="context">The context of the message.</param>
        /// <param name="tryNormalPublish">if set to <c>true</c> tries to do normal publish, before storing the notification for delayed publish.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">message</exception>
        public async Task DelayedPublishAsync(INotificationMessage message, string context, bool tryNormalPublish)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            var messageType = message.GetType();
            if (messageType == typeof(NotificationMessage))
            {
                MvxTrace.Warning("NotificationMessage publishing not allowed - this normally suggests non-specific generic used in calling code");
                return;
            }

            if (tryNormalPublish)
            {
                if (HasSubscriptionForContext(messageType, context))
                    await PublishAsync(message, context);
                else
                    MvxTrace.Warning(string.Format("Failed to normal publish the '{0}' message for context '{1}'. Adding it to delayed publish...", messageType.FullName, context));
            }

            AddToPendingNotificationList(message, context);
        }

        /// <summary>
        /// Publishes the pending async notifications for a particular context
        /// and makes sure that they're not published to the sender that added them to the pending list
        /// </summary>
        /// <param name="currentPublisher">The current publisher that is requesting to publish de pending messages.</param>
        /// <param name="context">The context of the pending messages.</param>
        /// <returns></returns>
        public async Task PublishPendingAsyncNotificationsAsync(object currentPublisher, string context)
        {
            if (string.IsNullOrEmpty(context))
            {
                MvxTrace.Warning("PublishPendingNotificationsFor - Context is null or empty. Assuming default");
                context = AsyncSubscription.DefaultContext;
            }

            if (_pendingNotifications.ContainsKey(context) && _pendingNotifications[context].Count() > 0)
            {
                var msgsCount = _pendingNotifications[context].Where(m => m.Sender != currentPublisher).Count();
                if (msgsCount > 0)
                {
                    MvxTrace.Trace("Found {0} pending message(s) with context {1} available for current publisher ({2})", msgsCount, context, currentPublisher);

                    foreach (var msg in _pendingNotifications[context].ToArray())
                    {
                        if (HasSubscriptionForContext(msg.GetType(), context))
                        {
                            await PublishAsync(msg, context);
                            _pendingNotifications[context].Remove(msg);
                        }
                    }
                }
                else
                {
                    MvxTrace.Trace("Found {0} pending message(s) with context {1} but NONE available for current publisher ({2})",
                        _pendingNotifications[context].Count(),
                        context,
                        currentPublisher);
                }
            }
            else
            {
                MvxTrace.Trace("No pending messages found with context {0}", context);
                return;
            }
        }

        #endregion

        #region TwoWay publish

        /// <summary>
        /// Publish a message to a single subscriptor (the first one), using the async/await pattern
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public Task<TResult> PublishAsync<TResult>(INotificationMessage message)
            where TResult : NotificationResult
        {
            return PublishAsync<TResult>(message, AsyncSubscription.DefaultContext);
        }

        /// <summary>
        /// Publish a message to a single subscriptor (the first one), using the async/await pattern
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">message</exception>
        public async Task<TResult> PublishAsync<TResult>(INotificationMessage message, string context)
            where TResult : NotificationResult
        {
            if (message == null)
                throw new ArgumentNullException("message");

            var messageType = message.GetType();
            if (messageType == typeof(NotificationMessage))
            {
                MvxTrace.Warning("NotificationMessage publishing not allowed - this normally suggests non-specific generic used in calling code");
                return default(TResult);
            }

            var toNotify = GetSingleSubscriptionFor(messageType, context);
            if (toNotify == null)
            {
                MvxTrace.Trace("Nothing registered for async messages of type {0} with context {1}", messageType.Name, context);
                return default(TResult);
            }

            var asyncSubscription = toNotify as SingleAsyncSubscription<TResult>;
            if (asyncSubscription == null)
            {
                MvxTrace.Trace("Nothing registered for async messages of type {0} with result {1}", messageType.Name, typeof(TResult).Name);
                return default(TResult);
            }

            return await asyncSubscription.InvokeAsync(message);
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Has subscription for TMessage
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public bool HasSubscriptionFor<TMessage>() where TMessage : NotificationMessage
        {
            return HasSubscriptionFor(typeof(TMessage));
        }

        /// <summary>
        /// Checks if there's subscriptions for messageType
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public bool HasSubscriptionFor(Type messageType)
        {
            ISubscription asyncSubscription;
            if (_subscriptions.TryGetValue(messageType, out asyncSubscription))
            {
                return asyncSubscription != null && asyncSubscription.IsAlive;
            }

            return false;
        }

        /// <summary>
        /// Has subscription for TMessage with a Context value of context
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="context">Context of the subscription</param>
        /// <returns></returns>
        public bool HasSubscriptionForContext<TMessage>(string context) where TMessage : NotificationMessage
        {
            return HasSubscriptionForContext(typeof(TMessage), context);
        }

        /// <summary>
        /// Check if there's subscriptions of messageType for the specified context
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public bool HasSubscriptionForContext(Type messageType, string context)
        {
            ISubscription asyncSubscription;
            if (_subscriptions.TryGetValue(messageType, out asyncSubscription))
            {
                return asyncSubscription != null && asyncSubscription.IsAlive && asyncSubscription.Context == context;
            }

            return false;
        }

        /// <summary>
        /// Get the context of the subscription for TMessage
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        public string GetSubscriptionContextFor<TMessage>() where TMessage : NotificationMessage
        {
            return GetSubscriptionContextFor(typeof(TMessage));
        }

        /// <summary>
        /// Get all the distinct context of subscriptions for messageType
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public string GetSubscriptionContextFor(Type messageType)
        {
            ISubscription asyncSubscription;
            if (_subscriptions.TryGetValue(messageType, out asyncSubscription))
            {
                if (asyncSubscription != null)
                    return asyncSubscription.Context;
            }

            return null;
        }


        private ISubscription GetSingleSubscriptionFor(Type messageType, string context)
        {
            ISubscription asyncSubscription;
            if (!_subscriptions.TryGetValue(messageType, out asyncSubscription))
            {
                return null;
            }

            return asyncSubscription;
        }

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
                MvxTrace.Warning("AddToPendingNotificationList - Context of message type {0} is null or empty. Assuming default", messageType.Name);
                context = AsyncSubscription.DefaultContext;
            }

            if (!_pendingNotifications.ContainsKey(context))
                _pendingNotifications.Add(context, new List<INotificationMessage>());

            MvxTrace.Trace("AddToPendingAsyncNotificationList - Adding message of type {0} with context '{1}' to pending notification list", messageType.Name, context);

            _pendingNotifications[context].Add(message);
        }

        #endregion

        #region Message purging

        /// <summary>
        /// Schedules a check on all subscribers for the specified messageType. If any are not alive, they will be removed
        /// </summary>
        /// <param name="messageType">The type of the message to check</param>
        public Task RequestPurgeAsync(Type messageType)
        {
            return SchedulePurgeAsync(messageType);
        }

        /// <summary>
        /// Schedules a check on all subscribers for all messageType. If any are not alive, they will be removed
        /// </summary>
        public Task RequestPurgeAllAsync()
        {
            return SchedulePurgeAsync(_subscriptions.Keys.ToArray());
        }

        private Task SchedulePurgeAsync(params Type[] messageTypes)
        {
            return Task.Run(async () =>
            {
                try
                {
                    await Task.Run(() => _purgeSemaphore.Wait());

                    foreach (var type in messageTypes)
                    {
                        ISubscription asyncSubscription;
                        if (_subscriptions.TryGetValue(type, out asyncSubscription))
                        {
                            if (!asyncSubscription.IsAlive)
                            {
                                MvxTrace.Trace("Purging subscription of type {0}", type.Name);
                                _subscriptions.Remove(type);
                            }
                        }

                    }
                }
                finally
                {
                    _purgeSemaphore.Release();
                }
            });
        }

        #endregion
    }
}
