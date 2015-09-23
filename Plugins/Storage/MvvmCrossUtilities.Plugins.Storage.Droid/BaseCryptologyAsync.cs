using System.IO;

namespace MvvmCrossUtilities.Plugins.Storage.Droid
{
    internal class BaseCryptologyAsync
    {
        internal static FileStream CreateFileStream(string filePath, FileMode fileMode, FileAccess fileAccess)
        {
            return new FileStream(filePath, fileMode, fileAccess);
        }
    }
}