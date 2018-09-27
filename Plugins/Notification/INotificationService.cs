using MvxExtensions.Plugins.Notification.Core;
using MvxExtensions.Plugins.Notification.Core.Async.Subscriptions;
using MvxExtensions.Plugins.Notification.Messages;
using MvxExtensions.Plugins.Notification.Messages.Base;
using MvxExtensions.Plugins.Notification.Messages.TwoWay.Question;
using MvxExtensions.Plugins.Notification.Messages.TwoWay.Result;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvxExtensions.Plugins.Notification
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
            where TMessage : INotificationMessage;

        /// <summary>
        /// Subscribes an one-way notification with a weak reference
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        SubscriptionToken Subscribe<TMessage>(Func<TMessage, Task> asyncDeliveryAction, string context)
            where TMessage : INotificationMessage;

        /// <summary>
        /// Subscribes an one-way notification
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        /// <param name="subscriptionReferenceType">Type of the subscription reference.</param>
        /// <returns></returns>
        SubscriptionToken Subscribe<TMessage>(Func<TMessage, Task> asyncDeliveryAction, string context, SubscriptionReferenceTypeEnum subscriptionReferenceType)
            where TMessage : INotificationMessage;


        /// <summary>
        /// Subscribes an one-way notification with the default context and with a weak reference
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <returns></returns>
        SubscriptionToken Subscribe(Type messageType, Func<INotificationMessage, Task> asyncDeliveryAction);

        /// <summary>
        /// Subscribes an one-way notification with a weak reference
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        SubscriptionToken Subscribe(Type messageType, Func<INotificationMessage, Task> asyncDeliveryAction, string context);

        /// <summary>
        /// Subscribes an one-way notification
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        /// <param name="subscriptionReferenceType">Type of the subscription reference.</param>
        /// <returns></returns>
        SubscriptionToken Subscribe(Type messageType, Func<INotificationMessage, Task> asyncDeliveryAction, string context, SubscriptionReferenceTypeEnum subscriptionReferenceType);

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
            where TMessage : INotificationMessage
            where TResult : INotificationResult;

        /// <summary>
        /// Subscribes a two-way notification with a weak reference
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        SubscriptionToken Subscribe<TMessage, TResult>(Func<TMessage, Task<TResult>> asyncDeliveryAction, string context)
            where TMessage : INotificationMessage
            where TResult : INotificationResult;

        /// <summary>
        /// Subscribes a two-way notification
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        /// <param name="subscriptionReferenceType">Type of the subscription reference.</param>
        /// <returns></returns>
        SubscriptionToken Subscribe<TMessage, TResult>(Func<TMessage, Task<TResult>> asyncDeliveryAction, string context, SubscriptionReferenceTypeEnum subscriptionReferenceType)
            where TMessage : INotificationMessage
            where TResult : INotificationResult;

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
        void Unsubscribe<TMessage>() where TMessage : INotificationMessage;

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
        /// Publish an one-way message
        /// </summary>
        /// <param name="message">The message to send.</param>
        /// <param name="context">The context of the message.</param>
        /// <returns></returns>
        Task PublishAsync(INotificationMessage message, string context = AsyncSubscription.DefaultContext);

        /// <summary>
        /// Adds a message to the pending notification queue
        /// </summary>
        /// <param name="message">The message to store.</param>
        /// <param name="context">The context of the message.</param>
        /// <param name="tryNormalPublish">if set to <c>true</c> tries to do normal publish, before storing the notification for delayed publish.</param>
        /// <returns></returns>
        Task DelayedPublishAsync(INotificationMessage message, string context = AsyncSubscription.DefaultContext, bool tryNormalPublish = false);

        /// <summary>
        /// Publishes the pending notifications for a particular context
        /// and makes sure that they're not published to the sender thata added them to the pending list
        /// </summary>
        /// <param name="currentPublisher">The current publisher that is requesting to publish de pending messages.</param>
        /// <param name="context">The context of the pending messages.</param>
        /// <returns></returns>
        Task PublishPendingNotificationsAsync(object currentPublisher, string context = AsyncSubscription.DefaultContext);


        #region Info Notifications

        /// <summary>
        /// Publishes an one-way information message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        Task PublishInfoNotificationAsync(string message, NotificationModeEnum mode = NotificationModeEnum.Default, string context = AsyncSubscription.DefaultContext);

        /// <summary>
        /// Adds an one-way information message to the pending notification queue
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        Task DelayedPublishInfoNotificationAsync(string message, NotificationModeEnum mode = NotificationModeEnum.Default, string context = AsyncSubscription.DefaultContext);

        #endregion

        #region Warning Notifications

        /// <summary>
        /// Publishes an one-way warning message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        Task PublishWarningNotificationAsync(string message, NotificationModeEnum mode = NotificationModeEnum.Default, string context = AsyncSubscription.DefaultContext);

        /// <summary>
        /// Adds an one-way warning message to the pending notification queue
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        Task DelayedPublishWarningNotificationAsync(string message, NotificationModeEnum mode = NotificationModeEnum.Default, string context = AsyncSubscription.DefaultContext);

        #endregion

        #region Error Notifications

        /// <summary>
        /// Publishes an one-way error message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        Task PublishErrorNotificationAsync(string message, NotificationModeEnum mode = NotificationModeEnum.Default, string context = AsyncSubscription.DefaultContext);

        /// <summary>
        /// Adds an one-way error message to the pending notification queue
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        Task DelayedPublishErrorNotificationAsync(string message, NotificationModeEnum mode = NotificationModeEnum.Default, string context = AsyncSubscription.DefaultContext);

        #endregion

        #region Success Notifications

        /// <summary>
        /// Publishes an one-way success message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        Task PublishSuccessNotificationAsync(string message, NotificationModeEnum mode = NotificationModeEnum.Default, string context = AsyncSubscription.DefaultContext);

        /// <summary>
        /// Adds an one-way error message to the pending notification queue
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        Task DelayedPublishSuccessNotificationAsync(string message, NotificationModeEnum mode = NotificationModeEnum.Default, string context = AsyncSubscription.DefaultContext);

        #endregion

        #endregion

        #region TwoWay Publish

        /// <summary>
        /// Publishes a two-way message
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="message">The message.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        Task<TResult> PublishAsync<TResult>(INotificationMessage message, string context = AsyncSubscription.DefaultContext)
            where TResult : INotificationResult;


        /// <summary>
        /// Publishes a two-way generic question message.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <param name="possibleAnswers">The possible answers.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        Task<NotificationGenericQuestionResult> PublishGenericQuestionNotificationAsync(string question, NotificationTwoWayAnswersGroupEnum possibleAnswers, string context = AsyncSubscription.DefaultContext);

        /// <summary>
        /// Publishes a two-way question with custom answer message.
        /// </summary>
        /// <param name="question">The question.</param>
        /// <param name="possibleAnswers">The possible answers.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        Task<NotificationQuestionCustomAnswerResult> PublishQuestionWithCustomAnswerNotificationAsync(string question, IList<string> possibleAnswers, string context = AsyncSubscription.DefaultContext);

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Checks if there are subscriptions for TMessage,
        /// for a specific subscription direction
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns></returns>
        bool HasSubscriptionsFor<TMessage>(SubscriptionDirectionEnum subscriptionDirection) where TMessage : INotificationMessage;

        /// <summary>
        /// Checks if there are subscriptions for messageType,
        /// for a specific subscription direction
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns></returns>
        bool HasSubscriptionsFor(Type messageType, SubscriptionDirectionEnum subscriptionDirection);

        /// <summary>
        /// Counts the number of subscriptions for TMessage
        /// for a specific subscription direction
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns></returns>
        int CountSubscriptionsFor<TMessage>(SubscriptionDirectionEnum subscriptionDirection) where TMessage : INotificationMessage;

        /// <summary>
        /// Counts the number of subscriptions for messageType,
        /// for a specific subscription direction
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns></returns>
        int CountSubscriptionsFor(Type messageType, SubscriptionDirectionEnum subscriptionDirection);

        /// <summary>
        /// Check if there're subscriptions of TMessage for the specified context,
        /// for a specific subscription direction
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="context">Context of the subscription</param>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns></returns>
        bool HasSubscriptionsForContext<TMessage>(string context, SubscriptionDirectionEnum subscriptionDirection) where TMessage : INotificationMessage;

        /// <summary>
        /// Check if there're subscriptions of messageType for the specified context,
        /// for a specific subscription direction
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="context">The context.</param>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns></returns>
        bool HasSubscriptionsForContext(Type messageType, string context, SubscriptionDirectionEnum subscriptionDirection);

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
        int CountSubscriptionsForContext<TMessage>(string context, SubscriptionDirectionEnum subscriptionDirection) where TMessage : INotificationMessage;

        /// <summary>
        /// Counts the subscriptions of messageType for the specified context,
        /// for a specific subscription direction
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="context">The context.</param>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns></returns>
        int CountSubscriptionsForContext(Type messageType, string context, SubscriptionDirectionEnum subscriptionDirection);

        /// <summary>
        /// Get all the distinct context of subscriptions for TMessage,
        /// for a specific subscription direction
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns></returns>
        IEnumerable<string> GetSubscriptionsContextFor<TMessage>(SubscriptionDirectionEnum subscriptionDirection) where TMessage : INotificationMessage;

        /// <summary>
        /// Get all the distinct context of subscriptions for messageType,
        /// for a specific subscription direction
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns></returns>
        IEnumerable<string> GetSubscriptionsContextFor(Type messageType, SubscriptionDirectionEnum subscriptionDirection);

        /// <summary>
        /// Schedules a check on all subscribers for the specified messageType,
        /// for a specific subscription direction.
        /// The dead ones will be removed
        /// </summary>
        /// <param name="messageType">The type of the message to check</param>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns></returns>
        Task RequestPurgeAsync(Type messageType, SubscriptionDirectionEnum subscriptionDirection);

        /// <summary>
        /// Schedules a check on all subscribers for all messageType for the specified subscriber direction.
        /// The dead ones will be removed
        /// </summary>
        /// <param name="subscriptionDirection">The subscription direction.</param>
        /// <returns></returns>
        Task RequestPurgeAllAsync(SubscriptionDirectionEnum subscriptionDirection);

        #endregion
    }
}
