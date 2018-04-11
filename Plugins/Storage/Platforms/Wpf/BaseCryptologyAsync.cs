using System.IO;

namespace MvxExtensions.Plugins.Storage.Wpf
{
    internal class BaseCryptologyAsync
    {
        internal static FileStream CreateFileStream(string filePath, FileMode fileMode, FileAccess fileAccess)
        {
            return new FileStream(filePath, fileMode, fileAccess);
        }
    }
}