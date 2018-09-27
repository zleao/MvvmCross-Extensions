using MvxExtensions.Plugins.Notification.Core.Async.ThreadRunners;
using MvxExtensions.Plugins.Notification.Messages;
using System;
using System.Threading.Tasks;

namespace MvxExtensions.Plugins.Notification.Core.Async.Subscriptions.OneWay
{
    /// <summary>
    /// Represents a one-way strong reference asynchronous subscription
    /// </summary>
    public class OneWayAsyncStrongSubscription : OneWayAsyncSubscription
    {
        private readonly Func<INotificationMessage, Task> _actionAsync;

        /// <summary>
        /// Indcates if the subscriber is still connected to this subscription instance
        /// </summary>
        public override bool IsAlive
        {
            get { return true; }
        }

        /// <summary>
        /// Executes the asynchronous action associated with this subscription
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">message</exception>
        public override async Task<bool> InvokeAsync(INotificationMessage message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            await CallAsync(() => { return _actionAsync.Invoke(message); });

            return true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneWayAsyncStrongSubscription" /> class.
        /// </summary>
        /// <param name="asyncActionRunner">The asynchronous action runner.</param>
        /// <param name="actionAsync">The action asynchronous.</param>
        public OneWayAsyncStrongSubscription(IAsyncActionRunner asyncActionRunner, Func<INotificationMessage, Task> actionAsync)
            : this(asyncActionRunner, actionAsync, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneWayAsyncStrongSubscription" /> class.
        /// </summary>
        /// <param name="asyncActionRunner">The asynchronous action runner.</param>
        /// <param name="actionAsync">The action asynchronous.</param>
        /// <param name="context">The context.</param>
        public OneWayAsyncStrongSubscription(IAsyncActionRunner asyncActionRunner, Func<INotificationMessage, Task> actionAsync, string context)
            : base(asyncActionRunner, context)
        {
            _actionAsync = actionAsync;
        }
    }

    /// <summary>
    /// Represents a one-way strong reference asynchronous subscription
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    public class OneWayAsyncStrongSubscription<TMessage> : OneWayAsyncSubscription
         where TMessage : INotificationMessage
    {
        private readonly Func<TMessage, Task> _actionAsync;

        /// <summary>
        /// Indcates if the subscriber is still connected to this subscription instance
        /// </summary>
        public override bool IsAlive
        {
            get { return true; }
        }

        /// <summary>
        /// Executes the asynchronous action associated with this subscription
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        public override async Task<bool> InvokeAsync(INotificationMessage message)
        {
            if (message == null || message.GetType() != typeof(TMessage))
                throw new Exception($"Unexpected message: ({message})");

            await CallAsync(() => { return _actionAsync.Invoke((TMessage)message); });

            return true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneWayAsyncStrongSubscription{TMessage}"/> class.
        /// </summary>
        /// <param name="asyncActionRunner">The asynchronous action runner.</param>
        /// <param name="actionAsync">The action asynchronous.</param>
        public OneWayAsyncStrongSubscription(IAsyncActionRunner asyncActionRunner, Func<TMessage, Task> actionAsync)
            : this(asyncActionRunner, actionAsync, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OneWayAsyncStrongSubscription{TMessage}"/> class.
        /// </summary>
        /// <param name="asyncActionRunner">The asynchronous action runner.</param>
        /// <param name="actionAsync">The action asynchronous.</param>
        /// <param name="context">The context.</param>
        public OneWayAsyncStrongSubscription(IAsyncActionRunner asyncActionRunner, Func<TMessage, Task> actionAsync, string context)
            : base(asyncActionRunner, context)
        {
            _actionAsync = actionAsync;
        }
    }
}
