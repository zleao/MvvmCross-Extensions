using System;
using UIKit;
using CoreGraphics;
using MvxExtensions.Platforms.iOS.Components.Interfaces;

namespace MvxExtensions.Platforms.iOS.Components.Controls
{
    public class LoadingOverlay : UIView, ILoadingIndicator
    {
        #region Fields

        float _alpha = 0.3f;
        UIActivityIndicatorView _activitySpinner;

        #endregion

        #region Methods

        public LoadingOverlay(CGRect frame, int offset = 0, UIActivityIndicatorViewStyle style = UIActivityIndicatorViewStyle.Gray) : base(frame)
        {
            // configurable bits
            BackgroundColor = UIColor.Clear;
            Alpha = _alpha;
            Hidden = false;
            AutoresizingMask = UIViewAutoresizing.All;
            nfloat centerX = Frame.Width / 2;
            nfloat centerY = Frame.Height / 2;

            // create the activity spinner, center it horizontall and put it 5 points above center x
            _activitySpinner = new UIActivityIndicatorView(style);
            _activitySpinner.Frame = new CGRect(centerX - (_activitySpinner.Frame.Width / 2),
                centerY - _activitySpinner.Frame.Height + 44 + offset,
                _activitySpinner.Frame.Width,
                _activitySpinner.Frame.Height);
            _activitySpinner.AutoresizingMask = UIViewAutoresizing.All;
            AddSubview(_activitySpinner);
        }

        public void Show(bool endEditing = true)
        {
            UIWindow window = UIApplication.SharedApplication.KeyWindow;
            if (endEditing)
            {
                window.EndEditing(true);
            }

            UIView.Animate(
                0.1, // duration
                () =>
                {
                    window.AddSubview(this);
                    _activitySpinner?.StartAnimating();
                    Alpha = _alpha;
                },
                () => { }
            );
        }

        public void Hide()
        {
            UIView.Animate(
                0.1, // duration
                () => { Alpha = 0; },
                () =>
                {
                    RemoveFromSuperview();
                    _activitySpinner?.StopAnimating();
                }
            );
        }

        #endregion
    }
}
