using Android.App;
using Android.Views;
using MvxExtensions.Libraries.Droid.Core.Views.SimpleMenu;
using MvxExtensions.Libraries.Portable.Core.ViewModels.SimpleMenu;
using System.Reflection;

namespace MvxExtensions.Samples.AllAround.Droid.Views.Base
{
    public abstract class SimpleMenuBaseView<TViewModel> : SimpleMenuActivityBase<TViewModel>
       where TViewModel : SimpleMenuViewModel
    {
        #region Lifecycle

        /// <summary>
        /// Called when view model set.
        /// </summary>
        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();

            RequestWindowFeature(WindowFeatures.ActionBar);

            if (ActionBar != null && TypedViewModel != null)
                ActionBar.Title = BaseContext.GetString(Resource.String.ApplicationName);
        }

        #endregion

        #region Methods

        protected override FieldInfo GetPageViewFieldInfo(string pageName)
        {
            return typeof(Resource.Layout).GetField(pageName);
        }

        protected override int GetResourceIdFromImageId(string imageId)
        {
            return Resources.GetIdentifier("ic_action_" + imageId, "drawable", ApplicationContext.PackageName);
        }

        #endregion
    }
}