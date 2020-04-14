using Android.App;
using Android.Runtime;
using MvxExtensions.Droid.Support.V7;
using Playground.Core;
using System;

namespace Playground.Droid
{
    [Application]
    public class MainApplication : AppCompatApplication<Setup, App>
    {
        public MainApplication(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }
    }
}

