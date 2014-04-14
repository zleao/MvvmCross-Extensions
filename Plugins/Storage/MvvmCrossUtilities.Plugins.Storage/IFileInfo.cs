using System;

namespace MvvmCrossUtilities.Plugins.Storage
{
    public interface IFileInfo
    {
        string Name { get; }

        string FolderName { get; }

        string FilePath { get; }

        string FolderPath { get; }

        long Length { get; }

        StorageLocation Location { get; }

        string RelativePath { get; }

        string RelativePathWithName { get; }

        DateTime CreationTime { get; }
        DateTime CreationTimeUtc { get; }

        DateTime LastAccessTime { get; }
        DateTime LastAccessTimeUtc { get; }

        DateTime LastWriteTime { get; }
        DateTime LastWriteTimeUtc { get; }
    }
}
