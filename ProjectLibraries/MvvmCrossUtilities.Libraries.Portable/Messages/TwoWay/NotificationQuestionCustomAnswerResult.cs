using MvvmCrossUtilities.Plugins.Notification.Messages.Base;
using MvvmCrossUtilities.Plugins.Notification.Messages.TwoWay;

namespace MvvmCrossUtilities.Libraries.Portable.Messages.TwoWay
{
    public class NotificationQuestionCustomAnswerResult : NotificationGenericQuestionResult
    {
        public string SelectedAnswer
        {
            get { return _selectedAnswer; }
        }
        private readonly string _selectedAnswer;

        public int SelectedAnswerIndex
        {
            get { return _selectedAnswerIndex; }
        }
        private readonly int _selectedAnswerIndex;

        public NotificationQuestionCustomAnswerResult(string selectedAnswer, int selectedAnswerIndex)
            : base(NotificationTwoWayAnswersEnum.Unknown)
        {
            _selectedAnswer = selectedAnswer;
            _selectedAnswerIndex = selectedAnswerIndex;
        }
    }
}
