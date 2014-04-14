using MvvmCrossUtilities.Plugins.Notification.Exceptions;
using MvvmCrossUtilities.Plugins.Notification.Messages;
using MvvmCrossUtilities.Plugins.Notification.Messages.Base;
using MvvmCrossUtilities.Plugins.Notification.ThreadRunners;
using System;

namespace MvvmCrossUtilities.Plugins.Notification.Subscriptions.OneWay
{
    public class TwoWayWeakSubscription<TMessage, TResult> : TwoWaySubscription<TMessage, TResult>
        where TMessage : NotificationTwoWayMessage
        where TResult : NotificationResult
    {
        private readonly WeakReference _weakReference;

        public override bool IsAlive
        {
            get { return _weakReference.IsAlive; }
        }

        protected override bool TypedInvoke(TMessage message, Action<TResult> OnResultCallBack, Action<NotificationErrorException> OnErrorCallback)
        {
            if (!_weakReference.IsAlive)
                return false;

            var action = _weakReference.Target as Func<TMessage, TResult>;
            if (action == null)
                return false;

            Call(() => 
            {
                try
                {
                    var result = action(message);

                    OnResultCallBack(result);
                }
                catch (Exception ex)
                {
                    OnErrorCallback(new NotificationErrorException(ex));
                }
            });

            return true;
        }

        public TwoWayWeakSubscription(IActionRunner actionRunner, Func<TMessage, TResult> listener)
            : this(actionRunner, listener, null)
        {
        }

        public TwoWayWeakSubscription(IActionRunner actionRunner, Func<TMessage, TResult> listener, string context)
            : base(actionRunner, context)
        {
            _weakReference = new WeakReference(listener);
        }
    }
}