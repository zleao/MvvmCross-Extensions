using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Widget;
using MvvmCross.Platform;
using MvvmCross.Platform.Core;
using MvvmCross.Platform.Platform;

namespace MvxExtensions.Libraries.Droid.Core.Components.Controls
{
    public class ImageUrlView : ImageView
    {
        #region Properties

        public string ImageUrl
        {
            get
            {
                if (_imageHelper == null)
                    return null;
                return _imageHelper.ImageUrl;
            }
            set
            {
                if (_imageHelper == null)
                    return;
                _imageHelper.ImageUrl = value;
            }
        }

        public string DefaultImagePath
        {
            get { return _imageHelper.DefaultImagePath; }
            set { _imageHelper.DefaultImagePath = value; }
        }

        public string ErrorImagePath
        {
            get { return _imageHelper.ErrorImagePath; }
            set { _imageHelper.ErrorImagePath = value; }
        }

        #endregion

        #region Constructor

        private readonly IMvxImageHelper<Bitmap> _imageHelper;

        public ImageUrlView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            if (!Mvx.TryResolve(out _imageHelper))
            {
                MvxTrace.Error(
                    "No IMvxImageHelper registered - you must provide an image helper before you can use a ImageUrlView");
            }
            else
            {
                _imageHelper.ImageChanged += ImageHelperOnImageChanged;
            }
        }

        public ImageUrlView(Context context)
            : base(context)
        {
            if (!Mvx.TryResolve(out _imageHelper))
            {
                MvxTrace.Error(
                    "No IMvxImageHelper registered - you must provide an image helper before you can use a MvxImageView");
            }
            else
            {
                _imageHelper.ImageChanged += ImageHelperOnImageChanged;
            }
        }
        
        #endregion

        #region Methods

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_imageHelper != null)
                    _imageHelper.Dispose();
            }

            base.Dispose(disposing);
        }

        private void ImageHelperOnImageChanged(object sender, MvxValueEventArgs<Bitmap> mvxValueEventArgs)
        {
            if (mvxValueEventArgs.Value != null)
                SetImageBitmap(mvxValueEventArgs.Value);
        }
        
        #endregion
    }
}