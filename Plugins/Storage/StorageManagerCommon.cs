using MvxExtensions.Plugins.Storage.Models;
using System.IO;
using System.Threading.Tasks;

namespace MvxExtensions.Plugins.Storage
{
    /// <summary>
    /// Base implementation of the Storage Manager
    /// </summary>
    public abstract class StorageManagerCommon
    {
         /// <summary>
        /// Object to be used in lock statements
        /// </summary>
        protected object LockObj = new object();

        /// <summary>
        /// Returns the platform specific full path, related to the specified location.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public string NativePath(StorageLocation location, string path)
        {
            return FullPath(location, path);
        }

        /// <summary>
        /// Combines the paths into a single path.
        /// </summary>
        /// <param name="paths">The paths.</param>
        /// <returns></returns>
        public string PathCombine(params string[] paths)
        {
            if (paths == null || paths.Length == 0)
                return string.Empty;

            var count = paths.Length;
            var trimmedPaths = new string[count];

            for (int i = 0; i < count; i++)
            {
                paths[i] = paths[i].Replace('\\', Path.DirectorySeparatorChar);
                paths[i] = paths[i].Replace('/', Path.DirectorySeparatorChar);
                if (i == 0)
                    trimmedPaths[i] = paths[i];
                else
                    trimmedPaths[i] = paths[i].TrimStart(Path.DirectorySeparatorChar);
            }

            return Path.Combine(trimmedPaths);
        }

        /// <summary>
        /// Returns the full physical path based on a location and a relative path
        /// </summary>
        /// <param name="location">The location.</param>
        /// <param name="path">The relative path.</param>
        /// <returns></returns>
        protected abstract string FullPath(StorageLocation location, string path);

        /// <summary>
        /// Ensures the folder exists.
        /// </summary>
        /// <param name="folderfullPath">The folderfull path.</param>
        /// <returns></returns>
        public virtual Task EnsureFolderExistsAsync(string folderfullPath)
        {
            return Task.Run(() =>
            {
                if (!Directory.Exists(folderfullPath))
                {
                    Directory.CreateDirectory(folderfullPath);
                }
            });
        }

        /// <summary>
        /// Checks if a files the exists in the specified path.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <returns></returns>
        public virtual Task<bool> FileExistsAsync(string fullPath)
        {
            return Task.FromResult(File.Exists(fullPath));
        }
    }
}
