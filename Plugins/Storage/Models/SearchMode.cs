namespace MvxExtensions.Plugins.Storage.Models
{
    /// <summary>
    /// SearchMode
    /// </summary>
    public enum SearchMode
    {
        /// <summary>
        /// Maches the pattern with the beginning of the string
        /// </summary>
        StartsWith,

        /// <summary>
        /// Maches the pattern with the end of the string
        /// </summary>
        EndsWith,

        /// <summary>
        /// Maches the pattern within the string
        /// </summary>
        Contains,

        /// <summary>
        /// Maches the pattern with the entire string
        /// </summary>
        Equals
    }
}
