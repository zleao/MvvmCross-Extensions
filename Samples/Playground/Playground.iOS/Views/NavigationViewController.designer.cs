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
    [Register ("NavigationViewController")]
    partial class NavigationViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ButtonNavigate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ButtonNavigateAndRemoveSelf { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelNavigate { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel LabelNavigateAndRemoveSelf { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ButtonNavigate != null) {
                ButtonNavigate.Dispose ();
                ButtonNavigate = null;
            }

            if (ButtonNavigateAndRemoveSelf != null) {
                ButtonNavigateAndRemoveSelf.Dispose ();
                ButtonNavigateAndRemoveSelf = null;
            }

            if (LabelNavigate != null) {
                LabelNavigate.Dispose ();
                LabelNavigate = null;
            }

            if (LabelNavigateAndRemoveSelf != null) {
                LabelNavigateAndRemoveSelf.Dispose ();
                LabelNavigateAndRemoveSelf = null;
            }
        }
    }
}