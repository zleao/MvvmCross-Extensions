using MvvmCrossUtilities.Plugins.Notification.Exceptions;
using MvvmCrossUtilities.Plugins.Notification.Messages;
using MvvmCrossUtilities.Plugins.Notification.Messages.Base;
using MvvmCrossUtilities.Plugins.Notification.ThreadRunners;
using System;

namespace MvvmCrossUtilities.Plugins.Notification.Subscriptions.OneWay
{
    public class TwoWayStrongSubscription<TMessage, TResult> : TwoWaySubscription<TMessage, TResult>
        where TMessage : NotificationTwoWayMessage
        where TResult : NotificationResult
    {
        private readonly Func<TMessage, TResult> _action;

        public override bool IsAlive
        {
            get { return true; }
        }

        protected override bool TypedInvoke(TMessage message, Action<TResult> OnResultCallBack, Action<NotificationErrorException> OnErrorCallback)
        {
            Call(() =>
            {
                try
                {
                    var result = _action(message);

                    OnResultCallBack(result);
                }
                catch (Exception ex)
                {
                    OnErrorCallback(new NotificationErrorException(ex));
                }
            });

            return true;
        }

        public TwoWayStrongSubscription(IActionRunner actionRunner, Func<TMessage, TResult> action)
            : this(actionRunner, action, null)
        {
        }

        public TwoWayStrongSubscription(IActionRunner actionRunner, Func<TMessage, TResult> action, string context)
            : base(actionRunner, context)
        {
            _action = action;
        }
    }
}
