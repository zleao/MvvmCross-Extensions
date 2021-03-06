﻿namespace MvxExtensions.Plugins.Storage.Models
{
    /// <summary>
    /// StreamMode
    /// </summary>
    public enum StreamMode
    {
        /// <summary>
        /// Opens an existing file to read. 
        /// </summary>
        Open,

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
