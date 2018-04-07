using MvvmCross.Platform.Exceptions;
using MvvmCross.Platform.Platform;
using MvxExtensions.Libraries.Portable.Core.Extensions;
using System;
using System.Text.RegularExpressions;

namespace MvxExtensions.Libraries.Portable.Core.Services.Logger
{
    /// <summary>
    /// Configuration class for the logger manager
    /// </summary>
    public class LoggerConfiguration
    {
        #region CONFIGURATIONS

        private const string DEFAULT_LOG_GROUP_TEMPLATE = "{DATE:yyyyMMdd}";
        private const string DEFAULT_LOG_FILENAME_PREFIX_TEMPLATE = "{DATE:yyyyMMddHHmmss}_";
        private const string DEFAULT_LOG_FILENAME = "ApplicationLog";
        private const string DEFAULT_LOG_SUFIX_TEMPLATE = "";

        private static Regex dateWithFormatReg = new Regex(@"\{DATE\:(.*)\}");
        private static Regex dateWithoutFormatReg = new Regex(@"\{DATE}");
        private static Regex guidWithFormatReg = new Regex(@"\{GUID\:(.*)\}");
        private static Regex guidWithoutFormatReg = new Regex(@"\{GUID}");

        #endregion

        #region Properties

        /// <summary>
        /// Indicates if the log files can be encrypted
        /// </summary>
        public bool CanEncryptLogs { get; set; }

        /// <summary>
        /// Password to be used to encrypt the log files, if the encryption is enabled
        /// </summary>
        public string LogPassword { get; set; }

        /// <summary>
        /// Indicates if the manager can purge expired logs
        /// </summary>
        public bool CanPurgeExpiredLogs { get; set; }

        /// <summary>
        /// Lifespan of the logs. If purge is enabled, the expired logs will be deleted
        /// </summary>
        public TimeSpan LogLifespan { get; set; }

        /// <summary>
        /// Indicates if the logs with errors should be preserved when a purge is executed
        /// </summary>
        public bool KeepLogswithErrors { get; set; }

        /// <summary>
        /// Indicates if the logs can be grouped in a folder, using the LogGroupFormat as name
        /// </summary>
        public bool EnableLogGrouping { get; set; }

        /// <summary>
        /// Template for the folder name used to group log files, if the log grouping is enabled
        /// Supported formatting tags:
        ///  'DATE' - used with DateTimne.Now
        ///  'DATE:dateformat' - used with DateTime.Now and the format specified
        ///  'GUID' - used with Guid.NewGuid
        ///  'GUID:guidformat' - used with Guid.NewGuid and the format specified
        /// Example of a template
        ///  "{DATE:yyyy}" -> grouping by year
        ///  "{GUID}" -> grouping by guid
        /// </summary>
        public string LogGroupTemplate { get; set; }

        /// <summary>
        /// Forced log group name. Used to enforce a log group name, overriding the 'LogGroupTemplate'
        /// Attention: the value of this property (if NOT NullOrEmpty) will override the 
        /// log group template in the definition of the directory name for the log grouping
        /// </summary>
        public string ForcedLogGroupName { get; set; }

        /// <summary>
        /// Used to indicate if the LogGroupName should be recreated everytime a log is about to be written.
        /// Usefull in cases where the group name could change during the application execution
        /// </summary>
        /// <value>
        /// <c>true</c> if the log group name is recreated at every log that is about to be written; otherwise, <c>false</c>.
        /// </value>
        public bool IsLogGroupNameVolatile { get; set; }

        /// <summary>
        /// Indicates if the prefix is to be added to the log file base name
        /// </summary>
        public bool EnablePrefix { get; set; }

        /// <summary>
        /// Prefix to be added at the beginning of the log file base name template,
        /// if the 'EnablePrefix' is true.
        /// Supported formatting tags:
        ///  'DATE' - used with DateTimne.Now
        ///  'DATE:dateformat' - used with DateTime.Now and the format specified
        ///  'GUID' - used with Guid.NewGuid
        ///  'GUID:guidformat' - used with Guid.NewGuid and the format specified
        /// Example of a template
        ///  "{DATE:yyyy}" -> grouping by year
        ///  "{GUID}" -> grouping by guid
        /// </summary>
        public string LogFileNamePrefixTemplate { get; set; }

        /// <summary>
        /// Forced log filename prefix. Used to enforce a prefix to the log filename, overriding the 'LogFileNamePrefixTemplate'
        /// Attention: the value of this property (if NOT NullOrEmpty) will override the 
        /// log file name prefix template in the construction of the final log filename
        /// </summary>
        public string ForcedLogFileNamePrefix { get; set; }

        /// <summary>
        /// Used to indicate if the LogFileNamePrefix should be recreated everytime a log is about to be written.
        /// Usefull in cases where the prefix could change during the application execution
        /// </summary>
        /// <value>
        /// <c>true</c> if the log group name is recreated at every log that is about to be written; otherwise, <c>false</c>.
        /// </value>
        public bool IsLogFileNamePrefixVolatile { get; set; }

        /// <summary>
        /// Base log filename. The final filename may have a preffix concatenated, according to the configuration
        /// </summary>
        public string LogFileName { get; set; }

        /// <summary>
        /// The detail level of log to be used
        /// </summary>
        public LogLevelEnum LogLevel { get; set; }

        /// <summary>
        /// Default tag to be added to each line of the log output (i.e. the logger manager type).
        /// </summary>
        public string DefaultLogTag { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerConfiguration"/> class.
        /// <para>Default values for configuration:</para>
        /// <para>CanEncryptLogs -> false</para>
        /// <para>LogPassword -> ''</para>
        /// <para>CanPurgeExpiredLogs -> true</para>
        /// <para>LogLifespan -> 7 Days</para>
        /// <para>KeepLogsWithErrors -> true</para>
        /// <para>EnableLogGrouping -> true</para>
        /// <para>LogGroupTemplate -> '{DATE:yyyyMMdd}'</para>
        /// <para>ForcedLogGroupName -> null</para>
        /// <para>IsLogGroupNameVolatile -> false</para>
        /// <para>EnablePrefix -> true</para>
        /// <para>LogFileNamePrefixTemplate -> '{DATE:yyyyMMddHHmmss}_'</para>
        /// <para>ForcedLogFileNamePrefix -> null</para>
        /// <para>IsLogFileNamePrefixVolatile -> false</para>
        /// <para>LogFileName -> 'ApplicationLog'</para>
        /// <para>LogLevel -> 'Error'</para>
        /// <para>DefaultLogTag -> ''</para>
        /// </summary>
        public LoggerConfiguration()
        {
            CanEncryptLogs = false;
            LogPassword = string.Empty;
            CanPurgeExpiredLogs = true;
            LogLifespan = new TimeSpan(7, 0, 0, 0);
            KeepLogswithErrors = true;
            EnableLogGrouping = true;
            LogGroupTemplate = DEFAULT_LOG_GROUP_TEMPLATE;
            ForcedLogGroupName = null;
            IsLogGroupNameVolatile = false;
            EnablePrefix = true;
            LogFileNamePrefixTemplate = DEFAULT_LOG_FILENAME_PREFIX_TEMPLATE;
            ForcedLogFileNamePrefix = null;
            IsLogFileNamePrefixVolatile = false;
            LogFileName = DEFAULT_LOG_FILENAME;
            LogLevel = LogLevelEnum.Error;
            DefaultLogTag = string.Empty;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the name of the log group.
        /// </summary>
        /// <returns></returns>
        internal string GetLogGroupName()
        {
            string logGroupName = null;

            try
            {
                if (!EnableLogGrouping)
                    return string.Empty;

                if (!ForcedLogGroupName.IsNullOrEmpty())
                    return ForcedLogGroupName;

                if (ParseSupportedTags(LogGroupTemplate, out logGroupName))
                    return logGroupName;

                return LogGroupTemplate;
            }
            catch (Exception ex)
            {
                MvxTrace.Error("Error getting log group name. Using default group template." + Environment.NewLine + ex.ToLongString());
                ParseSupportedTags(DEFAULT_LOG_GROUP_TEMPLATE, out logGroupName);
                return logGroupName;
            }
        }

        internal string GetPrefix()
        {
            string prefix = null;

            try
            {
                if (!EnablePrefix)
                    return string.Empty;

                if (!ForcedLogFileNamePrefix.IsNullOrEmpty())
                    return ForcedLogFileNamePrefix;

                if (ParseSupportedTags(LogFileNamePrefixTemplate, out prefix))
                    return prefix;

                return LogFileNamePrefixTemplate;
            }
            catch (Exception ex)
            {
                MvxTrace.Error("Error getting log file prefix. Using default prefix template." + Environment.NewLine + ex.ToLongString());
                ParseSupportedTags(DEFAULT_LOG_FILENAME_PREFIX_TEMPLATE, out prefix);
                return prefix;
            }
        }

        private static bool ParseSupportedTags(string stringToParse, out string outputString)
        {
            outputString = null;
            string fullMatch = null;
            string format = null;

            //Try parse Date with format
            if (TryParseTag(stringToParse, dateWithFormatReg, out fullMatch, "DATE", out format))
                outputString = stringToParse.Replace(fullMatch, DateTime.Now.ToString(format));

            //Try parse Date without format
            if (TryParseTag(stringToParse, dateWithoutFormatReg, out fullMatch))
                outputString = stringToParse.Replace(fullMatch, DateTime.Now.ToString());

            //Try parse Guid with format
            if (TryParseTag(stringToParse, guidWithFormatReg, out fullMatch, "GUID", out format))
                outputString = stringToParse.Replace(fullMatch, Guid.NewGuid().ToString(format));

            //Try parse Guid without format
            if (TryParseTag(stringToParse, guidWithoutFormatReg, out fullMatch))
                outputString = stringToParse.Replace(fullMatch, Guid.NewGuid().ToString());

            return !outputString.IsNullOrEmpty();
        }

        private static bool TryParseTag(string stringToTest, Regex reg, out string fullMatch)
        {
            fullMatch = null;

            if (reg.IsMatch(stringToTest))
                fullMatch = reg.Match(stringToTest).ToString();

            return !fullMatch.IsNullOrEmpty();

        }
        private static bool TryParseTag(string stringToTest, Regex reg, out string fullMatch, string tagType, out string format)
        {
            fullMatch = null;
            format = null;

            if (reg.IsMatch(stringToTest))
            {
                fullMatch = reg.Match(stringToTest).ToString();
                format = fullMatch.Replace("{" + tagType + ":", "").Replace("}", "");
            }

            return !(fullMatch.IsNullOrEmpty() || format.IsNullOrEmpty());
        }

        #endregion
    }
}
