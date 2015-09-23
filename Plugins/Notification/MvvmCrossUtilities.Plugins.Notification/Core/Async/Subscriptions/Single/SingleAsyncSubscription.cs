using MvvmCrossUtilities.Plugins.Notification.Core.Async.ThreadRunners;
using MvvmCrossUtilities.Plugins.Notification.Messages;
using System.Threading.Tasks;

namespace MvvmCrossUtilities.Plugins.Notification.Core.Async.Subscriptions.Single
{
    /// <summary>
    /// Single asynchronous subscription
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    public abstract class SingleAsyncSubscription<TResult> : AsyncSubscription<TResult>
        where TResult : NotificationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleAsyncSubscription{TResult}" /> class.
        /// </summary>
        /// <param name="asyncActionRunner">The asynchronous action runner.</param>
        /// <param name="context">The context.</param>
        protected SingleAsyncSubscription(IAsyncActionRunner<TResult> asyncActionRunner, string context)
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
