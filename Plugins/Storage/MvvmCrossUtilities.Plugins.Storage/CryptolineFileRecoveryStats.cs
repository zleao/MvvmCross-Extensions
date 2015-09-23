using System;

namespace MvvmCrossUtilities.Plugins.Storage
{
    /// <summary>
    /// CryptolineFileRecoveryStats
    /// </summary>
    public class CryptolineFileRecoveryStats
    {
        /// <summary>
        /// Gets or sets the source file path.
        /// </summary>
        /// <value>
        /// The source file path.
        /// </value>
        public string SourceFilePath { get; set; }
        /// <summary>
        /// Gets or sets the source file syze in bytes.
        /// </summary>
        /// <value>
        /// The source file syze in bytes.
        /// </value>
        public long SourceFileSyzeInBytes { get; set; }

        /// <summary>
        /// Gets or sets the recovered file path.
        /// </summary>
        /// <value>
        /// The recovered file path.
        /// </value>
        public string RecoveredFilePath { get; set; }
        /// <summary>
        /// Gets or sets the recovered file syze in bytes.
        /// </summary>
        /// <value>
        /// The recovered file syze in bytes.
        /// </value>
        public long RecoveredFileSyzeInBytes { get; set; }

        /// <summary>
        /// Gets or sets the bytes lost.
        /// </summary>
        /// <value>
        /// The bytes lost.
        /// </value>
        public long BytesLost { get; set; }
        /// <summary>
        /// Gets or sets the number of lines recovered.
        /// </summary>
        /// <value>
        /// The number of lines recovered.
        /// </value>
        public int NumberOfLinesRecovered { get; set; }
        /// <summary>
        /// Gets or sets the recovery time.
        /// </summary>
        /// <value>
        /// The recovery time.
        /// </value>
        public TimeSpan RecoveryTime { get; set; }
        /// <summary>
        /// Gets or sets the execution date time.
        /// </summary>
        /// <value>
        /// The execution date time.
        /// </value>
        public DateTime ExecutionDateTime { get; set; }
    }
}
