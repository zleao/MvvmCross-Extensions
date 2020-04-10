using System;
using System.IO;

namespace MvxExtensions.Plugins.Storage.Models
{
    /// <summary>
    /// Information of a file
    /// </summary>
    /// <seealso cref="BaseFileInfo" />
    public class FileInfo : BaseFileInfo
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FileInfo"/> class.
        /// </summary>
        /// <param name="fileInfo">The file information.</param>
        public FileInfo(System.IO.FileInfo fileInfo)
        {
            if (fileInfo != null)
            {
                FileName = fileInfo.Name;
                FolderName = fileInfo.DirectoryName;
                FileFullPath = fileInfo.FullName;
                FolderFullPath = fileInfo.Directory.FullName;
                Length = fileInfo.Length;
                CreationTime = fileInfo.CreationTime;
                CreationTimeUtc = fileInfo.CreationTime.ToUniversalTime();
                LastAccessTime = fileInfo.LastAccessTime;
                LastAccessTimeUtc = fileInfo.LastAccessTime.ToUniversalTime();
                LastWriteTime = fileInfo.LastWriteTime;
                LastWriteTimeUtc = fileInfo.LastWriteTime.ToUniversalTime();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileInfo"/> class.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="fileFullPath">The file full path.</param>
        /// <param name="fileLength">Length of the file.</param>
        /// <param name="creationTime">The creation time.</param>
        /// <param name="lastAccessTime">The last access time.</param>
        /// <param name="lastWriteTime">The last write time.</param>
        public FileInfo(string fileName, 
                        string fileFullPath, 
                        long fileLength,
                        DateTimeOffset creationTime,
                        DateTimeOffset lastAccessTime,
                        DateTimeOffset lastWriteTime)
        {
            FileName = fileName;
            FolderName = Path.GetDirectoryName(fileFullPath);
            FileFullPath = fileFullPath;
            FolderFullPath = fileFullPath.Substring(0, fileFullPath.LastIndexOf(Path.DirectorySeparatorChar));
            Length = fileLength;
            CreationTime = creationTime;
            CreationTimeUtc = creationTime.ToUniversalTime();
            LastAccessTime = LastAccessTime;
            LastAccessTimeUtc = LastAccessTime.ToUniversalTime();
            LastWriteTime = LastWriteTime;
            LastWriteTimeUtc = LastWriteTime.ToUniversalTime();
        }

        #endregion
    }
}
