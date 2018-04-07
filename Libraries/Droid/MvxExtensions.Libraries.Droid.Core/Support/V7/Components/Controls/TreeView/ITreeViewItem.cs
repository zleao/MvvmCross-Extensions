using Android.Widget;
using MvvmCross.Platform.Core;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.TreeView
{
    public interface ITreeViewItem : IMvxDataConsumer, ICheckable
    {
        int TemplateId { get; }
    }
}
