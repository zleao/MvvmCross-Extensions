using MvvmCross.Platforms.Uap;
using MvxExtensions.Plugins.Storage.Models;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace MvxExtensions.Plugins.Storage.Platforms.UWP
{
    /// <summary>
    /// Android implementation of the Storage plugin
    /// </summary>
    /// <seealso cref="StorageManager" />
    public class StorageManagerUWP : StorageManager
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

        /// <summary>
        /// Returns the available free space in bytes for a given path
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        public override async Task<ulong> GetAvailableFreeSpaceAsync(string fullPath)
        {
            var folder = await StorageFolder.GetFolderFromPathAsync(fullPath).AsTask().ConfigureAwait(false);
            var retrivedProperties = await folder.Properties.RetrievePropertiesAsync(new string[] { "System.FreeSpace" }).AsTask().ConfigureAwait(false);
            return ConvertBytesToMegabytes((ulong)retrivedProperties["System.FreeSpace"]);
        }

        private const long MEGA_BYTE = 1048576;
        /// <summary>
        /// Converts the bytes to megabytes.
        /// </summary>
        /// <param name="bytesToConvert">The bytes to convert.</param>
        /// <returns></returns>
        public static ulong ConvertBytesToMegabytes(ulong bytesToConvert)
        {
            return (ulong)(bytesToConvert / MEGA_BYTE);
        }

        /// <summary>
        /// Clones a file coming from the assets folder
        /// </summary>
        /// <param name="fromPath"></param>
        /// <param name="toLocation"></param>
        /// <param name="toPath"></param>
        /// <returns></returns>
        public override async Task CloneFileFromAppResourcesAsync(string fromPath, StorageLocation toLocation, string toPath)
        {
            var fromFolderPath = Path.Combine(Windows.ApplicationModel.Package.Current.InstalledLocation.Path, Path.GetDirectoryName(fromPath));
            var fromStorageFolder = await StorageFolder.GetFolderFromPathAsync(fromFolderPath).AsTask().ConfigureAwait(false);
            StorageFile file = await fromStorageFolder.GetFileAsync(Path.GetFileName(fromPath));
            if (File.Exists(file.Path))
            {
                var contents = File.ReadAllBytes(file.Path);
                await WriteFileAsync(toLocation, StorageMode.Create, toPath, contents).ConfigureAwait(false);
            }
        }
    }
}
