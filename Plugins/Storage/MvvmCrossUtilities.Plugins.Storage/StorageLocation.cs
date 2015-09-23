
namespace MvvmCrossUtilities.Plugins.Storage
{
    /// <summary>
    /// StorageLocation
    /// </summary>
    public enum StorageLocation
    {
        /// <summary>
        /// Files accessible only by the current app.
        /// Files removed when app is uninstalled.
        /// Best when you want to be sure that neither the user nor other apps can access your files.
        /// </summary>
        Internal,

        /// <summary>
        /// Files will be world-readable.
        /// Files removed when app is uninstalled.
        /// Best for files that are to be available to users and/or apps, but that don't provide value outside the app.
        /// (i.e.: aditional resources or temporary media files)
        /// </summary>
        ExternalPrivate,

        /// <summary>
        /// Files will be world-readable.
        /// Files will be persisted even after the app is uninstalled.
        /// Best for files that are to be available to users and/or apps, even after the app is uninstalled or updated.
        /// (i.e.: Photos captured by the app, log files, configuration files )
        /// </summary>
        ExternalPublic
    }
}
