using MvvmCross.Logging;
using MvxExtensions.Plugins.Storage.Models;
using MvxExtensions.Plugins.Storage.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MvxExtensions.Plugins.Storage
{
    /// <summary>
    /// Basse class for the storage plugin implementation
    /// </summary>
    /// <seealso cref="IStorageManager" />
    public abstract class StorageManager : StorageManagerCommon, IStorageManager
    {
        #region Generic Methods

        /// <summary>
        /// Gets a value indicating whether this instance is debug enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is debug enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsDebugEnabled { get; private set; }

        /// <summary>
        /// Sets the debug enabled.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public virtual void SetDebugEnabled(bool value)
        {
            IsDebugEnabled = value;
        
            CryptologyAsync.SetDebugEnabled(value);
        }

        /// <summary>
        /// Returns the available free space in bytes for a given path
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public Task<ulong> GetAvailableFreeSpaceAsync(StorageLocation location, string path)
        {
            return GetAvailableFreeSpaceAsync(FullPath(location, path));
        }

        /// <summary>
        /// Returns the available free space in bytes for a given path
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public abstract Task<ulong> GetAvailableFreeSpaceAsync(string fullPath);


        /// <summary>
        /// Clears the stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        protected void ClearStream(Stream stream)
        {
            if (stream != null)
            {
                stream.SetLength(0);
                if (stream.CanSeek)
                    stream.Seek(0, SeekOrigin.Begin);
            }
        }

        /// <summary>
        /// Gets the file mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        protected FileMode GetFileMode(StorageMode mode)
        {
            if (mode == StorageMode.Create)
                return FileMode.Create;

            return FileMode.Append;
        }

        /// <summary>
        /// Gets the file mode.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        protected FileMode GetFileMode(StreamMode mode)
        {
            if (mode == StreamMode.Open)
                return FileMode.Open;

            if (mode == StreamMode.Create)
                return FileMode.Create;

            return FileMode.Append;
        }

        /// <summary>
        /// Gets the search pattern string, from the search mode.
        /// </summary>
        /// <param name="originalPattern">The original pattern.</param>
        /// <param name="searchMode">The search mode.</param>
        /// <returns></returns>
        protected string GetPatternFromSearchMode(string originalPattern, SearchMode searchMode)
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
        
        #endregion

        #region Read Methods

        /// <summary>
        /// Tries to read text from a file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public Task<string> TryReadTextFileAsync(StorageLocation location, string path)
        {
            return TryReadTextFileAsync(FullPath(location, path));
        }

        /// <summary>
        /// Tries to read text from a file.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        public async Task<string> TryReadTextFileAsync(string fullPath)
        {
            string result = null;
            var readResult = await TryReadFileCommonAsync(fullPath, (stream) =>
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
        /// Tries to read a binary file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public async Task<Byte[]> TryReadBinaryFileAsync(StorageLocation location, string path)
        {
            Byte[] result = null;
            var readResult = await TryReadFileCommonAsync(FullPath(location, path), (stream) =>
            {
                using (var binaryReader = new BinaryReader(stream))
                {
                    var memoryBuffer = new byte[stream.Length];
                    if (binaryReader.Read(memoryBuffer, 0,
                                          memoryBuffer.Length) !=
                        memoryBuffer.Length)
                        return false; // TODO - do more here?

                    result = memoryBuffer;
                    return true;
                }
            });

            return result;
        }

        /// <summary>
        /// Tries to read a binary file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <param name="readMethod">The read method.</param>
        /// <returns></returns>
        public Task<bool> TryReadBinaryFileAsync(StorageLocation location, string path, Func<Stream, bool> readMethod)
        {
            return TryReadFileCommonAsync(FullPath(location, path), readMethod);
        }


        /// <summary>
        /// Common implementation to try to read a file.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <param name="streamAction">The stream action.</param>
        /// <returns></returns>
        protected virtual Task<bool> TryReadFileCommonAsync(string fullPath, Func<Stream, bool> streamAction)
        {
            return Task.Run<bool>(() =>
            {
                if (!File.Exists(fullPath))
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
        /// Writes a stream to a file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="path">The path.</param>
        /// <param name="sourceStream">The source stream.</param>
        /// <returns></returns>
        public Task WriteFileAsync(StorageLocation location, StorageMode mode, string path, Stream sourceStream)
        {
            if (sourceStream != null)
            {
                return WriteFileCommonAsync(location, mode, path, (stream) =>
                        {
                            if (stream != null)
                            {
                                //if (stream.CanSeek)
                                //    sourceStream.Seek(0, SeekOrigin.Begin);
                                sourceStream.CopyTo(stream);
                            }
                        });
            }

            return null;
        }

        /// <summary>
        /// Writes contents to a file, using an action passed by argument.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="path">The path.</param>
        /// <param name="writeMethod">The write method.</param>
        /// <returns></returns>
        public Task WriteFileAsync(StorageLocation location, StorageMode mode, string path, Action<Stream> writeMethod)
        {
            return WriteFileCommonAsync(location, mode, path, writeMethod);
        }

        /// <summary>
        /// Writes the contents to a file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        /// <returns></returns>
        public Task WriteFileAsync(StorageLocation location, StorageMode mode, string path, string contents)
        {
            return WriteFileCommonAsync(location, mode, path, (stream) =>
                    {
                        using (var sw = new StreamWriter(stream))
                        {
                            sw.Write(contents);
                            sw.Flush();
                        }
                    });
        }

        /// <summary>
        /// Writes the binary contents to a file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        /// <returns></returns>
        public Task WriteFileAsync(StorageLocation location, StorageMode mode, string path, IEnumerable<Byte> contents)
        {
            return WriteFileCommonAsync(location, mode, path, (stream) =>
                    {
                        using (var binaryWriter = new BinaryWriter(stream))
                        {
                            binaryWriter.Write(contents.ToArray());
                            binaryWriter.Flush();
                        }
                    });
        }

        /// <summary>
        /// Writes a stream to a file.
        /// The path passed by parameter, is the FULL PATH to where the file will be saved
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="sourceStream">The source stream.</param>
        /// <returns></returns>
        public Task WriteFileAsync(StorageMode mode, string fullPath, Stream sourceStream)
        {
            if (sourceStream != null)
            {
                return WriteFileCommonAsync(mode, fullPath, (stream) =>
                        {
                            if (stream != null)
                            {
                                if(sourceStream.CanSeek)
                                    sourceStream.Seek(0, SeekOrigin.Begin);
                                sourceStream.CopyTo(stream);
                            }
                        });
            }

            return null;
        }

        /// <summary>
        /// Writes contents to a file, using an action passed by argument.
        /// The path passed by parameter, is the FULL PATH to where the file will be saved
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="writeMethod">The write method.</param>
        /// <returns></returns>
        public Task WriteFileAsync(StorageMode mode, string fullPath, Action<Stream> writeMethod)
        {
            return WriteFileCommonAsync(mode, fullPath, writeMethod);
        }

        /// <summary>
        /// Writes the contents to a file.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="contents">The contents.</param>
        /// <returns></returns>
        public Task WriteFileAsync(StorageMode mode, string fullPath, string contents)
        {
            return WriteFileCommonAsync(mode, fullPath, (stream) =>
            {
                using (var sw = new StreamWriter(stream))
                {
                    sw.Write(contents);
                    sw.Flush();
                }
            });
        }


        /// <summary>
        /// Common implementation to write contents to a file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="path">The path.</param>
        /// <param name="streamAction">The stream action.</param>
        /// <returns></returns>
        protected Task WriteFileCommonAsync(StorageLocation location, StorageMode mode, string path, Action<Stream> streamAction)
        {
            var fullPath = FullPath(location, path);

            return WriteFileCommonAsync(mode, fullPath, streamAction);
        }

        /// <summary>
        /// Common implementation to write contents to a file.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="streamAction">The stream action.</param>
        /// <returns></returns>
        protected virtual async Task WriteFileCommonAsync(StorageMode mode, string fullPath, Action<Stream> streamAction)
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
        /// <param name="location">The location.</param>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="searchMode">The search mode (Not implemented for WP8).</param>
        /// <returns></returns>
        public Task<IEnumerable<IFolderInfo>> GetFoldersInAsync(StorageLocation location, string folderPath = "", string searchPattern = "", SearchMode searchMode = SearchMode.Contains)
        {
            var folderFullPath = FullPath(location, folderPath);
            return GetFoldersInAsync(folderFullPath, searchPattern, searchMode);
        }

        /// <summary>
        /// Returns a list of the folders in the specified location for a specified search pattern.
        /// </summary>
        /// <param name="folderFullPath">The folder full path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="searchMode">The search mode (Not implemented for WP8).</param>
        /// <returns></returns>
        public virtual Task<IEnumerable<IFolderInfo>> GetFoldersInAsync(string folderFullPath, string searchPattern = "", SearchMode searchMode = SearchMode.Contains)
        {
            var pattern = GetPatternFromSearchMode(searchPattern, searchMode);

            var di = new DirectoryInfo(folderFullPath);
            return Task.Run<IEnumerable<IFolderInfo>>(() => di.GetDirectories().Select(d => new BaseFolderInfo(d.Name)));
        }

        /// <summary>
        /// Determines whether the specified path points to a folder.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public Task<bool> IsFolderAsync(StorageLocation location, string path)
        {
            return IsFolderAsync(NativePath(location, path));
        }

        /// <summary>
        /// Determines whether the specified path points to a folder.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        public virtual async Task<bool> IsFolderAsync(string fullPath)
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
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public Task<bool> FolderExistsAsync(StorageLocation location, string path)
        {
            var fullPath = FullPath(location, path);
            return FolderExistsAsync(fullPath);
        }

        /// <summary>
        /// Checks if a folder exists in the specified location.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        public virtual Task<bool> FolderExistsAsync(string fullPath)
        {
            return Task.FromResult(Directory.Exists(fullPath));
        }

        /// <summary>
        /// Deletes the folder in the specified location.
        /// If the folder contains files, set "Recursive' to true, to delete those files
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="folderPath">The folder path.</param>
        /// <returns></returns>
        public Task DeleteFolderAsync(StorageLocation location, string folderPath)
        {
            var fullPath = FullPath(location, folderPath);
            return DeleteFolderAsync(fullPath);
        }

        /// <summary>
        /// Deletes the folder in the specified location.
        /// If the folder contains files, set "Recursive' to true, to delete those files
        /// </summary>
        /// <param name="folderFullPath">The folder full path.</param>
        /// <returns></returns>
        public virtual Task DeleteFolderAsync(string folderFullPath)
        {
            return Task.Run(() => Directory.Delete(folderFullPath, true));
        }

        /// <summary>
        /// Ensures the folder exists, by creating it if not allready present.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="folderPath">The folder path.</param>
        /// <returns></returns>
        public Task EnsureFolderExistsAsync(StorageLocation location, string folderPath)
        {
            var fullPath = FullPath(location, folderPath);
            return EnsureFolderExistsAsync(fullPath);
        }

        /// <summary>
        /// Clones a folder
        /// </summary>
        /// <param name="fromLocation">From location.</param>
        /// <param name="fromPath">From path.</param>
        /// <param name="toLocation">To location.</param>
        /// <param name="toPath">To path.</param>
        /// <param name="overwriteExistingTo">if set to <c>true</c> [overwrite existing to].</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns></returns>
        public Task CloneFolderAsync(StorageLocation fromLocation, string fromPath, StorageLocation toLocation, string toPath, bool overwriteExistingTo, bool recursive)
        {
            var fromFullPath = FullPath(fromLocation, fromPath);
            var toFullPath = FullPath(toLocation, toPath);

            return CloneFolderAsync(fromFullPath, toFullPath, overwriteExistingTo, recursive);
        }

        /// <summary>
        /// Clones a folder.
        /// </summary>
        /// <param name="fromFullPath">From full path.</param>
        /// <param name="toFullPath">To full path.</param>
        /// <param name="overwriteExistingTo">if set to <c>true</c> [overwrite existing to].</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns></returns>
        public virtual async Task CloneFolderAsync(string fromFullPath, string toFullPath, bool overwriteExistingTo, bool recursive)
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
                string tempPath = Path.Combine(toFullPath, file.Name);
                file.CopyTo(tempPath, overwriteExistingTo);
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
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public Task<bool> IsFileAsync(StorageLocation location, string path)
        {
            return IsFileAsync(NativePath(location, path));
        }

        /// <summary>
        /// Determines whether the specified path points to a file.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        public virtual async Task<bool> IsFileAsync(string fullPath)
        {
            if (await FileExistsAsync(fullPath))
            {
                var attr = File.GetAttributes(fullPath);
                return !attr.HasFlag(FileAttributes.Directory);
            }
            return false;
        }

        /// <summary>
        /// Checks if a files the exists in the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public Task<bool> FileExistsAsync(StorageLocation location, string path)
        {
            var fullPath = FullPath(location, path);
            return FileExistsAsync(fullPath);
        }

        /// <summary>
        /// Deletes the file in the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public Task DeleteFileAsync(StorageLocation location, string path)
        {
            var fullPath = FullPath(location, path);
            return DeleteFileAsync(fullPath);
        }

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        public virtual Task DeleteFileAsync(string fullPath)
        {
            return Task.Run(() => File.Delete(fullPath));
        }

        /// <summary>
        /// Returns a list of the files in the specified location for a specified search pattern.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="searchMode">The search mode (Not implemented for WP8).</param>
        /// <returns></returns>
        public Task<IEnumerable<IFileInfo>> GetFilesInAsync(StorageLocation location, bool recursive, string folderPath = "", string searchPattern = "", SearchMode searchMode = SearchMode.Contains)
        {
            var folderFullPath = FullPath(location, folderPath);
            return GetFilesInAsync(folderFullPath, recursive, searchPattern, searchMode);
        }

        /// <summary>
        /// Returns a list of the files in the specified location for a specified search pattern.
        /// </summary>
        /// <param name="folderFullPath">The folder full path.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="searchMode">The search mode (Not implemented for WP8).</param>
        /// <returns></returns>
        public virtual Task<IEnumerable<IFileInfo>> GetFilesInAsync(string folderFullPath, bool recursive, string searchPattern = "", SearchMode searchMode = SearchMode.Contains)
        {
            var pattern = GetPatternFromSearchMode(searchPattern, searchMode);
            var option = (recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            var di = new DirectoryInfo(folderFullPath);
            return Task.Run<IEnumerable<IFileInfo>>(() => di.GetFiles(pattern, option).Select(f => new Models.FileInfo(f)));
        }

        /// <summary>
        /// Gets the file creation time.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public virtual Task<DateTime> GetFileCreationTimeAsync(StorageLocation location, string path)
        {
            return Task.Run(() => File.GetCreationTime(FullPath(location, path)));
        }

        /// <summary>
        /// Tries the move a file.
        /// </summary>
        /// <param name="fromLocation">From location.</param>
        /// <param name="fromPath">From path.</param>
        /// <param name="toLocation">To location.</param>
        /// <param name="toPath">To path.</param>
        /// <param name="deleteExistingTo">if set to <c>true</c> [delete existing to].</param>
        /// <returns></returns>
        public Task<bool> TryMoveAsync(StorageLocation fromLocation, string fromPath, StorageLocation toLocation, string toPath, bool deleteExistingTo)
        {
            var fromFullPath = FullPath(fromLocation, fromPath);
            var toFullPath = FullPath(toLocation, toPath);

            return TryMoveAsync(fromFullPath, toFullPath, deleteExistingTo);
        }

        /// <summary>
        /// Tries the move a file.
        /// </summary>
        /// <param name="fromFullPath">From full path.</param>
        /// <param name="toFullPath">To full path.</param>
        /// <param name="deleteExistingTo">if set to <c>true</c> [delete existing to].</param>
        /// <returns></returns>
        public virtual async Task<bool> TryMoveAsync(string fromFullPath, string toFullPath, bool deleteExistingTo)
        {
            try
            {
                if (!System.IO.File.Exists(fromFullPath))
                    return false;

                await EnsureFolderExistsAsync(toFullPath.Replace(Path.GetFileName(toFullPath), string.Empty));

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
        /// <param name="fromLocation">From location.</param>
        /// <param name="fromPath">From path.</param>
        /// <param name="toLocation">To location.</param>
        /// <param name="toPath">To path.</param>
        /// <param name="overwriteExistingTo">if set to <c>true</c> [overwrite existing to].</param>
        /// <returns></returns>
        public Task CloneFileAsync(StorageLocation fromLocation, string fromPath, StorageLocation toLocation, string toPath, bool overwriteExistingTo)
        {
            var fromFullPath = FullPath(fromLocation, fromPath);
            var toFullPath = FullPath(toLocation, toPath);

            return CloneFileAsync(fromFullPath, toFullPath, overwriteExistingTo);
        }

        /// <summary>
        /// Clones a file.
        /// </summary>
        /// <param name="fromFullPath">From full path.</param>
        /// <param name="toFullPath">To full path.</param>
        /// <param name="overwriteExistingTo">if set to <c>true</c> [overwrite existing to].</param>
        /// <returns></returns>
        public virtual async Task CloneFileAsync(string fromFullPath, string toFullPath, bool overwriteExistingTo)
        {
            try
            {
                if (File.Exists(fromFullPath))
                {
                    await EnsureFolderExistsAsync(toFullPath.Replace(Path.GetFileName(toFullPath), string.Empty));

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
        /// Clones a file that exists in the app resources folder
        /// </summary>
        /// <param name="fromPath">From path.</param>
        /// <param name="toLocation">To location.</param>
        /// <param name="toPath">To path.</param>
        /// <returns></returns>
        public abstract Task CloneFileFromAppResourcesAsync(string fromPath, StorageLocation toLocation, string toPath);

        /// <summary>
        /// Gets a stream that points to the specified file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <param name="streamMode">The stream mode.</param>
        /// <returns></returns>
        public Task<Stream> GetStreamFromFileAsync(StorageLocation location, string path, StreamMode streamMode)
        {
            return GetStreamFromFileAsync(FullPath(location, path), streamMode);
        }

        /// <summary>
        /// Gets a stream that points to the specified file.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <param name="streamMode">The stream mode.</param>
        /// <returns></returns>
        public virtual async Task<Stream> GetStreamFromFileAsync(string fullPath, StreamMode streamMode)
        {
            if (streamMode == StreamMode.Open && !(await FileExistsAsync(fullPath)))
                throw new FileNotFoundException(fullPath);

            return new FileStream(fullPath, GetFileMode(streamMode));
        }

        #endregion

        #region Compression/Decompression

        /// <summary>
        /// Compresses the stream.
        /// </summary>
        /// <param name="streamToCompress">The stream to compress.</param>
        /// <returns></returns>
        public async Task<Stream> CompressStreamAsync(Stream streamToCompress)
        {
            if (streamToCompress != null && streamToCompress.CanSeek)
                streamToCompress.Seek(0, SeekOrigin.Begin);

            var compressedStream = new MemoryStream();

            using (var compressor = new DeflateStream(compressedStream, CompressionMode.Compress, true))
            {
                await streamToCompress.CopyToAsync(compressor);
            }

            compressedStream.Seek(0, SeekOrigin.Begin);
            return compressedStream;
        }

        /// <summary>
        /// Decompresses the stream.
        /// </summary>
        /// <param name="streamToDecompress">The stream to decompress.</param>
        /// <returns></returns>
        public async Task<Stream> DecompressStreamAsync(Stream streamToDecompress)
        {
            if (streamToDecompress != null && streamToDecompress.CanSeek)
                streamToDecompress.Seek(0, SeekOrigin.Begin);

            var decompressedStream = new MemoryStream();

            using (DeflateStream decompressor = new DeflateStream(streamToDecompress, CompressionMode.Decompress, true))
            {
                await decompressor.CopyToAsync(decompressedStream);
            }

            decompressedStream.Seek(0, SeekOrigin.Begin);
            return decompressedStream;
        }

        #endregion
    }
}