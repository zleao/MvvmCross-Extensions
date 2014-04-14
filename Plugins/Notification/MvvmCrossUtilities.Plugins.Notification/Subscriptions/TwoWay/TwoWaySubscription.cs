using MvvmCrossUtilities.Plugins.Notification.Exceptions;
using MvvmCrossUtilities.Plugins.Notification.Messages;
using MvvmCrossUtilities.Plugins.Notification.Messages.Base;
using MvvmCrossUtilities.Plugins.Notification.ThreadRunners;
using System;

namespace MvvmCrossUtilities.Plugins.Notification.Subscriptions.OneWay
{
    public abstract class TwoWaySubscription<TMessage, TResult> : Subscription
        where TMessage : NotificationTwoWayMessage
        where TResult : NotificationResult
    {
        protected TwoWaySubscription(IActionRunner actionRunner, string context) 
            : base(actionRunner, context)
        {
        }

        public bool Invoke(object message, Action<TResult> OnResultCallBack, Action<NotificationErrorException> OnErrorCallback)
        {
            var typedMessage = message as TMessage;
            if (typedMessage == null)
                throw new Exception("Unexpected message " + message);

            return TypedInvoke(typedMessage, OnResultCallBack, OnErrorCallback);
        }

        protected abstract bool TypedInvoke(TMessage message, Action<TResult> OnResultCallBack, Action<NotificationErrorException> OnErrorCallback);
    }
}