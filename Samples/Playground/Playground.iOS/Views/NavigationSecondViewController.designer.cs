// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Playground.iOS
{
    [Register ("NavigationSecondViewController")]
    partial class NavigationSecondViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ButtonNavigateAndClearBackstack { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelNavigateAndClearBackstack { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelNavigationModeDescription { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ButtonNavigateAndClearBackstack != null) {
                ButtonNavigateAndClearBackstack.Dispose ();
                ButtonNavigateAndClearBackstack = null;
            }

            if (LabelNavigateAndClearBackstack != null) {
                LabelNavigateAndClearBackstack.Dispose ();
                LabelNavigateAndClearBackstack = null;
            }

            if (LabelNavigationModeDescription != null) {
                LabelNavigationModeDescription.Dispose ();
                LabelNavigationModeDescription = null;
            }
        }
    }
}