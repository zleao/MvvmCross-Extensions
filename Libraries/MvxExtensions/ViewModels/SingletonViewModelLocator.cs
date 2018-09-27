using System;
using System.Collections.Concurrent;
using System.Reflection;
using MvvmCross.Navigation.EventArguments;
using MvvmCross.ViewModels;
using MvxExtensions.Attributes;

namespace MvxExtensions.ViewModels
{
    public class SingletonViewModelLocator : MvxDefaultViewModelLocator
    {
        private readonly Lazy<ConcurrentDictionary<Type, IMvxViewModel>> _lazyCachedViewModels;
        protected ConcurrentDictionary<Type, IMvxViewModel> CachedViewModels => _lazyCachedViewModels.Value;

        public SingletonViewModelLocator()
        {
            _lazyCachedViewModels = 
                new Lazy<ConcurrentDictionary<Type, IMvxViewModel>>(
                    () => new ConcurrentDictionary<Type, IMvxViewModel>());
        }

        public override IMvxViewModel Load(Type viewModelType, IMvxBundle parameterValues, IMvxBundle savedState, IMvxNavigateEventArgs navigationArgs)
        {
            var singletonAttr = viewModelType.GetTypeInfo().GetCustomAttribute<SingletonViewModelAttribute>(true);
            if (singletonAttr == null)
            {
                //USe base logic if the Singleton attribute is not present
                return base.Load(viewModelType, parameterValues, savedState, navigationArgs);
            }

            if (CachedViewModels.ContainsKey(viewModelType))
            {
                //run the reload logic, if the viewmodel already exists
                return Reload(CachedViewModels[viewModelType], parameterValues, savedState, navigationArgs);
            }

            //In case the viewmodel is not created yet, use base load logic and add it to the cache
            var loadedVm = base.Load(viewModelType, parameterValues, savedState, navigationArgs);
            CachedViewModels.AddOrUpdate(viewModelType, loadedVm, (type, model) => loadedVm);
            return loadedVm;
        }

        public override IMvxViewModel<TParameter> Load<TParameter>(Type viewModelType, TParameter param, IMvxBundle parameterValues, IMvxBundle savedState, IMvxNavigateEventArgs navigationArgs)
        {
            var singletonAttr = viewModelType.GetTypeInfo().GetCustomAttribute<SingletonViewModelAttribute>(true);
            if (singletonAttr == null)
            {
                //USe base logic if the Singleton attribute is not present
                return base.Load(viewModelType, param, parameterValues, savedState, navigationArgs);
            }

            if (CachedViewModels.ContainsKey(viewModelType))
            {
                var cachedVm = CachedViewModels[viewModelType] as IMvxViewModel<TParameter>;
                if (cachedVm != null)
                {
                    //run the reload logic, if the viewmodel already exists
                    return Reload(cachedVm, param, parameterValues, savedState, navigationArgs);
                }
            }

            //In case the viewmodel is not created yet, use base load logic and add it to the cache
            var loadedVm = base.Load(viewModelType, param, parameterValues, savedState, navigationArgs);
            CachedViewModels.AddOrUpdate(viewModelType, loadedVm, (type, model) => loadedVm);
            return loadedVm;
        }
    }
}
