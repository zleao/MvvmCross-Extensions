
namespace MvxExtensions.Plugins.Storage.Models
{
    /// <summary>
    /// StorageLocation
    /// Follows the same naming as the one present in Xamarin.Essentials.FileSystem
    /// </summary>
    public enum StorageLocation
    {
        /// <summary>
        /// Files accessible only by the current app.
        /// </summary>
        AppCacheDirectory,

        /// <summary>
        /// Files accessible only by the current app.
        /// </summary>
        AppDataDirectory,

        /// <summary>
        /// Files will be readable by other apps, if they have the correct permission.
        /// </summary>
        SharedDataDirectory
    }
}
