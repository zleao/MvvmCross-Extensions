using MvxExtensions.Libraries.Portable.Core.Models;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.SelectableRecyclerViewComponents
{
    public class SelectableItem : ISelectableItem
    {
        public bool IsSelected { get; set; }

        public object Item { get; set; }

        public SelectableItem(object item, bool isSelected = false)
        {
            Item = item;
            IsSelected = isSelected;
        }
    }
}