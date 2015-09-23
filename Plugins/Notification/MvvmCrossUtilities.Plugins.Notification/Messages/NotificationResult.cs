using MvvmCrossUtilities.Plugins.Notification.Messages.Base;

namespace MvvmCrossUtilities.Plugins.Notification.Messages
{
    /// <summary>
    /// Base representation of a notification result
    /// </summary>
    public class NotificationResult
    {
        /// <summary>
        /// Gets the answer.
        /// </summary>
        public NotificationTwoWayAnswersEnum Answer
        {
            get { return _answer; }
        }
        private readonly NotificationTwoWayAnswersEnum _answer;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationResult"/> class.
        /// </summary>
        /// <param name="answer">The answer.</param>
        public NotificationResult(NotificationTwoWayAnswersEnum answer)
            : base()
        {
            _answer = answer;      
        }
    }
}
