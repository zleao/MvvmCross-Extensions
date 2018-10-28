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
                case StorageLocation.Internal:
                    basePath = NSFileManager.DefaultManager.GetUrl(NSSearchPathDirectory.LibraryDirectory, NSSearchPathDomain.All, null, true, out nsError).Path;
                    break;

                case StorageLocation.ExternalPrivate:
                    basePath = NSFileManager.DefaultManager.GetUrl(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.All, null, true, out nsError).Path;
                    break;

                case StorageLocation.ExternalPublic:
                    basePath = NSFileManager.DefaultManager.GetUrl(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.All, null, true, out nsError).Path;
                    break;

                default:
                    break;
            }

            return PathCombine(basePath, path);
        }
    }
}
