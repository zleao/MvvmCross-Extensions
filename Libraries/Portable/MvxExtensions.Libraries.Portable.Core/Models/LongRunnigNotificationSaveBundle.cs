using System;

namespace MvxExtensions.Libraries.Portable.Core.Models
{
    /// <summary>
    /// Class used to store information about a LongRunningNotification subscription, in the bundle
    /// </summary>
    public class LongRunnigNotificationSaveBundle
    {
        /// <summary>
        /// Gets or sets the type of the message.
        /// </summary>
        /// <value>
        /// The type of the message.
        /// </value>
        public Type MessageType { get; set; }

        /// <summary>
        /// Gets or sets the name of the method.
        /// </summary>
        /// <value>
        /// The name of the method.
        /// </value>
        public string MethodName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [unsubscribe on arrival].
        /// </summary>
        public bool UnsubscribeOnArrival { get; set; }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        public string Context { get; set; }
    }
}
