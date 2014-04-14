using System;
using System.Collections.Generic;
using System.IO;

namespace MvvmCrossUtilities.Plugins.Storage
{
    public interface IStorageManager
    {
        /// <summary>
        /// Gets the directory separator character.
        /// </summary>
        /// <value>
        /// The directory separator character.
        /// </value>
        char DirectorySeparatorChar { get; }

        /// <summary>
        /// Tries to read text file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        /// <returns></returns>
        bool TryReadTextFile(StorageLocation location, string path, out string contents);

        /// <summary>
        /// Tries to read binary file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        /// <returns></returns>
        bool TryReadBinaryFile(StorageLocation location, string path, out Byte[] contents);

        /// <summary>
        /// Tries to read binary file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <param name="readMethod">The read method.</param>
        /// <returns></returns>
        bool TryReadBinaryFile(StorageLocation location, string path, Func<Stream, bool> readMethod);

        /// <summary>
        /// Writes a stream to a file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="path">The path.</param>
        /// <param name="writeMethod">The write method.</param>
        void WriteFile(StorageLocation location, StorageMode mode, string path, Stream sourceStream);

        /// <summary>
        /// Writes contents to a file, using an action passed by argument.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="path">The path.</param>
        /// <param name="writeMethod">The write method.</param>
        void WriteFile(StorageLocation location, StorageMode mode, string path, Action<Stream> writeMethod);

        /// <summary>
        /// Writes the binary contents to a file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        void WriteFile(StorageLocation location, StorageMode mode, string path, IEnumerable<Byte> contents);

        /// <summary>
        /// Writes the contents to a file.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        void WriteFile(StorageLocation location, StorageMode mode, string path, string contents);

        /// <summary>
        /// Writes a stream to a file.
        /// The path passed by parameter, is the FULL PATH to where the file will be saved
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="path">The path.</param>
        /// <param name="sourceStream">The source stream.</param>
        void WriteFile(StorageMode mode, string fullPath, Stream sourceStream);

        /// <summary>
        /// Writes contents to a file, using an action passed by argument.
        /// The path passed by parameter, is the FULL PATH to where the file will be saved
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <param name="fullPath">The full path.</param>
        /// <param name="writeMethod">The write method.</param>
        void WriteFile(StorageMode mode, string fullPath, Action<Stream> writeMethod);


        /// <summary>
        /// Writes the contents to a file, using the password to encrypt the data.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        /// <param name="password">The password.</param>
        void WriteEncryptedFile(StorageLocation location, StorageMode mode, string path, string contents, string password);

        /// <summary>
        /// Writes the byte array to a file, using the password to encrypt the data.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        /// <param name="password">The password.</param>
        void WriteEncryptedFile(StorageLocation location, StorageMode mode, string path, Byte[] contents, string password);

        /// <summary>
        /// Encrypts a file, saving it to a new file, specified in targetPath.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetPath">The target path.</param>
        /// <param name="password">The password.</param>
        void EncryptFile(StorageLocation location, string sourcePath, string targetPath, string password);

        /// <summary>
        /// Decrypts a file, saving it to a new file, specified in targetPath.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetPath">The target path.</param>
        /// <param name="password">The password.</param>
        void DecryptFile(StorageLocation location, string sourcePath, string targetPath, string password);

        /// <summary>
        /// Decrypts a file, and returns the correspondent stream.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetPath">The target path.</param>
        /// <param name="password">The password.</param>
        Stream DecryptFileToStream(StorageLocation location, string sourcePath, string password);

        /// <summary>
        /// Checks if a files the exists in thre specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        bool FileExists(StorageLocation location, string path);

        /// <summary>
        /// Checks if a folder exists in the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        bool FolderExists(StorageLocation location, string path);

        /// <summary>
        /// Returns the platform specific full path, related to the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        string NativePath(StorageLocation location, string path);

        /// <summary>
        /// Deletes the file in the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        void DeleteFile(StorageLocation location, string path);

        /// <summary>
        /// Deletes the file.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        void DeleteFile(string fullPath);

        /// <summary>
        /// Deletes the folder in the specified location.
        /// If the folder contains files, set "Recursive' to true, to delete those files
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="recursive">if set to <c>true</c> [recursive].</param>
        void DeleteFolder(StorageLocation location, string folderPath, bool recursive);

        /// <summary>
        /// Combines the paths into a single path.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <returns></returns>
        string PathCombine(params string[] paths);

        /// <summary>
        /// Returns a list of the filenames of a folder in the specified location
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="folderPath">The folder path.</param>
        /// <returns></returns>
        IEnumerable<IFileInfo> GetFilesIn(StorageLocation location, string folderPath);

        /// <summary>
        /// Returns a list of the files in the specified location for a specified search pattern.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="folderPath">The folder path.</param>
        /// <param name="searchPattern">The search pattern.</param>
        /// <returns></returns>
        IEnumerable<IFileInfo> GetFilesIn(StorageLocation location, string folderPath, string searchPattern);

        /// <summary>
        /// Ensures the folder exists, by creating it if not allready present.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        void EnsureFolderExists(string folderPath);

        /// <summary>
        /// Gets the file creation time.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        DateTime GetFileCreationTime(StorageLocation location, string path);

        /// <summary>
        /// Gets the file name without extension.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        string GetFileNameWithoutExtension(string path);

        /// <summary>
        /// Gets the file name with extension.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        string GetFileName(string path);

        /// <summary>
        /// Gets the file extension.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        string GetFileExtension(string path);

        /// <summary>
        /// Gets the name of the directory.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        string GetDirectoryName(string path);

        /// <summary>
        /// Compresses the stream.
        /// </summary>
        /// <param name="streamToCompress">The stream to compress.</param>
        /// <returns></returns>
        Stream CompressStream(Stream streamToCompress);

        /// <summary>
        /// Decompresses the stream.
        /// </summary>
        /// <param name="streamToDecompress">The stream to decompress.</param>
        /// <returns></returns>
        Stream DecompressStream(Stream streamToDecompress);

        /// <summary>
        /// Tries the move a file.
        /// </summary>
        /// <param name="fromLocation">From location.</param>
        /// <param name="fromPath">From path.</param>
        /// <param name="toLocation">To location.</param>
        /// <param name="toPath">To path.</param>
        /// <param name="deleteExistingTo">if set to <c>true</c> [delete existing to].</param>
        /// <returns></returns>
        bool TryMove(StorageLocation fromLocation, string fromPath, StorageLocation toLocation, string toPath, bool deleteExistingTo);

        /// <summary>
        /// Tries the move a file.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <param name="deleteExistingTo">if set to <c>true</c> [delete existing to].</param>
        /// <returns></returns>
        bool TryMove(string fromFullPath, string toFullPath, bool deleteExistingTo);

        /// <summary>
        /// Clones the file.
        /// </summary>
        /// <param name="fromLocation">From location.</param>
        /// <param name="fromPath">From path.</param>
        /// <param name="toLocation">To location.</param>
        /// <param name="toPath">To path.</param>
        /// <param name="overwriteExistingTo">if set to <c>true</c> [overwrite existing to].</param>
        void CloneFile(StorageLocation fromLocation, string fromPath, StorageLocation toLocation, string toPath, bool overwriteExistingTo);

        /// <summary>
        /// Clones the file.
        /// </summary>
        /// <param name="fromFullPath">From full path.</param>
        /// <param name="toFullPath">To full path.</param>
        /// <param name="overwriteExistingTo">if set to <c>true</c> [overwrite existing to].</param>
        void CloneFile(string fromFullPath, string toFullPath, bool overwriteExistingTo);
    }
}
