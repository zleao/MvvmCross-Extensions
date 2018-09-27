using System;
using System.IO;
using System.Threading.Tasks;

namespace MvxExtensions.Plugins.Storage
{
    /// <summary>
    /// Interface for encryption methods
    /// </summary>
    public interface IStorageEncryptionManager
    {
        #region Decryption Methods

        /// <summary>
        /// Tries to read text from encrypted file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <param name="password">The password.</param>
        /// <returns>Decripted text</returns>
        Task<string> TryReadTextEncryptedFileAsync(StorageLocation location, string path, string password);

        /// <summary>
        /// Tries to read text from encrypted file .
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <param name="password">The password.</param>
        /// <returns>Decripted text</returns>
        Task<string> TryReadTextEncryptedFileAsync(string fullPath, string password);

        /// <summary>
        /// Decrypts a file, saving it to a new file, with a name generated automatically
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="password">The password.</param>
        /// <returns>The destination file path</returns>
        Task<string> DecryptFileAsync(StorageLocation location, string sourcePath, string password);

        /// <summary>
        /// Decrypts a file, saving it to a new file, specified in targetPath.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetPath">The target path.</param>
        /// <param name="password">The password.</param>
        Task DecryptFileAsync(StorageLocation location, string sourcePath, string targetPath, string password);

        /// <summary>
        /// Decrypts a file, saving it to a new file, with a name generated automatically
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="password">The password.</param>
        /// <returns>The destination file path</returns>
        Task<string> DecryptFileAsync(string sourceFullPath, string password);

        /// <summary>
        /// Decrypts a file, saving it to a new file, specified in targetfullPath.
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="targetFullPath">The target full path.</param>
        /// <param name="password">The password.</param>
        Task DecryptFileAsync(string sourceFullPath, string targetFullPath, string password);

        /// <summary>
        /// Decrypts a file, and returns the correspondent stream.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        Task<Stream> DecryptFileToStreamAsync(StorageLocation location, string sourcePath, string password);

        /// <summary>
        /// Decrypts the file to stream asynchronous.
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        Task<Stream> DecryptFileToStreamAsync(string sourceFullPath, string password);

        /// <summary>
        /// Decrypts the string.
        /// </summary>
        /// <param name="stringToDecrypt">The string to decrypt.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        Task<string> DecryptStringAsync(string stringToDecrypt, string password);

        #endregion

        #region Encryption Methods

        /// <summary>
        /// Writes the contents to a file, using the password to encrypt the data.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        /// <param name="password">The password.</param>
        Task WriteEncryptedFileAsync(StorageLocation location, StorageMode mode, string path, string contents, string password);

        /// <summary>
        /// Writes the byte array to a file, using the password to encrypt the data.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        /// <param name="password">The password.</param>
        Task WriteEncryptedFileAsync(StorageLocation location, StorageMode mode, string path, Byte[] contents, string password);

        /// <summary>
        /// Writes the contents to a file, using the password to encrypt the data.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="contents">The contents.</param>
        /// <param name="password">The password.</param>
        Task WriteEncryptedFileAsync(StorageMode mode, string fullPath, string contents, string password);

        /// <summary>
        /// Encrypts a file, saving it to a new file, with a name generated automatically
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="password">The password.</param>
        /// <returns>The destination file path</returns>
        Task<string> EncryptFileAsync(StorageLocation location, string sourcePath, string password);

        /// <summary>
        /// Encrypts a file, saving it to a new file, specified in targetPath.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetPath">The target path.</param>
        /// <param name="password">The password.</param>
        Task EncryptFileAsync(StorageLocation location, string sourcePath, string targetPath, string password);

        /// <summary>
        /// Encrypts a file, saving it to a new file, with a name generated automatically
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="password">The password.</param>
        /// <returns>The destination file path</returns>
        Task<string> EncryptFileAsync(string sourceFullPath, string password);

        /// <summary>
        /// Encrypts a file, saving it to a new file, specified in targetfullPath.
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="targetFullPath">The target full path.</param>
        /// <param name="password">The password.</param>
        Task EncryptFileAsync(string sourceFullPath, string targetFullPath, string password);

        /// <summary>
        /// Encrypts the string.
        /// </summary>
        /// <param name="stringToEncrypt">The string to encrypt.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        Task<string> EncryptStringAsync(string stringToEncrypt, string password);

        #endregion

        #region Encrypted File Recovery

        /// <summary>
        /// Recovers the encrypted file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="password">The password.</param>
        /// <returns>The destination file path</returns>
        Task<string> RecoverEncryptedFileAsync(StorageLocation location, string sourcePath, string password);

        /// <summary>
        /// Recovers the encrypted file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetPath">The target path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        Task RecoverEncryptedFileAsync(StorageLocation location, string sourcePath, string targetPath, string password);

        /// <summary>
        /// Recovers the encrypted file.
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="password">The password.</param>
        /// <returns>The destination file path</returns>
        Task<string> RecoverEncryptedFileAsync(string sourceFullPath, string password);

        /// <summary>
        /// Recovers the encrypted file.
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="targetFullPath">The target full path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        Task RecoverEncryptedFileAsync(string sourceFullPath, string targetFullPath, string password);

        #endregion
    }
}
