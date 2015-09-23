using Cirrious.MvvmCross.Droid.Views;
using Cirrious.MvvmCross.ViewModels;
using MvvmCrossUtilities.Libraries.Portable.ViewModels;

namespace MvvmCrossUtilities.Libraries.Droid.Views
{
    public class AndroidViewDispatcher : MvxAndroidViewDispatcher, IViewDispatcher
    {
        private readonly IAndroidViewPresenter _viewPresenter;

        public AndroidViewDispatcher(IAndroidViewPresenter presenter)
            : base(presenter)
        {
            _viewPresenter = presenter;
        }

        public bool ShowViewModel(MvxViewModelRequest request, bool removeCurrentViewFromStack)
        {
            return RequestMainThreadAction(() => _viewPresenter.Show(request, removeCurrentViewFromStack));
        }
    }
}