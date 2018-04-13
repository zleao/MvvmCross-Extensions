using MvxExtensions.Plugins.Notification.Messages.TwoWay.Question;

namespace MvxExtensions.Plugins.Notification.Messages.TwoWay.Result
{
    /// <summary>
    /// Generic result for question notifications
    /// </summary>
    public class NotificationGenericQuestionResult : NotificationQuestionAnswerResult
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
