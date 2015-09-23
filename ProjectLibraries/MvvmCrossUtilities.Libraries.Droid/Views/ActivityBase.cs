using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.CrossCore.WeakSubscription;
using Cirrious.MvvmCross.Droid.Views;
using Cirrious.MvvmCross.ViewModels;
using MvvmCrossUtilities.Libraries.Portable.Extensions;
using MvvmCrossUtilities.Libraries.Portable.Messages.OneWay;
using MvvmCrossUtilities.Libraries.Portable.Messages.TwoWay;
using MvvmCrossUtilities.Libraries.Portable.Models;
using MvvmCrossUtilities.Libraries.Portable.ViewModels;
using MvvmCrossUtilities.Plugins.Notification;
using MvvmCrossUtilities.Plugins.Notification.Core;
using MvvmCrossUtilities.Plugins.Notification.Core.Async.Subscriptions;
using MvvmCrossUtilities.Plugins.Notification.Messages;
using MvvmCrossUtilities.Plugins.Notification.Messages.Base;
using MvvmCrossUtilities.Plugins.Notification.Messages.OneWay;
using MvvmCrossUtilities.Plugins.Notification.Messages.TwoWay;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MvvmCrossUtilities.Libraries.Droid.Views
{
    public abstract class ActivityBase : MvxActivity
    {
        #region Fields

        private volatile IList<SubscriptionToken> _messageTokens = new List<SubscriptionToken>();
        private volatile IList<SubscriptionToken> _longRunningMessageTokens = new List<SubscriptionToken>();

        private MvxNotifyPropertyChangedEventSubscription _propertyChangedSubscription = null;

        private Dictionary<int, string> _contextOptionsMappings = new Dictionary<int, string>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the typed view model.
        /// </summary>
        /// <value>
        /// The typed view model.
        /// </value>
        protected ViewModel TypedViewModel
        {
            get { return ViewModel as ViewModel; }
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
            get { return this.BaseContext.GetSystemService(Context.ActivityService) as ActivityManager; }
        }

        #endregion

        #region Lifecycle Methods

        /// <summary>
        /// Called when create.
        /// </summary>
        /// <param name="bundle">The bundle.</param>
        protected override void OnCreate(Android.OS.Bundle bundle)
        {
            base.OnCreate(bundle);

			SubscribeLongRunningMessageEvents();

            if (BusyIndicator == null)
            {
                _busyIndicator = new ProgressDialog(this);
                _busyIndicator.SetCancelable(false);
                _busyIndicator.SetCanceledOnTouchOutside(false);
            }
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
            UnsubscribeLongRunningMessageEvents();

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
        public virtual bool SubscribeGenericMessages
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether subscribe long running generic messages.
        /// </summary>
        /// <value>
        /// <c>true</c> if subscribe long running generic messages; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SubscribeLongRunningGenericMessages
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether to subscribe generic blocking messages.
        /// </summary>
        /// <value>
        /// <c>true</c> if subscribe generic blocking messages; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SubscribeGenericBlockingMessages
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether to subscribe generic question messages.
        /// </summary>
        /// <value>
        /// <c>true</c> if subscribe generic question messages; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SubscribeGenericQuestionMessages
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether to subscribe question with custom answer messages.
        /// </summary>
        /// <value>
        /// <c>true</c> if subscribe question with custom answer messages; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SubscribeQuestionWithCustomAnswerMessages
        {
            get { return true; }

        }

        /// <summary>
        /// Gets a value indicating whether to subscribe update menu message.
        /// </summary>
        /// <value>
        /// <c>true</c> if subscribe update menu message; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SubscribeUpdateMenuMessage
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether to subscribe terminate application message.
        /// </summary>
        /// <value>
        /// <c>true</c> if [subscribe terminate application message]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool SubscribeTerminateApplicationMessage
        {
            get { return true; }
        }


        /// <summary>
        /// Publishes the specified message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message.</param>
        protected Task PublishAsync<TMessage>(TMessage message, string context = AsyncSubscription.DefaultContext) where TMessage : NotificationOneWayMessage
        {
            return NotificationManager.PublishAsync(message, context);
        }

        /// <summary>
        /// Subscribes the event
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="asyncDeliveryAction">The asynchronous delivery action.</param>
        /// <param name="context">The context.</param>
        protected void SubscribeEvent<TMessage>(Func<TMessage, Task> asyncDeliveryAction, string context = AsyncSubscription.DefaultContext) 
            where TMessage : NotificationMessage
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
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult
        {
            var token = NotificationManager.Subscribe<TMessage, TResult>(asyncDeliveryAction, context);
            _messageTokens.Add(token);
        }

        /// <summary>
        /// Subscribes the long running event on the UI thread.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="deliveryAction">The delivery action.</param>
        /// <param name="context">The context.</param>
        protected void SubscribeLongRunningEvent<TMessage>(Func<TMessage, Task> asyncDeliveryAction, string context = AsyncSubscription.DefaultContext) 
            where TMessage : NotificationOneWayMessage
        {
            var token = NotificationManager.Subscribe<TMessage>(asyncDeliveryAction, context);
            _longRunningMessageTokens.Add(token);
        }

        /// <summary>
        /// Subscribes the two way long running event on the UI thread.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="deliveryAction">The delivery action.</param>
        /// <param name="context">The context.</param>
        protected void SubscribeLongRunningEvent<TMessage, TResult>(Func<TMessage, Task<TResult>> asyncDeliveryAction, string context = AsyncSubscription.DefaultContext)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult
        {
            var token = NotificationManager.Subscribe<TMessage, TResult>(asyncDeliveryAction, context);
            _longRunningMessageTokens.Add(token);
        }


        /// <summary>
        /// Subscribes the message events.
        /// </summary>
        protected virtual void SubscribeMessageEvents()
        {
            if (SubscribeBusyNotifications && TypedViewModel != null)
            {
                _propertyChangedSubscription = TypedViewModel.WeakSubscribe(OnPropertyChanged);
                SetBusyNotification(TypedViewModel.IsBusy);
            }

            if (SubscribeGenericMessages)
                SubscribeEvent<NotificationGenericMessage>(OnNotificationGenericMessageAsync);

            if (SubscribeGenericBlockingMessages)
                SubscribeEvent<NotificationGenericBlockingMessage, NotificationResult>(OnNotificationGenericBlockingAsync);

            if (SubscribeGenericQuestionMessages)
                SubscribeEvent<NotificationGenericQuestionMessage, NotificationGenericQuestionResult>(OnNotificationGenericQuestionAsync);

            if (SubscribeQuestionWithCustomAnswerMessages)
                SubscribeEvent<NotificationQuestionWithCustomAnswerMessage, NotificationQuestionCustomAnswerResult>(OnNotificationQuestionWithCustomAnswerAsync);

            if (SubscribeUpdateMenuMessage)
                SubscribeEvent<NotificationUpdateMenuMessage>(OnNotificationUpdateMenuMessageAsync);

            if (SubscribeTerminateApplicationMessage)
                SubscribeEvent<NotificationTerminateApplicationMessage>(OnNotificationTerminateApplicationMessageAsync);
        }

        /// <summary>
        /// Subscribes the long running message events.
        /// </summary>
        protected virtual void SubscribeLongRunningMessageEvents()
        {
            if (SubscribeLongRunningGenericMessages && TypedViewModel != null)
                SubscribeLongRunningEvent<NotificationLongRunningGenericMessage>(OnNotificationLongRunningGenericMessageAsync, TypedViewModel.LongRunningMessageContext);
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
            SetBusyNotification(false);
        }

        /// <summary>
        /// Unsubscribes the long running message events.
        /// </summary>
        protected virtual void UnsubscribeLongRunningMessageEvents()
        {
            foreach (var item in _longRunningMessageTokens)
            {
                NotificationManager.Unsubscribe(item);
            }
            _longRunningMessageTokens.Clear();
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
                        await Task.Run(async () => await this.ShowMessageBoxAsync(message.Message, message.Severity));
                        break;

                    case NotificationModeEnum.Default:
                    case NotificationModeEnum.Toast:
                        ShowToast(message.Message);
                        break;
                }
            }
        }

        /// <summary>
        /// Called when notification long running generic message.
        /// </summary>
        /// <param name="message">The object.</param>
        private async Task OnNotificationLongRunningGenericMessageAsync(NotificationLongRunningGenericMessage message)
        {
            if (message == null)
                return;

            switch (message.Mode)
            {
                case NotificationModeEnum.MessageBox:
                    await ShowMessageBoxAsync(message.Message, message.Severity);
                    break;

                case NotificationModeEnum.Default:
                case NotificationModeEnum.Toast:
                    this.ShowToast(message.Message);
                    break;
            }
        }

        /// <summary>
        /// Called when notification blocking error.
        /// </summary>
        /// <param name="message">The obj.</param>
        protected virtual async Task<NotificationResult> OnNotificationGenericBlockingAsync(NotificationGenericBlockingMessage message)
        {
            await ShowGenericBlockingDialogAsync(message.Message, message.Severity,
                                                 TypedViewModel.TextSource.GetText("Label_Common_Ok"),
                                                 null);

            return new NotificationResult(NotificationTwoWayAnswersEnum.Ok);
        }

        /// <summary>
        /// Called when notification generic question.
        /// </summary>
        /// <param name="arg">The argument.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected virtual async Task<NotificationGenericQuestionResult> OnNotificationGenericQuestionAsync(NotificationGenericQuestionMessage message)
        {
            var posBtnName = TypedViewModel.TextSource.GetText("Label_Common_Ok");
            var negBtnName = TypedViewModel.TextSource.GetText("Label_Common_Cancel");
            if (message.PossibleAnswers == NotificationTwoWayAnswersGroupEnum.YesNo)
            {
                posBtnName = TypedViewModel.TextSource.GetText("Label_Common_Yes");
                negBtnName = TypedViewModel.TextSource.GetText("Label_Common_No");
            }

            var result = await ShowGenericQuestionDialogAsync("", message.Question, posBtnName, negBtnName);
            if (message.PossibleAnswers == NotificationTwoWayAnswersGroupEnum.YesNo)
                return new NotificationGenericQuestionResult(result ? NotificationTwoWayAnswersEnum.Yes : NotificationTwoWayAnswersEnum.No);
            else
                return new NotificationGenericQuestionResult(result ? NotificationTwoWayAnswersEnum.Ok : NotificationTwoWayAnswersEnum.Cancel);
        }

        /// <summary>
        /// Called when notification question width custom answer.
        /// </summary>
        /// <param name="message">The object.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected virtual NotificationQuestionCustomAnswerResult OnNotificationQuestionWithCustomAnswer(NotificationQuestionWithCustomAnswerMessage message)
        {
            var selectedIndex = this.ShowBlockingSimpleSelectionDialog(message.Question, message.PossibleAnswers);
            if (selectedIndex < 0 || selectedIndex >= message.PossibleAnswers.SafeCount())
                return new NotificationQuestionCustomAnswerResult(null, selectedIndex);

            return new NotificationQuestionCustomAnswerResult(message.PossibleAnswers[selectedIndex], selectedIndex);
        }

        /// <summary>
        /// Called when notification update menu message.
        /// </summary>
        /// <param name="message">The object.</param>
        protected Task OnNotificationUpdateMenuMessageAsync(NotificationUpdateMenuMessage message)
        {
            InvalidateOptionsMenu();

            return Task.FromResult(true);
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
        protected virtual async Task<NotificationQuestionCustomAnswerResult> OnNotificationQuestionWithCustomAnswerAsync(NotificationQuestionWithCustomAnswerMessage message)
        {
            var selectedIndex = await Task.Run<int>(() => this.ShowBlockingSimpleSelectionDialog(message.Question, message.PossibleAnswers));
            if (selectedIndex < 0 || selectedIndex >= message.PossibleAnswers.SafeCount())
                return new NotificationQuestionCustomAnswerResult(null, selectedIndex);

            return new NotificationQuestionCustomAnswerResult(message.PossibleAnswers[selectedIndex], selectedIndex);
        }

        #endregion

        #region Action Bar Handling

        /// <summary>
        /// Called when the options menu is created.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <returns></returns>
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            return InitializeMenuOptions(menu);
        }

        /// <summary>
        /// Prepare the Screen's standard options menu to be displayed.
        /// </summary>
        /// <param name="menu">The options menu as last shown or first initialized by
        /// onCreateOptionsMenu().</param>
        /// <returns>
        /// To be added.
        /// </returns>
        /// <since version="Added in API level 1" />
        ///   <altmember cref="M:Android.App.Activity.OnCreateOptionsMenu(Android.Views.IMenu)" />
        /// <remarks>
        ///   <para tool="javadoc-to-mdoc">Prepare the Screen's standard options menu to be displayed.  This is
        /// called right before the menu is shown, every time it is shown.  You can
        /// use this method to efficiently enable/disable items or otherwise
        /// dynamically modify the contents.
        ///   </para>
        ///   <para tool="javadoc-to-mdoc">The default implementation updates the system menu items based on the
        /// activity's state.  Deriving classes should always call through to the
        /// base class implementation.</para>
        ///   <para tool="javadoc-to-mdoc">
        ///   <format type="text/html">
        ///   <a href="http://developer.android.com/reference/android/app/Activity.html#onPrepareOptionsMenu(android.view.Menu)" target="_blank">[Android Documentation]</a>
        ///   </format>
        ///   </para>
        /// </remarks>
        public override bool OnPrepareOptionsMenu(IMenu menu)
        {
            menu.RemoveGroup(1);
            UpdateVariableMenuOptions(menu);

            return base.OnPrepareOptionsMenu(menu);
        }

        /// <summary>
        /// Initializes the menu items.
        /// </summary>
        /// <param name="menu">The menu.</param>
        /// <returns></returns>
        protected virtual bool InitializeMenuOptions(IMenu menu)
        {
            UpdateVariableMenuOptions(menu);

            return true;
        }

        /// <summary>
        /// Updates the variable menu options.
        /// </summary>
        /// <param name="menu">The menu.</param>
        protected virtual void UpdateVariableMenuOptions(IMenu menu)
        {
            menu.Clear();

            _contextOptionsMappings = new Dictionary<int, string>();

            int menuItemId = 0;
            foreach (var option in TypedViewModel.ContextOptions)
            {
                if (!option.Value.IsActive || !ValidateContextOption(option.Value))
                    continue;

                if (option.Value.VisibleIfPossible)
                {
                    var visibleMenuItem = menu.Add(0, menuItemId, Menu.First, option.Value.Text);
                    visibleMenuItem.SetShowAsAction(ShowAsAction.IfRoom);
                    visibleMenuItem.SetIcon(GetContextOptionResourceId(option.Value.ImageId));
                    visibleMenuItem.SetEnabled(option.Value.IsEnabled);
                }
                else
                {
                    var hiddenMenuItem = menu.Add(1, menuItemId, Menu.First, option.Value.Text);
                    hiddenMenuItem.SetShowAsAction(ShowAsAction.Never);
                    hiddenMenuItem.SetEnabled(option.Value.IsEnabled);
                }

                _contextOptionsMappings.Add(menuItemId, option.Key);
                menuItemId++;
            }
        }

        /// <summary>
        /// Gets the context option resource identifier.
        /// </summary>
        /// <param name="imageId">The image identifier.</param>
        /// <returns></returns>
        protected abstract int GetContextOptionResourceId(string imageId);

        /// <summary>
        /// Validates the context option.
        /// </summary>
        /// <param name="option">The option.</param>
        /// <returns></returns>
        protected virtual bool ValidateContextOption(ContextOption option)
        {
            return true;
        }

        /// <summary>
        /// This hook is called whenever an item in your options menu is selected.
        /// </summary>
        /// <param name="item">The menu item that was selected.</param>
        /// <returns>
        /// To be added.
        /// </returns>
        /// <since version="Added in API level 1" />
        ///   <altmember cref="M:Android.App.Activity.OnCreateOptionsMenu(Android.Views.IMenu)" />
        /// <remarks>
        ///   <para tool="javadoc-to-mdoc">This hook is called whenever an item in your options menu is selected.
        /// The default implementation simply returns false to have the normal
        /// processing happen (calling the item's Runnable or sending a message to
        /// its Handler as appropriate).  You can use this method for any items
        /// for which you would like to do processing without those other
        /// facilities.
        ///   </para>
        ///   <para tool="javadoc-to-mdoc">Derived classes should call through to the base class for it to
        /// perform the default menu handling.</para>
        ///   <para tool="javadoc-to-mdoc">
        ///   <format type="text/html">
        ///   <a href="http://developer.android.com/reference/android/app/Activity.html#onOptionsItemSelected(android.view.MenuItem)" target="_blank">[Android Documentation]</a>
        ///   </format>
        ///   </para>
        /// </remarks>
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            if (!_contextOptionsMappings.IsNullOrEmpty() && _contextOptionsMappings.ContainsKey(item.ItemId))
            {
                var contextOptionId = _contextOptionsMappings[item.ItemId];
                TypedViewModel.ProcessContextOptionCommand.Execute(contextOptionId);
            }

            return true;
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

        /// <summary>
        /// Handles the OnPropertyChanged event of the ViewModel.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsBusy" || e.PropertyName == "BusyMessage")
            {
                SetBusyNotification(TypedViewModel.IsBusy);
            }
        }

        /// <summary>
        /// Sets the busy notification.
        /// </summary>
        /// <param name="showBusyNotification">if set to <c>true</c> [show busy notification].</param>
        protected void SetBusyNotification(bool showBusyNotification)
        {
            if (BusyIndicator != null)
            {
                if (showBusyNotification && !this.IsFinishing)
                {
                    //Show progressdialog
                    BusyIndicator.SetMessage(TypedViewModel.BusyMessage);
                    BusyIndicator.Show();
                }
                else
                {
                    //hide progressdialog
                    BusyIndicator.Hide();
                    BusyIndicator.SetMessage(string.Empty);
                }
            }
        }

        #endregion

        #region Methods

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

                case NotificationSeverityEnum.Pending:
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
                case NotificationSeverityEnum.Pending:
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
        /// Creates the generic blocking dialog.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="severity">The severity.</param>
        /// <returns></returns>
        protected virtual AlertDialog.Builder CreateGenericBlockingDialog(string message, NotificationSeverityEnum severity)
        {
            var title = this.GetTitleFromSeverity(severity);
            var icon = this.GetIconFromSeverity(severity);

            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle(title);
            builder.SetIcon(icon);
            builder.SetMessage(message);

            return builder;
        }
        /// <summary>
        /// Show a dialog and wait until user interact.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="severity">The severity.</param>
        /// <param name="okButtonName">Name of the ok button.</param>
        /// <param name="okButtonAction">The ok button action.</param>
        /// <exception cref="System.NotSupportedException">ShowBlockingErrorMessage must be called in a background thread!</exception>
        protected virtual async Task ShowGenericBlockingDialogAsync(string message, NotificationSeverityEnum severity, string okButtonName, EventHandler<DialogClickEventArgs> okButtonAction)
        {
            if (!Thread.CurrentThread.IsBackground)
            {
                await Task.Run(() => ShowGenericBlockingDialogAsync(message, severity, okButtonName, okButtonAction));
            }
            else
            {
                AutoResetEvent resetEvent = new AutoResetEvent(false);

                var bd = CreateGenericBlockingDialog(message, severity);
                if (bd != null)
                {

                    bd.SetNegativeButton(okButtonName, (sender, e) =>
                    {
                        if (okButtonAction != null)
                            okButtonAction(sender, e);
                        resetEvent.Set();
                    });

                    RunOnUiThread(() => bd.Show());

                    resetEvent.WaitOne();
                }
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
            AlertDialog.Builder builder = new AlertDialog.Builder(this);
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
        protected virtual AlertDialog.Builder CreateBlockingSimpleSelectionDialog(string title)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this);

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
        protected virtual int ShowBlockingSimpleSelectionDialog(string title, IList<string> options, int indexIfCancel = -1)
        {
            if (!Thread.CurrentThread.IsBackground)
                throw new System.UnauthorizedAccessException("Method ShowSimpleSelectionDialogSync has to run in a background thread.");

            if (IsFinishing)
                throw new System.UnauthorizedAccessException("Method ShowSimpleSelectionDialogSync is beeing called in a view that is finishing (probably as allready been destroyed).");

            AutoResetEvent resetEvent = new AutoResetEvent(false);
            int selectedIndex = indexIfCancel;

            var bss = CreateBlockingSimpleSelectionDialog(title);
            if (bss != null)
            {
                //Set avilable options list for the user
                bss.SetItems(options.ToArray(), (sender, args) =>
                {
                    selectedIndex = args.Which;
                    resetEvent.Set();
                });

                bss.SetOnCancelListener(new DialogCancelListener(resetEvent));

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

            AlertDialog.Builder builder = new AlertDialog.Builder(this);
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