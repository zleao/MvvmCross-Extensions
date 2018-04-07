using MvvmCross.Core.ViewModels;
using MvvmCross.Localization;
using MvvmCross.Platform.Platform;
using MvxExtensions.Libraries.Portable.Core.Extensions;
using MvxExtensions.Libraries.Portable.Core.Messages.OneWay;
using MvxExtensions.Libraries.Portable.Core.Models;
using MvxExtensions.Plugins.Notification;
using MvxExtensions.Plugins.Notification.Core.Async.Subscriptions;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MvxExtensions.Libraries.Portable.Core.ViewModels.SimpleMenu
{
    /// <summary>
    /// Base class for the simple menu viewmodel implementation
    /// </summary>
    public abstract class SimpleMenuViewModel : ViewModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModel" /> class.
        /// </summary>
        /// <exception cref="System.NullReferenceException">
        /// IMvxJsonConverter
        /// or
        /// INotificationService
        /// or
        /// IMvxLanguageBinder
        /// </exception>
        public SimpleMenuViewModel(IMvxLanguageBinder textSource,
                                   IMvxJsonConverter jsonConverter,
                                   INotificationService notificationManager)
            : base (textSource, jsonConverter, notificationManager)
        {
            _processContextOptionCommand = new MvxCommand<string>(OnProcessContextOptionCommand);
        }

        #endregion

        #region ContextOptions Management

        /// <summary>
        /// Gets the context options.
        /// </summary>
        /// <value>
        /// The context options.
        /// </value>
        public virtual IDictionary<string, ContextOption> ContextOptions
        {
            get
            {
                return _contextOptions.SafeCount() > 0 ? _contextOptions : (_contextOptions = CreateContextOptions());
            }
        }
        private IDictionary<string, ContextOption> _contextOptions;

        /// <summary>
        /// Gets the process context option command.
        /// </summary>
        /// <value>
        /// The process context option command.
        /// </value>
        public ICommand ProcessContextOptionCommand
        {
            get { return _processContextOptionCommand; }
        }
        private readonly ICommand _processContextOptionCommand;


        /// <summary>
        /// Creates the context options.
        /// </summary>
        /// <returns></returns>
        protected virtual IDictionary<string, ContextOption> CreateContextOptions()
        {
            return new Dictionary<string, ContextOption>();
        }

        /// <summary>
        /// Called when a context option is selected .
        /// </summary>
        /// <param name="selectedOption">The selected option.</param>
        private async void OnProcessContextOptionCommand(string selectedOption)
        {
            await DoWorkAsync(() => ProcessContextOptionAsync(selectedOption), TextSource.GetText("Message_Busy_Processing"));
        }

        /// <summary>
        /// Processes the selected context option.
        /// </summary>
        /// <param name="selectedOption">The selected option.</param>
        /// <returns></returns>
        protected virtual Task ProcessContextOptionAsync(string selectedOption)
        {
            return Task.FromResult(true);
        }

        /// <summary>
        /// Updates the context options.
        /// </summary>
        public virtual void UpdateContextOptions()
        {
            ContextOptions.SafeForEach((id, co) => co.IsEnabled = !IsBusy);
        }

        /// <summary>
        /// Updates the contextoptions and publishes an one-way 'update menu' message.
        /// </summary>
        /// <param name="context">The context to be used in the notification.</param>
        protected virtual Task UpdateMenuAsync(string context = AsyncSubscription.DefaultContext)
        {
            UpdateContextOptions();

            return NotificationManager.PublishAsync(new NotificationUpdateSimpleMenuMessage(this), context);
        }

        #endregion

        #region Busy Notification Management

        /// <summary>
        /// Signals the IsBusy to indicate that a new work has started
        /// </summary>
        /// <param name="isSilent">if set to <c>true</c> the IsBusy will no be signaled.</param>
        protected override void StartWork(bool isSilent = false)
        {
            base.StartWork(isSilent);

            if (!isSilent)
                lock (ContextOptions) { UpdateContextOptions(); }
        }

        /// <summary>
        /// Signals the IsBusy to indicate that work is finished.
        /// </summary>
        /// <param name="isSilent">if set to <c>true</c> the IsBusy will no be signaled.</param>
        public override async void FinishWork(bool isSilent = false)
        {
            base.FinishWork(isSilent);

            if (!isSilent)
                await UpdateMenuAsync();
        }

        #endregion
    }
}
