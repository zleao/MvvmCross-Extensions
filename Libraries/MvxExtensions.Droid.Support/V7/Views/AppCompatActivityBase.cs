using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross;
using MvvmCross.Droid.Support.V7.AppCompat;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.ViewModels;
using MvvmCross.WeakSubscription;
using MvxExtensions.Core.Extensions;
using MvxExtensions.Platforms.Droid.Views;
using MvxExtensions.Plugins.Notification;
using MvxExtensions.Plugins.Notification.Core;
using MvxExtensions.Plugins.Notification.Core.Async.Subscriptions;
using MvxExtensions.Plugins.Notification.Messages;
using MvxExtensions.Plugins.Notification.Messages.Base;
using MvxExtensions.Plugins.Notification.Messages.OneWay;
using MvxExtensions.Plugins.Notification.Messages.TwoWay.Question;
using MvxExtensions.Plugins.Notification.Messages.TwoWay.Result;
using MvxExtensions.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AndroidResource = Android.Resource;

namespace MvxExtensions.Droid.Support.V7.Views
{
    public abstract class AppCompatActivityBase<TViewModel> : MvxAppCompatActivity<TViewModel>
        where TViewModel : ViewModel
    {
        #region Fields

        private volatile IList<SubscriptionToken> _messageTokens = new List<SubscriptionToken>();

        private volatile MvxNotifyPropertyChangedEventSubscription _propertyChangedSubscription;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the notification manager.
        /// </summary>
        /// <value>
        /// The notification manager.
        /// </value>
        protected INotificationService NotificationManager => _notificationManager ?? (_notificationManager = Mvx.IoCProvider.Resolve<INotificationService>());
        private INotificationService _notificationManager;

        /// <summary>
        /// Gets the activity manager service.
        /// </summary>
        /// <value>
        /// The activity manager service.
        /// </value>
        protected ActivityManager ActivityManagerService => BaseContext.GetSystemService(ActivityService) as ActivityManager;

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
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            if (BusyIndicator == null)
                BusyIndicator = CreateBusyIndicatorView();
        }

        /// <summary>
        /// Called when resume.
        /// </summary>
        protected override void OnResume()
        {
            SubscribeMessageEvents();
            base.OnResume();
        }

        /// <summary>
        /// Called when pause.
        /// </summary>
        protected override void OnPause()
        {
            UnsubscribeMessageEvents();
            base.OnPause();
        }

        /// <summary>
        /// Called when destroy.
        /// </summary>
        protected override void OnDestroy()
        {
            if (BusyIndicator != null)
            {
                BusyIndicator = null;
            }

            base.OnDestroy();
        }

        #endregion

        #region Notification Management

        /// <summary>
        /// Gets a value indicating whether subscribe generic messages.
        /// </summary>
        /// <value>
        /// <c>true</c> if subscribe generic messages; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SubscribeGenericMessages => true;

        /// <summary>
        /// Gets a value indicating whether to subscribe generic question messages.
        /// </summary>
        /// <value>
        /// <c>true</c> if subscribe generic question messages; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SubscribeGenericQuestionMessages => true;

        /// <summary>
        /// Gets a value indicating whether to subscribe question with custom answer messages.
        /// </summary>
        /// <value>
        /// <c>true</c> if subscribe question with custom answer messages; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SubscribeQuestionWithCustomAnswerMessages => true;

        /// <summary>
        /// Gets a value indicating whether to subscribe terminate application message.
        /// </summary>
        /// <value>
        /// <c>true</c> if [subscribe terminate application message]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SubscribeTerminateApplicationMessage => true;


        /// <summary>
        /// Subscribes the event
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        protected void SubscribeEvent<TMessage>(Func<TMessage, Task> asyncDeliveryAction, string context = AsyncSubscription.DefaultContext)
            where TMessage : INotificationMessage
        {
            var token = NotificationManager.Subscribe(asyncDeliveryAction, context);
            _messageTokens.Add(token);
        }

        /// <summary>
        /// Subscribes two way events
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        protected void SubscribeEvent<TMessage, TResult>(Func<TMessage, Task<TResult>> asyncDeliveryAction, string context = AsyncSubscription.DefaultContext)
            where TMessage : INotificationTwoWayMessage
            where TResult : INotificationResult
        {
            var token = NotificationManager.Subscribe(asyncDeliveryAction, context);
            _messageTokens.Add(token);
        }


        /// <summary>
        /// Subscribes the message events.
        /// </summary>
        protected virtual void SubscribeMessageEvents()
        {
            if (SubscribeBusyNotifications && ViewModel != null)
            {
                _propertyChangedSubscription = ViewModel.WeakSubscribe(OnPropertyChanged);
                SetBusyIndicatorVisibility(ViewModel.IsBusy);
                SetBusyIndicatorMessage(ViewModel.BusyMessage);
            }

            if (SubscribeGenericMessages)
                SubscribeEvent<NotificationGenericMessage>(OnNotificationGenericMessageAsync);

            if (SubscribeGenericQuestionMessages)
                SubscribeEvent<NotificationGenericQuestionMessage, NotificationGenericQuestionResult>(OnNotificationGenericQuestionMessageAsync);

            if (SubscribeQuestionWithCustomAnswerMessages)
                SubscribeEvent<NotificationQuestionWithCustomAnswerMessage, NotificationQuestionCustomAnswerResult>(OnNotificationQuestionWithCustomAnswerMessageAsync);
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
                    default:
                        throw new NotSupportedException($"Message mode not supported ({message.Mode})");
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
            var selectedIndex = await Task.Run(() => ShowSimpleSelectionDialogAsync(message.Question, message.PossibleAnswers));
            if (selectedIndex < 0 || selectedIndex >= message.PossibleAnswers.SafeCount())
                return new NotificationQuestionCustomAnswerResult(null, selectedIndex);

            return new NotificationQuestionCustomAnswerResult(message.PossibleAnswers[selectedIndex], selectedIndex);
        }

        #endregion

        #region Busy Notification Handling

        /// <summary>
        /// Gets a value indicating whether subscribe busy notifications.
        /// </summary>
        /// <value>
        /// <c>true</c> if subscribe busy notifications; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SubscribeBusyNotifications => true;

        /// <summary>
        /// Gets the busy indicator.
        /// </summary>
        /// <value>
        /// The busy indicator.
        /// </value>
        protected View BusyIndicator { get; private set; }

        protected virtual View CreateBusyIndicatorView()
        {
            return new ProgressBar(this)
            {
                Indeterminate = true
            };
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
                SetBusyIndicatorVisibility(ViewModel.IsBusy);
            }
            else if (e.PropertyName == "BusyMessage")
            {
                SetBusyIndicatorMessage(ViewModel.BusyMessage);
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
                {
                    BusyIndicator.Visibility = ViewStates.Visible;
                    Window.AddFlags(WindowManagerFlags.NotTouchable);
                }
                else
                {
                    BusyIndicator.Visibility = ViewStates.Gone;
                    Window.ClearFlags(WindowManagerFlags.NotTouchable);
                }
            }
        }

        /// <summary>
        /// Sets the busy indicator message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected virtual void SetBusyIndicatorMessage(string message)
        {
            if (BusyIndicator != null && BusyIndicator.GetType() == typeof(ProgressBar))
            {
                //TODO: Implement a base progress indicator that supports text
            }
        }

        #endregion

        #region Generic Methods

        /// <summary>
        /// Gets the image resource identifier.
        /// </summary>
        /// <param name="imageId">The image identifier.</param>
        /// <returns></returns>
        protected abstract int GetResourceIdFromImageId(string imageId);


        protected virtual void GetAndSetBaseView()
        {
            SetContentView(GetBasePageNameResourceId());
        }

        protected virtual int GetBasePageNameResourceId()
        {
            var resName = "Page_" + GetType().Name;
            var field = GetPageViewFieldInfo(resName);
            if (field == null)
                throw new Resources.NotFoundException(resName);

            if (!(field.GetValue(null) is int resId))
                throw new NullReferenceException($"Id for resource '{resName}' not found");

            return resId;
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
        protected Intent CreateIntentFor(IMvxViewModel subViewModel, MvxViewModelRequest viewModelRequest)
        {
            var intentWithKey = Mvx.IoCProvider.Resolve<IMvxAndroidViewModelRequestTranslator>().GetIntentWithKeyFor(subViewModel, viewModelRequest);
            return intentWithKey.intent;
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
            var posBtnName = ViewModel.TextSourceCommon.GetText("Label_Ok");
            var negBtnName = ViewModel.TextSourceCommon.GetText("Label_Cancel");
            if (possibleAnswers == NotificationTwoWayAnswersGroupEnum.YesNo)
            {
                posBtnName = ViewModel.TextSourceCommon.GetText("Label_Yes");
                negBtnName = ViewModel.TextSourceCommon.GetText("Label_No");
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
                    return ViewModel.TextSourceCommon.GetText("Label_Dialog_Title_Error");

                case NotificationSeverityEnum.Success:
                    return ViewModel.TextSourceCommon.GetText("Label_Dialog_Title_Success");

                case NotificationSeverityEnum.Info:
                    return ViewModel.TextSourceCommon.GetText("Label_Dialog_Title_Information");

                case NotificationSeverityEnum.Warning:
                    return ViewModel.TextSourceCommon.GetText("Label_Dialog_Title_Warning");
            }

            return ViewModel.TextSourceCommon.GetText("Label_Dialog_Title_Message");
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
                    return AndroidResource.Drawable.IcDialogAlert;

                case NotificationSeverityEnum.Info:
                case NotificationSeverityEnum.Success:
                    return AndroidResource.Drawable.IcDialogInfo;
                default:
                    return AndroidResource.Drawable.IcDialogAlert;
            }
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
        /// It guarantees that the code executes in UI thread
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
        protected virtual AlertDialog.Builder CreateGenericQuestionDialog(string title, string message)
        {

            var builder = new AlertDialog.Builder(this);
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

        /// <summary>
        /// Creates the blocking simple selection dialog.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        protected virtual AlertDialog.Builder CreateSimpleSelectionDialog(string title)
        {
            var builder = new AlertDialog.Builder(this);

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
                var resetEvent = new AutoResetEvent(false);
                var selectedIndex = indexIfCancel;

                var bss = CreateSimpleSelectionDialog(title);
                if (bss != null)
                {
                    //Set available options list for the user
                    bss.SetItems(options.ToArray(), (sender, args) =>
                    {
                        selectedIndex = args.Which;
                        resetEvent.Set();
                    });

                    bss.SetOnCancelListener(new DialogCancelListener(resetEvent));

                    // ReSharper disable once AccessToModifiedClosure
                    // ReSharper disable once AccessToDisposedClosure
                    RunOnUiThread(() => bss.Show());

                    resetEvent.WaitOne();

                    bss.Dispose();
                    bss = null;
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
        protected virtual AlertDialog.Builder CreateMessageBox(string message, NotificationSeverityEnum severity)
        {
            var title = GetTitleFromSeverity(severity);
            var icon = GetIconFromSeverity(severity);

            var builder = new AlertDialog.Builder(this);
            builder.SetTitle(title);
            builder.SetIcon(icon);
            builder.SetMessage(message);

            return builder;

        }
        /// <summary>
        /// Shows the message box.
        /// </summary>
        /// <param name="message">The message to show.</param>
        /// <param name="severity">The severity of the message.</param>
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
                var resetEvent = new AutoResetEvent(false);

                var amb = CreateMessageBox(message, severity);
                if (amb != null)
                {
                    amb.SetPositiveButton(ViewModel.TextSourceCommon.GetText("Label_Ok"), (sender, e) => { resetEvent.Set(); });
                    RunOnUiThread(() => amb.Show());

                    resetEvent.WaitOne();
                }
            }
        }

        #endregion
    }
}
