using System.Collections.Generic;

namespace MvvmCrossUtilities.Libraries.Portable.Models
{
    public interface IExpandable
    {
        IList<IExpandable> Children { get; set; }

        bool HasChildren { get; }

        void GetChildren();
    }
}
