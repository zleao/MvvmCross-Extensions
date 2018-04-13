using System;

namespace MvxExtensions.Plugins.Logger
{
    /// <summary>
    /// Details associted with a log file
    /// </summary>
    public class LogDetails
    {
        #region Properties

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        /// <value>
        /// The creation date.
        /// </value>
        public DateTimeOffset CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the debug log count.
        /// </summary>
        /// <value>
        /// The debug log count.
        /// </value>
        public int DebugLogCount { get; set; }

        /// <summary>
        /// Gets or sets the information log count.
        /// </summary>
        /// <value>
        /// The information log count.
        /// </value>
        public int InfoLogCount { get; set; }

        /// <summary>
        /// Gets or sets the warning log count.
        /// </summary>
        /// <value>
        /// The warning log count.
        /// </value>
        public int WarningLogCount { get; set; }

        /// <summary>
        /// Gets or sets the error log count.
        /// </summary>
        /// <value>
        /// The error log count.
        /// </value>
        public int ErrorLogCount { get; set; }

        /// <summary>
        /// Gets or sets the fatal log count.
        /// </summary>
        /// <value>
        /// The fatal log count.
        /// </value>
        public int FatalLogCount { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Increments the log counter.
        /// </summary>
        /// <param name="logType">Type of the log.</param>
        public void IncrementLogCounter(LogTypeEnum logType)
        {
            switch (logType)
            {
                case LogTypeEnum.Debug:
                    DebugLogCount++;
                    break;

                case LogTypeEnum.Info:
                    InfoLogCount++;
                    break;

                case LogTypeEnum.Warning:
                    WarningLogCount++;
                    break;

                case LogTypeEnum.Error:
                    ErrorLogCount++;
                    break;

                case LogTypeEnum.Fatal:
                    FatalLogCount++;
                    break;
            }
        }

        #endregion
    }
}
