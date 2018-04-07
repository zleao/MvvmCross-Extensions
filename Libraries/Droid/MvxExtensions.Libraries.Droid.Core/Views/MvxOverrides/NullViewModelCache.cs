using Android.OS;
using MvvmCross.Core.ViewModels;
using MvvmCross.Droid.Views;

namespace MvxExtensions.Libraries.Droid.Core.Views.MvxOverrides
{
    internal class NullViewModelCache : IMvxSingleViewModelCache
    {
        public void Cache(IMvxViewModel toCache, Bundle bundle)
        {
        }

        public IMvxViewModel GetAndClear(Bundle bundle)
        {
            return null;
        }
    }
}