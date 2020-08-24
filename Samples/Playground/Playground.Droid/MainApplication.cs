using Android.App;
using Android.Runtime;
using MvxExtensions.Platforms.Droid.Views;
using Playground.Core;
using System;

namespace Playground.Droid
{
    [Application]
    public class MainApplication : AndroidApplication<Setup, App>
    {
        public MainApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
    }
}

