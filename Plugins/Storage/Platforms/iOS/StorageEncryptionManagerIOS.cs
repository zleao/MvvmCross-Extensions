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

            switch (location)
            {
                case StorageLocation.Internal:
                    basePath = AppDomain.CurrentDomain.BaseDirectory;
                    break;

                case StorageLocation.ExternalPrivate:
                    basePath = PathCombine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppName);
                    break;

                case StorageLocation.ExternalPublic:
                    basePath = PathCombine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), AppName);
                    break;

                default:
                    break;
            }

            return PathCombine(basePath, path);
        }
    }
}
