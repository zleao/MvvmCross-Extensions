using MvxExtensions.Plugins.Storage.Platforms.Common;
using System;
using System.Reflection;

namespace MvxExtensions.Plugins.Storage.Wpf
{
    /// <summary>
    /// WPF implementation of the Storage plugin
    /// </summary>
    /// <seealso cref="Platforms.Common.StorageManagerCommon_Droid_WPF" />
    public class StorageManager : StorageManagerCommon_Droid_WPF
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
