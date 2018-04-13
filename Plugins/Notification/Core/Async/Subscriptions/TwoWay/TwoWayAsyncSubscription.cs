using MvxExtensions.Plugins.Notification.Core.Async.ThreadRunners;
using MvxExtensions.Plugins.Notification.Messages;
using System.Threading.Tasks;

namespace MvxExtensions.Plugins.Notification.Core.Async.Subscriptions.TwoWay
{
    /// <summary>
    /// Two-way asynchronous subscription
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public abstract class TwoWayAsyncSubscription<TResult> : AsyncSubscription<TResult>
        where TResult : INotificationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TwoWayAsyncSubscription{TResult}" /> class.
        /// </summary>
        /// <param name="asyncActionRunner">The asynchronous action runner.</param>
        /// <param name="context">The context.</param>
        protected TwoWayAsyncSubscription(IAsyncActionRunner<TResult> asyncActionRunner, string context)
            : base(asyncActionRunner, context)
        {
        }

        /// <summary>
        /// Executes the asynchronous action associated with this subscription
        /// </summary>
        /// <param name="message">The message.</param>
        public abstract Task<TResult> InvokeAsync(INotificationMessage message);
    }
}
