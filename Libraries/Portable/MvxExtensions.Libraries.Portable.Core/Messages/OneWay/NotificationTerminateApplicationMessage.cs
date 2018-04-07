using MvxExtensions.Plugins.Notification.Messages.Base;

namespace MvxExtensions.Libraries.Portable.Core.Messages.OneWay
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
