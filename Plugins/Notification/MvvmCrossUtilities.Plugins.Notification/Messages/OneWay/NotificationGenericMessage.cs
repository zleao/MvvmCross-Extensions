namespace MvvmCrossUtilities.Plugins.Notification.Messages.OneWay
{
    /// <summary>
    /// Generic notification message
    /// </summary>
    public class NotificationGenericMessage : NotificationTextMessage
    {
        #region Properties

        /// <summary>
        /// Type of notification UI to use
        /// </summary>
        public NotificationModeEnum Mode { get; private set; }

        /// <summary>
        /// Notificaiton severity level
        /// </summary>
        public NotificationSeverityEnum Severity { get; private set; }
        
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationGenericMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The text message.</param>
        public NotificationGenericMessage(object sender, string message)
            : this(sender, message, NotificationModeEnum.Toast, NotificationSeverityEnum.Info)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationGenericMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        public NotificationGenericMessage(object sender, string message, NotificationModeEnum mode)
            : this(sender, message, mode, NotificationSeverityEnum.Info)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationGenericMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="severity">The severity.</param>
        public NotificationGenericMessage(object sender, string message, NotificationModeEnum mode, NotificationSeverityEnum severity)
            : base(sender, message)
        {
            this.Mode = mode;
            this.Severity = severity;
        }

        #endregion
    }
}
