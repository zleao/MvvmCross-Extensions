using System;
using Android.Runtime;
using MvvmCross.Core;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.ViewModels;
using MvxExtensions.Platforms.Android.Setup;

namespace MvxExtensions.Platforms.Android
{
    public abstract class AppCompatApplication<TMvxAndroidSetup, TApplication> : MvxAndroidApplication
        where TMvxAndroidSetup : AndroidAppCompatSetup<TApplication>, new()
        where TApplication : IMvxApplication, new()
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
