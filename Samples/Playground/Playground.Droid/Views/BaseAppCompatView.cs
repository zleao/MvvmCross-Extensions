using System.Reflection;
using MvxExtensions.Platforms.Droid.Support.V7.Views;
using MvxExtensions.ViewModels;

namespace Playground.Droid.Views
{
    public abstract class BaseAppCompatView<TViewModel> : AppCompatActivityBase<TViewModel>
        where TViewModel : ViewModel
    {
        protected override int GetResourceIdFromImageId(string imageId)
        {
            return Resources.GetIdentifier("ic_action_" + imageId, "drawable", ApplicationContext.PackageName);
        }

        protected override FieldInfo GetPageViewFieldInfo(string pageName)
        {
            return typeof(Resource.Layout).GetField(pageName);
        }
    }
}