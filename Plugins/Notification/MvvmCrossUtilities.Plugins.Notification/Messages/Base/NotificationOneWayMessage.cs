namespace MvvmCrossUtilities.Plugins.Notification.Messages.Base
{
    public abstract class NotificationOneWayMessage : NotificationMessage
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
