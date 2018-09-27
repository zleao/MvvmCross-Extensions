using Android.Content;
using System.Threading;

namespace MvxExtensions.Platforms.Android.Views
{
    internal class DialogCancelListener : Java.Lang.Object, IDialogInterfaceOnCancelListener
    {
        private readonly AutoResetEvent _resetEvent;

        public DialogCancelListener(AutoResetEvent resetEvent)
        {
            _resetEvent = resetEvent;
        }

        public void OnCancel(IDialogInterface dialog)
        {
            _resetEvent?.Set();
        }
    }
}