using MvxExtensions.Libraries.Droid.Core.Support.V4.Components.Controls;
using MvxExtensions.Libraries.Droid.Core.Support.V4.Components.Targets;
using MvvmCross.Binding.Bindings.Target.Construction;

namespace MvxExtensions.Libraries.Droid.Core.Support.V4.Setup
{
    public static class SupportV4SetupHelper
    {
        public static void FillTargetFactories(IMvxTargetBindingFactoryRegistry registry)
        {
            registry.RegisterFactory(new MvxCustomBindingFactory<FragmentViewPager>("CurrentPageIndex",
                                                                                    fragmentViewPager => new FragmentViewPagerCurrentPageIndexTargetBinding(fragmentViewPager)));

            registry.RegisterFactory(new MvxCustomBindingFactory<FragmentViewPager>("CurrentPage",
                                                                                    fragmentViewPager => new FragmentViewPagerCurrentPageTargetBinding(fragmentViewPager)));
        }
    }
}