using System.Collections;

namespace MvxExtensions.Libraries.Portable.Core.Models
{
    /// <summary>
    /// Represents an item that is used to group a set of items.
    /// It also has the ability to expand itself. Refer to <see cref="IExpandableItem"/>
    /// </summary>
    public interface IGroupItem : IExpandableItem
    {
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        string Title { get; set; }

        /// <summary>
        /// Items that belong to this group
        /// </summary>
        IEnumerable Children { get; set; }
    }
}
