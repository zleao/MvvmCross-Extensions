using MvxExtensions.Plugins.Storage.Models;
using Windows.Storage;

namespace MvxExtensions.Plugins.Storage.Platforms.UWP
{
    /// <summary>
    /// Android implementation of the Storage Encryption plugin
    /// </summary>
    public class StorageEncryptionManagerUWP : StorageEncryptionManager
    {
        /// <summary>
        /// Returns the full physical path based on a location and a relative path
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The relative path.</param>
        /// <returns></returns>
        protected override string FullPath(StorageLocation location, string path)
        {
            var basePath = string.Empty;

            switch (location)
            {
                case StorageLocation.AppCacheDirectory:
                    basePath = ApplicationData.Current.LocalCacheFolder.Path;
                    break;

                case StorageLocation.AppDataDirectory:
                    basePath = ApplicationData.Current.LocalFolder.Path;
                    break;

                case StorageLocation.SharedDataDirectory:
                    basePath = ApplicationData.Current.SharedLocalFolder.Path;
                    break;
            }

            return PathCombine(basePath, path);
        }
    }
}
