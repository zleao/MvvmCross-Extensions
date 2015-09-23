namespace MvvmCrossUtilities.Plugins.Notification.Messages
{
    /// <summary>
    /// Interface for base notification messages
    /// </summary>
    public interface INotificationMessage
    {
        /// <summary>
        /// Store a WeakReference to the sender just in case anyone is daft enough to
        /// keep the message around and prevent the sender from being collected.
        /// </summary>
        object Sender { get; }
    }
}
