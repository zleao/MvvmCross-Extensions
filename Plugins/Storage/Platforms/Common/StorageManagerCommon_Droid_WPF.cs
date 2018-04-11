using MvvmCross.Logging;
using MvvmCross.Plugin.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MvxExtensions.Plugins.Storage.Platforms.Common
{
    /// <summary>
    /// Storage plugin implementation for Android and WPF
    /// </summary>
    /// <seealso cref="StorageManagerCommonEncryption" />
    public abstract class StorageManagerCommon_Droid_WPF : StorageManagerCommonEncryption
    {
        #region Constructor

        /// <summary>
        /// Finalizes an instance of the <see cref="StorageManagerCommon_Droid_WPF"/> class.
        /// </summary>
        ~StorageManagerCommon_Droid_WPF()
        {
            CryptologyAsync.DisposeAlgorithmAndPassword();
        }

        #endregion

        #region Generic Methods

        /// <summary>
        /// Gets the directory separator character.
        /// </summary>
        /// <value>
        /// The directory separator character.
        /// </value>
        public override char DirectorySeparatorChar
        {
            get { return Path.DirectorySeparatorChar; }
        }

        private string GetPatternFromSearchMode(string originalPattern, SearchMode searchMode)
        {
            var pattern = "*";
            if (!string.IsNullOrEmpty(originalPattern))
            {
                switch (searchMode)
                {
                    case SearchMode.StartsWith:
                        pattern = originalPattern + "*";
                        break;

                    case SearchMode.EndsWith:
                        pattern = "*" + originalPattern;
                        break;

                    case SearchMode.Contains:
                        pattern = "*" + originalPattern + "*";
                        break;

                    case SearchMode.Equals:
                        pattern = originalPattern;
                        break;
                }
            }

            return pattern;
        }

        /// <summary>
        /// Sets the debug enabled.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public override void SetDebugEnabled(bool value)
        {
            base.SetDebugEnabled(value);

            CryptologyAsync.SetDebugEnabled(value);
        }

        #endregion


        #region Read Methods

        /// <summary>
        /// Common implementation to try to read a file.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <param name="streamAction">The stream action.</param>
        /// <returns></returns>
        protected override Task<bool> TryReadFileCommonAsync(string fullPath, Func<Stream, bool> streamAction)
        {
            return Task.Run<bool>(() =>
            {
                if (!System.IO.File.Exists(fullPath))
                {
                    return false;
                }

                using (var fileStream = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    return streamAction(fileStream);
                }
            });
        }

        #endregion

        #region Write Methods

        /// <summary>
        /// Common implementation to write contents to a file.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="streamAction">The stream action.</param>
        /// <returns></returns>
        protected override async Task WriteFileCommonAsync(StorageMode mode, string fullPath, Action<Stream> streamAction)
        {
            await EnsureFolderExistsAsync(Path.GetDirectoryName(fullPath));

            await Task.Run(() =>
            {
                lock (LockObj)
                {
                    using (var fileStream = File.Open(fullPath, GetFileMode(mode), FileAccess.Write, FileShare.Read))
                    {
                        streamAction(fileStream);
                    }
                }
            });
        }

        #endregion

        #region Folder Management

        /// <summary>
        /// Returns a list of the folders in the specified location for a specified search pattern.
        /// </summary>
        /// <param name="folderFullPath">The folder full path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="searchMode">The search mode (Not implemented for WP8).</param>
        /// <returns></returns>
        public override Task<IEnumerable<IFolderInfo>> GetFoldersInAsync(string folderFullPath, string searchPattern = "", SearchMode searchMode = SearchMode.Contains)
        {
            var pattern = GetPatternFromSearchMode(searchPattern, searchMode);

            var di = new DirectoryInfo(folderFullPath);
            return Task.Run<IEnumerable<IFolderInfo>>(() => di.GetDirectories().Select(d => new BaseFolderInfo(d.Name)));
        }

        /// <summary>
        /// Determines whether the specified path points to a folder.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        public override async Task<bool> IsFolderAsync(string fullPath)
        {
            if (await FolderExistsAsync(fullPath))
            {
                var attr = File.GetAttributes(fullPath);
                return attr.HasFlag(FileAttributes.Directory);
            }
            return false;
        }

        /// <summary>
        /// Checks if a folder exists in the specified location.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        public override Task<bool> FolderExistsAsync(string fullPath)
        {
            return Task.FromResult(Directory.Exists(fullPath));
        }

        /// <summary>
        /// Deletes the folder in the specified location.
        /// If the folder contains files, set "Recursive' to true, to delete those files
        /// </summary>
        /// <param name="folderFullPath">The folder full path.</param>
        /// <returns></returns>
        public override Task DeleteFolderAsync(string folderFullPath)
        {
            return Task.Run(() => Directory.Delete(folderFullPath, true));
        }

        /// <summary>
        /// Ensures the folder exists.
        /// </summary>
        /// <param name="folderfullPath">The folderfull path.</param>
        /// <returns></returns>
        public override Task EnsureFolderExistsAsync(string folderfullPath)
        {
            return Task.Run(() =>
            {
                if (!Directory.Exists(folderfullPath))
                    Directory.CreateDirectory(folderfullPath);
            });
        }

        /// <summary>
        /// Clones a folder.
        /// </summary>
        /// <param name="fromFullPath">From full path.</param>
        /// <param name="toFullPath">To full path.</param>
        /// <param name="overwriteExistingTo">if set to <c>true</c> [overwrite existing to].</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns></returns>
        /// <exception cref="System.IO.DirectoryNotFoundException">Source directory does not exist or could not be found: "
        ///                     + fromFullPath</exception>
        public override async Task CloneFolderAsync(string fromFullPath, string toFullPath, bool overwriteExistingTo, bool recursive)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(fromFullPath);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + fromFullPath);
            }

            // If the destination directory doesn't exist, create it. 
            await EnsureFolderExistsAsync(toFullPath);

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                string temppath = Path.Combine(toFullPath, file.Name);
                file.CopyTo(temppath, overwriteExistingTo);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (recursive)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(toFullPath, subdir.Name);
                    await CloneFolderAsync(subdir.FullName, tempPath, overwriteExistingTo, recursive);
                }
            }

        }

        #endregion

        #region File Management

        /// <summary>
        /// Determines whether the specified path points to a file.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        public override async Task<bool> IsFileAsync(string fullPath)
        {
            if (await FileExistsAsync(fullPath))
            {
                var attr = File.GetAttributes(fullPath);
                return !attr.HasFlag(FileAttributes.Directory);
            }
            return false;
        }

        /// <summary>
        /// Checks if a files the exists in the specified path.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        public override Task<bool> FileExistsAsync(string fullPath)
        {
            return Task.FromResult(File.Exists(fullPath));
        }

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        public override Task DeleteFileAsync(string fullPath)
        {
            return Task.Run(() => File.Delete(fullPath));
        }

        /// <summary>
        /// Returns a list of the files in the specified location for a specified search pattern.
        /// </summary>
        /// <param name="folderFullPath">The folder full path.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="searchMode">The search mode (Not implemented for WP8).</param>
        /// <returns></returns>
        public override Task<IEnumerable<IFileInfo>> GetFilesInAsync(string folderFullPath, bool recursive, string searchPattern = "", SearchMode searchMode = SearchMode.Contains)
        {
            var pattern = GetPatternFromSearchMode(searchPattern, searchMode);
            var option = (recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            var di = new DirectoryInfo(folderFullPath);
            return Task.Run<IEnumerable<IFileInfo>>(() => di.GetFiles(pattern, option).Select(f => new FileInfo(f)));
        }

        /// <summary>
        /// Gets the file creation time.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public override Task<DateTime> GetFileCreationTimeAsync(StorageLocation location, string path)
        {
            return Task.Run(() => File.GetCreationTime(FullPath(location, path)));
        }

        /// <summary>
        /// Tries the move a file.
        /// </summary>
        /// <param name="fromFullPath">From full path.</param>
        /// <param name="toFullPath">To full path.</param>
        /// <param name="deleteExistingTo">if set to <c>true</c> [delete existing to].</param>
        /// <returns></returns>
        public override async Task<bool> TryMoveAsync(string fromFullPath, string toFullPath, bool deleteExistingTo)
        {
            try
            {
                if (!System.IO.File.Exists(fromFullPath))
                    return false;

                await EnsureFolderExistsAsync(toFullPath.Replace(GetFileName(toFullPath), string.Empty));

                return await Task.Run<bool>(() =>
                {
                    if (File.Exists(toFullPath))
                    {
                        if (deleteExistingTo)
                            File.Delete(toFullPath);
                        else
                            return false;
                    }

                    File.Move(fromFullPath, toFullPath);
                    return true;
                });
            }
            catch (AggregateException ag)
            {
                if (ag.InnerException.GetType() == typeof(ThreadAbortException))
                    throw;
                else
                {
                     MvxPluginLog.Instance.Error("Error during file move {0} : {1} : {2}", fromFullPath, toFullPath, ag.Message);
                    return false;
                }
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception exception)
            {
                 MvxPluginLog.Instance.Error("Error during file move {0} : {1} : {2}", fromFullPath, toFullPath, exception.Message);
                return false;
            }
        }

        /// <summary>
        /// Clones a file.
        /// </summary>
        /// <param name="fromFullPath">From full path.</param>
        /// <param name="toFullPath">To full path.</param>
        /// <param name="overwriteExistingTo">if set to <c>true</c> [overwrite existing to].</param>
        /// <returns></returns>
        public override async Task CloneFileAsync(string fromFullPath, string toFullPath, bool overwriteExistingTo)
        {
            try
            {
                if (File.Exists(fromFullPath))
                {
                    await EnsureFolderExistsAsync(toFullPath.Replace(GetFileName(toFullPath), string.Empty));

                    await Task.Run(() => File.Copy(fromFullPath, toFullPath, overwriteExistingTo));
                }
            }
            catch (AggregateException ag)
            {
                if (ag.InnerException.GetType() == typeof(ThreadAbortException))
                    throw;
                else
                {
                     MvxPluginLog.Instance.Error("Error during file move {0} : {1} : {2}", fromFullPath, toFullPath, ag.Message);
                    throw;
                }
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception exception)
            {
                MvxPluginLog.Instance.Trace("Error during file clone {0} : {1} : {2}", fromFullPath, toFullPath, exception.Message);
                throw;
            }
        }

        /// <summary>
        /// Gets a stream that points to the specified file.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <param name="streamMode">The stream mode.</param>
        /// <returns></returns>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        public override async Task<Stream> GetStreamFromFileAsync(string fullPath, StreamMode streamMode)
        {
            if (streamMode == StreamMode.Open && !(await FileExistsAsync(fullPath)))
                throw new FileNotFoundException(fullPath);

            return new FileStream(fullPath, GetFileMode(streamMode));
        }

        #endregion


        #region Decryption Methods

        /// <summary>
        /// Decrypts a file, saving it to a new file, specified in targetfullPath.
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="targetFullPath">The target full path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public override async Task DecryptFileAsync(string sourceFullPath, string targetFullPath, string password)
        {
            if (sourceFullPath == targetFullPath)
                throw new NotSupportedException(string.Format("Source and Target paths cannot be equal: {0} | {1}", sourceFullPath, targetFullPath));

            if (await FileExistsAsync(sourceFullPath))
                await CryptologyAsync.DecryptFileAsync(sourceFullPath, targetFullPath, password);
        }

        /// <summary>
        /// Decrypts the file to stream asynchronous.
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public override async Task<Stream> DecryptFileToStreamAsync(string sourceFullPath, string password)
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
        public override Task<string> DecryptStringAsync(string stringToDecrypt, string password)
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
        protected override async Task<bool> TryReadEncryptedFileCommonAsync(string fullPath, string password, Func<Stream, bool> streamAction)
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
        /// Encrypts a file, saving it to a new file, specified in targetfullPath.
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="targetFullPath">The target full path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public override async Task EncryptFileAsync(string sourceFullPath, string targetFullPath, string password)
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
        public override async Task<string> EncryptStringAsync(string stringToEncrypt, string password)
        {
            return Convert.ToBase64String(await CryptologyAsync.EncryptStringToBytesAsync(stringToEncrypt, password));
        }

        /// <summary>
        /// Common file to write encrypted content to a file.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="streamAction">The stream action.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        protected override async Task WriteEncryptedFileCommonAsync(StorageMode mode, string fullPath, Action<Stream> streamAction, string password)
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
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="targetFullPath">The target full path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public override Task RecoverEncryptedFileAsync(string sourceFullPath, string targetFullPath, string password)
        {
            return CryptologyAsync.RecoverEncryptedFileAsync(sourceFullPath, targetFullPath, password);
        }

        #endregion
    }
}
