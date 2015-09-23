using MvvmCrossUtilities.Plugins.Notification.Core.Async.ThreadRunners;
using MvvmCrossUtilities.Plugins.Notification.Messages;
using System.Threading.Tasks;

namespace MvvmCrossUtilities.Plugins.Notification.Core.Async.Subscriptions.Void
{
    /// <summary>
    /// Asynchronous subscription that has no return information
    /// </summary>
    public abstract class VoidAsyncSubscription : AsyncSubscription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VoidAsyncSubscription" /> class.
        /// </summary>
        /// <param name="asyncActionRunner">The asynchronous action runner.</param>
        /// <param name="context">The context.</param>
        protected VoidAsyncSubscription(IAsyncActionRunner asyncActionRunner, string context)
            : base(asyncActionRunner, context)
        {
        }

        /// <summary>
        /// Executes the asynchronous action associated with this subscription
        /// </summary>
        /// <param name="message">The message.</param>
        public abstract Task InvokeAsync(INotificationMessage message);
    }
}
