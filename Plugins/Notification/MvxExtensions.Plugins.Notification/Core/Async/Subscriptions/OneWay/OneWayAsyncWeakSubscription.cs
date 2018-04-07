using MvxExtensions.Plugins.Notification.Core.Async.ThreadRunners;
using MvxExtensions.Plugins.Notification.Messages;
using System;
using System.Threading.Tasks;

namespace MvxExtensions.Plugins.Notification.Core.Async.Subscriptions.OneWay
{
    /// <summary>
    /// Represents a one-way weak reference asynchronous subscription
    /// </summary>
    public class OneWayAsyncWeakSubscription : OneWayAsyncSubscription
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
        public override async Task<bool> InvokeAsync(INotificationMessage message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            if (!_weakReference.IsAlive)
                return false;

            var actionAsync = _weakReference.Target as Func<INotificationMessage, Task>;
            if (actionAsync == null)
                return false;

            await CallAsync(() => { return actionAsync.Invoke(message); });

            return true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneWayAsyncWeakSubscription"/> class.
        /// </summary>
        /// <param name="asyncActionRunner">The asynchronous action runner.</param>
        /// <param name="actionAsync">The action asynchronous.</param>
        public OneWayAsyncWeakSubscription(IAsyncActionRunner asyncActionRunner, Func<INotificationMessage, Task> actionAsync)
            : this(asyncActionRunner, actionAsync, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneWayAsyncWeakSubscription"/> class.
        /// </summary>
        /// <param name="asyncActionRunner">The asynchronous action runner.</param>
        /// <param name="actionAsync">The action asynchronous.</param>
        /// <param name="context">The context.</param>
        public OneWayAsyncWeakSubscription(IAsyncActionRunner asyncActionRunner, Func<INotificationMessage, Task> actionAsync, string context)
            : base(asyncActionRunner, context)
        {
            _weakReference = new WeakReference(actionAsync);
        }
    }

    /// <summary>
    /// Represents a one-way weak reference asynchronous subscription
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    public class OneWayAsyncWeakSubscription<TMessage> : OneWayAsyncSubscription
         where TMessage : INotificationMessage
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
        public override async Task<bool> InvokeAsync(INotificationMessage message)
        {
            if (message == null || message.GetType() != typeof(TMessage))
                throw new Exception($"Unexpected message: ({message})");

            if (!_weakReference.IsAlive)
                return false;

            var actionAsync = _weakReference.Target as Func<TMessage, Task>;
            if (actionAsync == null)
                return false;

            await CallAsync(() => { return actionAsync.Invoke((TMessage)message); });

            return true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneWayAsyncWeakSubscription{TMessage}"/> class.
        /// </summary>
        /// <param name="asyncActionRunner">The asynchronous action runner.</param>
        /// <param name="actionAsync">The action asynchronous.</param>
        public OneWayAsyncWeakSubscription(IAsyncActionRunner asyncActionRunner, Func<TMessage, Task> actionAsync)
            : this(asyncActionRunner, actionAsync, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneWayAsyncWeakSubscription{TMessage}"/> class.
        /// </summary>
        /// <param name="asyncActionRunner">The asynchronous action runner.</param>
        /// <param name="actionAsync">The action asynchronous.</param>
        /// <param name="context">The context.</param>
        public OneWayAsyncWeakSubscription(IAsyncActionRunner asyncActionRunner, Func<TMessage, Task> actionAsync, string context)
            : base(asyncActionRunner, context)
        {
            _weakReference = new WeakReference(actionAsync);
        }
    }
}
