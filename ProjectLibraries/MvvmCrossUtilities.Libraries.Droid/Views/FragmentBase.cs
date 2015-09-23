using Cirrious.CrossCore;
using Cirrious.MvvmCross.Droid.FullFragging.Fragments;
using MvvmCrossUtilities.Libraries.Portable.ViewModels;
using MvvmCrossUtilities.Plugins.Notification;
using MvvmCrossUtilities.Plugins.Notification.Core;
using MvvmCrossUtilities.Plugins.Notification.Core.Async.Subscriptions;
using MvvmCrossUtilities.Plugins.Notification.Messages;
using MvvmCrossUtilities.Plugins.Notification.Messages.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MvvmCrossUtilities.Libraries.Droid.Views
{
    public abstract class FragmentBase : MvxFragment
    {
        #region Fields

        private volatile IList<SubscriptionToken> _messageTokens = new List<SubscriptionToken>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the typed view model.
        /// </summary>
        /// <value>
        /// The typed view model.
        /// </value>
        private ViewModel TypedViewModel
        {
            get { return ViewModel as ViewModel; }
        }

        /// <summary>
        /// Gets the notification manager.
        /// </summary>
        /// <value>
        /// The notification manager.
        /// </value>
        protected INotificationService NotificationManager
        {
            get { return _notificationManager ?? (_notificationManager = Mvx.Resolve<INotificationService>()); }
        }
        private INotificationService _notificationManager;

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Called when resume.
        /// </summary>
        public override void OnResume()
        {
            SubscribeMessageEvents();

            base.OnResume();
        }

        /// <summary>
        /// Called when pause.
        /// </summary>
        public override void OnPause()
        {
            UnsubscribeMessageEvents();

            base.OnPause();
        }

        /// <summary>
        /// Called when stop.
        /// </summary>
        public override void OnStop()
        {
            UnsubscribeMessageEvents();

            base.OnStop();
        }

        #endregion

        #region Notification Management

        /// <summary>
        /// Publishes the specified message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message.</param>
        protected Task PublishAsync<TMessage>(TMessage message, string context = AsyncSubscription.DefaultContext) where TMessage : NotificationOneWayMessage
        {
            return NotificationManager.PublishAsync(message, context);
        }

        /// <summary>
        /// Subscribes the event.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="deliveryAction">The delivery action.</param>
        protected void SubscribeEvent<TMessage>(Func<TMessage, Task> asyncDeliveryAction, string context = AsyncSubscription.DefaultContext) where TMessage : NotificationOneWayMessage
        {
            var token = NotificationManager.Subscribe<TMessage>(asyncDeliveryAction, context);
            _messageTokens.Add(token);
        }

        /// <summary>
        /// Subscribes two way events
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="deliveryAction">The delivery action.</param>
        protected void SubscribeEvent<TMessage, TResult>(Func<TMessage, Task<TResult>> asyncDeliveryAction, string context = AsyncSubscription.DefaultContext)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult
        {
            var token = NotificationManager.Subscribe<TMessage, TResult>(asyncDeliveryAction, context);
            _messageTokens.Add(token);
        }


        /// <summary>
        /// Subscribes the message events.
        /// </summary>
        protected virtual void SubscribeMessageEvents()
        {
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

        #endregion
    }
}