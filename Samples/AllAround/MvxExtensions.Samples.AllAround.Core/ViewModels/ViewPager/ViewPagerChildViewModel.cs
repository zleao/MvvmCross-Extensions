using MvvmCrossUtilities.Samples.AllAround.Core.ViewModels.Base;

namespace MvvmCrossUtilities.Samples.AllAround.Core.ViewModels.ViewPager
{
    public class ViewPagerChildViewModel : AllAroundChildViewModel<AllAroundViewModel>
    {
         #region Properties

        public override string PageTitle
        {
            get { return _pageTitle; }
        }
        private readonly string _pageTitle;

        public string Body
        {
            get { return _body; }
            private set
            {
                if (_body != value)
                {
                    _body = value;
                    RaisePropertyChanged(() => Body);
                }
            }
        }
        private string _body;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewPagerChildViewModel{AllAroundViewModel}" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="title">The title.</param>
        /// <param name="body">The body.</param>
        public ViewPagerChildViewModel(AllAroundViewModel parent, string title, string body)
            : base(parent)
        {
            _pageTitle = title;
            _body = body;
        }

        #endregion
    }
}
