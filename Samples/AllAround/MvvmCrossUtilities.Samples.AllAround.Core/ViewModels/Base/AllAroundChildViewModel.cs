using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Plugins.JsonLocalisation;
using MvvmCrossUtilities.Libraries.Portable.ViewModels;

namespace MvvmCrossUtilities.Samples.AllAround.Core.ViewModels.Base
{
    public abstract class AllAroundChildViewModel<TParentViewModel> : ChildViewModel<TParentViewModel>
        where TParentViewModel : AllAroundViewModel
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="AllAroundChildViewModel{TParentViewModel}"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        public AllAroundChildViewModel(TParentViewModel parent)
            : base(parent)
        {
        }

        #endregion

        #region ViewModel Implementation

        /// <summary>
        /// Changes the text source language.
        /// </summary>
        /// <param name="newLanguage">The new language.</param>
        /// <returns></returns>
        protected override bool ChangeTextSourceLanguage(string newLanguage)
        {
            try
            {
                var textBuilder = Mvx.Resolve<IMvxTextProviderBuilder>();

                textBuilder.LoadResources(newLanguage);

                return true;
            }
            catch (System.Exception ex)
            {
                MvxTrace.Trace("ChangeTextSourceLanguage could not update text provider. Message: {0}", ex.Message);
                return false;
            }
        }

        #endregion
    }
}
