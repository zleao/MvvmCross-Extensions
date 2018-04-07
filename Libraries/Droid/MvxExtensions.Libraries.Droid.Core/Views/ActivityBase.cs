using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Views;
using MvvmCross.Platform;
using MvvmCross.Platform.WeakSubscription;
using MvxExtensions.Libraries.Portable.Core.Extensions;
using MvxExtensions.Libraries.Portable.Core.Messages.OneWay;
using MvxExtensions.Libraries.Portable.Core.ViewModels;
using MvxExtensions.Plugins.Notification;
using MvxExtensions.Plugins.Notification.Core;
using MvxExtensions.Plugins.Notification.Core.Async.Subscriptions;
using MvxExtensions.Plugins.Notification.Messages;
using MvxExtensions.Plugins.Notification.Messages.Base;
using MvxExtensions.Plugins.Notification.Messages.OneWay;
using MvxExtensions.Plugins.Notification.Messages.TwoWay.Question;
using MvxExtensions.Plugins.Notification.Messages.TwoWay.Result;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MvxExtensions.Libraries.Droid.Core.Views
{
    public abstract class ActivityBase<TViewModel> : MvxActivity
        where TViewModel : ViewModel
    {
        #region Fields

        private volatile IList<SubscriptionToken> _messageTokens = new List<SubscriptionToken>();

        private volatile MvxNotifyPropertyChangedEventSubscription _propertyChangedSubscription = null;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the typed view model.
        /// </summary>
        /// <value>
        /// The typed view model.
        /// </value>
        protected TViewModel TypedViewModel
        {
            get { return ViewModel as TViewModel; }
        }

        /// <summary>
        /// Gets the notification manager.
        /// </summary>
        /// <value>
        /// The notification manager.
        /// </value>
        protected INotificationService NotificationManager
        {
            get { return _notificationManager ?? (_notificationManager = Mvx.Resolve<INotificationService>()); }
        }
        private INotificationService _notificationManager;

        /// <summary>
        /// Gets the activity manager service.
        /// </summary>
        /// <value>
        /// The activity manager service.
        /// </value>
        protected ActivityManager ActivityManagerService
        {
            get { return this.BaseContext.GetSystemService(ActivityService) as ActivityManager; }
        }

        #endregion

        #region Constructor

        internal ActivityBase()
        {
            //Construtor (internal) exists just to insure that this class is not directly inherited from outside this library
        }

        #endregion

        #region Lifecycle Methods

        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();

            GetAndSetBaseView();
        }

        /// <summary>
        /// Called when create.
        /// </summary>
        /// <param name="bundle">The bundle.</param>
        protected override void OnCreate(Android.OS.Bundle bundle)
        {
            base.OnCreate(bundle);

            if (BusyIndicator == null)
                _busyIndicator = CreateProgressDialog();
        }

        /// <summary>
        /// Called when resume.
        /// </summary>
        protected override void OnResume()
        {
            SubscribeMessageEvents();

            base.OnResume();

            if (TypedViewModel != null)
                TypedViewModel.ChangeVisibility(true);
        }

        /// <summary>
        /// Called when pause.
        /// </summary>
        protected override void OnPause()
        {
            if (TypedViewModel != null)
                TypedViewModel.ChangeVisibility(false);

            UnsubscribeMessageEvents();

            base.OnPause();
        }

        /// <summary>
        /// Called when destroy.
        /// </summary>
        protected override void OnDestroy()
        {
            if (_busyIndicator != null)
            {
                _busyIndicator.Dismiss();
                _busyIndicator = null;
            }

            base.OnDestroy();

            if (TypedViewModel != null)
                TypedViewModel.KillMe();
        }

        #endregion

        #region Notification Management

        /// <summary>
        /// Gets a value indicating whether subscribe generic messages.
        /// </summary>
        /// <value>
        /// <c>true</c> if subscribe generic messages; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SubscribeGenericMessages { get { return true; } }

        /// <summary>
        /// Gets a value indicating whether to subscribe generic question messages.
        /// </summary>
        /// <value>
        /// <c>true</c> if subscribe generic question messages; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SubscribeGenericQuestionMessages { get { return true; } }

        /// <summary>
        /// Gets a value indicating whether to subscribe question with custom answer messages.
        /// </summary>
        /// <value>
        /// <c>true</c> if subscribe question with custom answer messages; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SubscribeQuestionWithCustomAnswerMessages { get { return true; } }

        /// <summary>
        /// Gets a value indicating whether to subscribe terminate application message.
        /// </summary>
        /// <value>
        /// <c>true</c> if [subscribe terminate application message]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SubscribeTerminateApplicationMessage { get { return true; } }


        /// <summary>
        /// Subscribes the event
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        protected void SubscribeEvent<TMessage>(Func<TMessage, Task> asyncDeliveryAction, string context = AsyncSubscription.DefaultContext)
            where TMessage : INotificationMessage
        {
            var token = NotificationManager.Subscribe<TMessage>(asyncDeliveryAction, context);
            _messageTokens.Add(token);
        }

        /// <summary>
        /// Subscribes two way events
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="deliveryAction">The delivery action.</param>
        protected void SubscribeEvent<TMessage, TResult>(Func<TMessage, Task<TResult>> asyncDeliveryAction, string context = AsyncSubscription.DefaultContext)
            where TMessage : INotificationTwoWayMessage
            where TResult : INotificationResult
        {
            var token = NotificationManager.Subscribe<TMessage, TResult>(asyncDeliveryAction, context);
            _messageTokens.Add(token);
        }


        /// <summary>
        /// Subscribes the message events.
        /// </summary>
        protected virtual void SubscribeMessageEvents()
        {
            if (SubscribeBusyNotifications && TypedViewModel != null)
            {
                _propertyChangedSubscription = TypedViewModel.WeakSubscribe(OnPropertyChanged);
                SetBusyIndicatorVisibility(TypedViewModel.IsBusy);
                SetBusyIndicatorMessage(TypedViewModel.BusyMessage);
            }

            if (SubscribeGenericMessages)
                SubscribeEvent<NotificationGenericMessage>(OnNotificationGenericMessageAsync);

            if (SubscribeGenericQuestionMessages)
                SubscribeEvent<NotificationGenericQuestionMessage, NotificationGenericQuestionResult>(OnNotificationGenericQuestionMessageAsync);

            if (SubscribeQuestionWithCustomAnswerMessages)
                SubscribeEvent<NotificationQuestionWithCustomAnswerMessage, NotificationQuestionCustomAnswerResult>(OnNotificationQuestionWithCustomAnswerMessageAsync);

            if (SubscribeTerminateApplicationMessage)
                SubscribeEvent<NotificationTerminateApplicationMessage>(OnNotificationTerminateApplicationMessageAsync);
        }

        /// <summary>
        /// Unsubscribes the message events.
        /// </summary>
        protected virtual void UnsubscribeMessageEvents()
        {
            foreach (var item in _messageTokens)
            {
                NotificationManager.Unsubscribe(item);
            }
            _messageTokens.Clear();

            if (_propertyChangedSubscription != null)
            {
                _propertyChangedSubscription.Dispose();
                _propertyChangedSubscription = null;
            }
            SetBusyIndicatorVisibility(false);
            SetBusyIndicatorMessage(string.Empty);
        }


        /// <summary>
        /// Called when notification error message.
        /// </summary>
        /// <param name="message">The obj.</param>
        protected virtual async Task OnNotificationGenericMessageAsync(NotificationGenericMessage message)
        {
            if (message != null)
            {
                switch (message.Mode)
                {
                    case NotificationModeEnum.MessageBox:
                        await Task.Run(async () => await ShowMessageBoxAsync(message.Message, message.Severity));
                        break;

                    case NotificationModeEnum.Default:
                    case NotificationModeEnum.Toast:
                        ShowToast(message.Message);
                        break;
                }
            }
        }

        /// <summary>
        /// Called when notification generic question message asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        protected virtual async Task<NotificationGenericQuestionResult> OnNotificationGenericQuestionMessageAsync(NotificationGenericQuestionMessage message)
        {
            var buttons = GetButtonsName(message.PossibleAnswers);

            var result = await ShowGenericQuestionDialogAsync(string.Empty, message.Question, buttons[0], buttons[1]);

            return new NotificationGenericQuestionResult(ConvertBool2NotificationTwoWayAnswersEnum(result, message.PossibleAnswers));
        }

        /// <summary>
        /// Called when notification question width custom answer asynchronous.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        protected virtual async Task<NotificationQuestionCustomAnswerResult> OnNotificationQuestionWithCustomAnswerMessageAsync(NotificationQuestionWithCustomAnswerMessage message)
        {
            var selectedIndex = await Task.Run<int>(() => this.ShowSimpleSelectionDialogAsync(message.Question, message.PossibleAnswers));
            if (selectedIndex < 0 || selectedIndex >= message.PossibleAnswers.SafeCount())
                return new NotificationQuestionCustomAnswerResult(null, selectedIndex);

            return new NotificationQuestionCustomAnswerResult(message.PossibleAnswers[selectedIndex], selectedIndex);
        }

        /// <summary>
        /// Called when notification terminate application message.
        /// </summary>
        /// <param name="message">The object.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        protected virtual Task OnNotificationTerminateApplicationMessageAsync(NotificationTerminateApplicationMessage message)
        {
            Finish();

            return Task.FromResult(true);
        }

        #endregion

        #region Busy Notification Handling

        /// <summary>
        /// Gets a value indicating whether subscribe busy notifications.
        /// </summary>
        /// <value>
        /// <c>true</c> if subscribe busy notifications; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SubscribeBusyNotifications
        {
            get { return true; }
        }

        /// <summary>
        /// Gets the busy indicator.
        /// </summary>
        /// <value>
        /// The busy indicator.
        /// </value>
        protected ProgressDialog BusyIndicator
        {
            get { return _busyIndicator; }
        }
        private ProgressDialog _busyIndicator;


        protected virtual ProgressDialog CreateProgressDialog()
        {
            var busyIndicator = new ProgressDialog(this);
            busyIndicator.SetCancelable(false);
            busyIndicator.SetCanceledOnTouchOutside(false);

            return busyIndicator;
        }

        /// <summary>
        /// Handles the OnPropertyChanged event of the ViewModel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsBusy")
            {
                SetBusyIndicatorVisibility(TypedViewModel.IsBusy);
            }
            else if (e.PropertyName == "BusyMessage")
            {
                SetBusyIndicatorMessage(TypedViewModel.BusyMessage);
            }
        }

        /// <summary>
        /// Sets the busy indicator visibility.
        /// </summary>
        /// <param name="showBusyNotification">if set to <c>true</c> [show busy notification].</param>
        protected virtual void SetBusyIndicatorVisibility(bool showBusyNotification)
        {
            if (BusyIndicator != null)
            {
                if (showBusyNotification && !IsFinishing)
                    BusyIndicator.Show();
                else
                    BusyIndicator.Hide();
            }
        }

        /// <summary>
        /// Sets the busy indicator message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected virtual void SetBusyIndicatorMessage(string message)
        {
            if (BusyIndicator != null)
            {
                if (!IsFinishing)
                    BusyIndicator.SetMessage(TypedViewModel.BusyMessage ?? string.Empty);
                else
                    BusyIndicator.SetMessage(string.Empty);
            }
        }

        #endregion

        #region Generic Methods

        /// <summary>
        /// Gets the image resource identifier.
        /// </summary>
        /// <param name="imageName">Name of the image.</param>
        /// <returns></returns>
        protected abstract int GetResourceIdFromImageId(string imageId);


        protected virtual void GetAndSetBaseView()
        {
            SetContentView(GetBasePageNameResourceId());
        }

        protected virtual int GetBasePageNameResourceId()
        {
            var resName = "Page_" + this.GetType().Name;
            var field = GetPageViewFieldInfo(resName);
            if (field == null)
                throw new Android.Content.Res.Resources.NotFoundException(resName);

            var resId = field.GetValue(null) as int?;
            if (resId == null)
                throw new NullReferenceException(string.Format("Id for resource '{0}' not found", resName));

            return resId.Value;
        }

        protected abstract FieldInfo GetPageViewFieldInfo(string pageName);
        //{
        //    return typeof(Resource.Layout).GetField(pageName);
        //}

        /// <summary>
        /// Creates the intent for a existing view model.
        /// </summary>
        /// <param name="subViewModel">The view model.</param>
        /// <returns></returns>
        protected Intent CreateIntentFor(IMvxViewModel subViewModel)
        {
            var intentWithKey = Mvx.Resolve<IMvxAndroidViewModelRequestTranslator>().GetIntentWithKeyFor(subViewModel);
            return intentWithKey.Item1;
        }

        protected NotificationTwoWayAnswersEnum ConvertBool2NotificationTwoWayAnswersEnum(bool value, NotificationTwoWayAnswersGroupEnum possibleAnswers)
        {
            switch (possibleAnswers)
            {
                case NotificationTwoWayAnswersGroupEnum.Ok:
                    return value ? NotificationTwoWayAnswersEnum.Ok : NotificationTwoWayAnswersEnum.Unknown;

                case NotificationTwoWayAnswersGroupEnum.YesNo:
                    return value ? NotificationTwoWayAnswersEnum.Yes : NotificationTwoWayAnswersEnum.No;

                case NotificationTwoWayAnswersGroupEnum.OkCancel:
                    return value ? NotificationTwoWayAnswersEnum.Ok : NotificationTwoWayAnswersEnum.Cancel;
            }

            return NotificationTwoWayAnswersEnum.Unknown;
        }

        protected List<string> GetButtonsName(NotificationTwoWayAnswersGroupEnum possibleAnswers)
        {
            var posBtnName = TypedViewModel.TextSource.GetText("Label_Common_Ok");
            var negBtnName = TypedViewModel.TextSource.GetText("Label_Common_Cancel");
            if (possibleAnswers == NotificationTwoWayAnswersGroupEnum.YesNo)
            {
                posBtnName = TypedViewModel.TextSource.GetText("Label_Common_Yes");
                negBtnName = TypedViewModel.TextSource.GetText("Label_Common_No");
            }

            return new List<string>() { posBtnName, negBtnName };
        }


        /// <summary>
        /// Gets the title from severity level.
        /// </summary>
        /// <param name="severity">The severity level.</param>
        /// <returns></returns>
        protected virtual string GetTitleFromSeverity(NotificationSeverityEnum severity)
        {
            switch (severity)
            {
                case NotificationSeverityEnum.Error:
                    return TypedViewModel.TextSource.GetText("Label_Dialog_Title_Error");

                case NotificationSeverityEnum.Info:
                case NotificationSeverityEnum.Success:
                    return TypedViewModel.TextSource.GetText("Label_Dialog_Title_Information");

                case NotificationSeverityEnum.Warning:
                    return TypedViewModel.TextSource.GetText("Label_Dialog_Title_Warning");
            }

            return TypedViewModel.TextSource.GetText("Label_Dialog_Title_Message");
        }

        /// <summary>
        /// Gets the icon from severity level.
        /// </summary>
        /// <param name="severity">The severity level.</param>
        /// <returns></returns>
        protected virtual int GetIconFromSeverity(NotificationSeverityEnum severity)
        {
            switch (severity)
            {
                case NotificationSeverityEnum.Error:
                case NotificationSeverityEnum.Warning:
                    return Android.Resource.Drawable.IcDialogAlert;

                case NotificationSeverityEnum.Info:
                case NotificationSeverityEnum.Success:
                    return Android.Resource.Drawable.IcDialogInfo;
            }

            return Android.Resource.Drawable.IcDialogAlert;
        }


        /// <summary>
        /// Creates a new instance of a toast control
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="toastLength">Length of time the toast will be shown.</param>
        /// <returns></returns>
        protected virtual Toast CreateToast(string message, ToastLength toastLength = ToastLength.Long)
        {
            return Toast.MakeText(this, message, toastLength);
        }
        /// <summary>
        /// Calls CreateToast and show the toast.
        /// It garantees that the code executes in UI thread
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="toastLength">Length of time the toast will be shown.</param>
        protected virtual void ShowToast(string message, ToastLength toastLength = ToastLength.Long)
        {
            if (Thread.CurrentThread.IsBackground)
            {
                RunOnUiThread(() => ShowToast(message, toastLength));
            }
            else
            {
                var toast = CreateToast(message, toastLength);
                if (toast != null)
                    toast.Show();
            }
        }

        /// <summary>
        /// Creates the generic question dialog.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        protected virtual Android.Support.V7.App.AlertDialog.Builder CreateGenericQuestionDialog(string title, string message)
        {

            var builder = new Android.Support.V7.App.AlertDialog.Builder(this);
            builder.SetTitle(title);
            builder.SetMessage(message);

            return builder;
        }
        /// <summary>
        /// Shows a question dialog.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <param name="positiveButtonName">Name of the positive button.</param>
        /// <param name="negativeButtonName">Name of the negative button.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">ShowBlockingQuestionMessage must be called in a background thread!</exception>
        protected virtual async Task<bool> ShowGenericQuestionDialogAsync(string title, string message, string positiveButtonName, string negativeButtonName)
        {
            var result = false;

            if (!Thread.CurrentThread.IsBackground)
            {
                return await Task.Run(() => ShowGenericQuestionDialogAsync(title, message, positiveButtonName, negativeButtonName));
            }
            else
            {
                AutoResetEvent resetEvent = new AutoResetEvent(false);

                var mb = CreateGenericQuestionDialog(title, message);
                if (mb != null)
                {
                    mb.SetPositiveButton(positiveButtonName, (sender, e) =>
                    {
                        result = true;
                        resetEvent.Set();
                    });

                    mb.SetNegativeButton(negativeButtonName, (sender, e) =>
                    {
                        resetEvent.Set();
                    });

                    RunOnUiThread(() => mb.Show());

                    resetEvent.WaitOne();
                }

                return result;
            }
        }

        /// <summary>
        /// Creates the blocking simple selection dialog.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        protected virtual Android.Support.V7.App.AlertDialog.Builder CreateSimpleSelectionDialog(string title)
        {
            var builder = new Android.Support.V7.App.AlertDialog.Builder(this);

            //Set Custom title to allow multiline text
            var titleView = LayoutInflater.Inflate(Resource.Layout.Dialog_Common_TitleMultiline, null);
            var textView = titleView.FindViewById<TextView>(Resource.Id.dialogTitle);
            textView.Text = title;
            builder.SetCustomTitle(titleView);

            return builder;
        }
        /// <summary>
        /// Shows a simple selection dialog and waits for user interaction
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="options">The options.</param>
        /// <param name="indexIfCancel">The index if cancel.</param>
        /// <returns></returns>
        /// <exception cref="System.UnauthorizedAccessException">
        /// Method ShowSimpleSelectionDialogSync has to run in a background thread.
        /// or
        /// Method ShowSimpleSelectionDialogSync is beeing called in a view that is finishing (probably as allready been destroyed).
        /// </exception>
        protected virtual async Task<int> ShowSimpleSelectionDialogAsync(string title, IList<string> options, int indexIfCancel = -1)
        {
            if (IsFinishing)
                throw new UnauthorizedAccessException("Method ShowSimpleSelectionDialogSync is beeing called in a view that is finishing (probably as allready been destroyed).");

            if (!Thread.CurrentThread.IsBackground)
            {
                return await Task.Run(() => ShowSimpleSelectionDialogAsync(title, options, indexIfCancel));
            }
            else
            {
                AutoResetEvent resetEvent = new AutoResetEvent(false);
                int selectedIndex = indexIfCancel;

                var bss = CreateSimpleSelectionDialog(title);
                if (bss != null)
                {
                    //Set avilable options list for the user
                    bss.SetItems(options.ToArray(), (sender, args) =>
                    {
                        selectedIndex = args.Which;
                        resetEvent.Set();
                    });

                    bss.SetOnCancelListener(new Droid.Core.Views.DialogCancelListener(resetEvent));

                    RunOnUiThread(() => bss.Show());

                    resetEvent.WaitOne();

                    if (bss != null)
                    {
                        bss.Dispose();
                        bss = null;
                    }
                }

                return selectedIndex;
            }
        }

        /// <summary>
        /// Creates the async message box.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="severity">The severity.</param>
        /// <returns></returns>
        protected virtual Android.Support.V7.App.AlertDialog.Builder CreateMessageBox(string message, NotificationSeverityEnum severity)
        {
            var title = GetTitleFromSeverity(severity);
            var icon = GetIconFromSeverity(severity);

            var builder = new Android.Support.V7.App.AlertDialog.Builder(this);
            builder.SetTitle(title);
            builder.SetIcon(icon);
            builder.SetMessage(message);

            return builder;

        }
        /// <summary>
        /// Shows the message box.
        /// </summary>
        /// <param name="titleResource">The title resource.</param>
        /// <param name="message">The message.</param>
        /// <param name="iconResource">The icon resource.</param>
        /// <returns></returns>
        /// <exception cref="System.NotSupportedException">ShowBlockingQuestionMessage must be called in a background thread!</exception>
        protected virtual async Task ShowMessageBoxAsync(string message, NotificationSeverityEnum severity)
        {
            if (!Thread.CurrentThread.IsBackground)
            {
                await Task.Run(() => ShowMessageBoxAsync(message, severity));
            }
            else
            {
                AutoResetEvent resetEvent = new AutoResetEvent(false);

                var amb = CreateMessageBox(message, severity);
                if (amb != null)
                {
                    amb.SetPositiveButton(TypedViewModel.TextSource.GetText("Label_Common_Ok"), (sender, e) => { resetEvent.Set(); });
                    RunOnUiThread(() => amb.Show());

                    resetEvent.WaitOne();
                }
            }
        }

        #endregion
    }
}