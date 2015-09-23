namespace MvvmCrossUtilities.Plugins.Notification.Messages.Base
{
    /// <summary>
    /// Two-Way notificaiton possible answers group
    /// </summary>
    public enum NotificationTwoWayAnswersGroupEnum
    {
        /// <summary>
        /// No option available (fallback scenario)
        /// </summary>
        None,

        /// <summary>
        /// Ok option available
        /// </summary>
        Ok,

        /// <summary>
        /// Yes and No options available
        /// </summary>
        YesNo,

        /// <summary>
        /// Ok and Cancel options available
        /// </summary>
        OkCancel
    }
}
