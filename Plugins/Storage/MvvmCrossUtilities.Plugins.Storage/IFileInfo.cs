using System;

namespace MvvmCrossUtilities.Plugins.Storage
{
    /// <summary>
    /// IFileInfo
    /// </summary>
    public interface IFileInfo
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Gets the name of the folder.
        /// </summary>
        /// <value>
        /// The name of the folder.
        /// </value>
        string FolderName { get; }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        string FilePath { get; }

        /// <summary>
        /// Gets the folder path.
        /// </summary>
        /// <value>
        /// The folder path.
        /// </value>
        string FolderPath { get; }

        /// <summary>
        /// Gets the length.
        /// </summary>
        /// <value>
        /// The length.
        /// </value>
        long Length { get; }

        /// <summary>
        /// Gets the location.
        /// </summary>
        /// <value>
        /// The location.
        /// </value>
        StorageLocation Location { get; }

        /// <summary>
        /// Gets the relative path.
        /// </summary>
        /// <value>
        /// The relative path.
        /// </value>
        string RelativePath { get; }

        /// <summary>
        /// Gets the name of the relative path with.
        /// </summary>
        /// <value>
        /// The name of the relative path with.
        /// </value>
        string RelativePathWithName { get; }

        /// <summary>
        /// Gets the creation time.
        /// </summary>
        /// <value>
        /// The creation time.
        /// </value>
        DateTime CreationTime { get; }
        /// <summary>
        /// Gets the creation time UTC.
        /// </summary>
        /// <value>
        /// The creation time UTC.
        /// </value>
        DateTime CreationTimeUtc { get; }

        /// <summary>
        /// Gets the last access time.
        /// </summary>
        /// <value>
        /// The last access time.
        /// </value>
        DateTime LastAccessTime { get; }
        /// <summary>
        /// Gets the last access time UTC.
        /// </summary>
        /// <value>
        /// The last access time UTC.
        /// </value>
        DateTime LastAccessTimeUtc { get; }

        /// <summary>
        /// Gets the last write time.
        /// </summary>
        /// <value>
        /// The last write time.
        /// </value>
        DateTime LastWriteTime { get; }
        /// <summary>
        /// Gets the last write time UTC.
        /// </summary>
        /// <value>
        /// The last write time UTC.
        /// </value>
        DateTime LastWriteTimeUtc { get; }
    }
}
