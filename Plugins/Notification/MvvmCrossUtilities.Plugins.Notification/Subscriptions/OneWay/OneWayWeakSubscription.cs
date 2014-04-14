using MvvmCrossUtilities.Plugins.Notification.Messages.Base;
using MvvmCrossUtilities.Plugins.Notification.ThreadRunners;
using System;

namespace MvvmCrossUtilities.Plugins.Notification.Subscriptions.OneWay
{
    public class OneWayWeakSubscription<TMessage> : OneWaySubscription<TMessage>
        where TMessage : NotificationOneWayMessage
    {
        private readonly WeakReference _weakReference;

        public override bool IsAlive
        {
            get { return _weakReference.IsAlive; }
        }

        protected override bool TypedInvoke(TMessage message)
        {
            if (!_weakReference.IsAlive)
                return false;

            var action = _weakReference.Target as Action<TMessage>;
            if (action == null)
                return false;

            Call(() => action(message));
            return true;
        }

        public OneWayWeakSubscription(IActionRunner actionRunner, Action<TMessage> listener)
            : this(actionRunner, listener,  null)
        {
        }

        public OneWayWeakSubscription(IActionRunner actionRunner, Action<TMessage> listener, string context)
            : base(actionRunner, context)
        {
            _weakReference = new WeakReference(listener);
        }
    }
}