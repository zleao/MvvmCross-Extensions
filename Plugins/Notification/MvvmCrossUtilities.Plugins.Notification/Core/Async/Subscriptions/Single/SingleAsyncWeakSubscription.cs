using MvvmCrossUtilities.Plugins.Notification.Core.Async.ThreadRunners;
using MvvmCrossUtilities.Plugins.Notification.Messages;
using System;
using System.Threading.Tasks;

namespace MvvmCrossUtilities.Plugins.Notification.Core.Async.Subscriptions.Single
{
    /// <summary>
    /// Represents a weak reference asynchronous subscription
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class SingleAsyncWeakSubscription<TMessage, TResult> : SingleAsyncSubscription<TResult>
        where TMessage : NotificationMessage
        where TResult : NotificationResult
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
        /// <returns></returns>
        public override Task<TResult> InvokeAsync(INotificationMessage message)
        {
            var typedMessage = message as TMessage;
            if (typedMessage == null)
                throw new Exception("Unexpected message " + message);

            if (!_weakReference.IsAlive)
                return Task.FromResult(default(TResult));

            var actionAsync = _weakReference.Target as Func<TMessage, Task<TResult>>;
            if (actionAsync == null)
                return Task.FromResult(default(TResult));

            return CallAsync(() => { return actionAsync.Invoke(typedMessage); });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleAsyncWeakSubscription{TMessage, TResult}"/> class.
        /// </summary>
        /// <param name="asyncActionRunner">The asynchronous action runner.</param>
        /// <param name="actionAsync">The action asynchronous.</param>
        public SingleAsyncWeakSubscription(IAsyncActionRunner<TResult> asyncActionRunner, Func<TMessage, Task<TResult>> actionAsync)
            : this(asyncActionRunner, actionAsync, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleAsyncWeakSubscription{TMessage, TResult}"/> class.
        /// </summary>
        /// <param name="asyncActionRunner">The asynchronous action runner.</param>
        /// <param name="actionAsync">The action asynchronous.</param>
        /// <param name="context">The context.</param>
        public SingleAsyncWeakSubscription(IAsyncActionRunner<TResult> asyncActionRunner, Func<TMessage, Task<TResult>> actionAsync, string context)
            : base(asyncActionRunner, context)
        {
            _weakReference = new WeakReference(actionAsync);
        }
    }
}
