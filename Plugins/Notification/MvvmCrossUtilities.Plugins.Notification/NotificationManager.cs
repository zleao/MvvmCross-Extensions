using System;
using System.Collections.Generic;
using System.Linq;
using Cirrious.CrossCore.Core;
using MvvmCrossUtilities.Plugins.Notification.Exceptions;
using MvvmCrossUtilities.Plugins.Notification.Messages;
using MvvmCrossUtilities.Plugins.Notification.Messages.Base;
using MvvmCrossUtilities.Plugins.Notification.Subscriptions;
using MvvmCrossUtilities.Plugins.Notification.Subscriptions.OneWay;
using MvvmCrossUtilities.Plugins.Notification.ThreadRunners;

namespace MvvmCrossUtilities.Plugins.Notification
{
    public class NotificationManager : INotificationService
    {
        #region Fields

        private readonly Dictionary<Type, List<Subscription>> _subscriptions = new Dictionary<Type, List<Subscription>>();

        private readonly Dictionary<Type, bool> _scheduledPurges = new Dictionary<Type, bool>();

        #endregion

        #region Subscribe/Unsubscribe message

        #region OneWay Subscription

        public SubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction) where TMessage : NotificationOneWayMessage
        {
            return Subscribe<TMessage>(deliveryAction, Subscription.DefaultContext);
        }

        public SubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, string context) where TMessage : NotificationOneWayMessage
        {
            return Subscribe<TMessage>(deliveryAction, context, SubscriptionTypeEnum.Weak);
        }

        public SubscriptionToken Subscribe<TMessage>(Action<TMessage> deliveryAction, string context, SubscriptionTypeEnum reference) where TMessage : NotificationOneWayMessage
        {
            return SubscribeInternal(deliveryAction, new SimpleActionRunner(), reference, context);
        }

        public SubscriptionToken SubscribeOnMainThread<TMessage>(Action<TMessage> deliveryAction, string context = Subscription.DefaultContext, SubscriptionTypeEnum reference = SubscriptionTypeEnum.Weak) where TMessage : NotificationOneWayMessage
        {
            return SubscribeInternal(deliveryAction, new MainThreadActionRunner(), reference, context);
        }

        public SubscriptionToken SubscribeOnThreadPoolThread<TMessage>(Action<TMessage> deliveryAction, string context = Subscription.DefaultContext, SubscriptionTypeEnum reference = SubscriptionTypeEnum.Weak) where TMessage : NotificationOneWayMessage
        {
            return SubscribeInternal(deliveryAction, new ThreadPoolActionRunner(), reference, context);
        }

        private SubscriptionToken SubscribeInternal<TMessage>(Action<TMessage> deliveryAction, IActionRunner actionRunner, SubscriptionTypeEnum reference, string context) where TMessage : NotificationOneWayMessage
        {
            if (deliveryAction == null)
            {
                throw new ArgumentNullException("deliveryAction");
            }

            Subscription subscription;

            switch (reference)
            {
                case SubscriptionTypeEnum.Strong:
                    subscription = new OneWayStrongSubscription<TMessage>(actionRunner, deliveryAction, context);
                    break;
                case SubscriptionTypeEnum.Weak:
                    subscription = new OneWayWeakSubscription<TMessage>(actionRunner, deliveryAction, context);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("reference", "reference type unexpected " + reference);
            }

            lock (this)
            {
                List<Subscription> messageSubscriptions;

                if (!_subscriptions.TryGetValue(typeof(TMessage), out messageSubscriptions))
                {
                    messageSubscriptions = new List<Subscription>();
                    _subscriptions[typeof(TMessage)] = messageSubscriptions;
                }

                messageSubscriptions.Add(subscription);

                PublishSubscriberChangeMessage<TMessage>(messageSubscriptions);
            }

            return new SubscriptionToken(typeof(TMessage), context, subscription.Id, deliveryAction);
        }

        #endregion

        #region TwoWay Subscription

        public SubscriptionToken Subscribe<TMessage, TResult>(Func<TMessage, TResult> deliveryAction)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult
        {
            return Subscribe<TMessage, TResult>(deliveryAction, Subscription.DefaultContext);
        }

        public SubscriptionToken Subscribe<TMessage, TResult>(Func<TMessage, TResult> deliveryAction, string context)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult
        {
            return Subscribe<TMessage, TResult>(deliveryAction, context, SubscriptionTypeEnum.Weak);
        }

        public SubscriptionToken Subscribe<TMessage, TResult>(Func<TMessage, TResult> deliveryAction, string context, SubscriptionTypeEnum reference)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult
        {
            return SubscribeInternal<TMessage, TResult>(deliveryAction, new SimpleActionRunner(), reference, context);
        }

        public SubscriptionToken SubscribeOnMainThread<TMessage, TResult>(Func<TMessage, TResult> deliveryAction, string context = Subscription.DefaultContext, SubscriptionTypeEnum reference = SubscriptionTypeEnum.Weak)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult
        {
            return SubscribeInternal(deliveryAction, new MainThreadActionRunner(), reference, context);
        }

        public SubscriptionToken SubscribeOnThreadPoolThread<TMessage, TResult>(Func<TMessage, TResult> deliveryAction, string context = Subscription.DefaultContext, SubscriptionTypeEnum reference = SubscriptionTypeEnum.Weak)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult
        {
            return SubscribeInternal(deliveryAction, new ThreadPoolActionRunner(), reference, context);
        }

        private SubscriptionToken SubscribeInternal<TMessage, TResult>(Func<TMessage, TResult> deliveryAction, IActionRunner actionRunner, SubscriptionTypeEnum reference, string context)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult
        {
            if (deliveryAction == null)
            {
                throw new ArgumentNullException("deliveryAction");
            }

            Subscription subscription;

            switch (reference)
            {
                case SubscriptionTypeEnum.Strong:
                    subscription = new TwoWayStrongSubscription<TMessage, TResult>(actionRunner, deliveryAction, context);
                    break;
                case SubscriptionTypeEnum.Weak:
                    subscription = new TwoWayWeakSubscription<TMessage, TResult>(actionRunner, deliveryAction, context);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("reference", "reference type unexpected " + reference);
            }

            lock (this)
            {
                List<Subscription> messageSubscriptions;

                if (!_subscriptions.TryGetValue(typeof(TMessage), out messageSubscriptions))
                {
                    messageSubscriptions = new List<Subscription>();
                    _subscriptions[typeof(TMessage)] = messageSubscriptions;
                }

                messageSubscriptions.Add(subscription);

                PublishSubscriberChangeMessage<TMessage>(messageSubscriptions);
            }

            return new SubscriptionToken(typeof(TMessage), context, subscription.Id, deliveryAction);
        }

        #endregion

        #region Unsubscription

        public void Unsubscribe(SubscriptionToken subscriptionToken)
        {
            if (subscriptionToken != null)
            {
                lock (this)
                {
                    List<Subscription> messageSubscriptions;

                    if (_subscriptions.TryGetValue(subscriptionToken.MessageType, out messageSubscriptions))
                    {
                        var subscriptionToRemove = messageSubscriptions.FirstOrDefault(s => s.Id.Equals(subscriptionToken.Id));
                        if (subscriptionToRemove != null)
                        {
                            messageSubscriptions.Remove(subscriptionToRemove);

                            PublishSubscriberChangeMessage(subscriptionToken.MessageType, messageSubscriptions);
                        }
                    }
                }
            }
        }

        public void Unsubscribe<TMessage>() where TMessage : NotificationMessage
        {
            var messageType = typeof(TMessage);
            lock (this)
            {
                List<Subscription> messageSubscriptions;

                if (_subscriptions.TryGetValue(messageType, out messageSubscriptions))
                {
                    if (messageSubscriptions.Count > 0)
                    {
                        foreach (var subscription in messageSubscriptions)
                        {
                            messageSubscriptions.Remove(subscription);
                        }

                        PublishSubscriberChangeMessage(messageType, messageSubscriptions);
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Publish message

        public void Publish<TMessage>(TMessage message) where TMessage : NotificationOneWayMessage
        {
            Publish<TMessage>(message, Subscription.DefaultContext);
        }

        public void Publish<TMessage>(TMessage message, string context) where TMessage : NotificationOneWayMessage
        {
            if (message == null)
                throw new ArgumentNullException("message");

            var messageType = typeof(TMessage);
            if (messageType == typeof(NotificationMessage))
            {
                return;
            }


            var toNotify = GetSubscriptionsFor<TMessage>(context);
            if (toNotify == null || toNotify.Count() == 0)
            {
                return;
            }

            var allSucceeded = true;
            foreach (var subscription in toNotify)
            {
                var typedSubscription = subscription as OneWaySubscription<TMessage>;
                allSucceeded &= typedSubscription.Invoke(message);
            }

            if (!allSucceeded)
            {
                SchedulePurge(messageType);
            }
        }


        public void Publish<TMessage, TResult>(TMessage message, Action<TResult> OnResultCallback, Action<NotificationErrorException> OnErrorCallback)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult
        {
            Publish<TMessage, TResult>(message, OnResultCallback, OnErrorCallback, Subscription.DefaultContext);
        }

        public void Publish<TMessage, TResult>(TMessage message, Action<TResult> OnResultCallback, Action<NotificationErrorException> OnErrorCallback, string context)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult
        {
            if (message == null)
                throw new ArgumentNullException("message");

            var messageType = typeof(TMessage);
            if (messageType == typeof(NotificationMessage))
            {
                return;
            }


            var toNotify = GetSubscriptionsFor<TMessage>(context);
            if (toNotify == null || toNotify.Count() == 0)
            {
                return;
            }

            var allSucceeded = true;
            foreach (var subscription in toNotify)
            {
                var typedSubscription = subscription as TwoWaySubscription<TMessage, TResult>;
                allSucceeded &= typedSubscription.Invoke(message, OnResultCallback, OnErrorCallback);
            }

            if (!allSucceeded)
            {
                SchedulePurge(messageType);
            }
        }

        #endregion

        #region General methods

        private IEnumerable<Subscription> GetSubscriptionsFor<TMessage>(string context) where TMessage : NotificationMessage
        {
            lock (this)
            {
                List<Subscription> messageSubscriptions;
                if (!_subscriptions.TryGetValue(typeof(TMessage), out messageSubscriptions))
                {
                    return null;
                }

                //The .ToArray() is used to create a static list.
                return (messageSubscriptions.Where(s => s.Context == context)).ToArray();
            }
        }


        protected virtual void PublishSubscriberChangeMessage<TMessage>(List<Subscription> messageSubscriptions) where TMessage : NotificationMessage
        {
            PublishSubscriberChangeMessage(typeof(TMessage), messageSubscriptions);
        }

        protected virtual void PublishSubscriberChangeMessage(Type messageType, List<Subscription> messageSubscriptions)
        {
            //var newCount = messageSubscriptions == null ? 0 : messageSubscriptions.Count;
            //Publish(new NotificationSubscriberChangeMessage(this, messageType, newCount));
        }


        public bool HasSubscriptionsFor<TMessage>() where TMessage : NotificationMessage
        {
            lock (this)
            {
                List<Subscription> messageSubscriptions;
                if (!_subscriptions.TryGetValue(typeof(TMessage), out messageSubscriptions))
                {
                    return false;
                }
                return messageSubscriptions.Any();
            }
        }

        public int CountSubscriptionsFor<TMessage>() where TMessage : NotificationMessage
        {
            lock (this)
            {
                List<Subscription> messageSubscriptions;
                if (!_subscriptions.TryGetValue(typeof(TMessage), out messageSubscriptions))
                {
                    return 0;
                }
                return messageSubscriptions.Count;
            }
        }

        public bool HasSubscriptionsForContext<TMessage>(string context) where TMessage : NotificationMessage
        {
            lock (this)
            {
                List<Subscription> messageSubscriptions;
                if (!_subscriptions.TryGetValue(typeof(TMessage), out messageSubscriptions))
                {
                    return false;
                }
                return messageSubscriptions.Any(x => x.Context == context);
            }
        }

        public int CountSubscriptionsForContext<TMessage>(string context) where TMessage : NotificationMessage
        {
            lock (this)
            {
                List<Subscription> messageSubscriptions;
                if (!_subscriptions.TryGetValue(typeof(TMessage), out messageSubscriptions))
                {
                    return 0;
                }
                return messageSubscriptions.Count(x => x.Context == context);
            }
        }

        public IEnumerable<string> GetSubscriptionsContextFor<TMessage>() where TMessage : NotificationMessage
        {
            lock (this)
            {
                List<Subscription> messageSubscriptions;
                if (!_subscriptions.TryGetValue(typeof(TMessage), out messageSubscriptions))
                {
                    return new List<string>(0);
                }

                //The .ToArray() is used to create a static list.
                return (messageSubscriptions.Select(x => x.Context).Distinct()).ToArray();
            }
        }

        #endregion

        #region Message purging

        public void RequestPurge(Type messageType)
        {
            SchedulePurge(messageType);
        }

        public void RequestPurgeAll()
        {
            lock (this)
            {
                SchedulePurge(_subscriptions.Keys.ToArray());
            }
        }

        private void SchedulePurge(params Type[] messageTypes)
        {
            lock (this)
            {
                var threadPoolTaskAlreadyRequested = _scheduledPurges.Count > 0;
                foreach (var messageType in messageTypes)
                    _scheduledPurges[messageType] = true;

                if (!threadPoolTaskAlreadyRequested)
                {
                    MvxAsyncDispatcher.BeginAsync(DoPurge);
                }
            }
        }

        private void DoPurge()
        {
            List<Type> toPurge = null;
            lock (this)
            {
                toPurge = _scheduledPurges.Select(x => x.Key).ToList();
                _scheduledPurges.Clear();
            }

            foreach (var type in toPurge)
            {
                PurgeMessagesOfType(type);
            }
        }

        private void PurgeMessagesOfType(Type type)
        {
            lock (this)
            {
                List<Subscription> messageSubscriptions;
                if (!_subscriptions.TryGetValue(type, out messageSubscriptions))
                {
                    return;
                }

                var deadSubscriptions = new List<Subscription>();
                deadSubscriptions.AddRange(messageSubscriptions.Where(s => !s.IsAlive));

                foreach (var item in deadSubscriptions)
                {
                    messageSubscriptions.Remove(item);
                }

                PublishSubscriberChangeMessage(type, messageSubscriptions);
            }
        }

        #endregion
    }
}
