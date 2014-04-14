using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Android.App;
using Android.Content;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.CrossCore.WeakSubscription;
using Cirrious.MvvmCross.Droid.Fragging;
using Cirrious.MvvmCross.Droid.Views;
using Cirrious.MvvmCross.ViewModels;
using MvvmCrossUtilities.Libraries.Portable.Messages;
using MvvmCrossUtilities.Libraries.Portable.Messages.TwoWay;
using MvvmCrossUtilities.Libraries.Portable.ViewModels;
using MvvmCrossUtilities.Plugins.Notification;
using MvvmCrossUtilities.Plugins.Notification.Messages;
using MvvmCrossUtilities.Plugins.Notification.Messages.Base;
using MvvmCrossUtilities.Plugins.Notification.Messages.OneWay;
using MvvmCrossUtilities.Plugins.Notification.Messages.TwoWay;
using MvvmCrossUtilities.Plugins.Notification.Subscriptions;

namespace MvvmCrossUtilities.Libraries.Droid.Views
{
    public abstract class FragmentActivity : MvxFragmentActivity
    {
        #region Fields

        private readonly IList<SubscriptionToken> _messageTokens = new List<SubscriptionToken>();
        private readonly IList<SubscriptionToken> _longRunningMessageTokens = new List<SubscriptionToken>();

        private MvxNotifyPropertyChangedEventSubscription _propertyChangedSubscription = null;

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
        /// Publishes the specified message.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="message">The message.</param>
        protected void Publish<TMessage>(TMessage message, string context = Subscription.DefaultContext) where TMessage : NotificationOneWayMessage
        {
            NotificationManager.Publish<TMessage>(message, context);
        }

        /// <summary>
        /// Subscribes the event on the main thread.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="deliveryAction">The delivery action.</param>
        protected void SubscribeEvent<TMessage>(Action<TMessage> deliveryAction, string context = Subscription.DefaultContext) where TMessage : NotificationOneWayMessage
        {
            var token = NotificationManager.SubscribeOnMainThread<TMessage>(deliveryAction, context);
            _messageTokens.Add(token);
        }

        /// <summary>
        /// Subscribes the event on background thread.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="deliveryAction">The delivery action.</param>
        /// <param name="context">The context.</param>
        protected void SubscribeEventOnBackground<TMessage>(Action<TMessage> deliveryAction, string context = Subscription.DefaultContext) where TMessage : NotificationOneWayMessage
        {
            var token = NotificationManager.SubscribeOnThreadPoolThread<TMessage>(deliveryAction, context);
            _messageTokens.Add(token);
        }

        /// <summary>
        /// Subscribes two way events
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="deliveryAction">The delivery action.</param>
        protected void SubscribeEvent<TMessage, TResult>(Func<TMessage, TResult> deliveryAction, string context = Subscription.DefaultContext)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult
        {
            var token = NotificationManager.SubscribeOnMainThread<TMessage, TResult>(deliveryAction, context);
            _messageTokens.Add(token);
        }

        /// <summary>
        /// Subscribes the two way event on background.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="deliveryAction">The delivery action.</param>
        /// <param name="context">The context.</param>
        protected void SubscribeEventOnBackground<TMessage, TResult>(Func<TMessage, TResult> deliveryAction, string context = Subscription.DefaultContext)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult
        {
            var token = NotificationManager.SubscribeOnThreadPoolThread<TMessage, TResult>(deliveryAction, context);
            _messageTokens.Add(token);
        }


        /// <summary>
        /// Subscribes the long running event on the UI thread.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="deliveryAction">The delivery action.</param>
        /// <param name="context">The context.</param>
        protected void SubscribeLongRunningEvent<TMessage>(Action<TMessage> deliveryAction, string context = Subscription.DefaultContext) where TMessage : NotificationOneWayMessage
        {
            var token = NotificationManager.SubscribeOnMainThread<TMessage>(deliveryAction, context);
            _longRunningMessageTokens.Add(token);
        }

        /// <summary>
        /// Subscribes the two way long running event on the UI thread.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="deliveryAction">The delivery action.</param>
        /// <param name="context">The context.</param>
        protected void SubscribeLongRunningEvent<TMessage, TResult>(Func<TMessage, TResult> deliveryAction, string context = Subscription.DefaultContext)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult
        {
            var token = NotificationManager.SubscribeOnMainThread<TMessage, TResult>(deliveryAction, context);
            _longRunningMessageTokens.Add(token);
        }

        /// <summary>
        /// Subscribes the two way long runnig event on background.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="deliveryAction">The delivery action.</param>
        /// <param name="context">The context.</param>
        protected void SubscribeLongRunnigEventOnBackground<TMessage, TResult>(Func<TMessage, TResult> deliveryAction, string context = Subscription.DefaultContext)
            where TMessage : NotificationTwoWayMessage
            where TResult : NotificationResult
        {
            var token = NotificationManager.SubscribeOnThreadPoolThread<TMessage, TResult>(deliveryAction, context);
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
                SubscribeEvent<NotificationGenericMessage>(OnNotificationGenericMessage);

            if (SubscribeGenericBlockingMessages)
                SubscribeEventOnBackground<NotificationGenericBlockingMessage, NotificationResult>(OnNotificationGenericBlocking);

            if (SubscribeGenericQuestionMessages)
                SubscribeEventOnBackground<NotificationGenericQuestionMessage, NotificationGenericQuestionResult>(OnNotificationGenericQuestion);

            if (SubscribeQuestionWithCustomAnswerMessages)
                SubscribeEventOnBackground<NotificationQuestionWithCustomAnswerMessage, NotificationQuestionCustomAnswerResult>(OnNotificationQuestionWidthCustomAnswer);

            if (SubscribeUpdateMenuMessage)
                SubscribeEvent<NotificationUpdateMenuMessage>(OnNotificationUpdateMenuMessage);
        }

        /// <summary>
        /// Subscribes the long running message events.
        /// </summary>
        protected virtual void SubscribeLongRunningMessageEvents()
        {
            if (SubscribeLongRunningGenericMessages && TypedViewModel != null)
                SubscribeLongRunningEvent<NotificationLongRunningGenericMessage>(OnNotificationLongRunningGenericMessage, TypedViewModel.LongRunningMessageContext);
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
        /// <param name="obj">The obj.</param>
        protected virtual void OnNotificationGenericMessage(NotificationGenericMessage obj)
        {
            if (obj == null)
                return;

            switch (obj.Mode)
            {
                case NotificationModeEnum.MessageBox:
                    var title = this.GetTitleFromSeverity(obj.Severity);
                    var icon = this.GetIconFromSeverity(obj.Severity);
                    this.ShowMessageBox(title, obj.Message, icon);
                    break;

                case NotificationModeEnum.Default:
                case NotificationModeEnum.Toast:
                    this.ShowToast(obj.Message);
                    break;
            }
        }

        /// <summary>
        /// Called when notification long running generic message.
        /// </summary>
        /// <param name="obj">The object.</param>
        private void OnNotificationLongRunningGenericMessage(NotificationLongRunningGenericMessage obj)
        {
            if (obj == null)
                return;

            switch (obj.Mode)
            {
                case NotificationModeEnum.MessageBox:
                    var title = this.GetTitleFromSeverity(obj.Severity);
                    var icon = this.GetIconFromSeverity(obj.Severity);
                    this.ShowMessageBox(title, obj.Message, icon);
                    break;

                case NotificationModeEnum.Default:
                case NotificationModeEnum.Toast:
                    this.ShowToast(obj.Message);
                    break;
            }
        }

        /// <summary>
        /// Called when notification blocking error.
        /// </summary>
        /// <param name="obj">The obj.</param>
        protected virtual NotificationResult OnNotificationGenericBlocking(NotificationGenericBlockingMessage obj)
        {
            var title = TypedViewModel.TextSource.GetText("Label_Dialog_Title_Message");

            switch (obj.Severity)
            {
                case NotificationSeverityEnum.Error:
                    title = TypedViewModel.TextSource.GetText("Label_Dialog_Title_Error");
                    break;

                case NotificationSeverityEnum.Info:
                    title = TypedViewModel.TextSource.GetText("Label_Dialog_Title_Information");
                    break;

                case NotificationSeverityEnum.Warning:
                    title = TypedViewModel.TextSource.GetText("Label_Dialog_Title_Warning");
                    break;
            }

            this.ShowGenericBlockingDialog(title,
                                           obj.Message,
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
        protected virtual NotificationGenericQuestionResult OnNotificationGenericQuestion(NotificationGenericQuestionMessage obj)
        {
            var posBtnName = TypedViewModel.TextSource.GetText("Label_Common_Ok");
            var negBtnName = TypedViewModel.TextSource.GetText("Label_Common_Cancel");
            if (obj.PossibleAnswers == NotificationTwoWayAnswersGroupEnum.YesNo)
            {
                posBtnName = TypedViewModel.TextSource.GetText("Label_Common_Yes");
                negBtnName = TypedViewModel.TextSource.GetText("Label_Common_No");
            }

            var result = this.ShowGenericQuestionDialog("", obj.Question, posBtnName, negBtnName);
            if (obj.PossibleAnswers == NotificationTwoWayAnswersGroupEnum.YesNo)
                return new NotificationGenericQuestionResult(result ? NotificationTwoWayAnswersEnum.Yes : NotificationTwoWayAnswersEnum.No);
            else
                return new NotificationGenericQuestionResult(result ? NotificationTwoWayAnswersEnum.Ok : NotificationTwoWayAnswersEnum.Cancel);
        }

        /// <summary>
        /// Called when notification question width custom answer.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        protected virtual NotificationQuestionCustomAnswerResult OnNotificationQuestionWidthCustomAnswer(NotificationQuestionWithCustomAnswerMessage obj)
        {
            var selectedIndex = this.ShowBlockingSimpleSelectionDialog(obj.Question, obj.PossibleAnswers);
            return new NotificationQuestionCustomAnswerResult(obj.PossibleAnswers[selectedIndex], selectedIndex);
        }

        /// <summary>
        /// Called when notification update menu message.
        /// </summary>
        /// <param name="obj">The object.</param>
        protected void OnNotificationUpdateMenuMessage(NotificationUpdateMenuMessage obj)
        {
            InvalidateOptionsMenu();
        }

        #endregion

        #region Busy Notification Handling

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

        #endregion
    }
}