namespace MvvmCrossUtilities.Plugins.Notification.Messages
{
    /// <summary>
    /// Type of notification interface to use
    /// </summary>
    public enum NotificationModeEnum
    {
        /// <summary>
        /// The default type
        /// </summary>
        Default,
        
        /// <summary>
        /// Use a message box to show the notification message
        /// </summary>
        MessageBox,

        /// <summary>
        /// Use a self expriring message control (toast) to show the notification message
        /// </summary>
        Toast
    }
}
