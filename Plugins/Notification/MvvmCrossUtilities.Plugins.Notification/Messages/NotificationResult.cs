using MvvmCrossUtilities.Plugins.Notification.Messages.Base;

namespace MvvmCrossUtilities.Plugins.Notification.Messages
{
    public class NotificationResult
    {
        public NotificationTwoWayAnswersEnum Answer
        {
            get { return _answer; }
        }
        private readonly NotificationTwoWayAnswersEnum _answer;

        public NotificationResult(NotificationTwoWayAnswersEnum answer)
            : base()
        {
            _answer = answer;      
        }
    }
}
