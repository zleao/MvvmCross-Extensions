namespace MvvmCrossUtilities.Plugins.Notification.Messages.OneWay
{
    public class NotificationGenericMessage : NotificationTextMessage
    {
        #region Properties

        public NotificationModeEnum Mode { get; private set; }

        public NotificationSeverityEnum Severity { get; private set; }

        #endregion

        #region Constructor

        public NotificationGenericMessage(object sender, string message)
            : this(sender, message, NotificationModeEnum.Toast, NotificationSeverityEnum.Info)
        {

        }

        public NotificationGenericMessage(object sender, string message, NotificationModeEnum mode)
            : this(sender, message, mode, NotificationSeverityEnum.Info)
        {
        }

        public NotificationGenericMessage(object sender, string message, NotificationModeEnum mode, NotificationSeverityEnum severity)
            : base(sender, message)
        {
            this.Mode = mode;
            this.Severity = severity;
        }

        #endregion
    }
}
