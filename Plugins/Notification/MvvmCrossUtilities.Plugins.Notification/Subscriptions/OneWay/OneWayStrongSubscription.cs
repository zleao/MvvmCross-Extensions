using System;
using MvvmCrossUtilities.Plugins.Notification.Messages.Base;
using MvvmCrossUtilities.Plugins.Notification.ThreadRunners;

namespace MvvmCrossUtilities.Plugins.Notification.Subscriptions.OneWay
{
    public class OneWayStrongSubscription<TMessage> : OneWaySubscription<TMessage>
        where TMessage : NotificationOneWayMessage
    {
        private readonly Action<TMessage> _action;

        public override bool IsAlive
        {
            get { return true; }
        }

        protected override bool TypedInvoke(TMessage message)
        {
            Call(() => _action(message));
            return true;
        }

        public OneWayStrongSubscription(IActionRunner actionRunner, Action<TMessage> action)
            : this(actionRunner, action, null)
        {
        }

        public OneWayStrongSubscription(IActionRunner actionRunner, Action<TMessage> action, string context)
            : base(actionRunner, context)
        {
            _action = action;
        }
    }
}
