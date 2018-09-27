namespace MvxExtensions.Plugins.Storage
{
    /// <summary>
    /// EncryptionModeEnum
    /// </summary>
    public enum EncryptionModeEnum
    {
        /// <summary>
        /// Unknown or unsuported encription mode
        /// </summary>
        UNKNOWN,
        /// <summary>
        /// Full file encryption using Crypto
        /// </summary>
        CRYPTOFULL,
        /// <summary>
        /// Line encryption using Crypto
        /// </summary>
        CRYPTOLINE,
    }
}
