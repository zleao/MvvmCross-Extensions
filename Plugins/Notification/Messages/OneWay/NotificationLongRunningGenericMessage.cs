namespace MvxExtensions.Plugins.Notification.Messages.OneWay
{
    /// <summary>
    /// Notification used for long running (less volatile) subscriptions
    /// </summary>
    public class NotificationLongRunningGenericMessage : NotificationGenericMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationLongRunningGenericMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The text message.</param>
        public NotificationLongRunningGenericMessage(object sender, string message)
            : base(sender, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationLongRunningGenericMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        public NotificationLongRunningGenericMessage(object sender, string message, NotificationModeEnum mode)
            : base(sender, message, mode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationLongRunningGenericMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="severity">The severity.</param>
        public NotificationLongRunningGenericMessage(object sender, string message, NotificationModeEnum mode, NotificationSeverityEnum severity)
            : base(sender, message, mode, severity)
        {
        }
    }
}
