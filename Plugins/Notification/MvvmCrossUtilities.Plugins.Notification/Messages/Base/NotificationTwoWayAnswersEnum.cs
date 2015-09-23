namespace MvvmCrossUtilities.Plugins.Notification.Messages.Base
{
    /// <summary>
    /// Two-Way notification possible results
    /// </summary>
    public enum NotificationTwoWayAnswersEnum
    {
        /// <summary>
        /// Unknown answer (fallback scenario)
        /// </summary>
        Unknown,

        /// <summary>
        /// Answer possible in the YesNo answers group
        /// </summary>
        Yes,

        /// <summary>
        /// Answer possible in the YesNo answers group
        /// </summary>
        No,

        /// <summary>
        /// Answer possible in the Ok and OkCancel answers group
        /// </summary>
        Ok,

        /// <summary>
        /// Answer possible in the OkCancel answers group
        /// </summary>
        Cancel
    }
}
