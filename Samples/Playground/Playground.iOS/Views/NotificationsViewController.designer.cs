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
    [Register ("NotificationsViewController")]
    partial class NotificationsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton DelayedNotifBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton ErrorNotifBtn { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton QuestionNotifBtn { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (DelayedNotifBtn != null) {
                DelayedNotifBtn.Dispose ();
                DelayedNotifBtn = null;
            }

            if (ErrorNotifBtn != null) {
                ErrorNotifBtn.Dispose ();
                ErrorNotifBtn = null;
            }

            if (QuestionNotifBtn != null) {
                QuestionNotifBtn.Dispose ();
                QuestionNotifBtn = null;
            }
        }
    }
}