namespace MvvmCrossUtilities.Plugins.Notification.Messages.OneWay
{
    public class NotificationLongRunningGenericMessage : NotificationGenericMessage
    {
        public NotificationLongRunningGenericMessage(object sender, string message)
            : base(sender, message)
        {
        }

        public NotificationLongRunningGenericMessage(object sender, string message, NotificationModeEnum mode)
            : base(sender, message, mode)
        {
        }

        public NotificationLongRunningGenericMessage(object sender, string message, NotificationModeEnum mode, NotificationSeverityEnum severity)
            : base(sender, message, mode, severity)
        {
        }
    }
}
