namespace MvxExtensions.Models
{
    /// <summary>
    /// Represents an item that has the ability to be selected (visual effect)
    /// </summary>
    public interface ISelectableItem
    {
        /// <summary>
        /// Indicates if this item is selected
        /// </summary>
        bool IsSelected { get; set; }
    }
}
