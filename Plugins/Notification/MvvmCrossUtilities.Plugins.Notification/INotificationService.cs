using MvvmCrossUtilities.Plugins.Notification.Core;
using MvvmCrossUtilities.Plugins.Notification.Messages;
using System;
using System.Threading.Tasks;

namespace MvvmCrossUtilities.Plugins.Notification
{
    /// <summary>
    /// Interface for Notification Services
    /// </summary>
    public interface INotificationService
    {
        #region Subscription/Unsubscription

        #region One-Way subscription

        /// <summary>
        /// Subscribes an one-way notification with the default context and with a weak reference
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <returns></returns>
        SubscriptionToken Subscribe<TMessage>(Func<TMessage, Task> asyncDeliveryAction)
            where TMessage : NotificationMessage;

        /// <summary>
        /// Subscribes an one-way notification with a weak reference
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        SubscriptionToken Subscribe<TMessage>(Func<TMessage, Task> asyncDeliveryAction, string context)
            where TMessage : NotificationMessage;

        /// <summary>
        /// Subscribes an one-way notification
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        /// <param name="reference">The reference.</param>
        /// <returns></returns>
        SubscriptionToken Subscribe<TMessage>(Func<TMessage, Task> asyncDeliveryAction, string context, SubscriptionTypeEnum reference)
            where TMessage : NotificationMessage;

        #endregion

        #region Two-Way subscription

        /// <summary>
        /// Subscribes a two-way notification with the default context and with a weak reference
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <returns></returns>
        SubscriptionToken Subscribe<TMessage, TResult>(Func<TMessage, Task<TResult>> asyncDeliveryAction)
            where TMessage : NotificationMessage
            where TResult : NotificationResult;

        /// <summary>
        /// Subscribes a two-way notification with a weak reference
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        SubscriptionToken Subscribe<TMessage, TResult>(Func<TMessage, Task<TResult>> asyncDeliveryAction, string context)
            where TMessage : NotificationMessage
            where TResult : NotificationResult;

        /// <summary>
        /// Subscribes a two-way notification
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        /// <param name="reference">The reference.</param>
        /// <returns></returns>
        SubscriptionToken Subscribe<TMessage, TResult>(Func<TMessage, Task<TResult>> asyncDeliveryAction, string context, SubscriptionTypeEnum reference)
            where TMessage : NotificationMessage
            where TResult : NotificationResult;

        #endregion

        #region Unsubscription

        /// <summary>
        /// Unsubscribesthe message with the specified token
        /// </summary>
        /// <param name="subscriptionToken">The subscription token.</param>
        void Unsubscribe(SubscriptionToken subscriptionToken);

        /// <summary>
        /// Unsubscribe all messages from a particular message type.
        /// </summary>
        /// <typeparam name="TMessage">Type of message</typeparam>
        void Unsubscribe<TMessage>() where TMessage : NotificationMessage;

        /// <summary>
        /// Unsubscribe all messages from a particular message type
        /// </summary>
        /// <param name="messageType">Type of message.</param>
        void Unsubscribe(Type messageType);

        #endregion

        #endregion

        #region Publish

        #region OneWay Publish

        /// <summary>
        /// Publish a message, using the async/await pattern
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <returns></returns>
        Task PublishAsync(INotificationMessage message);

        /// <summary>
        /// Publish a message, using the async/await pattern
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="context">The context of the message.</param>
        /// <returns></returns>
        Task PublishAsync(INotificationMessage message, string context);

        /// <summary>
        /// Adds a message to the pending notification queue
        /// </summary>
        /// <param name="message">The message to store.</param>
        /// <param name="context">The context of the message.</param>
        /// <param name="tryNormalPublish">if set to <c>true</c> tries to do normal publish, before storing the notification for delayed publish.</param>
        /// <returns></returns>
        Task DelayedPublishAsync(INotificationMessage message, string context, bool tryNormalPublish);

        /// <summary>
        /// Publishes the pending notifications for a particular context
        /// and makes sure that they're not published to the sender thata added them to the pending list
        /// </summary>
        /// <param name="currentPublisher">The current publisher that is requesting to publish de pending messages.</param>
        /// <param name="context">The context of the pending messages.</param>
        /// <returns></returns>
        Task PublishPendingAsyncNotificationsAsync(object currentPublisher, string context);

        #endregion

        #region TwoWay Publish

        /// <summary>
        /// Publish a message to a single subscriptor (the first one), using the async/await pattern
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        Task<TResult> PublishAsync<TResult>(INotificationMessage message)
            where TResult : NotificationResult;

        /// <summary>
        /// Publish a message to a single subscriptor (the first one), using the async/await pattern
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        Task<TResult> PublishAsync<TResult>(INotificationMessage message, string context)
            where TResult : NotificationResult;

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Checks if there's subscriptions for TMessage
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        bool HasSubscriptionFor<TMessage>() where TMessage : NotificationMessage;

        /// <summary>
        /// Checks if there's subscriptions for messageType
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        bool HasSubscriptionFor(Type messageType);

        /// <summary>
        /// Check if there's subscriptions of TMessage for the specified context
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="context">Context of the subscription</param>
        bool HasSubscriptionForContext<TMessage>(string context) where TMessage : NotificationMessage;

        /// <summary>
        /// Check if there's subscriptions of messageType for the specified context
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        bool HasSubscriptionForContext(Type messageType, string context);

        /// <summary>
        /// Get all the distinct context of subscriptions for TMessage
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        string GetSubscriptionContextFor<TMessage>() where TMessage : NotificationMessage;

        /// <summary>
        /// Get all the distinct context of subscriptions for messageType
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        string GetSubscriptionContextFor(Type messageType);

        /// <summary>
        /// Schedules a check on all subscribers for the specified messageType. If any are not alive, they will be removed
        /// </summary>
        /// <param name="messageType">The type of the message to check</param>
        Task RequestPurgeAsync(Type messageType);

        /// <summary>
        /// Schedules a check on all subscribers for all messageType. If any are not alive, they will be removed
        /// </summary>
        Task RequestPurgeAllAsync();

        #endregion
    }
}
