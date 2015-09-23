﻿using System.Collections.Generic;

namespace MvvmCrossUtilities.Libraries.Portable.Models
{
    /// <summary>
    /// IExpandable
    /// </summary>
    public interface IExpandable
    {
        /// <summary>
        /// Gets or sets the children.
        /// </summary>
        /// <value>
        /// The children.
        /// </value>
        IList<IExpandable> Children { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has children.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has children; otherwise, <c>false</c>.
        /// </value>
        bool HasChildren { get; }

        /// <summary>
        /// Gets the children.
        /// </summary>
        void GetChildren();

        /// <summary>
        /// Gets the level.
        /// </summary>
        /// <value>
        /// The level.
        /// </value>
        int Level { get; }
    }
}
