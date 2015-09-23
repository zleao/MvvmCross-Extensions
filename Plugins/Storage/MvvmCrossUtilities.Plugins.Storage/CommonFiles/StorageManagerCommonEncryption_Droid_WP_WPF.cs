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
    public abstract class StorageManagerCommonEncryption_Droid_WP_WPF : StorageManagerCommonEncryption
    {
        public override void SetDebugEnabled(bool value)
        {
            base.SetDebugEnabled(value);

            CryptologyAsync.SetDebugEnabled(value);
        }

        #region Asynchronous Encryption

        public override async Task EncryptFileAsync(string sourceFullPath, string targetFullPath, string password)
        {
            if (sourceFullPath == targetFullPath)
                throw new NotSupportedException(string.Format("Source and Target paths cannot be equal: {0} | {1}", sourceFullPath, targetFullPath));

            if (await FileExistsAsync(sourceFullPath))
                await CryptologyAsync.EncryptFileAsync(sourceFullPath, targetFullPath, password);
        }

        public override async Task DecryptFileAsync(string sourceFullPath, string targetFullPath, string password)
        {
            if (sourceFullPath == targetFullPath)
                throw new NotSupportedException(string.Format("Source and Target paths cannot be equal: {0} | {1}", sourceFullPath, targetFullPath));

            if (await FileExistsAsync(sourceFullPath))
                await CryptologyAsync.DecryptFileAsync(sourceFullPath, targetFullPath, password);
        }

        public override async Task<Stream> DecryptFileToStreamAsync(StorageLocation location, string sourcePath, string password)
        {
            if (await FileExistsAsync(location, sourcePath))
                return await CryptologyAsync.DecryptFileToStreamAsync(FullPath(location, sourcePath), password);

            return null;
        }

        public override async Task<string> EncryptStringAsync(string stringToEncrypt, string password)
        {
            return Convert.ToBase64String(await CryptologyAsync.EncryptStringToBytesAsync(stringToEncrypt, password));
        }

        public override Task<string> DecryptStringAsync(string stringToDecrypt, string password)
        {
            return CryptologyAsync.DecryptStringFromBytesAsync(Convert.FromBase64String(stringToDecrypt), password);
        }

        public override Task RecoverEncryptedFileAsync(string sourceFullPath, string targetFullPath, string password)
        {
            return CryptologyAsync.RecoverEncryptedFileAsync(sourceFullPath, targetFullPath, password);
        }

        protected override async Task WriteEncryptedFileCommonAsync(StorageMode mode, string fullPath, Action<Stream> streamAction, string password)
        {
            await EnsureFolderExistsAsync(Path.GetDirectoryName(fullPath));

            
            using (var memoryStream = new MemoryStream())
            {
                lock (lockObj)
                {
                    streamAction(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                }
                await CryptologyAsync.EncryptStreamToFileAsync(memoryStream, fullPath, password, mode == StorageMode.Create ? EncryptionModeEnum.CRYPTOFULL : EncryptionModeEnum.CRYPTOLINE);
            }
        }

        #endregion
    }
}