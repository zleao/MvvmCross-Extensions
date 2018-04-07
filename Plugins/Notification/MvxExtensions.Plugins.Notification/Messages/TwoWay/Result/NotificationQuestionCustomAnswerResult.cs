using MvxExtensions.Plugins.Notification.Messages.Base;

namespace MvxExtensions.Plugins.Notification.Messages.TwoWay.Result
{
    /// <summary>
    /// Message for question custom answer result notification
    /// </summary>
    public class NotificationQuestionCustomAnswerResult : INotificationResult
    {
        /// <summary>
        /// Gets the selected answer.
        /// </summary>
        /// <value>
        /// The selected answer.
        /// </value>
        public string SelectedAnswer
        {
            get { return _selectedAnswer; }
        }
        private readonly string _selectedAnswer;

        /// <summary>
        /// Gets the index of the selected answer.
        /// </summary>
        /// <value>
        /// The index of the selected answer.
        /// </value>
        public int SelectedAnswerIndex
        {
            get { return _selectedAnswerIndex; }
        }
        private readonly int _selectedAnswerIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationQuestionCustomAnswerResult"/> class.
        /// </summary>
        /// <param name="selectedAnswer">The selected answer.</param>
        /// <param name="selectedAnswerIndex">Index of the selected answer.</param>
        public NotificationQuestionCustomAnswerResult(string selectedAnswer, int selectedAnswerIndex)
        {
            _selectedAnswer = selectedAnswer;
            _selectedAnswerIndex = selectedAnswerIndex;
        }
    }
}
