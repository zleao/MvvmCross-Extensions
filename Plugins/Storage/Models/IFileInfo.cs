using System;

namespace MvxExtensions.Plugins.Storage.Models
{
    /// <summary>
    /// IFileInfo
    /// </summary>
    public interface IFileInfo
    {
        /// <summary>
        /// file name with extension.
        /// </summary>
        string FileName { get; }

        /// <summary>
        ///Name of the folder where the file is located.
        /// </summary>
        string FolderName { get; }

        /// <summary>
        /// File's full path.
        /// </summary>
        string FileFullPath { get; }

        /// <summary>
        /// Folder's full path.
        /// </summary>
        string FolderFullPath { get; }

        /// <summary>
        /// Length of the file.
        /// </summary>
        long Length { get; }

        /// <summary>
        /// File's creation time.
        /// </summary>
        DateTimeOffset CreationTime { get; }
        /// <summary>
        /// File's creation time in UTC.
        /// </summary>
        DateTimeOffset CreationTimeUtc { get; }

        /// <summary>
        /// File's last access time.
        /// </summary>
        DateTimeOffset LastAccessTime { get; }
        /// <summary>
        /// File's last access time in UTC.
        /// </summary>
        DateTimeOffset LastAccessTimeUtc { get; }

        /// <summary>
        /// FIle's last write time.
        /// </summary>
        DateTimeOffset LastWriteTime { get; }
        /// <summary>
        /// File's last write time UTC.
        /// </summary>
        DateTimeOffset LastWriteTimeUtc { get; }
    }
}
