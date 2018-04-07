using MvxExtensions.Plugins.Logger;
using System;

namespace MvxExtensions.Libraries.Portable.Core.Services.Logger
{
    /// <summary>
    /// Log level
    /// </summary>
    [Flags]
    public enum LogLevelEnum
    {
        /// <summary>
        /// The debug level (includes Debug, Info, Warning, Error and Fatal log types)
        /// </summary>
        Debug = LogTypeEnum.Debug | LogTypeEnum.Info | LogTypeEnum.Warning | LogTypeEnum.Error | LogTypeEnum.Fatal,
        /// <summary>
        /// The information level (includes Info, Warning, Error and Fatal log types)
        /// </summary>
        Info = LogTypeEnum.Info | LogTypeEnum.Warning | LogTypeEnum.Error | LogTypeEnum.Fatal,
        /// <summary>
        /// The warning level (includes Warning, Error and Fatal log types)
        /// </summary>
        Warning = LogTypeEnum.Warning | LogTypeEnum.Error | LogTypeEnum.Fatal,
        /// <summary>
        /// The error level (includes Error and Fatal log types)
        /// </summary>
        Error = LogTypeEnum.Error | LogTypeEnum.Fatal,
        /// <summary>
        /// The fatal level (includes Fatal log type)
        /// </summary>
        Fatal = LogTypeEnum.Fatal,
        /// <summary>
        /// The none
        /// </summary>
        None = 0
    }
}
