using System.Collections.ObjectModel;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using MvvmCrossUtilities.Samples.AllAround.Core.ViewModels.Base;
using MvvmCrossUtilities.Samples.AllAround.Core.ViewModels.ViewPager;

namespace MvvmCrossUtilities.Samples.AllAround.Core.ViewModels
{
    public class ViewPagerViewModel : AllAroundViewModel
    {
        #region Properties

        public bool BlockSwipe
        {
            get { return _blockSwipe; }
            set
            {
                if (_blockSwipe != value)
                {
                    _blockSwipe = value;
                    RaisePropertyChanged(() => BlockSwipe);
                }
            }
        }
        private bool _blockSwipe;
        
        public int CurrentPageIndex
        {
            get { return _currentPageIndex; }
            set
            {
                if (_currentPageIndex != value)
                {
                    _currentPageIndex = value;
                    RaisePropertyChanged(() => CurrentPageIndex);
                }
            }
        }
        private int _currentPageIndex;

        public string PageSelectedText
        {
            get { return _pageSelectedText; }
            set
            {
                if (_pageSelectedText != value)
                {
                    _pageSelectedText = value;
                    RaisePropertyChanged(() => PageSelectedText);
                }
            }
        }
        private string _pageSelectedText;
        
        public ObservableCollection<ViewPagerChildViewModel> ChildPages 
        {
            get { return _childPages; }
        }
        private ObservableCollection<ViewPagerChildViewModel> _childPages = new ObservableCollection<ViewPagerChildViewModel>();

        #endregion

        #region Commands

        public ICommand PageSelectedCommand
        {
            get { return _pageSelectedCommand; }
        }
        private readonly ICommand _pageSelectedCommand;

        #endregion

        #region Constructor

        public ViewPagerViewModel()
        {
            _pageSelectedCommand = new MvxCommand<int>(OnPageSelected);

            CurrentPageIndex = -1;

            for (int i = 1; i < 6; i++)
            {
                _childPages.Add(new ViewPagerChildViewModel(this, "Page " + i, "Body " + i));
            }
        }

        #endregion

        #region Methods

        private void OnPageSelected(int pageIndex)
        {
            PageSelectedText = "Page selected " + (pageIndex + 1);
        }

        #endregion
    }
}
