using MvvmCross.Core.ViewModels;
using MvvmCross.Localization;
using MvvmCross.Platform.Platform;
using MvxExtensions.Libraries.Portable.Core.Services.Logger;
using MvxExtensions.Plugins.Notification;
using MvxExtensions.Samples.AllAround.Core.Models;
using MvxExtensions.Samples.AllAround.Core.Resources;
using MvxExtensions.Samples.AllAround.Core.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace MvxExtensions.Samples.AllAround.Core.ViewModels
{
    public class MainViewModel : SimpleMenuBaseViewModel
    {
        #region Properties

        public override string PageTitle
        {
            get { return "Choose option..."; }
        }

        /// <summary>
        /// Gets the menu options.
        /// </summary>
        /// <value>
        /// The menu options.
        /// </value>
        public ObservableCollection<MenuOption> MenuOptions
        {
            get { return _menuOptions; }
        }
        private readonly ObservableCollection<MenuOption> _menuOptions = new ObservableCollection<MenuOption>();

        #endregion

        #region Command

        public ICommand NavigateCommand
        {
            get { return _navigateCommand; }
        }
        private readonly ICommand _navigateCommand;

        #endregion

        #region Constructor

        public MainViewModel(IMvxLanguageBinder textSource,
                             IMvxJsonConverter jsonConverter,
                             INotificationService notificationManager,
                             ILoggerManager loggerManager)
            : base(textSource, jsonConverter, notificationManager, loggerManager)
        {
            _navigateCommand = new MvxCommand<MenuOption>(OnNavigation);

            //AddOptionIfImplemented(new MenuOption("Context options", typeof(ContextOptionsViewModel)));
            AddOptionIfImplemented(new MenuOption(TextSource.GetText(TextResourcesKeys.Label_Button_Color), typeof(ColorViewModel)));
            //AddOptionIfImplemented(new MenuOption("Custom EditText", typeof(CustomEditTextViewModel)));
            //AddOptionIfImplemented(new MenuOption("Device info", typeof(DeviceViewModel)));
            //AddOptionIfImplemented(new MenuOption("Dialog demos", typeof(DialogViewModel)));
            AddOptionIfImplemented(new MenuOption(TextSource.GetText(TextResourcesKeys.Label_Button_DragAndDropList), typeof(DragAndDropListViewModel)));
            AddOptionIfImplemented(new MenuOption(TextSource.GetText(TextResourcesKeys.Label_Button_List), typeof(ListViewModel)));
            AddOptionIfImplemented(new MenuOption(TextSource.GetText(TextResourcesKeys.Label_Button_LoggerManager), typeof(LoggerManagerViewModel)));
            AddOptionIfImplemented(new MenuOption(TextSource.GetText(TextResourcesKeys.Label_Button_Notifications), typeof(NotificationsViewModel)));
            //AddOptionIfImplemented(new MenuOption(TextSource.GetText(TextResourcesKeys.Label_Button_TreeViewList), typeof(TreeViewListViewModel)));
            //AddOptionIfImplemented(new MenuOption(TextSource.GetText(TextResourcesKeys.Label_Button_ViewPager), typeof(ViewPagerViewModel)));
        }

        private void OnNavigation(MenuOption option)
        {
            ShowViewModel(option.ViewModelType);
        }

        private void AddOptionIfImplemented(MenuOption option)
        {
            if (HasRegisteredViewFor(option.ViewModelType))
                MenuOptions.Add(option);
        }

        #endregion
    }
}
