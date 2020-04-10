using Foundation;
using MvxExtensions.Plugins.Storage.Models;
using System;
using System.Reflection;

namespace MvxExtensions.Plugins.Storage.Platforms.iOS
{
    public class StorageEncryptionManagerIOS : StorageEncryptionManager
    {
        private string AppName
        {
            get { return Assembly.GetEntryAssembly().GetName().Name; }
        }

        /// <summary>
        /// Returns the full physical path based on a location and a relative path
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The relative path.</param>
        /// <returns></returns>
        protected override string FullPath(StorageLocation location, string path)
        {
            var basePath = string.Empty;

            NSError nsError;

            switch (location)
            {
                case StorageLocation.AppCacheDirectory:
                    basePath = GetDirectory(NSSearchPathDirectory.CachesDirectory);
                    break;

                case StorageLocation.AppDataDirectory:
                    basePath = GetDirectory(NSSearchPathDirectory.LibraryDirectory);
                    break;

                case StorageLocation.SharedDataDirectory:
                    basePath = NSFileManager.DefaultManager.GetUrl(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.All, null, true, out nsError).Path;
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
