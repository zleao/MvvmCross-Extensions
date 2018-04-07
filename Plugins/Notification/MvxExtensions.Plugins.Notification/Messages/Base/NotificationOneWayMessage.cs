
namespace MvxExtensions.Plugins.Notification.Messages.Base
{
    /// <summary>
    /// One-way notification
    /// </summary>
    public abstract class NotificationOneWayMessage : NotificationMessage, INotificationOneWayMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationOneWayMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public NotificationOneWayMessage(object sender)
            : base(sender)
        {
        }
    }
}
