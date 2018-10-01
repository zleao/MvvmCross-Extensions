using MvxExtensions.Plugins.Storage.Models;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace MvxExtensions.Plugins.Storage.Platforms.iOS
{
    public class StorageManagerIOS : StorageManager
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

        /// <summary>
        /// Clones the file from application resources asynchronous.
        /// </summary>
        /// <param name="fromPath">From path.</param>
        /// <param name="toLocation">To location.</param>
        /// <param name="toPath">To path.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Task CloneFileFromAppResourcesAsync(string fromPath, StorageLocation toLocation, string toPath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the available free space asynchronous.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Task<ulong> GetAvailableFreeSpaceAsync(string fullPath)
        {
            throw new NotImplementedException();
        }
    }
}
