using MvvmCrossUtilities.Plugins.Notification.Messages.Base;

namespace MvvmCrossUtilities.Libraries.Portable.Messages
{
    public class NotificationUpdateMenuMessage : NotificationOneWayMessage
    {
        public NotificationUpdateMenuMessage(object sender)
            : base(sender)
        {
        }
    }
}
