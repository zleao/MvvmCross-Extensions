using MvvmCrossUtilities.Plugins.Notification.Messages.Base;

namespace MvvmCrossUtilities.Libraries.Portable.Messages.OneWay
{
    /// <summary>
    /// Message for terminate application notification
    /// </summary>
    public class NotificationTerminateApplicationMessage : NotificationOneWayMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationTerminateApplicationMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public NotificationTerminateApplicationMessage(object sender)
            : base(sender)
        {
        }
    }
}
