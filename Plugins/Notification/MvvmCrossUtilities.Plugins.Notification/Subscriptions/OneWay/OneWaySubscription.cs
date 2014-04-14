using MvvmCrossUtilities.Plugins.Notification.Messages.Base;
using MvvmCrossUtilities.Plugins.Notification.ThreadRunners;
using System;

namespace MvvmCrossUtilities.Plugins.Notification.Subscriptions.OneWay
{
    public abstract class OneWaySubscription<TMessage> : Subscription
        where TMessage : NotificationOneWayMessage
    {
        protected OneWaySubscription(IActionRunner actionRunner, string context) 
            : base(actionRunner, context)
        {
        }

        public bool Invoke(object message)
        {
            var typedMessage = message as TMessage;
            if (typedMessage == null)
                throw new Exception("Unexpected message " + message);

            return TypedInvoke(typedMessage);
        }

        protected abstract bool TypedInvoke(TMessage message);
    }
}