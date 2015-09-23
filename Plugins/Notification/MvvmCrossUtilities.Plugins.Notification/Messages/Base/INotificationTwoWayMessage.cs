
namespace MvvmCrossUtilities.Plugins.Notification.Messages.Base
{
    /// <summary>
    /// Two-Way notification interface
    /// </summary>
    public interface INotificationTwoWayMessage : INotificationMessage
    {
        /// <summary>
        /// Gets the possible answers.
        /// </summary>
        /// <value>
        /// The possible answers.
        /// </value>
        NotificationTwoWayAnswersGroupEnum PossibleAnswers { get; }
    }
}
