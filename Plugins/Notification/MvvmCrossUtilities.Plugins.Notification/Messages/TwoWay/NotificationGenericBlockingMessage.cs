using MvvmCrossUtilities.Plugins.Notification.Messages.Base;

namespace MvvmCrossUtilities.Plugins.Notification.Messages.TwoWay
{
    public class NotificationGenericBlockingMessage : NotificationTwoWayMessage
    {
        public NotificationSeverityEnum Severity
        {
            get { return _severity; }
        }
        private readonly NotificationSeverityEnum _severity;

        public string Message
        {
            get { return _message; }
        }
        private readonly string _message;

        public NotificationGenericBlockingMessage(object sender, NotificationSeverityEnum severity, string message)
            : base(sender, NotificationTwoWayAnswersGroupEnum.Ok)
        {
            _message = message;
            _severity = severity;
        }
    }
}
