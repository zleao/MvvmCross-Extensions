using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Java.Lang;
using MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.DragSortListView.Interfaces;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.DragSortListView
{
    /// <summary>
    /// Simple implementation of the FloatViewManager class. Uses list
    /// items as they appear in the ListView to create the floating View.
    /// </summary>
    public class AppCompatSimpleFloatViewManager : Object, IFloatViewManager
    {
        #region Fields

        private Bitmap _floatBitmap;

        private ImageView _imageView;

        private Color _floatBGColor = Color.Black;

        private ListViewCompat _listView;

        #endregion

        #region Constructor

        public AppCompatSimpleFloatViewManager(ListViewCompat lv)
        {
            _listView = lv;
        }

        #endregion

        #region Methods

        public void SetBackgroundColor(Color color)
        {
            _floatBGColor = color;
        }

        /// <summary>
        /// This simple implementation creates a Bitmap copy of the
        /// list item currently shown at ListView <code>position</code>.
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public virtual View OnCreateFloatView(int position)
        {
            // Guaranteed that this will not be null? I think so. Nope, got
            // a NullPointerException once...
            View v = _listView.GetChildAt(position + _listView.HeaderViewsCount - _listView.FirstVisiblePosition);

            if (v == null)
            {
                return null;
            }

            v.Pressed = false;

            // Create a copy of the drawing cache so that it does not get
            // recycled by the framework when the list tries to clean up memory
            //v.setDrawingCacheQuality(View.DRAWING_CACHE_QUALITY_HIGH);
            v.DrawingCacheEnabled = true;
            _floatBitmap = Bitmap.CreateBitmap(v.DrawingCache);
            v.DrawingCacheEnabled = false;

            if (_imageView == null)
            {
                _imageView = new ImageView(_listView.Context);
            }
            //_imageView.SetBackgroundColor(_floatBGColor);
            _imageView.SetPadding(0, 0, 0, 0);
            _imageView.SetImageBitmap(_floatBitmap);
            _imageView.LayoutParameters = new ViewGroup.LayoutParams(v.Width, v.Height);

            return _imageView;
        }


        /// <summary>
        /// This does nothing
        /// </summary>
        /// <param name="floatView"></param>
        /// <param name="position"></param>
        /// <param name="touch"></param>
        public virtual void OnDragFloatView(View floatView, Point position, Point touch)
        {
            // do nothing
        }

        /// <summary>
        /// Removes the Bitmap from the ImageView created in
        /// onCreateFloatView() and tells the system to recycle it.
        /// </summary>
        /// <param name="floatView"></param>
        public void OnDestroyFloatView(View floatView)
        {
            ((ImageView)floatView).SetImageDrawable(null);

            _floatBitmap.Recycle();
            _floatBitmap = null;
        }

        #endregion
    }
}