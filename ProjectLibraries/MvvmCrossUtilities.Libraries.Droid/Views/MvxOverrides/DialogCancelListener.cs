using System.Threading;
using Android.Content;

namespace MvvmCrossUtilities.Libraries.Droid.Views
{
    internal class DialogCancelListener : Java.Lang.Object, IDialogInterfaceOnCancelListener
    {
        private readonly AutoResetEvent _resetEvent = null;

        public DialogCancelListener(AutoResetEvent resetEvent)
        {
            _resetEvent = resetEvent;
        }

        public void OnCancel(IDialogInterface dialog)
        {
            if (_resetEvent != null)
                _resetEvent.Set();
        }
    }
}