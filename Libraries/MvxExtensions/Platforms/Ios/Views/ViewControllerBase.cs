using Foundation;
using GlobalToast;
using MvvmCross;
using MvvmCross.Platforms.Ios.Views;
using MvvmCross.WeakSubscription;
using MvxExtensions.Extensions;
using MvxExtensions.Platforms.iOS.Components.Controls;
using MvxExtensions.Platforms.iOS.Components.Interfaces;
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
using System.Threading;
using System.Threading.Tasks;
using UIKit;

namespace MvxExtensions.Platforms.iOS.Views
{
    public abstract class ViewControllerBase<TViewModel> : MvxViewController<TViewModel>
        where TViewModel : ViewModel
    {
        #region Fields

        private volatile IList<SubscriptionToken> _messageTokens = new List<SubscriptionToken>();
        private volatile MvxNotifyPropertyChangedEventSubscription _propertyChangedSubscription;
        private bool _eventsSubscribed;

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

        #endregion

        #region Constructor

        public ViewControllerBase()
        {
        }

        public ViewControllerBase(IntPtr handle)
            : base(handle)
        {
        }

        protected ViewControllerBase(string nibName, NSBundle bundle)
            : base(nibName, bundle)
        {
        }

        #endregion

        #region Lifecycle Methods

        public override void ViewDidLoad()
        {
            if (BusyIndicator == null)
                BusyIndicator = CreateBusyIndicatorView();

            base.ViewDidLoad();
        }

        public override void ViewWillAppear(bool animated)
        {
            if (!_eventsSubscribed)
            {
                SubscribeMessageEvents();
            }

            base.ViewWillAppear(animated);
        }

        public override void ViewWillDisappear(bool animated)
        {
            UnsubscribeMessageEvents();

            base.ViewWillDisappear(animated);
        }

        public override void ViewWillUnload()
        {
            if (BusyIndicator != null)
            {
                BusyIndicator = null;
            }

            base.ViewWillUnload();
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
        protected ILoadingIndicator BusyIndicator { get; private set; }

        protected virtual ILoadingIndicator CreateBusyIndicatorView()
        {
            return new LoadingOverlay(UIScreen.MainScreen.Bounds);
        }

        /// <summary>
        /// Handles the OnPropertyChanged event of the ViewModel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.IsBusy))
            {
                SetBusyIndicatorVisibility(ViewModel.IsBusy);
            }
            else if (e.PropertyName == nameof(ViewModel.BusyMessage))
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
                if (showBusyNotification)
                {
                    BusyIndicator.Show();
                }
                else
                {
                    BusyIndicator.Hide();
                }
            }
        }

        /// <summary>
        /// Sets the busy indicator message.
        /// </summary>
        /// <param name="message">The message.</param>
        protected virtual void SetBusyIndicatorMessage(string message)
        {
            if (BusyIndicator != null && BusyIndicator.GetType() == typeof(LoadingOverlay))
            {
                //TODO: Implement a base progress indicator that supports text
            }
        }

        #endregion
    
        #region Generic Methods

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

        /// <summary>
        /// Gets the name of the buttons concerning the possible answers.
        /// </summary>
        /// <param name="possibleAnswers">The possible answers.</param>
        /// <returns></returns>
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
        /// Calls CreateToast and show the toast.
        /// It garantees that the code executes in UI thread
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="toastLength">Length of time the toast will be shown.</param>
        protected virtual void ShowToast(string message)
        {
            if (Thread.CurrentThread.IsBackground)
            {
                InvokeOnMainThread(() => ShowToast(message));
            }
            else
            {
                Toast.ShowToast(message);
            }
        }

        /// <summary>
        /// Creates the generic question dialog.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        protected virtual UIAlertController CreateGenericQuestionDialog(string title, string message)
        {
            return UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
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

            var qd = CreateGenericQuestionDialog(title, message);
            if (qd != null)
            {
                //Add Actions
	            qd.AddAction(UIAlertAction.Create(positiveButtonName, UIAlertActionStyle.Default, alert => 
                { 
                    result = true;
                    resetEvent.Set();
                }));
	            
                qd.AddAction(UIAlertAction.Create(negativeButtonName, UIAlertActionStyle.Cancel, alert => resetEvent.Set()));

                InvokeOnMainThread(() => PresentViewController(qd, true, null));

                resetEvent.WaitOne();

                qd.Dispose();
                qd=null;
            }

            return result;
        }

        /// <summary>
        /// Creates the blocking simple selection dialog.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <returns></returns>
        protected virtual UIAlertController CreateSimpleSelectionDialog(string title)
        {
            return UIAlertController.Create(title, string.Empty, UIAlertControllerStyle.ActionSheet);
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
            if (!Thread.CurrentThread.IsBackground)
            {
                return await Task.Run(() => ShowSimpleSelectionDialogAsync(title, options, indexIfCancel));
            }
            else
            {
                var resetEvent = new AutoResetEvent(false);
                var selectedIndex = indexIfCancel;

                var ssd = CreateSimpleSelectionDialog(title);
                if (ssd != null)
                {
                    options.ToArray().SafeForEach(option => 
                    {
                        ssd.AddAction(UIAlertAction.Create(option, UIAlertActionStyle.Default, alert => 
                        { 
                            selectedIndex = options.SafeIndexOf(alert.Title);
                            resetEvent.Set();
                        }));
                    });

                    InvokeOnMainThread(() => PresentViewController(ssd, true, null));

                    resetEvent.WaitOne();

                    ssd.Dispose();
                    ssd = null;
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
        protected virtual UIAlertController CreateMessageBox(string message, NotificationSeverityEnum severity)
        {
            return UIAlertController.Create(GetTitleFromSeverity(severity), message, UIAlertControllerStyle.Alert);
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

                var mb = CreateMessageBox(message, severity);
                if (mb != null)
                {
                    mb.AddAction(UIAlertAction.Create(ViewModel.TextSourceCommon.GetText("Label_Ok"), UIAlertActionStyle.Default, alert => resetEvent.Set()));
                    
                    InvokeOnMainThread(() => PresentViewController(mb, true, null));

                    resetEvent.WaitOne();

                    mb.Dispose();
                    mb = null;
                }
            }
        }

        #endregion
    }
}
