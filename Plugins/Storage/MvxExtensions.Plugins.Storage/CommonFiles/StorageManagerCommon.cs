using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

#if MONODROID
namespace MvxExtensions.Plugins.Storage.Droid
#else
namespace MvxExtensions.Plugins.Storage.Wpf
#endif
{
    /// <summary>
    /// Basse class for the storage plugin implementation
    /// </summary>
    /// <seealso cref="MvxExtensions.Plugins.Storage.IStorageManager" />
    public abstract class StorageManagerCommon : IStorageManager
    {
        /// <summary>
        /// Object to be used in lock statements
        /// </summary>
        protected object LockObj = new object();

        #region Generic Methods

        /// <summary>
        /// Gets a value indicating whether this instance is debug enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is debug enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsDebugEnabled
        {
            get { return _isDebugEnabled; }
        }
        private bool _isDebugEnabled = false;

        /// <summary>
        /// Sets the debug enabled.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public virtual void SetDebugEnabled(bool value)
        {
            _isDebugEnabled = value;
        }

        /// <summary>
        /// Gets the directory separator character.
        /// </summary>
        /// <value>
        /// The directory separator character.
        /// </value>
        public abstract char DirectorySeparatorChar { get; }

        /// <summary>
        /// Returns the platform specific full path, related to the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public string NativePath(StorageLocation location, string path)
        {
            return FullPath(location, path);
        }

        /// <summary>
        /// Combines the paths into a single path.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <returns></returns>
        public string PathCombine(params string[] paths)
        {
            if (paths == null || paths.Count() <= 0)
                return string.Empty;

            var count = paths.Count();
            var trimmedPaths = new string[count];


            for (int i = 0; i < count; i++)
            {
                paths[i] = paths[i].Replace('\\', DirectorySeparatorChar);
                paths[i] = paths[i].Replace('/', DirectorySeparatorChar);
                if (i == 0)
                    trimmedPaths[i] = paths[i];
                else
                    trimmedPaths[i] = paths[i].TrimStart(DirectorySeparatorChar);
            }

            return Path.Combine(trimmedPaths);
        }

        /// <summary>
        /// Gets the file name without extension.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);

        }

        /// <summary>
        /// Gets the file name with extension.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public string GetFileExtension(string path)
        {
            return Path.GetExtension(path);
        }

        /// <summary>
        /// Gets the name of the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
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
        public virtual Task<ulong> GetAvailableFreeSpaceAsync(string fullPath)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns the full physical path based on a location and a relative path
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The relative path.</param>
        /// <returns></returns>
        protected abstract string FullPath(StorageLocation location, string path);

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
        protected abstract Task<bool> TryReadFileCommonAsync(string fullPath, Func<Stream, bool> streamAction);

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
        protected abstract Task WriteFileCommonAsync(StorageMode mode, string fullPath, Action<Stream> streamAction);

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
        public abstract Task<IEnumerable<IFolderInfo>> GetFoldersInAsync(string folderFullPath, string searchPattern = "", SearchMode searchMode = SearchMode.Contains);

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
        public abstract Task<bool> IsFolderAsync(string fullPath);

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
        public abstract Task<bool> FolderExistsAsync(string fullPath);

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
        public abstract Task DeleteFolderAsync(string folderFullPath);

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
        /// Ensures the folder exists.
        /// </summary>
        /// <param name="folderfullPath">The folderfull path.</param>
        /// <returns></returns>
        public abstract Task EnsureFolderExistsAsync(string folderfullPath);

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
        public abstract Task CloneFolderAsync(string fromFullPath, string toFullPath, bool overwriteExistingTo, bool recursive);

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
        public abstract Task<bool> IsFileAsync(string fullPath);

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
        /// Checks if a files the exists in the specified path.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        public abstract Task<bool> FileExistsAsync(string fullPath);

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
        public abstract Task DeleteFileAsync(string fullPath);

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
        public abstract Task<IEnumerable<IFileInfo>> GetFilesInAsync(string folderFullPath, bool recursive, string searchPattern = "", SearchMode searchMode = SearchMode.Contains);

        /// <summary>
        /// Gets the file creation time.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public abstract Task<DateTime> GetFileCreationTimeAsync(StorageLocation location, string path);

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
        public abstract Task<bool> TryMoveAsync(string fromFullPath, string toFullPath, bool deleteExistingTo);

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
        public abstract Task CloneFileAsync(string fromFullPath, string toFullPath, bool overwriteExistingTo);

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
        public abstract Task<Stream> GetStreamFromFileAsync(string fullPath, StreamMode streamMode);

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
        public abstract Task<string> TryReadTextEncryptedFileAsync(StorageLocation location, string path, string password);

        /// <summary>
        /// Tries to read text from encrypted file .
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// Decripted text
        /// </returns>
        public abstract Task<string> TryReadTextEncryptedFileAsync(string fullPath, string password);

        /// <summary>
        /// Decrypts a file, saving it to a new file, specified in targetPath.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetPath">The target path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public abstract Task DecryptFileAsync(StorageLocation location, string sourcePath, string targetPath, string password);

        /// <summary>
        /// Decrypts a file, saving it to a new file, specified in targetfullPath.
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="targetFullPath">The target full path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public abstract Task DecryptFileAsync(string sourceFullPath, string targetFullPath, string password);

        /// <summary>
        /// Decrypts a file, saving it to a new file, with a name generated automatically
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// The destination file path
        /// </returns>
        public abstract Task<string> DecryptFileAsync(StorageLocation location, string sourcePath, string password);

        /// <summary>
        /// Decrypts a file, saving it to a new file, with a name generated automatically
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// The destination file path
        /// </returns>
        public abstract Task<string> DecryptFileAsync(string sourceFullPath, string password);

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
        public abstract Task<Stream> DecryptFileToStreamAsync(string sourceFullPath, string password);

        /// <summary>
        /// Decrypts the string.
        /// </summary>
        /// <param name="stringToDecrypt">The string to decrypt.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public abstract Task<string> DecryptStringAsync(string stringToDecrypt, string password);

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
        public abstract Task WriteEncryptedFileAsync(StorageLocation location, StorageMode mode, string path, string contents, string password);

        /// <summary>
        /// Writes the byte array to a file, using the password to encrypt the data.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public abstract Task WriteEncryptedFileAsync(StorageLocation location, StorageMode mode, string path, byte[] contents, string password);

        /// <summary>
        /// Writes the contents to a file, using the password to encrypt the data.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="contents">The contents.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public abstract Task WriteEncryptedFileAsync(StorageMode mode, string fullPath, string contents, string password);

        /// <summary>
        /// Encrypts a file, saving it to a new file, specified in targetPath.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetPath">The target path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public abstract Task EncryptFileAsync(StorageLocation location, string sourcePath, string targetPath, string password);

        /// <summary>
        /// Encrypts a file, saving it to a new file, specified in targetfullPath.
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="targetFullPath">The target full path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public abstract Task EncryptFileAsync(string sourceFullPath, string targetFullPath, string password);

        /// <summary>
        /// Encrypts a file, saving it to a new file, with a name generated automatically
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// The destination file path
        /// </returns>
        public abstract Task<string> EncryptFileAsync(StorageLocation location, string sourcePath, string password);

        /// <summary>
        /// Encrypts a file, saving it to a new file, with a name generated automatically
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// The destination file path
        /// </returns>
        public abstract Task<string> EncryptFileAsync(string sourceFullPath, string password);

        /// <summary>
        /// Encrypts the string.
        /// </summary>
        /// <param name="stringToEncrypt">The string to encrypt.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public abstract Task<string> EncryptStringAsync(string stringToEncrypt, string password);

        /// <summary>
        /// Common file to write encrypted content to a file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="path">The path.</param>
        /// <param name="streamAction">The stream action.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        protected abstract Task WriteEncryptedFileCommonAsync(StorageLocation location, StorageMode mode, string path, Action<Stream> streamAction, string password);

        /// <summary>
        /// Common file to write encrypted content to a file.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="streamAction">The stream action.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        protected abstract Task WriteEncryptedFileCommonAsync(StorageMode mode, string fullPath, Action<Stream> streamAction, string password);

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
        public abstract Task<string> RecoverEncryptedFileAsync(StorageLocation location, string sourcePath, string password);

        /// <summary>
        /// Recovers the encrypted file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetPath">The target path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public abstract Task RecoverEncryptedFileAsync(StorageLocation location, string sourcePath, string targetPath, string password);

        /// <summary>
        /// Recovers the encrypted file.
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// The destination file path
        /// </returns>
        public abstract Task<string> RecoverEncryptedFileAsync(string sourceFullPath, string password);

        /// <summary>
        /// Recovers the encrypted file.
        /// </summary>
        /// <param name="sourceFullPath">The source full path.</param>
        /// <param name="targetFullPath">The target full path.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        public abstract Task RecoverEncryptedFileAsync(string sourceFullPath, string targetFullPath, string password);

        /// <summary>
        /// Common method to read encrypted files.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <param name="password">The password.</param>
        /// <param name="streamAction">The stream action.</param>
        /// <returns></returns>
        protected abstract Task<bool> TryReadEncryptedFileCommonAsync(string fullPath, string password, Func<Stream, bool> streamAction);

        #endregion
    }
}