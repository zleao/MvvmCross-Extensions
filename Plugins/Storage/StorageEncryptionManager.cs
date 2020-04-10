using MvxExtensions.Plugins.Storage.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MvxExtensions.Plugins.Storage
{
    /// <summary>
    /// storage encryption plugin implementation
    /// </summary>
    /// <seealso cref="IStorageEncryptionManager" />
    public abstract class StorageEncryptionManager : StorageManagerCommon, IStorageEncryptionManager
    {
        #region Constructor

        //TODO: use a disposable pattern instead of finalizer. 
        /// <summary>
        /// Finalizes an instance of the <see cref="StorageEncryptionManager"/> class.
        /// </summary>
        ~StorageEncryptionManager()
        {
            CryptologyAsync.DisposeAlgorithmAndPassword();
        }

        #endregion

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
        public virtual Task<string> TryReadTextEncryptedFileAsync(StorageLocation location, string path, string password)
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
        public virtual async Task<string> TryReadTextEncryptedFileAsync(string fullPath, string password)
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
        public virtual async Task<string> DecryptFileAsync(string sourceFullPath, string password)
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
        public virtual Task<string> DecryptFileAsync(StorageLocation location, string sourcePath, string password)
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
        public virtual Task DecryptFileAsync(StorageLocation location, string sourcePath, string targetPath, string password)
        {
            return DecryptFileAsync(FullPath(location, sourcePath), FullPath(location, targetPath), password);
        }

        /// <summary>
        /// Decrypts a file, saving it to a new file, specified in targetfullPath.
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="targetFullPath">The target full path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public virtual async Task DecryptFileAsync(string sourceFullPath, string targetFullPath, string password)
        {
            if (sourceFullPath == targetFullPath)
                throw new NotSupportedException(string.Format("Source and Target paths cannot be equal: {0} | {1}", sourceFullPath, targetFullPath));

            if (await FileExistsAsync(sourceFullPath))
                await CryptologyAsync.DecryptFileAsync(sourceFullPath, targetFullPath, password);
        }

        /// <summary>
        /// Decrypts a file, and returns the correspondent stream.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public Task<Stream> DecryptFileToStreamAsync(StorageLocation location, string sourcePath, string password)
        {
            return DecryptFileToStreamAsync(FullPath(location, sourcePath), password);
        }

        /// <summary>
        /// Decrypts the file to stream asynchronous.
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public virtual async Task<Stream> DecryptFileToStreamAsync(string sourceFullPath, string password)
        {
            if (await FileExistsAsync(sourceFullPath))
                return await CryptologyAsync.DecryptFileToStreamAsync(sourceFullPath, password);

            return null;
        }

        /// <summary>
        /// Decrypts the string.
        /// </summary>
        /// <param name="stringToDecrypt">The string to decrypt.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public virtual Task<string> DecryptStringAsync(string stringToDecrypt, string password)
        {
            return CryptologyAsync.DecryptStringFromBytesAsync(Convert.FromBase64String(stringToDecrypt), password);
        }


        /// <summary>
        /// Tries the read an encrypted file.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <param name="password">The password.</param>
        /// <param name="streamAction">The stream action.</param>
        /// <returns>true if succeeds, otherwhise false</returns>
        protected virtual async Task<bool> TryReadEncryptedFileCommonAsync(string fullPath, string password, Func<Stream, bool> streamAction)
        {
            if (!File.Exists(fullPath))
            {
                return false;
            }

            using (var fileStream = await CryptologyAsync.DecryptFileToStreamAsync(fullPath, password))
            {
                return streamAction(fileStream);
            }
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
        public virtual Task WriteEncryptedFileAsync(StorageLocation location, StorageMode mode, string path, string contents, string password)
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
        public virtual Task WriteEncryptedFileAsync(StorageLocation location, StorageMode mode, string path, byte[] contents, string password)
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
        public virtual Task WriteEncryptedFileAsync(StorageMode mode, string fullPath, string contents, string password)
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
        public virtual async Task<string> EncryptFileAsync(string sourceFullPath, string password)
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
        public virtual Task<string> EncryptFileAsync(StorageLocation location, string sourcePath, string password)
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
        public virtual Task EncryptFileAsync(StorageLocation location, string sourcePath, string targetPath, string password)
        {
            return EncryptFileAsync(FullPath(location, sourcePath), FullPath(location, targetPath), password);
        }

        /// <summary>
        /// Encrypts a file, saving it to a new file, specified in targetfullPath.
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="targetFullPath">The target full path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public virtual async Task EncryptFileAsync(string sourceFullPath, string targetFullPath, string password)
        {
            if (sourceFullPath == targetFullPath)
                throw new NotSupportedException(string.Format("Source and Target paths cannot be equal: {0} | {1}", sourceFullPath, targetFullPath));

            if (await FileExistsAsync(sourceFullPath))
                await CryptologyAsync.EncryptFileAsync(sourceFullPath, targetFullPath, password);
        }

        /// <summary>
        /// Encrypts the string.
        /// </summary>
        /// <param name="stringToEncrypt">The string to encrypt.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public virtual async Task<string> EncryptStringAsync(string stringToEncrypt, string password)
        {
            return Convert.ToBase64String(await CryptologyAsync.EncryptStringToBytesAsync(stringToEncrypt, password));
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
        protected virtual Task WriteEncryptedFileCommonAsync(StorageLocation location, StorageMode mode, string path, Action<Stream> streamAction, string password)
        {
            var fullPath = FullPath(location, path);

            return WriteEncryptedFileCommonAsync(mode, fullPath, streamAction, password);
        }

        /// <summary>
        /// Common file to write encrypted content to a file.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="streamAction">The stream action.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        protected virtual async Task WriteEncryptedFileCommonAsync(StorageMode mode, string fullPath, Action<Stream> streamAction, string password)
        {
            await EnsureFolderExistsAsync(Path.GetDirectoryName(fullPath));

            using (var memoryStream = new MemoryStream())
            {
                lock (LockObj)
                {
                    streamAction(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                }
                await CryptologyAsync.EncryptStreamToFileAsync(memoryStream, fullPath, password, mode == StorageMode.Create ? EncryptionModeEnum.CRYPTOFULL : EncryptionModeEnum.CRYPTOLINE);
            }
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
        public virtual Task<string> RecoverEncryptedFileAsync(StorageLocation location, string sourcePath, string password)
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
        public virtual Task RecoverEncryptedFileAsync(StorageLocation location, string sourcePath, string targetPath, string password)
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
        public virtual async Task<string> RecoverEncryptedFileAsync(string sourceFullPath, string password)
        {
            var targetFullPath = sourceFullPath.Replace(Path.GetFileNameWithoutExtension(sourceFullPath), Path.GetFileNameWithoutExtension(sourceFullPath) + "_Recovered");
            await RecoverEncryptedFileAsync(sourceFullPath, targetFullPath, password);
            return targetFullPath;
        }

        /// <summary>
        /// Recovers the encrypted file.
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="targetFullPath">The target full path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public virtual Task RecoverEncryptedFileAsync(string sourceFullPath, string targetFullPath, string password)
        {
            return CryptologyAsync.RecoverEncryptedFileAsync(sourceFullPath, targetFullPath, password);
        }

        #endregion
    }
}