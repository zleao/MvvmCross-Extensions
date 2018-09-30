﻿using Android.App;
using System;
using Android.Runtime;
using MvxExtensions.Platforms.Droid.Support.V7;
using Playground.Core;

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

