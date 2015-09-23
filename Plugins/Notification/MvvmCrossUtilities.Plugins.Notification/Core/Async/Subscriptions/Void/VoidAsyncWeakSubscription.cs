using MvvmCrossUtilities.Plugins.Notification.Core.Async.ThreadRunners;
using MvvmCrossUtilities.Plugins.Notification.Messages;
using System;
using System.Threading.Tasks;

namespace MvvmCrossUtilities.Plugins.Notification.Core.Async.Subscriptions.Void
{
    /// <summary>
    /// Represents a weak reference void asynchronous subscription
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    public class VoidAsyncWeakSubscription<TMessage> : VoidAsyncSubscription
         where TMessage : NotificationMessage
    {
        private readonly WeakReference _weakReference;

        /// <summary>
        /// Indcates if the subscriber is still connected to this subscription instance
        /// </summary>
        public override bool IsAlive
        {
            get { return _weakReference.IsAlive; }
        }

        /// <summary>
        /// Executes the asynchronous action associated with this subscription
        /// </summary>
        /// <param name="message">The message.</param>
        public override Task InvokeAsync(INotificationMessage message)
        {
            var typedMessage = message as TMessage;
            if (typedMessage == null)
                throw new Exception("Unexpected message " + message);

            if (!_weakReference.IsAlive)
                return Task.FromResult(false);

            var actionAsync = _weakReference.Target as Func<TMessage, Task>;
            if (actionAsync == null)
                return Task.FromResult(false);

            return CallAsync(() => { return actionAsync.Invoke(typedMessage); });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoidAsyncWeakSubscription{TMessage}"/> class.
        /// </summary>
        /// <param name="asyncActionRunner">The asynchronous action runner.</param>
        /// <param name="actionAsync">The action asynchronous.</param>
        public VoidAsyncWeakSubscription(IAsyncActionRunner asyncActionRunner, Func<TMessage, Task> actionAsync)
            : this(asyncActionRunner, actionAsync, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoidAsyncWeakSubscription{TMessage}"/> class.
        /// </summary>
        /// <param name="asyncActionRunner">The asynchronous action runner.</param>
        /// <param name="actionAsync">The action asynchronous.</param>
        /// <param name="context">The context.</param>
        public VoidAsyncWeakSubscription(IAsyncActionRunner asyncActionRunner, Func<TMessage, Task> actionAsync, string context)
            : base(asyncActionRunner, context)
        {
            _weakReference = new WeakReference(actionAsync);
        }
    }
}
