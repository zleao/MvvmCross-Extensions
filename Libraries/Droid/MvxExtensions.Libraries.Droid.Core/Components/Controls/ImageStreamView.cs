using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Widget;
using System;
using System.IO;

namespace MvxExtensions.Libraries.Droid.Core.Components.Controls
{
    public class ImageStreamView : ImageView
    {
        #region Properties

        public MemoryStream ImageStream
        {
            get { return _imageStream; }
            set
            {
                if (_imageStream != value)
                {
                    _imageStream = value;
                    UpdateImageStream();
                }
            }
        }
        public MemoryStream _imageStream; 
        
        #endregion

        #region Constructor

        public ImageStreamView(Context context)
            : base(context)
        {
        }

        protected ImageStreamView(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public ImageStreamView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        #endregion

        #region Methods

        private void UpdateImageStream()
        {
            if (ImageStream != null)
            {
                byte[] byteArray = ImageStream.ToArray();
                Bitmap bmp = BitmapFactory.DecodeByteArray(byteArray, 0, byteArray.Length);

                this.SetImageBitmap(bmp);
            }
            else
            {
                this.SetImageResource(0);
            }
        } 
        
        #endregion
    }
}