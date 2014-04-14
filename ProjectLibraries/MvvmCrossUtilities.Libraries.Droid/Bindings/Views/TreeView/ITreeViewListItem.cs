using Android.Widget;
using Cirrious.CrossCore.Core;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views.TreeView
{
    public interface ITreeViewListItem : IMvxDataConsumer, ICheckable
    {
        int TemplateId { get; }
    }
}
