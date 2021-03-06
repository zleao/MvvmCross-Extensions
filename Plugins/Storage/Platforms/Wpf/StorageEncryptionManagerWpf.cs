﻿using MvxExtensions.Plugins.Storage.Models;
using System;
using System.Reflection;

namespace MvxExtensions.Plugins.Storage.Wpf
{
    /// <summary>
    /// WPF implementation of the Storage plugin
    /// </summary>
    /// <seealso cref="StorageManagerCommon" />
    public class StorageEncryptionManagerWpf : StorageEncryptionManager
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
                case StorageLocation.AppCacheDirectory:
                    basePath = PathCombine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppName);
                    break;
                case StorageLocation.AppDataDirectory:
                    basePath = PathCombine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);
                    break;
                case StorageLocation.SharedDataDirectory:
                    basePath = PathCombine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), AppName);
                    break;
            }

            return PathCombine(basePath, path);
        }
    }
}
