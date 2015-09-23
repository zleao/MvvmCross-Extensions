using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using MvvmCrossUtilities.Samples.AllAround.Core.Resources;
using MvvmCrossUtilities.Samples.AllAround.Core.ViewModels.Base;

namespace MvvmCrossUtilities.Samples.AllAround.Core.ViewModels
{
    public sealed class MenuOption
    {
        #region Properties

        public string Text
        {
            get { return _text; }
        }
        private readonly string _text;

        public Type ViewModelType
        {
            get { return _viewModelType; }
        }
        private readonly Type _viewModelType;

        #endregion

        #region Constructor

        public MenuOption(string text, Type viewModelType)
        {
            _text = text;
            _viewModelType = viewModelType;
        }

        #endregion
    }

    public class MainViewModel : AllAroundViewModel
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

        public MainViewModel()
        {
            _navigateCommand = new MvxCommand<MenuOption>(OnNavigation); //new MvxCommand<MenuOption>((o) => ShowViewModel(o.ViewModelType));

            AddOptionIfImplemented(new MenuOption("Context options", typeof(ContextOptionsViewModel)));
            AddOptionIfImplemented(new MenuOption("Contrast converter", typeof(ColorViewModel)));
            AddOptionIfImplemented(new MenuOption("Custom EditText", typeof(CustomEditTextViewModel)));
            AddOptionIfImplemented(new MenuOption("Device info", typeof(DeviceViewModel)));
            AddOptionIfImplemented(new MenuOption("Dialog demos", typeof(DialogViewModel)));
            AddOptionIfImplemented(new MenuOption("DragAndDrop List", typeof(DragAndDropListViewModel)));
            AddOptionIfImplemented(new MenuOption(TextSource.GetText(TextResourcesKeys.Label_Button_List), typeof(ListViewModel)));
            AddOptionIfImplemented(new MenuOption("Logger", typeof(LoggerViewModel)));
            AddOptionIfImplemented(new MenuOption("Notifications", typeof(NotificationsViewModel)));
            AddOptionIfImplemented(new MenuOption(TextSource.GetText(TextResourcesKeys.Label_Button_TreeViewList), typeof(TreeViewListViewModel)));
            AddOptionIfImplemented(new MenuOption(TextSource.GetText(TextResourcesKeys.Label_Button_ViewPager), typeof(ViewPagerViewModel)));
        }

        private void OnNavigation(MenuOption option)
        {
            if (option.ViewModelType == typeof(ContextOptionsViewModel))
                ShowViewModel(option.ViewModelType, new { mainViewModelContext = ViewModelContext });
            else
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
