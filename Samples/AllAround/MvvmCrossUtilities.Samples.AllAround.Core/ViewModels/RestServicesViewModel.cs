using System.Collections.ObjectModel;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using MvvmCrossUtilities.Samples.AllAround.Core.Rest;
using MvvmCrossUtilities.Samples.AllAround.Core.ViewModels.Base;

namespace MvvmCrossUtilities.Samples.AllAround.Core.ViewModels
{
    public class RestServicesViewModel : AllAroundViewModel
    {
        #region Properties

        public override string PageTitle
        {
            get { return "REST"; }
        }

        public ObservableCollection<MenuOption> ServicesList
        {
            get { return _servicesList; }
        }
        private readonly ObservableCollection<MenuOption> _servicesList = new ObservableCollection<MenuOption>();

        #endregion

        #region Command

        public ICommand NavigateCommand
        {
            get { return _navigateCommand; }
        }
        private readonly ICommand _navigateCommand;

        #endregion

        #region Constructor

        public RestServicesViewModel()
        {
            _navigateCommand = new MvxCommand<MenuOption>((o) => ShowViewModel(o.ViewModelType));

            ServicesList.Add(new MenuOption("Articles", typeof(ArticlesViewModel)));
        }

        #endregion
    }
}
