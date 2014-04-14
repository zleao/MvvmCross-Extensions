using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Widget;
using MvvmCrossUtilities.Plugins.Notification.Messages;

namespace MvvmCrossUtilities.Libraries.Droid.Views
{
    public static class Extensions
    {
        #region Messages

        /// <summary>
        /// Shows the toast.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="message">The message.</param>
        /// <param name="toastLength">Length of the toast.</param>
        public static void ShowToast(this Activity source, string message, ToastLength toastLength = ToastLength.Long)
        {
            if (source != null)
            {
                if (Thread.CurrentThread.IsBackground)
                {
                    source.RunOnUiThread(() => source.ShowToast(message, toastLength));
                }
                else
                {
                    Toast.MakeText(source, message, ToastLength.Long).Show();
                }
            }
        }

        /// <summary>
        /// Shows the message box.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="icon">The icon.</param>
        public static void ShowMessageBox(this Activity source, int titleResource, string message, int iconResource)
        {
            if (source != null)
            {
                AlertDialog.Builder builder = new AlertDialog.Builder(source);
                builder.SetTitle(titleResource);
                builder.SetIcon(iconResource);
                builder.SetMessage(message);
                builder.SetPositiveButton(Android.Resource.String.Ok, (sender, e) => { });
                builder.Show();
            }
        }

        /// <summary>
        /// Gets the title from severity.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="severity">The severity.</param>
        /// <returns></returns>
        public static int GetTitleFromSeverity(this Activity source, NotificationSeverityEnum severity)
        {
            switch (severity)
            {
                case NotificationSeverityEnum.Error:
                case NotificationSeverityEnum.Info:
                case NotificationSeverityEnum.Pending:
                case NotificationSeverityEnum.Warning:
                case NotificationSeverityEnum.Success:
                    return Android.Resource.String.DialogAlertTitle;
            }

            return Android.Resource.String.Untitled;
        }

        /// <summary>
        /// Gets the icon from severity.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="severity">The severity.</param>
        /// <returns></returns>
        public static int GetIconFromSeverity(this Activity source, NotificationSeverityEnum severity)
        {
            switch (severity)
            {
                case NotificationSeverityEnum.Error:
                case NotificationSeverityEnum.Warning:
                case NotificationSeverityEnum.Pending:
                    return Android.Resource.Drawable.IcDialogAlert;

                case NotificationSeverityEnum.Info:
                case NotificationSeverityEnum.Success:
                    return Android.Resource.Drawable.IcDialogInfo;
            }

            return Android.Resource.Drawable.IcDialogAlert;
        }

        /// <summary>
        /// Show a dialog and wait until user interact.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="okButtonName">Name of the ok button.</param>
        /// <param name="okButtonAction">The ok button action.</param>
        /// <exception cref="System.NotSupportedException">ShowBlockingErrorMessage must be called in a background thread!</exception>
        public static void ShowGenericBlockingDialog(this Activity source,
                                                       string title, string message,
                                                       string okButtonName, EventHandler<DialogClickEventArgs> okButtonAction)
        {
            if (source != null)
            {
                if (!Thread.CurrentThread.IsBackground)
                    throw new NotSupportedException("ShowGenericBlockingMessage must be called in a background thread!");

                AutoResetEvent resetEvent = new AutoResetEvent(false);

                AlertDialog.Builder builder = new AlertDialog.Builder(source);
                builder.SetTitle(title);
                builder.SetIcon(Android.Resource.Drawable.IcDialogAlert);
                builder.SetMessage(message);

                builder.SetNegativeButton(okButtonName, (sender, e) =>
                {
                    if (okButtonAction != null)
                        okButtonAction(sender, e);
                    resetEvent.Set();
                });

                source.RunOnUiThread(() => builder.Show());

                resetEvent.WaitOne();
            }
        }

        /// <summary>
        /// Shows a question dialog.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="positiveButtonName">Name of the positive button.</param>
        /// <param name="positiveButtonAction">The positive button action.</param>
        /// <param name="negativeButtonName">Name of the negative button.</param>
        /// <param name="negativeButtonAction">The negative button action.</param>
        /// <exception cref="System.NotSupportedException">ShowBlockingQuestionMessage must be called in a background thread!</exception>
        public static bool ShowGenericQuestionDialog(this Activity source,
                                                     string title, string message,
                                                     string positiveButtonName,
                                                     string negativeButtonName)
        {
            var result = false;

            if (source != null)
            {
                if (!Thread.CurrentThread.IsBackground)
                    throw new NotSupportedException("ShowBlockingQuestionMessage must be called in a background thread!");

                AutoResetEvent resetEvent = new AutoResetEvent(false);

                AlertDialog.Builder builder = new AlertDialog.Builder(source);
                builder.SetTitle(title);
                builder.SetMessage(message);

                builder.SetPositiveButton(positiveButtonName, (sender, e) =>
                {
                    result = true;
                    resetEvent.Set();
                });

                builder.SetNegativeButton(negativeButtonName, (sender, e) =>
                {
                    resetEvent.Set();
                });

                source.RunOnUiThread(() => builder.Show());

                resetEvent.WaitOne();
            }

            return result;
        }

        /// <summary>
        /// Shows a simple selection dialog and waits for user interaction
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="title">The title.</param>
        /// <param name="options">The options.</param>
        /// <param name="indexIfCancel">The index if cancel.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">
        /// Method ShowSimpleSelectionDialogSync has to run in a background thread.
        /// or
        /// Method ShowSimpleSelectionDialogSync is beeing called in a view that is finishing (probably as allready been destroyed).
        /// </exception>
        public static int ShowBlockingSimpleSelectionDialog(this Activity source, string title, IList<string> options, int indexIfCancel = -1)
        {
            if (!Thread.CurrentThread.IsBackground)
                throw new System.UnauthorizedAccessException("Method ShowSimpleSelectionDialogSync has to run in a background thread.");

            if (source.IsFinishing)
                throw new System.UnauthorizedAccessException("Method ShowSimpleSelectionDialogSync is beeing called in a view that is finishing (probably as allready been destroyed).");

            AutoResetEvent resetEvent = new AutoResetEvent(false);
            int selectedIndex = indexIfCancel;

            AlertDialog.Builder builderSingle = new AlertDialog.Builder(source);

            //Set Custom title to allow multiline text
            var titleView = source.LayoutInflater.Inflate(Resource.Layout.Dialog_Common_TitleMultiline, null);
            var textView = titleView.FindViewById<TextView>(Resource.Id.dialogTitle);
            textView.Text = title;
            builderSingle.SetCustomTitle(titleView);

            //Set avilable options list for the user
            builderSingle.SetItems(options.ToArray(), (sender, args) =>
            {
                selectedIndex = args.Which;
                resetEvent.Set();
            });

            builderSingle.SetOnCancelListener(new DialogCancelListener(resetEvent));

            source.RunOnUiThread(() => builderSingle.Show());

            resetEvent.WaitOne();

            if (builderSingle != null)
            {
                builderSingle.Dispose();
                builderSingle = null;
            }

            return selectedIndex;
        }

        #endregion
    }
}