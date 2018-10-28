using System;

namespace MvxExtensions.Plugins.Storage.Models
{
    /// <summary>
    /// Base abstract implementation fo the IFileInfo interface
    /// </summary>
    public abstract class BaseFileInfo : IFileInfo
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string FileName { get; protected set; }

        /// <summary>
        /// Gets the name of the folder.
        /// </summary>
        /// <value>
        /// The name of the folder.
        /// </value>
        public string FolderName { get; protected set; }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string FileFullPath { get; protected set; }

        /// <summary>
        /// Gets the folder path.
        /// </summary>
        /// <value>
        /// The folder path.
        /// </value>
        public string FolderFullPath { get; protected set; }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        public long Length { get; protected set; }

        /// <summary>
        /// Gets the creation time.
        /// </summary>
        /// <value>
        /// The creation time.
        /// </value>
        public DateTimeOffset CreationTime { get; protected set; }
        /// <summary>
        /// Gets the creation time UTC.
        /// </summary>
        /// <value>
        /// The creation time UTC.
        /// </value>
        public DateTimeOffset CreationTimeUtc { get; protected set; }

        /// <summary>
        /// Gets the last access time.
        /// </summary>
        /// <value>
        /// The last access time.
        /// </value>
        public DateTimeOffset LastAccessTime { get; protected set; }
        /// <summary>
        /// Gets the last access time UTC.
        /// </summary>
        /// <value>
        /// The last access time UTC.
        /// </value>
        public DateTimeOffset LastAccessTimeUtc { get; protected set; }

        /// <summary>
        /// Gets the last write time.
        /// </summary>
        /// <value>
        /// The last write time.
        /// </value>
        public DateTimeOffset LastWriteTime { get; protected set; }
        /// <summary>
        /// Gets the last write time UTC.
        /// </summary>
        /// <value>
        /// The last write time UTC.
        /// </value>
        public DateTimeOffset LastWriteTimeUtc { get; protected set; }
    }
}
