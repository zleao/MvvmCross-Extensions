using MvxExtensions.Plugins.Notification.Core.Async.ThreadRunners;
using MvxExtensions.Plugins.Notification.Messages;
using System;
using System.Threading.Tasks;

namespace MvxExtensions.Plugins.Notification.Core.Async.Subscriptions.TwoWay
{
    /// <summary>
    /// Represents a two-way strong reference asynchronous subscription
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public class TwoWayAsyncStrongSubscription<TMessage, TResult> : TwoWayAsyncSubscription<TResult>
        where TMessage : INotificationMessage
        where TResult : INotificationResult
    {
        private readonly Func<TMessage, Task<TResult>> _actionAsync;

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
        public override Task<TResult> InvokeAsync(INotificationMessage message)
        {
            if (message == null || message.GetType() != typeof(TMessage))
                throw new Exception($"Unexpected message: ({message})");

            return CallAsync(() => { return _actionAsync.Invoke((TMessage)message); });
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwoWayAsyncStrongSubscription{TMessage, TResult}"/> class.
        /// </summary>
        /// <param name="asyncActionRunner">The asynchronous action runner.</param>
        /// <param name="actionAsync">The action asynchronous.</param>
        public TwoWayAsyncStrongSubscription(IAsyncActionRunner<TResult> asyncActionRunner, Func<TMessage, Task<TResult>> actionAsync)
            : this(asyncActionRunner, actionAsync, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TwoWayAsyncStrongSubscription{TMessage, TResult}"/> class.
        /// </summary>
        /// <param name="asyncActionRunner">The asynchronous action runner.</param>
        /// <param name="actionAsync">The action asynchronous.</param>
        /// <param name="context">The context.</param>
        public TwoWayAsyncStrongSubscription(IAsyncActionRunner<TResult> asyncActionRunner, Func<TMessage, Task<TResult>> actionAsync, string context)
            : base(asyncActionRunner, context)
        {
            _actionAsync = actionAsync;
        }
    }
}
