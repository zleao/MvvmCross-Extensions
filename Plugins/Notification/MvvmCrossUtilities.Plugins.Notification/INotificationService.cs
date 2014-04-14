using System;
using System.Collections.Generic;
using MvvmCrossUtilities.Plugins.Notification.Exceptions;
using MvvmCrossUtilities.Plugins.Notification.Messages;
using MvvmCrossUtilities.Plugins.Notification.Messages.Base;
using MvvmCrossUtilities.Plugins.Notification.Subscriptions;
using MvvmCrossUtilities.Plugins.Notification;

namespace MvvmCrossUtilities.Plugins.Notification
{
    public interface INotificationService
    {
        #region One way subscription

        /// <summary>
        /// Subscribe to a message type with the given destination and delivery action in the specified context.
        /// The context will be the default.
        /// Subscription type will be weak
        /// </summary>
        /// <typeparam name="TMessage">Type of message</typeparam>
        /// <param name="deliveryAction">Action to invoke when message is delivered</param>
        /// <returns>MessageSubscription used to unsubscribing</returns>
        SubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction) where TMessage : NotificationOneWayMessage;

        /// <summary>
        /// Subscribe to a message type with the given destination and delivery action in the specified context.
        /// Subscription type will be weak
        /// </summary>
        /// <typeparam name="TMessage">Type of message</typeparam>
        /// <param name="deliveryAction">Action to invoke when message is delivered</param>
        /// <param name="context">The context of this subscription</param>
        /// <returns>MessageSubscription used to unsubscribing</returns>
        SubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, string context) where TMessage : NotificationOneWayMessage;

        /// <summary>
        /// Subscribe to a message type with the given destination and delivery action in the specified context.
        /// </summary>
        /// <typeparam name="TMessage">Type of message</typeparam>
        /// <param name="deliveryAction">Action to invoke when message is delivered</param>
        /// <param name="context">The context of this subscription</param>
        /// /// <param name="reference">Use a strong or weak reference to the deliveryAction</param>
        /// <returns>MessageSubscription used to unsubscribing</returns>
        SubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, string context, SubscriptionTypeEnum reference) where TMessage : NotificationOneWayMessage;

        /// Subscribe to a message type with the given destination and delivery action in the specified context.
        /// This subscription always invokes the delivery Action on the Main thread.
        /// </summary>
        /// <typeparam name="TMessage">Type of message</typeparam>
        /// <param name="deliveryAction">Action to invoke when message is delivered</param>
        /// <param name="context">The context of this subscription</param>
        /// <param name="reference">Use a strong or weak reference to the deliveryAction</param>
        /// <returns>MessageSubscription used to unsubscribing</returns>
        SubscriptionToken SubscribeOnMainThread<TMessage>(Action<TMessage> deliveryAction, string context = Subscription.DefaultContext, SubscriptionTypeEnum reference = SubscriptionTypeEnum.Weak) where TMessage : NotificationOneWayMessage;

        /// <summary>
        /// Subscribe to a message type with the given destination and delivery action in the specified context.
        /// This subscription always invokes the delivery Action called on a threadpool thread.
        /// </summary>
        /// <typeparam name="TMessage">Type of message</typeparam>
        /// <param name="deliveryAction">Action to invoke when message is delivered</param>
        /// <param name="context">The context of this subscription</param>
        /// <param name="reference">Use a strong or weak reference to the deliveryAction</param>
        /// <returns>MessageSubscription used to unsubscribing</returns>
        SubscriptionToken SubscribeOnThreadPoolThread<TMessage>(Action<TMessage> deliveryAction, string context = Subscription.DefaultContext, SubscriptionTypeEnum reference = SubscriptionTypeEnum.Weak) where TMessage : NotificationOneWayMessage;

        #endregion

        #region Two way subscription

        SubscriptionToken Subscribe<TMessage, TResult>(Func<TMessage, TResult> deliveryAction)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult;

        SubscriptionToken Subscribe<TMessage, TResult>(Func<TMessage, TResult> deliveryAction, string context)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult;

        SubscriptionToken Subscribe<TMessage, TResult>(Func<TMessage, TResult> deliveryAction, string context, SubscriptionTypeEnum reference)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult;

        SubscriptionToken SubscribeOnMainThread<TMessage, TResult>(Func<TMessage, TResult> deliveryAction, string context = Subscription.DefaultContext, SubscriptionTypeEnum reference = SubscriptionTypeEnum.Weak)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult;

        SubscriptionToken SubscribeOnThreadPoolThread<TMessage, TResult>(Func<TMessage, TResult> deliveryAction, string context = Subscription.DefaultContext, SubscriptionTypeEnum reference = SubscriptionTypeEnum.Weak)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult;

        #endregion


        /// <summary>
        /// Unsubscribesthe message with the specified token
        /// </summary>
        /// <param name="subscriptionToken">The subscription token.</param>
        void Unsubscribe(SubscriptionToken subscriptionToken);

        /// <summary>
        /// Unsubscribe all messages from a particular message type.
        /// </summary>
        /// <typeparam name="TMessage">Type of message</typeparam>
        /// <param name="subscriptionToken">Token of the subscription to remove</param>
        void Unsubscribe<TMessage>() where TMessage : NotificationMessage;


        /// <summary>
        /// Publish a message to any subscribers
        /// The context will be the default
        /// </summary>
        /// <typeparam name="TMessage">Type of message</typeparam>
        /// <param name="message">Message to deliver</param>
        void Publish<TMessage>(TMessage message) where TMessage : NotificationOneWayMessage;

        /// <summary>
        /// Publish a message to any subscribers
        /// </summary>
        /// <typeparam name="TMessage">Type of message</typeparam>
        /// <param name="message">Message to deliver</param>
        /// <param name="context">The context of the message</param>
        void Publish<TMessage>(TMessage message, string context) where TMessage : NotificationOneWayMessage;

        /// <summary>
        /// Publish a message to any subscribers and provides a callback for a result.
        /// The context will be the default
        /// </summary>
        /// <typeparam name="TMessage">Type of message</typeparam>
        /// <typeparam name="TResult">Type of result</typeparam>
        /// <param name="message">Message to deliver</param>
        /// <param name="OnResultCallback">Result callback</param>
        /// <param name="OnErrorCallback">Error callback</param>
        void Publish<TMessage, TResult>(TMessage message, Action<TResult> OnResultCallback, Action<NotificationErrorException> OnErrorCallback)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult;

        /// <summary>
        /// Publish a message to any subscribers and provides a callback for a result.
        /// </summary>
        /// <typeparam name="TMessage">Type of message</typeparam>
        /// <typeparam name="TResult">Type of result</typeparam>
        /// <param name="message">Message to deliver</param>
        /// <param name="OnResultCallback">Result callback</param>
        /// <param name="OnErrorCallback">Error callback</param>
        /// <param name="context">The context of the message</param>
        void Publish<TMessage, TResult>(TMessage message, Action<TResult> OnResultCallback, Action<NotificationErrorException> OnErrorCallback, string context)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult;


        /// <summary>
        /// Has subscriptions for TMessage
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        bool HasSubscriptionsFor<TMessage>() where TMessage : NotificationMessage;

        /// <summary>
        /// Number of subscriptions for TMessage
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        int CountSubscriptionsFor<TMessage>() where TMessage : NotificationMessage;

        /// <summary>
        /// Has subscriptions for TMessage with a Context value of context
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="context">Context of the subscription</param>
        /// <returns></returns>
        bool HasSubscriptionsForContext<TMessage>(string context) where TMessage : NotificationMessage;

        /// <summary>
        /// Number of subscriptions for TMessage with a Context value of context
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="tag">An optional tag to include with this subscription</param>
        /// <returns></returns>
        int CountSubscriptionsForContext<TMessage>(string context) where TMessage : NotificationMessage;

        /// <summary>
        /// Get all the distinct context of subscriptions for TMessage
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <returns></returns>
        IEnumerable<string> GetSubscriptionsContextFor<TMessage>() where TMessage : NotificationMessage;


        /// <summary>
        /// Schedules a check on all subscribers for the specified messageType. If any are not alive, they will be removed
        /// </summary>
        /// <param name="messageType">The type of the message to check</param>
        void RequestPurge(Type messageType);

        /// <summary>
        /// Schedules a check on all subscribers for all messageType. If any are not alive, they will be removed
        /// </summary>
        void RequestPurgeAll();
    }
}
