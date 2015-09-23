using Cirrious.MvvmCross.Droid.Views;
using Cirrious.MvvmCross.ViewModels;

namespace MvvmCrossUtilities.Libraries.Droid.Views
{
    public class AndroidViewPresenter : MvxAndroidViewPresenter, IAndroidViewPresenter
    {
        public void Show(MvxViewModelRequest request, bool removeCurrentViewFromStack)
        {
            this.Show(request);

            if (removeCurrentViewFromStack)
            {
                if (Activity != null)
                    Activity.Finish();
            }
        }
    }
}