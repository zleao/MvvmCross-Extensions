using MvxExtensions.Plugins.Notification.Messages.TwoWay.Question;

namespace MvxExtensions.Plugins.Notification.Messages.TwoWay.Result
{
    /// <summary>
    /// Result for question notifications
    /// </summary>
    public class NotificationQuestionAnswerResult : INotificationResult
    {
        /// <summary>
        /// The answer.
        /// </summary>
        public NotificationTwoWayAnswersEnum Answer
        {
            get { return _answer; }
        }
        private readonly NotificationTwoWayAnswersEnum _answer;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationQuestionAnswerResult" /> class.
        /// </summary>
        /// <param name="answer">The answer.</param>
        public NotificationQuestionAnswerResult(NotificationTwoWayAnswersEnum answer)
        {
            _answer = answer;
        }
    }
}
