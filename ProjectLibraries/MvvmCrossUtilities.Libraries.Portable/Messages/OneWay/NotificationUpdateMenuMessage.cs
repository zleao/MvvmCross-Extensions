using MvvmCrossUtilities.Plugins.Notification.Messages.Base;

namespace MvvmCrossUtilities.Libraries.Portable.Messages.OneWay
{
    /// <summary>
    /// Message for update menu notification
    /// </summary>
    public class NotificationUpdateMenuMessage : NotificationOneWayMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationUpdateMenuMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public NotificationUpdateMenuMessage(object sender)
            : base(sender)
        {
        }
    }
}
