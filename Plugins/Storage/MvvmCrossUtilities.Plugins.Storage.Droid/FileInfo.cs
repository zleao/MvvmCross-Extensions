using System;
using System.IO;

namespace MvvmCrossUtilities.Plugins.Storage
{
    public class FileInfo : IFileInfo
    {
        #region Properties

        public string Name { get; private set; }

        public string FolderName { get; private set; }

        public string FilePath { get; private set; }

        public string FolderPath { get; private set; }

        public long Length { get; private set; }

        public StorageLocation Location { get; private set; }

        public string RelativePath { get; private set; }

        public string RelativePathWithName { get; private set; }

        public DateTime CreationTime { get; private set; }
        public DateTime CreationTimeUtc { get; private set; }

        public DateTime LastAccessTime { get; private set; }
        public DateTime LastAccessTimeUtc { get; private set; }

        public DateTime LastWriteTime { get; private set; }
        public DateTime LastWriteTimeUtc { get; private set; }
        
        #endregion

        #region Constructor
        
        public FileInfo(System.IO.FileInfo fileInfo)
        {
            if (fileInfo != null)
            {
                Name = fileInfo.Name;
                FolderName = fileInfo.DirectoryName;
                FilePath = fileInfo.FullName;
                FolderPath = fileInfo.Directory.FullName;
                Length = fileInfo.Length;
                CreationTime = fileInfo.CreationTime;
                CreationTimeUtc = fileInfo.CreationTimeUtc;
                LastAccessTime = fileInfo.LastAccessTime;
                LastAccessTimeUtc = fileInfo.LastAccessTimeUtc;
                LastWriteTime = fileInfo.LastWriteTime;
                LastWriteTimeUtc = fileInfo.LastWriteTimeUtc;
            }
        }

        public FileInfo(System.IO.FileInfo fileInfo, StorageLocation location, string relativePath)
            : this(fileInfo)
        {
            Location = location;
            RelativePath = relativePath;
            RelativePathWithName = Path.Combine(relativePath, Name);
        }

        #endregion
    }
}
