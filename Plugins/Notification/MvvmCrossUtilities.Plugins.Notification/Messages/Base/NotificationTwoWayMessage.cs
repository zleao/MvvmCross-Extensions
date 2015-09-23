
namespace MvvmCrossUtilities.Plugins.Notification.Messages.Base
{
    /// <summary>
    /// Two-Way notification
    /// </summary>
    public abstract class NotificationTwoWayMessage : NotificationMessage, INotificationTwoWayMessage
    {
        /// <summary>
        /// Gets the possible answers.
        /// </summary>
        /// <value>
        /// The possible answers.
        /// </value>
        public NotificationTwoWayAnswersGroupEnum PossibleAnswers
        {
            get { return _possibleAnswers; }
        }
        private readonly NotificationTwoWayAnswersGroupEnum _possibleAnswers;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationTwoWayMessage" /> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="possibleAnswers">The possible answers.</param>
        public NotificationTwoWayMessage(object sender, NotificationTwoWayAnswersGroupEnum possibleAnswers)
            : base(sender)
        {
            _possibleAnswers = possibleAnswers;
        }

        #endregion
    }
}
