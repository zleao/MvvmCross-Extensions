using MvvmCrossUtilities.Plugins.Notification.Messages.Base;

namespace MvvmCrossUtilities.Plugins.Notification.Messages.TwoWay
{
    public class NotificationGenericQuestionResult : NotificationResult
    {
        public NotificationGenericQuestionResult(NotificationTwoWayAnswersEnum answer)
            : base(answer)
        {
        }
    }
}
