using System;

namespace MvxExtensions.Plugins.Logger
{
    /// <summary>
    /// LogTypeEnum
    /// </summary>
    [Flags]
    public enum LogTypeEnum
    {
        /// <summary>
        /// Debug
        /// </summary>
        Debug = 1,
        /// <summary>
        /// Information
        /// </summary>
        Info = 2,
        /// <summary>
        /// Warning
        /// </summary>
        Warning = 4,
        /// <summary>
        /// Error
        /// </summary>
        Error = 8,
        /// <summary>
        /// Fatal
        /// </summary>
        Fatal = 16
    }
}
