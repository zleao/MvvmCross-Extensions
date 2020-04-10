using Android.Content;
using Android.Content.Res;
using Android.OS;
using MvvmCross;
using MvvmCross.Platforms.Android;
using MvxExtensions.Plugins.Storage.Models;

namespace MvxExtensions.Plugins.Storage.Platforms.Droid
{
    /// <summary>
    /// Android implementation of the Storage Encryption plugin
    /// </summary>
    public class StorageEncryptionManagerDroid : StorageEncryptionManager
    {
        private const string BASE_PRIVATE_ANDROID_DATA_PATH = "Android/data";
        private const string BASE_PUBLIC_ANDROID_DATA_PATH = "Data";

        private Context Context
        {
            get
            {
                return _context ?? (_context = Mvx.IoCProvider.Resolve<IMvxAndroidGlobals>().ApplicationContext);
            }
        }
        private Context _context;

        private AssetManager Assets => _assets ?? (_assets = Mvx.IoCProvider.Resolve<IMvxAndroidGlobals>().ApplicationContext.Assets);
        private AssetManager _assets;

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
                    basePath = Context.CacheDir.Path;
                    break;

                case StorageLocation.AppDataDirectory:
                    basePath = Context.FilesDir.Path;
                    break;

                case StorageLocation.SharedDataDirectory:
                    var cachePath = PathCombine(BASE_PUBLIC_ANDROID_DATA_PATH, Context.PackageName, "files");
                    basePath = Context.GetExternalFilesDir(cachePath).Path;
                    break;
            }

            return PathCombine(basePath, path);
        }
    }
}
