using Foundation;
using MvxExtensions.Plugins.Storage.Models;

namespace MvxExtensions.Plugins.Storage.Platforms.iOS
{
    /// <summary>
    /// StorageEncryptionManagerIOS
    /// </summary>
    public class StorageEncryptionManagerIOS : StorageEncryptionManager
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
                    basePath = GetDirectory(NSSearchPathDirectory.CachesDirectory);
                    break;

                case StorageLocation.AppDataDirectory:
                    basePath = GetDirectory(NSSearchPathDirectory.LibraryDirectory);
                    break;

                case StorageLocation.SharedDataDirectory:
                    basePath = NSFileManager.DefaultManager.GetUrl(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.All, null, true, out _).Path;
                    break;
            }

            return PathCombine(basePath, path);
        }

        private string GetDirectory(NSSearchPathDirectory directory)
        {
            var dirs = NSSearchPath.GetDirectories(directory, NSSearchPathDomain.User);
            if (dirs == null || dirs.Length == 0)
            {
                // this should never happen...
                return null;
            }
            return dirs[0];
        }
    }
}
