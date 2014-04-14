using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using Cirrious.CrossCore.Platform;

namespace MvvmCrossUtilities.Plugins.Storage
{
    public abstract class BaseStorageManager : IStorageManager
    {
        internal enum EncryptionMode
        {
            UNKNOWN,
            /// <summary>
            /// Full file encryption using Crypto
            /// </summary>
            CRYPTOFULL,
            /// <summary>
            /// Line encryption using Crypto
            /// </summary>
            CRYPTOLINE,
        }

        #region Constructor

        ~BaseStorageManager()
        {
            Cryptology.DisposeAlgorithmAndPassword();
        }

        #endregion

        #region Abstract Methods

        protected abstract string FullPath(StorageLocation location, string path);

        #endregion

        #region IStorageManager Members

        public char DirectorySeparatorChar
        {
            get { return Path.DirectorySeparatorChar; }
        }

        public bool TryReadTextFile(StorageLocation location, string path, out string contents)
        {
            string result = null;
            var toReturn = TryReadFileCommon(location, path, (stream) =>
            {
                using (var streamReader = new StreamReader(stream))
                {
                    result = streamReader.ReadToEnd();
                }
                return true;
            });
            contents = result;
            return toReturn;
        }

        public bool TryReadBinaryFile(StorageLocation location, string path, out Byte[] contents)
        {
            Byte[] result = null;
            var toReturn = TryReadFileCommon(location, path, (stream) =>
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
            contents = result;
            return toReturn;
        }

        public bool TryReadBinaryFile(StorageLocation location, string path, Func<Stream, bool> readMethod)
        {
            return TryReadFileCommon(location, path, readMethod);
        }

        public void WriteFile(StorageLocation location, StorageMode mode, string path, Stream sourceStream)
        {
            if (sourceStream != null)
            {
                WriteFileCommon(location, mode, path, (stream) =>
                {
                    if (stream != null)
                    {
                        if (stream.CanSeek)
                            sourceStream.Seek(0, SeekOrigin.Begin);
                        sourceStream.CopyTo(stream);
                    }
                });
            }
        }

        public void WriteFile(StorageLocation location, StorageMode mode, string path, Action<Stream> writeMethod)
        {
            WriteFileCommon(location, mode, path, writeMethod);
        }

        public void WriteFile(StorageLocation location, StorageMode mode, string path, string contents)
        {
            WriteFileCommon(location, mode, path, (stream) =>
            {
                using (var sw = new StreamWriter(stream))
                {
                    sw.Write(contents);
                    sw.Flush();
                }
            });
        }

        public void WriteFile(StorageLocation location, StorageMode mode, string path, IEnumerable<Byte> contents)
        {
            WriteFileCommon(location, mode, path, (stream) =>
            {
                using (var binaryWriter = new BinaryWriter(stream))
                {
                    binaryWriter.Write(contents.ToArray());
                    binaryWriter.Flush();
                }
            });
        }

        public void WriteFile(StorageMode mode, string fullPath, Stream sourceStream)
        {
            if (sourceStream != null)
            {
                WriteFileCommon(mode, fullPath, (stream) =>
                {
                    if (stream != null)
                    {
                        sourceStream.Seek(0, SeekOrigin.Begin);
                        sourceStream.CopyTo(stream);
                    }
                });
            }
        }

        public void WriteFile(StorageMode mode, string fullPath, Action<Stream> writeMethod)
        {
            WriteFileCommon(mode, fullPath, writeMethod);
        }


        public void WriteEncryptedFile(StorageLocation location, StorageMode mode, string path, string contents, string password)
        {
            WriteEncryptedFileCommon(location, mode, path, (stream) =>
            {
                var sw = new StreamWriter(stream);
                sw.Write(contents);
                sw.Flush();
            },
            password);
        }

        public void WriteEncryptedFile(StorageLocation location, StorageMode mode, string path, Byte[] contents, string password)
        {
            WriteEncryptedFileCommon(location, mode, path, (stream) =>
            {
                stream.Write(contents, 0, contents.Length);
            },
            password);
        }

        public void EncryptFile(StorageLocation location, string sourcePath, string targetPath, string password)
        {
            if (sourcePath == targetPath)
                throw new NotSupportedException(string.Format("Source and Target paths cannot be equal: {0} | {1}", sourcePath, targetPath));

            if (FileExists(location, sourcePath))
                Cryptology.EncryptFile(FullPath(location, sourcePath), FullPath(location, targetPath), password);
        }

        public void DecryptFile(StorageLocation location, string sourcePath, string targetPath, string password)
        {
            if (sourcePath == targetPath)
                throw new NotSupportedException(string.Format("Source and Target paths cannot be equal: {0} | {1}", sourcePath, targetPath));

            if (FileExists(location, sourcePath))
                Cryptology.DecryptFile(FullPath(location, sourcePath), FullPath(location, targetPath), password);
        }

        public Stream DecryptFileToStream(StorageLocation location, string sourcePath, string password)
        {
            if (FileExists(location, sourcePath))
                return Cryptology.DecryptFileToStream(FullPath(location, sourcePath), password);

            return null;
        }


        public bool FileExists(StorageLocation location, string path)
        {
            var fullPath = FullPath(location, path);
            return System.IO.File.Exists(fullPath);
        }

        public bool FolderExists(StorageLocation location, string path)
        {
            var fullPath = FullPath(location, path);
            return Directory.Exists(fullPath);
        }

        public string NativePath(StorageLocation location, string path)
        {
            return FullPath(location, path);
        }

        public void DeleteFile(StorageLocation location, string path)
        {
            var fullPath = FullPath(location, path);
            System.IO.File.Delete(fullPath);
        }

        public void DeleteFile(string fullPath)
        {
            System.IO.File.Delete(fullPath);
        }

        public void DeleteFolder(StorageLocation location, string folderPath, bool recursive)
        {
            var fullPath = FullPath(location, folderPath);
            Directory.Delete(fullPath, recursive);
        }

        public string PathCombine(params string[] paths)
        {
            if (paths == null || paths.Count() <= 0)
                return string.Empty;

            var count = paths.Count();
            var trimmedPaths = new string[count];


            for (int i = 0; i < count; i++)
            {
                paths[i] = paths[i].Replace("\\", "/");
                if (i == 0)
                    trimmedPaths[i] = paths[i];
                else
                    trimmedPaths[i] = paths[i].TrimStart('\\', '/');
            }

            return Path.Combine(trimmedPaths);
        }

        public IEnumerable<IFileInfo> GetFilesIn(StorageLocation location, string folderPath)
        {
            return GetFilesIn(location, folderPath, null);
        }

        public IEnumerable<IFileInfo> GetFilesIn(StorageLocation location, string folderPath, string searchPattern)
        {
            var fullPath = FullPath(location, folderPath);
            var di = new DirectoryInfo(fullPath);

            if (string.IsNullOrEmpty(searchPattern))
                return di.GetFiles().Select(f => new MvvmCrossUtilities.Plugins.Storage.Droid.FileInfo(f, location, folderPath));

            return di.GetFiles(searchPattern).Select(f => new MvvmCrossUtilities.Plugins.Storage.Droid.FileInfo(f, location, folderPath));
        }

        public void EnsureFolderExists(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
        }

        public DateTime GetFileCreationTime(StorageLocation location, string path)
        {
            return File.GetCreationTime(FullPath(location, path));
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

        public Stream CompressStream(Stream streamToCompress)
        {
            try
            {
                if (streamToCompress != null && streamToCompress.CanSeek)
                    streamToCompress.Seek(0, SeekOrigin.Begin);

                var compressedStream = new MemoryStream();

                using (var compressor = new DeflateStream(compressedStream, CompressionMode.Compress, true))
                {
                    streamToCompress.CopyTo(compressor);
                    compressor.Close();
                    compressedStream.Seek(0, SeekOrigin.Begin);
                    return compressedStream;
                }
            }
            catch (Exception ex)
            {
                Android.Util.Log.Wtf(this.GetType().FullName, ex.Message);
                return null;
            }
        }

        public Stream DecompressStream(Stream streamToDecompress)
        {
            try
            {
                if (streamToDecompress != null && streamToDecompress.CanSeek)
                    streamToDecompress.Seek(0, SeekOrigin.Begin);

                var decompressedStream = new MemoryStream();

                using (DeflateStream decompressor = new DeflateStream(streamToDecompress, CompressionMode.Decompress))
                {
                    decompressor.CopyTo(decompressedStream);
                    decompressor.Close();
                    decompressedStream.Seek(0, SeekOrigin.Begin);
                    return decompressedStream;
                }
            }
            catch (Exception ex)
            {
                Android.Util.Log.Wtf(this.GetType().FullName, ex.Message);
                return null;
            }
        }

        public bool TryMove(StorageLocation fromLocation, string fromPath, StorageLocation toLocation, string toPath, bool deleteExistingTo)
        {
            var fromFullPath = FullPath(fromLocation, fromPath);
            var toFullPath = FullPath(toLocation, toPath);

            return TryMove(fromFullPath, toFullPath, deleteExistingTo);
        }

        public bool TryMove(string fromFullPath, string toFullPath, bool deleteExistingTo)
        {
            try
            {
                if (!System.IO.File.Exists(fromFullPath))
                    return false;

                EnsureFolderExists(toFullPath.Replace(GetFileName(toFullPath), string.Empty));

                if (System.IO.File.Exists(toFullPath))
                {
                    if (deleteExistingTo)
                        System.IO.File.Delete(toFullPath);
                    else
                        return false;
                }

                System.IO.File.Move(fromFullPath, toFullPath);
                return true;
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception exception)
            {
                MvxTrace.Error("Error during file move {0} : {1} : {2}", fromFullPath, toFullPath, exception.Message);
                return false;
            }
        }

        public void CloneFile(StorageLocation fromLocation, string fromPath, StorageLocation toLocation, string toPath, bool overwriteExistingTo)
        {
            var fromFullPath = FullPath(fromLocation, fromPath);
            var toFullPath = FullPath(toLocation, toPath);

            CloneFile(fromFullPath, toFullPath, overwriteExistingTo);
        }

        public void CloneFile(string fromFullPath, string toFullPath, bool overwriteExistingTo)
        {
            try
            {
                if (System.IO.File.Exists(fromFullPath))
                {
                    EnsureFolderExists(toFullPath.Replace(GetFileName(toFullPath), string.Empty));

                    System.IO.File.Copy(fromFullPath, toFullPath, overwriteExistingTo);
                }
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception exception)
            {
                MvxTrace.Error("Error during file clone {0} : {1} : {2}", fromFullPath, toFullPath, exception.Message);
                throw;
            }
        }

        #endregion

        #region Private Methods

        private bool TryReadFileCommon(StorageLocation location, string path, Func<Stream, bool> streamAction)
        {
            var fullPath = FullPath(location, path);
            if (!System.IO.File.Exists(fullPath))
            {
                return false;
            }

            using (var fileStream = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                return streamAction(fileStream);
            }
        }

        private object lockObj = new object();
        private void WriteFileCommon(StorageLocation location, StorageMode mode, string path, Action<Stream> streamAction)
        {
            var fullPath = FullPath(location, path);

            WriteFileCommon(mode, fullPath, streamAction);
        }
        private void WriteFileCommon(StorageMode mode, string fullPath, Action<Stream> streamAction)
        {
            EnsureFolderExists(Path.GetDirectoryName(fullPath));

            lock (lockObj)
            {
                using (var fileStream = File.Open(fullPath, GetFileMode(mode), FileAccess.Write, FileShare.Read))
                {
                    streamAction(fileStream);
                }
            }
        }

        private void WriteEncryptedFileCommon(StorageLocation location, StorageMode mode, string path, Action<Stream> streamAction, string password)
        {
            var fullPath = FullPath(location, path);

            WriteEncryptedFileCommon(mode, fullPath, streamAction, password);
        }
        private void WriteEncryptedFileCommon(StorageMode mode, string fullPath, Action<Stream> streamAction, string password)
        {
            EnsureFolderExists(Path.GetDirectoryName(fullPath));

            lock (lockObj)
            {
                using (var memoryStream = new MemoryStream())
                {
                    streamAction(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    Cryptology.EncryptStreamToFile(memoryStream, fullPath, password, mode == StorageMode.Create ? EncryptionMode.CRYPTOFULL : EncryptionMode.CRYPTOLINE);
                }
            }
        }

        private FileMode GetFileMode(StorageMode mode)
        {
            if (mode == StorageMode.Create)
                return FileMode.Create;

            return FileMode.Append;
        }

        private void ClearStream(Stream stream)
        {
            if (stream != null)
            {
                stream.SetLength(0);
                if (stream.CanSeek)
                    stream.Seek(0, SeekOrigin.Begin);
            }
        }

        #endregion
    }
}
