using MvxExtensions.Libraries.Portable.Core.Attributes;
using MvvmCross.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MvxExtensions.Libraries.Portable.Core.ViewModels.Utilities
{
    /// <summary>
    /// ViewModel locator used to cache viewmodels marked with the <see cref="SingletonViewModelAttribute"/>
    /// </summary>
    public class SingletonViewModelLocator : MvxDefaultViewModelLocator
    {
        private volatile Dictionary<Type, IMvxViewModel> _cachedViewModels = new Dictionary<Type, IMvxViewModel>();

        /// <summary>
        /// Loads the specified view model type.
        /// </summary>
        /// <param name="viewModelType">Type of the view model.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <param name="savedState">State of the saved.</param>
        /// <returns></returns>
        public override IMvxViewModel Load(Type viewModelType, IMvxBundle parameterValues, IMvxBundle savedState)
        {
            IMvxViewModel candidateVM = null;

            var singletonAttr = viewModelType.GetTypeInfo().GetCustomAttribute<SingletonViewModelAttribute>(true);
            if (singletonAttr != null)
            {
                if (_cachedViewModels.ContainsKey(viewModelType))
                {
                    candidateVM = _cachedViewModels[viewModelType];

                    var myCandidateVM = candidateVM as ViewModel;
                    var callInitMethods = (myCandidateVM != null && myCandidateVM.IsViewKilled);

                    if (callInitMethods)
                        CallCustomInitMethods(candidateVM, parameterValues);

                    if (savedState != null)
                        CallReloadStateMethods(candidateVM, savedState);

                    if (callInitMethods)
                        myCandidateVM.Start();
                }
                else
                {
                    candidateVM = base.Load(viewModelType, parameterValues, savedState);
                    _cachedViewModels.Add(viewModelType, candidateVM);
                }
            }
            else
            {
                candidateVM = base.Load(viewModelType, parameterValues, savedState);
            }

            return candidateVM;
        }
    }
}
