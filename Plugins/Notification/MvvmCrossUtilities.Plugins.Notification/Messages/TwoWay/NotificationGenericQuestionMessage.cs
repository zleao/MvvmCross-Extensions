using MvvmCrossUtilities.Plugins.Notification.Messages.Base;

namespace MvvmCrossUtilities.Plugins.Notification.Messages.TwoWay
{
    /// <summary>
    /// Generic notification for question interface
    /// </summary>
    public class NotificationGenericQuestionMessage : NotificationTwoWayMessage
    {
        /// <summary>
        /// Gets the question.
        /// </summary>
        public string Question
        {
            get { return _question; }
        }
        private readonly string _question;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationGenericQuestionMessage" /> class.
        /// Will assume Yes and No as the possible answers
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="question">The question.</param>
        public NotificationGenericQuestionMessage(object sender, string question)
            : this(sender, question, NotificationTwoWayAnswersGroupEnum.YesNo)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationGenericQuestionMessage" /> class.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="question">The question.</param>
        /// <param name="possibleAnswers">The possible answers.</param>
        public NotificationGenericQuestionMessage(object sender, string question, NotificationTwoWayAnswersGroupEnum possibleAnswers)
            : base(sender, possibleAnswers)
        {
            _question = question;
        }
    }
}
