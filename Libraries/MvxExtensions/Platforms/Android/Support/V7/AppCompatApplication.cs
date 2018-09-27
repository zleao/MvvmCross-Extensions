using System;
using Android.Runtime;
using MvvmCross.Core;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.ViewModels;
using MvxExtensions.Platforms.Android.Support.V7.Setup;

namespace MvxExtensions.Platforms.Android.Support.V7
{
    public abstract class AppCompatApplication<TMvxAndroidSetup, TApplication> : MvxAndroidApplication
        where TMvxAndroidSetup : AndroidAppCompatSetup<TApplication>, new()
        where TApplication : class, IMvxApplication, new()
    {
        protected AppCompatApplication() : base()
        {
        }

        protected AppCompatApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        protected override void RegisterSetup()
        {
            this.RegisterSetupType<TMvxAndroidSetup>();
        }
    }
}
