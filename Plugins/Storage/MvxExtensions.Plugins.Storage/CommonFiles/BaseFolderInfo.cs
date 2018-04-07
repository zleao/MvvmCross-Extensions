namespace MvxExtensions.Plugins.Storage.CommonFiles
{
    /// <summary>
    /// Represents the information regarding a folder
    /// </summary>
    public class BaseFolderInfo : IFolderInfo
    {
        /// <summary>
        /// Name of the folder
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFolderInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the folder.</param>
        public BaseFolderInfo(string name)
        {
            Name = name;
        }
    }
}
