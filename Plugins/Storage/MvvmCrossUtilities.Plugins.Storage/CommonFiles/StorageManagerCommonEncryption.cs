using System;
using System.IO;
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
    public abstract class StorageManagerCommonEncryption : StorageManagerCommon
    {
        #region Asynchronous Encryption

        public override Task<string> TryReadTextEncryptedFileAsync(StorageLocation location, string path, string password)
        {
            var fullPath = FullPath(location, path);
            return TryReadTextEncryptedFileAsync(fullPath, password);
        }

        public override async Task<string> TryReadTextEncryptedFileAsync(string fullPath, string password)
        {
            string result = null;
            var toReturn = await TryReadEncryptedFileCommonAsync(fullPath, password, (stream) =>
            {
                using (var streamReader = new StreamReader(stream))
                {
                    result = streamReader.ReadToEnd();
                }
                return true;
            });

            return result;
        }

        public override Task WriteEncryptedFileAsync(StorageLocation location, StorageMode mode, string path, string contents, string password)
        {
            var fullPath = FullPath(location, path);
            return WriteEncryptedFileAsync(mode, fullPath, contents, password);
        }

        public override Task WriteEncryptedFileAsync(StorageLocation location, StorageMode mode, string path, byte[] contents, string password)
        {
            return WriteEncryptedFileCommonAsync(location, mode, path, (stream) =>
            {
                stream.Write(contents, 0, contents.Length);
            },
            password);
        }

        public override Task WriteEncryptedFileAsync(StorageMode mode, string fullPath, string contents, string password)
        {
            return WriteEncryptedFileCommonAsync(mode, fullPath, (stream) =>
            {
                var sw = new StreamWriter(stream);
                sw.Write(contents);
                sw.Flush();
            },
           password);
        }

        public override async Task<string> EncryptFileAsync(string sourceFullPath, string password)
        {
            var targetFullPath = sourceFullPath.Replace(Path.GetFileNameWithoutExtension(sourceFullPath), Path.GetFileNameWithoutExtension(sourceFullPath) + "_Encrypted");
            await EncryptFileAsync(sourceFullPath, targetFullPath, password);
            return targetFullPath;
        }

        public override Task<string> EncryptFileAsync(StorageLocation location, string sourcePath, string password)
        {
            return EncryptFileAsync(FullPath(location, sourcePath), password);
        }

        public override Task EncryptFileAsync(StorageLocation location, string sourcePath, string targetPath, string password)
        {
            return EncryptFileAsync(FullPath(location, sourcePath), FullPath(location, targetPath), password);
        }

        public override async Task<string> DecryptFileAsync(string sourceFullPath, string password)
        {
            var targetFullPath = sourceFullPath.Replace(Path.GetFileNameWithoutExtension(sourceFullPath), Path.GetFileNameWithoutExtension(sourceFullPath) + "_Decrypted");
            await DecryptFileAsync(sourceFullPath, targetFullPath, password);
            return targetFullPath;
        }

        public override Task<string> DecryptFileAsync(StorageLocation location, string sourcePath, string password)
        {
            return DecryptFileAsync(FullPath(location, sourcePath), password);
        }

        public override Task DecryptFileAsync(StorageLocation location, string sourcePath, string targetPath, string password)
        {
            return DecryptFileAsync(FullPath(location, sourcePath), FullPath(location, targetPath), password);
        }

        public override Task<string> RecoverEncryptedFileAsync(StorageLocation location, string sourcePath, string password)
        {
            return RecoverEncryptedFileAsync(FullPath(location, sourcePath), password);
        }

        public override Task RecoverEncryptedFileAsync(StorageLocation location, string sourcePath, string targetPath, string password)
        {
            return RecoverEncryptedFileAsync(FullPath(location, sourcePath), FullPath(location, targetPath), password);
        }

        public override async Task<string> RecoverEncryptedFileAsync(string sourceFullPath, string password)
        {
            var targetFullPath = sourceFullPath.Replace(Path.GetFileNameWithoutExtension(sourceFullPath), Path.GetFileNameWithoutExtension(sourceFullPath) + "_Recovered");
            await RecoverEncryptedFileAsync(sourceFullPath, targetFullPath, password);
            return targetFullPath;
        }

        protected override Task WriteEncryptedFileCommonAsync(StorageLocation location, StorageMode mode, string path, Action<Stream> streamAction, string password)
        {
            var fullPath = FullPath(location, path);

            return WriteEncryptedFileCommonAsync(mode, fullPath, streamAction, password);
        }

        #endregion
    }
}