using System.Collections.Generic;
using MvvmCrossUtilities.Plugins.Notification.Messages.Base;
using MvvmCrossUtilities.Plugins.Notification.Messages.TwoWay;

namespace MvvmCrossUtilities.Libraries.Portable.Messages.TwoWay
{
    public class NotificationQuestionWithCustomAnswerMessage : NotificationGenericQuestionMessage
    {
        public IList<string> PossibleAnswers
        {
            get { return _possibleAnswers; }
        }
        private readonly IList<string> _possibleAnswers;

        public NotificationQuestionWithCustomAnswerMessage(object sender, string question, IList<string> possibleAnswers)
            : base(sender, question, NotificationTwoWayAnswersGroupEnum.None)
        {
            _possibleAnswers = possibleAnswers;
        }
    }
}
