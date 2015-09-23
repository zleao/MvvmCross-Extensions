using MvvmCrossUtilities.Plugins.Notification.Messages.Base;

namespace MvvmCrossUtilities.Plugins.Notification.Messages.TwoWay
{
    /// <summary>
    /// Generic result for question notifications
    /// </summary>
    public class NotificationGenericQuestionResult : NotificationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationGenericQuestionResult"/> class.
        /// </summary>
        /// <param name="answer">The answer.</param>
        public NotificationGenericQuestionResult(NotificationTwoWayAnswersEnum answer)
            : base(answer)
        {
        }
    }
}
