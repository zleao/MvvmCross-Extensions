using System.IO;

namespace MvxExtensions.Plugins.Storage.Droid
{
    /// <summary>
    /// Base imnplementation for the cryptology implementation.
    /// </summary>
    internal class BaseCryptologyAsync
    {
        /// <summary>
        /// Creates a stream to access the specified file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="fileMode">The file mode.</param>
        /// <param name="fileAccess">The file access.</param>
        /// <returns></returns>
        internal static FileStream CreateFileStream(string filePath, FileMode fileMode, FileAccess fileAccess)
        {
            return new FileStream(filePath, fileMode, fileAccess);
        }
    }
}