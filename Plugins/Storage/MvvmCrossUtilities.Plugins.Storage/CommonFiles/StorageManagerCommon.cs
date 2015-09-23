using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

#if WINDOWS_PHONE
namespace MvvmCrossUtilities.Plugins.Storage.WindowsPhone
#elif MONODROID
namespace MvvmCrossUtilities.Plugins.Storage.Droid
#elif UNIVERSAL_APPS
namespace MvvmCrossUtilities.Plugins.Storage.WindowsCommon
#else
namespace MvvmCrossUtilities.Plugins.Storage.Wpf
#endif
{
    public abstract class StorageManagerCommon : IStorageManager
    {
        protected object lockObj = new object();

        #region Common (Sync & Async)

        public bool IsDebugEnabled
        {
            get { return _isDebugEnabled; }
        }
        private bool _isDebugEnabled = false;

        public virtual void SetDebugEnabled(bool value)
        {
            _isDebugEnabled = value;
        }

        public abstract char DirectorySeparatorChar { get; }

        public string NativePath(StorageLocation location, string path)
        {
            return FullPath(location, path);
        }

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

        public string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);

        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public string GetFileExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }


        protected abstract string FullPath(StorageLocation location, string path);

        protected void ClearStream(Stream stream)
        {
            if (stream != null)
            {
                stream.SetLength(0);
                if (stream.CanSeek)
                    stream.Seek(0, SeekOrigin.Begin);
            }
        }

#if UNIVERSAL_APPS
        internal static Windows.Storage.CreationCollisionOption GetFileMode(StorageMode mode)
        {
            if (mode == StorageMode.Create)
                return Windows.Storage.CreationCollisionOption.ReplaceExisting;

            return Windows.Storage.CreationCollisionOption.OpenIfExists;
        }
#else
        protected FileMode GetFileMode(StorageMode mode)
        {
            if (mode == StorageMode.Create)
                return FileMode.Create;

            return FileMode.Append;
        }
#endif
        #endregion

        #region Asynchronous

        public Task<string> TryReadTextFileAsync(StorageLocation location, string path)
        {
            return TryReadTextFileAsync(FullPath(location, path));
        }

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

        public Task<bool> TryReadBinaryFileAsync(StorageLocation location, string path, Func<Stream, bool> readMethod)
        {
            return TryReadFileCommonAsync(FullPath(location, path), readMethod);
        }

        protected abstract Task<bool> TryReadFileCommonAsync(string fullPath, Func<Stream, bool> streamAction);


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

        public Task WriteFileAsync(StorageLocation location, StorageMode mode, string path, Action<Stream> writeMethod)
        {
            return WriteFileCommonAsync(location, mode, path, writeMethod);
        }

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

        public Task WriteFileAsync(StorageMode mode, string fullPath, Stream sourceStream)
        {
            if (sourceStream != null)
            {
                return WriteFileCommonAsync(mode, fullPath, (stream) =>
                        {
                            if (stream != null)
                            {
                                sourceStream.Seek(0, SeekOrigin.Begin);
                                sourceStream.CopyTo(stream);
                            }
                        });
            }

            return null;
        }

        public Task WriteFileAsync(StorageMode mode, string fullPath, Action<Stream> writeMethod)
        {
            return WriteFileCommonAsync(mode, fullPath, writeMethod);
        }

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

        protected Task WriteFileCommonAsync(StorageLocation location, StorageMode mode, string path, Action<Stream> streamAction)
        {
            var fullPath = FullPath(location, path);

            return WriteFileCommonAsync(mode, fullPath, streamAction);
        }

        protected abstract Task WriteFileCommonAsync(StorageMode mode, string fullPath, Action<Stream> streamAction);


        public async Task<bool> FileExistsAsync(StorageLocation location, string path)
        {
            var fullPath = FullPath(location, path);
            return await FileExistsAsync(fullPath);
        }

        public abstract Task<bool> FileExistsAsync(string fullPath);

        public Task<bool> FolderExistsAsync(StorageLocation location, string path)
        {
            var fullPath = FullPath(location, path);
            return FolderExistsAsync(fullPath);
        }

        public abstract Task<bool> FolderExistsAsync(string fullPath);


        public Task DeleteFileAsync(StorageLocation location, string path)
        {
            var fullPath = FullPath(location, path);
            return DeleteFileAsync(fullPath);
        }

        public abstract Task DeleteFileAsync(string fullPath);

        public Task DeleteFolderAsync(StorageLocation location, string folderPath)
        {
            var fullPath = FullPath(location, folderPath);
            return DeleteFolderAsync(fullPath);
        }

        public abstract Task DeleteFolderAsync(string folderFullPath);


        public abstract Task<IEnumerable<IFileInfo>> GetFilesInAsync(StorageLocation location, bool recursive, string folderPath = "", string searchPattern = "", SearchMode searchMode = SearchMode.Contains);

        public abstract Task<IEnumerable<IFileInfo>> GetFilesInAsync(string folderFullPath, bool recursive, string searchPattern = "", SearchMode searchMode = SearchMode.Contains);


        public Task EnsureFolderExistsAsync(StorageLocation location, string folderPath)
        {
            var fullPath = FullPath(location, folderPath);
            return EnsureFolderExistsAsync(fullPath);
        }

        public abstract Task EnsureFolderExistsAsync(string folderfullPath);

        public abstract Task<DateTime> GetFileCreationTimeAsync(StorageLocation location, string path);


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


        public Task<bool> TryMoveAsync(StorageLocation fromLocation, string fromPath, StorageLocation toLocation, string toPath, bool deleteExistingTo)
        {
            var fromFullPath = FullPath(fromLocation, fromPath);
            var toFullPath = FullPath(toLocation, toPath);

            return TryMoveAsync(fromFullPath, toFullPath, deleteExistingTo);
        }

        public abstract Task<bool> TryMoveAsync(string fromFullPath, string toFullPath, bool deleteExistingTo);


        public Task CloneFileAsync(StorageLocation fromLocation, string fromPath, StorageLocation toLocation, string toPath, bool overwriteExistingTo)
        {
            var fromFullPath = FullPath(fromLocation, fromPath);
            var toFullPath = FullPath(toLocation, toPath);

            return CloneFileAsync(fromFullPath, toFullPath, overwriteExistingTo);
        }

        public abstract Task CloneFileAsync(string fromFullPath, string toFullPath, bool overwriteExistingTo);


        public Task CloneFolderAsync(StorageLocation fromLocation, string fromPath, StorageLocation toLocation, string toPath, bool overwriteExistingTo, bool recursive)
        {
            var fromFullPath = FullPath(fromLocation, fromPath);
            var toFullPath = FullPath(toLocation, toPath);

            return CloneFolderAsync(fromFullPath, toFullPath, overwriteExistingTo, recursive);
        }

        public abstract Task CloneFolderAsync(string fromFullPath, string toFullPath, bool overwriteExistingTo, bool recursive);

        #endregion

        #region Asynchronous Encryption Abstract

        public abstract Task<string> TryReadTextEncryptedFileAsync(StorageLocation location, string path, string password);

        public abstract Task<string> TryReadTextEncryptedFileAsync(string fullPath, string password);

        public abstract Task WriteEncryptedFileAsync(StorageLocation location, StorageMode mode, string path, string contents, string password);

        public abstract Task WriteEncryptedFileAsync(StorageLocation location, StorageMode mode, string path, byte[] contents, string password);

        public abstract Task WriteEncryptedFileAsync(StorageMode mode, string fullPath, string contents, string password);

        public abstract Task EncryptFileAsync(StorageLocation location, string sourcePath, string targetPath, string password);

        public abstract Task EncryptFileAsync(string sourceFullPath, string targetFullPath, string password);

        public abstract Task<string> EncryptFileAsync(StorageLocation location, string sourcePath, string password);

        public abstract Task<string> EncryptFileAsync(string sourceFullPath, string password);

        public abstract Task DecryptFileAsync(StorageLocation location, string sourcePath, string targetPath, string password);

        public abstract Task DecryptFileAsync(string sourceFullPath, string targetFullPath, string password);

        public abstract Task<string> DecryptFileAsync(StorageLocation location, string sourcePath, string password);

        public abstract Task<string> DecryptFileAsync(string sourceFullPath, string password);

        public abstract Task<Stream> DecryptFileToStreamAsync(StorageLocation location, string sourcePath, string password);

        public abstract Task<string> EncryptStringAsync(string stringToEncrypt, string password);

        public abstract Task<string> DecryptStringAsync(string stringToDecrypt, string password);

        public abstract Task<string> RecoverEncryptedFileAsync(StorageLocation location, string sourcePath, string password);

        public abstract Task RecoverEncryptedFileAsync(StorageLocation location, string sourcePath, string targetPath, string password);

        public abstract Task<string> RecoverEncryptedFileAsync(string sourceFullPath, string password);

        public abstract Task RecoverEncryptedFileAsync(string sourceFullPath, string targetFullPath, string password);

        protected abstract Task<bool> TryReadEncryptedFileCommonAsync(string fullPath, string password, Func<Stream, bool> streamAction);

        protected abstract Task WriteEncryptedFileCommonAsync(StorageLocation location, StorageMode mode, string path, Action<Stream> streamAction, string password);

        protected abstract Task WriteEncryptedFileCommonAsync(StorageMode mode, string fullPath, Action<Stream> streamAction, string password);

        #endregion
    }
}