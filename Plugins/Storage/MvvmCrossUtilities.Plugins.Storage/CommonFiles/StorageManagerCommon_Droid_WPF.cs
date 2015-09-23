using Cirrious.CrossCore.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

#if MONODROID
namespace MvvmCrossUtilities.Plugins.Storage.Droid
#else
namespace MvvmCrossUtilities.Plugins.Storage.Wpf
#endif
{
    public abstract class StorageManagerCommon_Droid_WPF : StorageManagerCommonEncryption_Droid_WP_WPF
    {
        #region Constructor

        ~StorageManagerCommon_Droid_WPF()
        {
            CryptologyAsync.DisposeAlgorithmAndPassword();
        }

        #endregion

        #region Common (Sync & Async)

        public override char DirectorySeparatorChar
        {
            get { return Path.DirectorySeparatorChar; }
        }

        private string GetPatternFromSearchMode(string originalPattern, SearchMode searchMode)
        {
            var pattern = "*";
            if (!string.IsNullOrEmpty(originalPattern))
            {
                switch (searchMode)
                {
                    case SearchMode.StartsWith:
                        pattern = originalPattern + "*";
                        break;

                    case SearchMode.EndsWith:
                        pattern = "*" + originalPattern;
                        break;

                    case SearchMode.Contains:
                        pattern = "*" + originalPattern + "*";
                        break;

                    case SearchMode.Equals:
                        pattern = originalPattern;
                        break;
                }
            }

            return pattern;
        }

        #endregion

        #region Asynchronous

        protected override async Task<bool> TryReadFileCommonAsync(string fullPath, Func<Stream, bool> streamAction)
        {
            return await Task.Run<bool>(() =>
            {
                if (!System.IO.File.Exists(fullPath))
                {
                    return false;
                }

                using (var fileStream = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    return streamAction(fileStream);
                }
            });
        }

        protected override async Task WriteFileCommonAsync(StorageMode mode, string fullPath, Action<Stream> streamAction)
        {
            await EnsureFolderExistsAsync(Path.GetDirectoryName(fullPath));

            await Task.Run(() =>
            {
                lock (lockObj)
                {
                    using (var fileStream = File.Open(fullPath, GetFileMode(mode), FileAccess.Write, FileShare.Read))
                    {
                        streamAction(fileStream);
                    }
                }
            });
        }

        public override async Task<bool> FileExistsAsync(string fullPath)
        {
            return await Task.Run(() => File.Exists(fullPath));
        }

        public override async Task<bool> FolderExistsAsync(string fullPath)
        {
            return await Task.Run(() => Directory.Exists(fullPath));
        }

        public override async Task DeleteFileAsync(string fullPath)
        {
            await Task.Run(() => File.Delete(fullPath));
        }

        public override async Task DeleteFolderAsync(string folderFullPath)
        {
            await Task.Run(() => Directory.Delete(folderFullPath, true));
        }

        public override async Task<IEnumerable<IFileInfo>> GetFilesInAsync(StorageLocation location, bool recursive, string folderPath = "", string searchPattern = "", SearchMode searchMode = SearchMode.Contains)
        {
            var fullPath = FullPath(location, folderPath);
            var pattern = GetPatternFromSearchMode(searchPattern, searchMode);
            var option = (recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            var di = new DirectoryInfo(fullPath);
            return await Task.Run<IEnumerable<IFileInfo>>(() => di.GetFiles(pattern, option).Select(f => new FileInfo(f, location, folderPath)));
        }

        public override async Task<IEnumerable<IFileInfo>> GetFilesInAsync(string folderFullPath, bool recursive, string searchPattern = "", SearchMode searchMode = SearchMode.Contains)
        {
            var pattern = GetPatternFromSearchMode(searchPattern, searchMode);
            var option = (recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

            var di = new DirectoryInfo(folderFullPath);
            return await Task.Run<IEnumerable<IFileInfo>>(() => di.GetFiles(pattern, option).Select(f => new FileInfo(f)));
        }

        public override async Task EnsureFolderExistsAsync(string folderfullPath)
        {
            await Task.Run(() =>
            {
                if (!Directory.Exists(folderfullPath))
                    Directory.CreateDirectory(folderfullPath);
            });
        }

        public override async Task<DateTime> GetFileCreationTimeAsync(StorageLocation location, string path)
        {
            return await Task.Run(() => File.GetCreationTime(FullPath(location, path)));
        }

        public override async Task<bool> TryMoveAsync(string fromFullPath, string toFullPath, bool deleteExistingTo)
        {
            try
            {
                if (!System.IO.File.Exists(fromFullPath))
                    return false;

                await EnsureFolderExistsAsync(toFullPath.Replace(GetFileName(toFullPath), string.Empty));

                return await Task.Run<bool>(() =>
                {
                    if (File.Exists(toFullPath))
                    {
                        if (deleteExistingTo)
                            File.Delete(toFullPath);
                        else
                            return false;
                    }

                    File.Move(fromFullPath, toFullPath);
                    return true;
                });
            }
            catch(AggregateException ag)
            {
                if (ag.InnerException.GetType() == typeof(ThreadAbortException))
                    throw;
                else
                {
                    MvxTrace.Error("Error during file move {0} : {1} : {2}", fromFullPath, toFullPath, ag.Message);
                    return false;
                }
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception exception)
            {
                MvxTrace.Error("Error during file move {0} : {1} : {2}", fromFullPath, toFullPath, exception.Message);
                return false;
            }
        }

        public override async Task CloneFileAsync(string fromFullPath, string toFullPath, bool overwriteExistingTo)
        {
            try
            {
                if (System.IO.File.Exists(fromFullPath))
                {
                    await EnsureFolderExistsAsync(toFullPath.Replace(GetFileName(toFullPath), string.Empty));

                    await Task.Run(() => File.Copy(fromFullPath, toFullPath, overwriteExistingTo));
                }
            }
            catch (AggregateException ag)
            {
                if (ag.InnerException.GetType() == typeof(ThreadAbortException))
                    throw;
                else
                {
                    MvxTrace.Error("Error during file move {0} : {1} : {2}", fromFullPath, toFullPath, ag.Message);
                    throw;
                }
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (Exception exception)
            {
                MvxTrace.Error("Error during file clone {0} : {1} : {2}", fromFullPath, toFullPath, exception.Message);
                throw;
            }
        }

        public override async Task CloneFolderAsync(string fromFullPath, string toFullPath, bool overwriteExistingTo, bool recursive)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(fromFullPath);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + fromFullPath);
            }

            // If the destination directory doesn't exist, create it. 
            await EnsureFolderExistsAsync(toFullPath);

            // Get the files in the directory and copy them to the new location.
            var files = dir.GetFiles();
            foreach (var file in files)
            {
                string temppath = Path.Combine(toFullPath, file.Name);
                file.CopyTo(temppath, overwriteExistingTo);
            }

            // If copying subdirectories, copy them and their contents to new location. 
            if (recursive)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(toFullPath, subdir.Name);
                    await CloneFolderAsync(subdir.FullName, tempPath, overwriteExistingTo, recursive);
                }
            }

        }

        #endregion

        #region Asynchronous Encryption

        protected override async Task<bool> TryReadEncryptedFileCommonAsync(string fullPath, string password, Func<Stream, bool> streamAction)
        {
            if (!File.Exists(fullPath))
            {
                return false;
            }

            using (var fileStream = await CryptologyAsync.DecryptFileToStreamAsync(fullPath, password))
            {
                return streamAction(fileStream);
            }
        }

        #endregion
    }
}
