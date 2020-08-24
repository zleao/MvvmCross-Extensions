using MvxExtensions.Platforms.Droid.Views;
using MvxExtensions.ViewModels;
using System.Reflection;

namespace Playground.Droid.Views
{
    public abstract class BaseAppCompatView<TViewModel> : ActivityBase<TViewModel>
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