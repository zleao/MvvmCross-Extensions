using MvvmCrossUtilities.Plugins.Notification.Messages.Base;

namespace MvvmCrossUtilities.Plugins.Notification.Messages
{
    /// <summary>
    /// One-Way message with string message
    /// </summary>
    public abstract class NotificationTextMessage : NotificationOneWayMessage
    {
        #region Properties

        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public string Message
        {
            get { return _message; }
        }
        private readonly string _message;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationTextMessage" /> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The text message.</param>
        public NotificationTextMessage(object sender, string message)
            : base(sender)
        {
            _message = message;
        }

        #endregion
    }
}