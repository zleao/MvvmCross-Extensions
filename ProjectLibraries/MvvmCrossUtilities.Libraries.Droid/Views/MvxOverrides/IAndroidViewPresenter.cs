using Cirrious.MvvmCross.Droid.Views;
using Cirrious.MvvmCross.ViewModels;

namespace MvvmCrossUtilities.Libraries.Droid.Views
{
    public interface IAndroidViewPresenter : IMvxAndroidViewPresenter
    {
        void Show(MvxViewModelRequest request, bool removeCurrentViewFromStack);
    }
}