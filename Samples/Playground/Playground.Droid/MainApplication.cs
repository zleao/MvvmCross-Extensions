using Android.App;
using Android.Widget;
using Android.OS;
using MvvmCross.Droid.Support.V7.AppCompat;
using System;
using Android.Runtime;
using Playground.Core;

namespace Playground.Droid
{
    [Application]
    public class MainApplication : MvxAppCompatApplication<Setup, App>
    {
        public MainApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
    }
}

