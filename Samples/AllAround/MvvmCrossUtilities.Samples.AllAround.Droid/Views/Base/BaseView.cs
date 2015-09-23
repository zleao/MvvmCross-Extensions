using System;
using Android.App;
using Android.Views;
using MvvmCrossUtilities.Libraries.Droid.Views;
using MvvmCrossUtilities.Libraries.Portable.ViewModels;

namespace MvvmCrossUtilities.Samples.AllAround.Droid.Views.Base
{
    public abstract class BaseView<TViewModel> : ActivityBase
       where TViewModel : ViewModel
    {
        #region Properties

        /// <summary>
        /// Gets my view model.
        /// </summary>
        /// <value>
        /// My view model.
        /// </value>
        protected TViewModel MyViewModel
        {
            get { return ViewModel as TViewModel; }
        }

        #endregion

        #region Lifecycle

        /// <summary>
        /// Called when view model set.
        /// </summary>
        protected override void OnViewModelSet()
        {
            base.OnViewModelSet();

            RequestWindowFeature(WindowFeatures.ActionBar);

            if (ActionBar != null && MyViewModel != null)
                ActionBar.Title = BaseContext.GetString(Resource.String.ApplicationName);

            GetAndSetContentView();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the view id and uses 'SetContentView' method
        /// to set the layout.
        /// </summary>
        /// <exception cref="Android.Content.Res.Resources.NotFoundException">Thrown if the view filename does not exist</exception>
        /// <exception cref="System.NullReferenceException">Thrown if the there's no id value for the view</exception>
        protected virtual void GetAndSetContentView()
        {
            try
            {
                var resName = "Page_" + this.GetType().Name;
                var field = typeof(Resource.Layout).GetField(resName);
                if (field == null)
                    throw new Android.Content.Res.Resources.NotFoundException(resName);

				var resId = typeof(Resource.Layout).GetField(resName).GetValue(null) as int?;
                if (resId == null)
                    throw new NullReferenceException(string.Format("Id for resource '{0}' not found", resName));

                SetContentView(resId.Value);
            }
            catch
            {
                throw;
            }
        }

        protected override int GetContextOptionResourceId(string imageId)
        {
			return Resources.GetIdentifier("ic_action_" + imageId, "drawable", ApplicationContext.PackageName);
        }

        #endregion
    }
}