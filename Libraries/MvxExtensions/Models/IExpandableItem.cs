namespace MvxExtensions.Models
{
    /// <summary>
    /// Represents an item that has the ability to expand (visual effect)
    /// </summary>
    public interface IExpandableItem
    {
        /// <summary>
        /// Indicates if this item should be expanded when the is created.
        /// </summary>
        bool IsInitiallyExpanded { get; set; }

        /// <summary>
        /// Indicates if this item is expanded
        /// </summary>
        bool IsExpanded { get; set; }
    }
}
