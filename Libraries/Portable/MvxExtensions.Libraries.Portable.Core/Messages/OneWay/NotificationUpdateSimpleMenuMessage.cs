using MvxExtensions.Plugins.Notification.Messages.Base;

namespace MvxExtensions.Libraries.Portable.Core.Messages.OneWay
{
    /// <summary>
    /// Message for update menu notification
    /// </summary>
    public class NotificationUpdateSimpleMenuMessage : NotificationOneWayMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationUpdateSimpleMenuMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        public NotificationUpdateSimpleMenuMessage(object sender)
            : base(sender)
        {
        }
    }
}
