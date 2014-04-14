using Android.Content;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Droid;

namespace MvvmCrossUtilities.Plugins.Storage.Droid
{
    public class StorageManager : BaseStorageManager
    {
        private const string BASE_PRIVATE_ANDROID_DATA_PATH = "Android/data";
        private const string BASE_PUBLIC_ANDROID_DATA_PATH = "Data";

        private Context Context
        {
            get
            {
                return _context ?? (_context = Mvx.Resolve<IMvxAndroidGlobals>().ApplicationContext);
            }
        }
        private Context _context;

        protected override string FullPath(StorageLocation location, string path)
        {
            var basePath = string.Empty;

            switch (location)
            {
                case StorageLocation.Internal:
                    basePath = Context.FilesDir.Path;
                    break;

                case StorageLocation.ExternalPrivate:
                    var cachePath = PathCombine(BASE_PRIVATE_ANDROID_DATA_PATH, Context.PackageName, "files");
                    basePath = Context.GetExternalFilesDir(cachePath).Path;
                    break;

                case StorageLocation.ExternalPublic:
                    var filesPath = PathCombine(BASE_PUBLIC_ANDROID_DATA_PATH, Context.PackageName, "files");
                    basePath = Android.OS.Environment.GetExternalStoragePublicDirectory(filesPath).Path;
                    break;

                default:
                    break;
            }

            return PathCombine(basePath, path);
        }
    }
}