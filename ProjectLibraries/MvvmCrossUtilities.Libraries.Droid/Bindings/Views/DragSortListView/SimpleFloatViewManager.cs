using Android.Graphics;
using Android.Views;
using Android.Widget;
using Java.Lang;
using MvvmCrossUtilities.Libraries.Droid.Bindings.Views.DragSortListView.Interfaces;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views.DragSortListView
{
    /// <summary>
    /// Simple implementation of the FloatViewManager class. Uses list
    /// items as they appear in the ListView to create the floating View.
    /// </summary>
    public class SimpleFloatViewManager : Object, IFloatViewManager
    {

        private Bitmap _floatBitmap;

        private ImageView _imageView;

        private Color _floatBGColor = Color.Black;

        private ListView _listView;

        public SimpleFloatViewManager(ListView lv)
        {
            _listView = lv;
        }

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
    }
}