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
            _navigateCommand = new MvxCommand<MenuOption>((o) => ShowViewModel(o.ViewModelType));

            MenuOptions.Add(new MenuOption("Contrast converter", typeof(ColorViewModel)));
            MenuOptions.Add(new MenuOption("Device info", typeof(DeviceViewModel)));
            MenuOptions.Add(new MenuOption("Dialog demos", typeof(DialogViewModel)));
            MenuOptions.Add(new MenuOption(TextSource.GetText(TextResourcesKeys.Label_Button_List), typeof(ListViewModel)));
            MenuOptions.Add(new MenuOption("Logger", typeof(LoggerViewModel)));
            MenuOptions.Add(new MenuOption("Rest", typeof(RestServicesViewModel)));
            MenuOptions.Add(new MenuOption(TextSource.GetText(TextResourcesKeys.Label_Button_TreeViewList), typeof(TreeViewListViewModel)));
            MenuOptions.Add(new MenuOption(TextSource.GetText(TextResourcesKeys.Label_Button_ViewPager), typeof(ViewPagerViewModel)));
            
            //MenuOptions.Add(new MenuOption(TextSource.GetText(TextResourcesKeys.Label_Button_ExpandableList), typeof(ExpandableListViewModel)));
            
        }

        #endregion
    }
}
