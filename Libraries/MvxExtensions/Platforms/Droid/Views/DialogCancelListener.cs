using System.Threading;
using Android.Content;

namespace MvxExtensions.Platforms.Droid.Views
{
    public class DialogCancelListener : Java.Lang.Object, IDialogInterfaceOnCancelListener
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