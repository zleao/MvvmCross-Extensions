using System;
using System.IO;
using System.Threading.Tasks;

namespace MvxExtensions.Plugins.Storage.Platforms.Common
{
    /// <summary>
    /// Storage plugin implementation for the common part of the encryption functionality
    /// </summary>
    /// <seealso cref="StorageManagerCommon" />
    public abstract class StorageManagerCommonEncryption : StorageManagerCommon
    {
        #region Decryption Methods

        /// <summary>
        /// Tries to read text from encrypted file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// Decripted text
        /// </returns>
        public override Task<string> TryReadTextEncryptedFileAsync(StorageLocation location, string path, string password)
        {
            var fullPath = FullPath(location, path);
            return TryReadTextEncryptedFileAsync(fullPath, password);
        }

        /// <summary>
        /// Tries to read text from encrypted file .
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// Decripted text
        /// </returns>
        public override async Task<string> TryReadTextEncryptedFileAsync(string fullPath, string password)
        {
            string result = null;
            var toReturn = await TryReadEncryptedFileCommonAsync(fullPath, password, (stream) =>
            {
                using (var streamReader = new StreamReader(stream))
                {
                    result = streamReader.ReadToEnd();
                }
                return true;
            });

            return result;
        }

        /// <summary>
        /// Decrypts a file, saving it to a new file, with a name generated automatically
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// The destination file path
        /// </returns>
        public override async Task<string> DecryptFileAsync(string sourceFullPath, string password)
        {
            var targetFullPath = sourceFullPath.Replace(Path.GetFileNameWithoutExtension(sourceFullPath), Path.GetFileNameWithoutExtension(sourceFullPath) + "_Decrypted");
            await DecryptFileAsync(sourceFullPath, targetFullPath, password);
            return targetFullPath;
        }

        /// <summary>
        /// Decrypts a file, saving it to a new file, with a name generated automatically
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// The destination file path
        /// </returns>
        public override Task<string> DecryptFileAsync(StorageLocation location, string sourcePath, string password)
        {
            return DecryptFileAsync(FullPath(location, sourcePath), password);
        }

        /// <summary>
        /// Decrypts a file, saving it to a new file, specified in targetPath.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetPath">The target path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public override Task DecryptFileAsync(StorageLocation location, string sourcePath, string targetPath, string password)
        {
            return DecryptFileAsync(FullPath(location, sourcePath), FullPath(location, targetPath), password);
        }

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
        /// <returns></returns>
        public override Task WriteEncryptedFileAsync(StorageLocation location, StorageMode mode, string path, string contents, string password)
        {
            var fullPath = FullPath(location, path);
            return WriteEncryptedFileAsync(mode, fullPath, contents, password);
        }

        /// <summary>
        /// Writes the byte array to a file, using the password to encrypt the data.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public override Task WriteEncryptedFileAsync(StorageLocation location, StorageMode mode, string path, byte[] contents, string password)
        {
            return WriteEncryptedFileCommonAsync(location, mode, path, (stream) =>
            {
                stream.Write(contents, 0, contents.Length);
            },
            password);
        }

        /// <summary>
        /// Writes the contents to a file, using the password to encrypt the data.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="contents">The contents.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public override Task WriteEncryptedFileAsync(StorageMode mode, string fullPath, string contents, string password)
        {
            return WriteEncryptedFileCommonAsync(mode, fullPath, (stream) =>
            {
                var sw = new StreamWriter(stream);
                sw.Write(contents);
                sw.Flush();
            },
           password);
        }

        /// <summary>
        /// Encrypts a file, saving it to a new file, with a name generated automatically
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// The destination file path
        /// </returns>
        public override async Task<string> EncryptFileAsync(string sourceFullPath, string password)
        {
            var targetFullPath = sourceFullPath.Replace(Path.GetFileNameWithoutExtension(sourceFullPath), Path.GetFileNameWithoutExtension(sourceFullPath) + "_Encrypted");
            await EncryptFileAsync(sourceFullPath, targetFullPath, password);
            return targetFullPath;
        }

        /// <summary>
        /// Encrypts a file, saving it to a new file, with a name generated automatically
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// The destination file path
        /// </returns>
        public override Task<string> EncryptFileAsync(StorageLocation location, string sourcePath, string password)
        {
            return EncryptFileAsync(FullPath(location, sourcePath), password);
        }

        /// <summary>
        /// Encrypts a file, saving it to a new file, specified in targetPath.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetPath">The target path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public override Task EncryptFileAsync(StorageLocation location, string sourcePath, string targetPath, string password)
        {
            return EncryptFileAsync(FullPath(location, sourcePath), FullPath(location, targetPath), password);
        }

        /// <summary>
        /// Common file to write encrypted content to a file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="path">The path.</param>
        /// <param name="streamAction">The stream action.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        protected override Task WriteEncryptedFileCommonAsync(StorageLocation location, StorageMode mode, string path, Action<Stream> streamAction, string password)
        {
            var fullPath = FullPath(location, path);

            return WriteEncryptedFileCommonAsync(mode, fullPath, streamAction, password);
        }

        #endregion

        #region Encrypted File Recovery

        /// <summary>
        /// Recovers the encrypted file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// The destination file path
        /// </returns>
        public override Task<string> RecoverEncryptedFileAsync(StorageLocation location, string sourcePath, string password)
        {
            return RecoverEncryptedFileAsync(FullPath(location, sourcePath), password);
        }

        /// <summary>
        /// Recovers the encrypted file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetPath">The target path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public override Task RecoverEncryptedFileAsync(StorageLocation location, string sourcePath, string targetPath, string password)
        {
            return RecoverEncryptedFileAsync(FullPath(location, sourcePath), FullPath(location, targetPath), password);
        }

        /// <summary>
        /// Recovers the encrypted file.
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// The destination file path
        /// </returns>
        public override async Task<string> RecoverEncryptedFileAsync(string sourceFullPath, string password)
        {
            var targetFullPath = sourceFullPath.Replace(Path.GetFileNameWithoutExtension(sourceFullPath), Path.GetFileNameWithoutExtension(sourceFullPath) + "_Recovered");
            await RecoverEncryptedFileAsync(sourceFullPath, targetFullPath, password);
            return targetFullPath;
        }

        #endregion
    }
}