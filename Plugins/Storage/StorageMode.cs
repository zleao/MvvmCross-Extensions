
namespace MvxExtensions.Plugins.Storage
{
    /// <summary>
    /// StorageMode
    /// </summary>
    public enum StorageMode
    {
        /// <summary>
        /// Allways creates a new file. 
        /// If a file with the same name already exists, it gets overwritten
        /// </summary>
        Create,

        /// <summary>
        /// Uses the existing file if present, otherwise creates a new one
        /// </summary>
        CreateOrAppend,
    }
}
