using MvxExtensions.Plugins.Storage.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MvxExtensions.Plugins.Storage
{
    /// <summary>
    /// Stoarge manager interface
    /// </summary>
    public interface IStorageManager
    {
        #region Generic Methods

        /// <summary>
        /// Gets a value indicating whether this instance is debug enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is debug enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsDebugEnabled { get; }

        /// <summary>
        /// Sets the debug enabled.
        /// </summary>
        /// <param name="value">if set to <c>true</c> [value].</param>
        void SetDebugEnabled(bool value);

        /// <summary>
        /// Combines the paths into a single path.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <returns></returns>
        string PathCombine(params string[] paths);

        /// <summary>
        /// Returns the platform specific full path, related to the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        string NativePath(StorageLocation location, string path);

        /// <summary>
        /// Returns the available free space in bytes for a given path
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        Task<ulong> GetAvailableFreeSpaceAsync(StorageLocation location, string path);

        /// <summary>
        /// Returns the available free space in bytes for a given path
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        Task<ulong> GetAvailableFreeSpaceAsync(string fullPath);

        #endregion

        #region Read Methods

        /// <summary>
        /// Tries to read text file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        Task<string> TryReadTextFileAsync(StorageLocation location, string path);

        /// <summary>
        /// Tries to read text file.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        Task<string> TryReadTextFileAsync(string fullPath);

        /// <summary>
        /// Tries to read binary file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        Task<Byte[]> TryReadBinaryFileAsync(StorageLocation location, string path);

        /// <summary>
        /// Tries to read binary file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <param name="readMethod">The read method.</param>
        /// <returns></returns>
        Task<bool> TryReadBinaryFileAsync(StorageLocation location, string path, Func<Stream, bool> readMethod);

        #endregion

        #region Write Methods

        /// <summary>
        /// Writes a stream to a file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="path">The path.</param>
        /// <param name="sourceStream">The source stream.</param>
        Task WriteFileAsync(StorageLocation location, StorageMode mode, string path, Stream sourceStream);

        /// <summary>
        /// Writes contents to a file, using an action passed by argument.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="path">The path.</param>
        /// <param name="writeMethod">The write method.</param>
        Task WriteFileAsync(StorageLocation location, StorageMode mode, string path, Action<Stream> writeMethod);

        /// <summary>
        /// Writes the binary contents to a file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        Task WriteFileAsync(StorageLocation location, StorageMode mode, string path, IEnumerable<Byte> contents);

        /// <summary>
        /// Writes the contents to a file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        /// <returns></returns>
        Task WriteFileAsync(StorageLocation location, StorageMode mode, string path, string contents);

        /// <summary>
        /// Writes a stream to a file.
        /// The path passed by parameter, is the FULL PATH to where the file will be saved
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="sourceStream">The source stream.</param>
        Task WriteFileAsync(StorageMode mode, string fullPath, Stream sourceStream);

        /// <summary>
        /// Writes contents to a file, using an action passed by argument.
        /// The path passed by parameter, is the FULL PATH to where the file will be saved
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="writeMethod">The write method.</param>
        Task WriteFileAsync(StorageMode mode, string fullPath, Action<Stream> writeMethod);

        /// <summary>
        /// Writes the contents to a file.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="contents">The contents.</param>
        Task WriteFileAsync(StorageMode mode, string fullPath, string contents);

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
        Task<IEnumerable<IFolderInfo>> GetFoldersInAsync(StorageLocation location, string folderPath = "", string searchPattern = "", SearchMode searchMode = SearchMode.Contains);

        /// <summary>
        /// Returns a list of the folders in the specified location for a specified search pattern.
        /// </summary>
        /// <param name="folderFullPath">The folder full path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="searchMode">The search mode (Not implemented for WP8).</param>
        /// <returns></returns>
        Task<IEnumerable<IFolderInfo>> GetFoldersInAsync(string folderFullPath, string searchPattern = "", SearchMode searchMode = SearchMode.Contains);

        /// <summary>
        /// Determines whether the specified path points to a folder.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        Task<bool> IsFolderAsync(StorageLocation location, string path);

        /// <summary>
        /// Determines whether the specified path points to a folder.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        Task<bool> IsFolderAsync(string fullPath);

        /// <summary>
        /// Checks if a folder exists in the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        Task<bool> FolderExistsAsync(StorageLocation location, string path);

        /// <summary>
        /// Checks if a folder exists in the specified location.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        Task<bool> FolderExistsAsync(string fullPath);

        /// <summary>
        /// Deletes the folder in the specified location.
        /// If the folder contains files, set "Recursive' to true, to delete those files
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="folderPath">The folder path.</param>
        /// <returns></returns>
        Task DeleteFolderAsync(StorageLocation location, string folderPath);

        /// <summary>
        /// Deletes the folder in the specified location.
        /// If the folder contains files, set "Recursive' to true, to delete those files
        /// </summary>
        /// <param name="folderFullPath">The folder full path.</param>
        /// <returns></returns>
        Task DeleteFolderAsync(string folderFullPath);

        /// <summary>
        /// Ensures the folder exists, by creating it if not allready present.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        Task EnsureFolderExistsAsync(string folderPath);

        /// <summary>
        /// Ensures the folder exists, by creating it if not allready present.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="folderPath">The folder path.</param>
        Task EnsureFolderExistsAsync(StorageLocation location, string folderPath);

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
        Task CloneFolderAsync(StorageLocation fromLocation, string fromPath, StorageLocation toLocation, string toPath, bool overwriteExistingTo, bool recursive);

        /// <summary>
        /// Clones a folder.
        /// </summary>
        /// <param name="fromFullPath">From full path.</param>
        /// <param name="toFullPath">To full path.</param>
        /// <param name="overwriteExistingTo">if set to <c>true</c> [overwrite existing to].</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <returns></returns>
        Task CloneFolderAsync(string fromFullPath, string toFullPath, bool overwriteExistingTo, bool recursive);

        #endregion

        #region File Management

        /// <summary>
        /// Determines whether the specified path points to a file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        Task<bool> IsFileAsync(StorageLocation location, string path);

        /// <summary>
        /// Determines whether the specified path points to a file.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        Task<bool> IsFileAsync(string fullPath);

        /// <summary>
        /// Checks if a files the exists in the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        Task<bool> FileExistsAsync(StorageLocation location, string path);

        /// <summary>
        /// Checks if a files the exists in the specified path.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        Task<bool> FileExistsAsync(string fullPath);

        /// <summary>
        /// Deletes the file in the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        Task DeleteFileAsync(StorageLocation location, string path);

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        Task DeleteFileAsync(string fullPath);

        /// <summary>
        /// Returns a list of the files in the specified location for a specified search pattern.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="searchMode">The search mode (Not implemented for WP8).</param>
        /// <returns></returns>
        Task<IEnumerable<IFileInfo>> GetFilesInAsync(StorageLocation location, bool recursive, string folderPath = "", string searchPattern = "", SearchMode searchMode = SearchMode.Contains);

        /// <summary>
        /// Returns a list of the files in the specified location for a specified search pattern.
        /// </summary>
        /// <param name="folderFullPath">The folder full path.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <param name="searchMode">The search mode (Not implemented for WP8).</param>
        /// <returns></returns>
        Task<IEnumerable<IFileInfo>> GetFilesInAsync(string folderFullPath, bool recursive, string searchPattern = "", SearchMode searchMode = SearchMode.Contains);

        /// <summary>
        /// Gets the file creation time.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        Task<DateTime> GetFileCreationTimeAsync(StorageLocation location, string path);

        /// <summary>
        /// Tries the move a file.
        /// </summary>
        /// <param name="fromLocation">From location.</param>
        /// <param name="fromPath">From path.</param>
        /// <param name="toLocation">To location.</param>
        /// <param name="toPath">To path.</param>
        /// <param name="deleteExistingTo">if set to <c>true</c> [delete existing to].</param>
        /// <returns></returns>
        Task<bool> TryMoveAsync(StorageLocation fromLocation, string fromPath, StorageLocation toLocation, string toPath, bool deleteExistingTo);

        /// <summary>
        /// Tries the move a file.
        /// </summary>
        /// <param name="fromFullPath">From full path.</param>
        /// <param name="toFullPath">To full path.</param>
        /// <param name="deleteExistingTo">if set to <c>true</c> [delete existing to].</param>
        /// <returns></returns>
        Task<bool> TryMoveAsync(string fromFullPath, string toFullPath, bool deleteExistingTo);

        /// <summary>
        /// Clones a file.
        /// </summary>
        /// <param name="fromLocation">From location.</param>
        /// <param name="fromPath">From path.</param>
        /// <param name="toLocation">To location.</param>
        /// <param name="toPath">To path.</param>
        /// <param name="overwriteExistingTo">if set to <c>true</c> [overwrite existing to].</param>
        Task CloneFileAsync(StorageLocation fromLocation, string fromPath, StorageLocation toLocation, string toPath, bool overwriteExistingTo);

        /// <summary>
        /// Clones a file.
        /// </summary>
        /// <param name="fromFullPath">From full path.</param>
        /// <param name="toFullPath">To full path.</param>
        /// <param name="overwriteExistingTo">if set to <c>true</c> [overwrite existing to].</param>
        Task CloneFileAsync(string fromFullPath, string toFullPath, bool overwriteExistingTo);

        /// <summary>
        /// Clones a file that exists in the app resources folder
        /// </summary>
        /// <param name="fromPath">From path.</param>
        /// <param name="toLocation">To location.</param>
        /// <param name="toPath">To path.</param>
        Task CloneFileFromAppResourcesAsync(string fromPath, StorageLocation toLocation, string toPath);

        /// <summary>
        /// Gets a stream that points to the specified file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <param name="streamMode">The stream mode.</param>
        /// <returns></returns>
        Task<Stream> GetStreamFromFileAsync(StorageLocation location, string path, StreamMode streamMode);

        /// <summary>
        /// Gets a stream that points to the specified file.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <param name="streamMode">The stream mode.</param>
        /// <returns></returns>
        Task<Stream> GetStreamFromFileAsync(string fullPath, StreamMode streamMode);

        #endregion

        #region Compression/Decompression

        /// <summary>
        /// Compresses the stream.
        /// </summary>
        /// <param name="streamToCompress">The stream to compress.</param>
        /// <returns></returns>
        Task<Stream> CompressStreamAsync(Stream streamToCompress);

        /// <summary>
        /// Decompresses the stream.
        /// </summary>
        /// <param name="streamToDecompress">The stream to decompress.</param>
        /// <returns></returns>
        Task<Stream> DecompressStreamAsync(Stream streamToDecompress);

        #endregion
    }
}
