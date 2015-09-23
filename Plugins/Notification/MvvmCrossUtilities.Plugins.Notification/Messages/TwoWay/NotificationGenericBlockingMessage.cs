using MvvmCrossUtilities.Plugins.Notification.Messages.Base;

namespace MvvmCrossUtilities.Plugins.Notification.Messages.TwoWay
{
    /// <summary>
    /// Generic notification for messages that should block the UI from other interactions
    /// </summary>
    public class NotificationGenericBlockingMessage : NotificationTwoWayMessage
    {
        /// <summary>
        /// Notification severity level
        /// </summary>
        public NotificationSeverityEnum Severity
        {
            get { return _severity; }
        }
        private readonly NotificationSeverityEnum _severity;

        /// <summary>
        /// Message to show.
        /// </summary>
        public string Message
        {
            get { return _message; }
        }
        private readonly string _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationGenericBlockingMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="severity">The severity.</param>
        /// <param name="message">The message.</param>
        public NotificationGenericBlockingMessage(object sender, NotificationSeverityEnum severity, string message)
            : base(sender, NotificationTwoWayAnswersGroupEnum.Ok)
        {
            _message = message;
            _severity = severity;
        }
    }
}
