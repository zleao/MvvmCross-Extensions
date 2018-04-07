using MvxExtensions.Libraries.Portable.Core.Views;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Views;

namespace MvxExtensions.Libraries.Droid.Core.Views
{
    public class AndroidViewDispatcher : MvxAndroidViewDispatcher, IViewDispatcher
    {
        private readonly IAndroidViewPresenter _viewPresenter;

        public AndroidViewDispatcher(IAndroidViewPresenter presenter)
            : base(presenter)
        {
            _viewPresenter = presenter;
        }

        public bool ShowViewModel(MvxViewModelRequest request, bool removeCurrentViewFromStack, bool clearBackStack)
        {
            return RequestMainThreadAction(() => _viewPresenter.Show(request, removeCurrentViewFromStack, clearBackStack));
        }
    }
}