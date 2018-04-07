using System.Collections.Generic;
using System.Threading;

namespace MvxExtensions.Plugins.Storage.CommonFiles
{
    /// <summary>
    /// Storage helpers
    /// </summary>
    public static class StorageHelpers
    {
        private static readonly Dictionary<string, SemaphoreSlim> _semaphores = new Dictionary<string, SemaphoreSlim>();

        /// <summary>
        /// Gets the semaphore registered for the specified key.
        /// If there's no semaphore registered yet, it creates a new one and returns it.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static SemaphoreSlim GetSemaphore(string key)
        {
            if (_semaphores.ContainsKey(key))
                return _semaphores[key];

            var semaphore = new SemaphoreSlim(1, 1);
            _semaphores.Add(key, semaphore);
            return semaphore;
        }
    }
}
