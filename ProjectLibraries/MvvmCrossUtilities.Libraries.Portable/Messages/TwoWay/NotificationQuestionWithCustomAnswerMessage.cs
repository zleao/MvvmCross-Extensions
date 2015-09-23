using MvvmCrossUtilities.Plugins.Notification.Messages.Base;
using MvvmCrossUtilities.Plugins.Notification.Messages.TwoWay;
using System.Collections.Generic;

namespace MvvmCrossUtilities.Libraries.Portable.Messages.TwoWay
{
    /// <summary>
    /// Message for question with custom answer notification
    /// </summary>
    public class NotificationQuestionWithCustomAnswerMessage : NotificationGenericQuestionMessage
    {
        /// <summary>
        /// Gets the possible answers.
        /// </summary>
        /// <value>
        /// The possible answers.
        /// </value>
        public new IList<string> PossibleAnswers
        {
            get { return _possibleAnswers; }
        }
        private readonly IList<string> _possibleAnswers;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationQuestionWithCustomAnswerMessage"/> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="question">The question.</param>
        /// <param name="possibleAnswers">The possible answers.</param>
        public NotificationQuestionWithCustomAnswerMessage(object sender, string question, IList<string> possibleAnswers)
            : base(sender, question, NotificationTwoWayAnswersGroupEnum.None)
        {
            _possibleAnswers = possibleAnswers;
        }
    }
}
