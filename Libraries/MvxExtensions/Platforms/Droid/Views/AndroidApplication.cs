using Android.Runtime;
using MvvmCross.Core;
using MvvmCross.Platforms.Android.Views;
using MvvmCross.ViewModels;
using MvxExtensions.Platforms.Droid.Setup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvxExtensions.Platforms.Droid.Views
{
    public abstract class AndroidApplication : MvxAndroidApplication
    {
        public AndroidApplication()
        {
        }

        public AndroidApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
    }

    public abstract class AndroidApplication<TAndroidSetup, TApplication> : AndroidApplication
      where TAndroidSetup : AndroidSetup<TApplication>, new()
      where TApplication : class, IMvxApplication, new()
    {
        public AndroidApplication() : base()
        {
        }

        public AndroidApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        protected override void RegisterSetup()
        {
            this.RegisterSetupType<TAndroidSetup>();
        }
    }
}
