using MvxExtensions.Plugins.Notification.Core.Async.ThreadRunners;
using MvxExtensions.Plugins.Notification.Messages;
using System.Threading.Tasks;

namespace MvxExtensions.Plugins.Notification.Core.Async.Subscriptions.OneWay
{
    /// <summary>
    /// Asynchronous one-way asynchronous subscription
    /// </summary>
    public abstract class OneWayAsyncSubscription : AsyncSubscription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneWayAsyncSubscription" /> class.
        /// </summary>
        /// <param name="asyncActionRunner">The asynchronous action runner.</param>
        /// <param name="context">The context.</param>
        protected OneWayAsyncSubscription(IAsyncActionRunner asyncActionRunner, string context)
            : base(asyncActionRunner, context)
        {
        }

        /// <summary>
        /// Executes the asynchronous action associated with this subscription
        /// </summary>
        /// <param name="message">The message.</param>
        public abstract Task<bool> InvokeAsync(INotificationMessage message);
    }
}
