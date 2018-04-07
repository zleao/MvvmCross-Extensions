using Android.Views;
using Android.Widget;
using MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.DragSortListView.Enums;
using System;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.DragSortListView
{
    /// <summary>
    /// Class that starts and stops item drags on a {@link DragSortListView}
    /// based on touch gestures. This class also inherits from
    /// {@link SimpleFloatViewManager}, which provides basic float View
    /// creation.
    /// An instance of this class is meant to be passed to the methods
    /// {@link DragSortListView#setTouchListener()} and
    /// {@link DragSortListView#setFloatViewManager()} of your
    /// {@link DragSortListView} instance.
    /// </summary>
    public class AppCompatDragSortController : AppCompatSimpleFloatViewManager, View.IOnTouchListener, GestureDetector.IOnGestureListener
    {
        #region Constants

        public static int MISS = -1;

        #endregion

        #region Fields and Properties

        private AppCompatBaseDragSortListView _dslv;

        public DragInitModeEnum DragInitMode
        {
            get { return _dragInitMode; }
            set { _dragInitMode = value; }
        }
        private DragInitModeEnum _dragInitMode = DragInitModeEnum.OnDown;

        /// <summary>
        /// Enable/Disable list item sorting. Disabling is useful if only item
        /// removal is desired. Prevents drags in the vertical direction.
        /// </summary>
        public bool SortEnabled
        {
            get { return _sortEnabled; }
            set { _sortEnabled = value; }
        }
        private bool _sortEnabled = true;

        public RemoveModeEnum RemoveMode
        {
            get { return _removeMode; }
            set { _removeMode = value; }
        }
        private RemoveModeEnum _removeMode;

        /// <summary>
        /// Enable/Disable item removal without affecting remove mode.
        /// </summary>
        public bool RemoveEnabled
        {
            get { return _removeEnabled; }
            set { _removeEnabled = value; }
        }
        private bool _removeEnabled = false;

        private bool _isRemoving = false;

        private GestureDetector _detector;

        private GestureDetector _flingRemoveDetector;

        private int _touchSlop;

        private int _hitPos = MISS;
        private int _flingHitPos = MISS;

        private int _clickRemoveHitPos = MISS;

        private int[] _tempLoc = new int[2];

        private int _itemX;
        private int _itemY;

        private int _currX;
        private int _currY;

        private bool _dragging = false;

        private float _flingSpeed = 500f;

        /// <summary>
        /// Set the resource id for the View that represents the drag
        /// handle in a list item.
        /// </summary>
        public int DragHandleId
        {
            get { return _dragHandleId; }
            set { _dragHandleId = value; }
        }
        private int _dragHandleId;

        /// <summary>
        /// Set the resource id for the View that represents click
        /// removal button.
        /// </summary>
        public int ClickRemoveId
        {
            get { return _clickRemoveId; }
            set { _clickRemoveId = value; }
        }
        private int _clickRemoveId;

        /// <summary>
        /// Set the resource id for the View that represents the fling
        /// handle in a list item.
        /// </summary>
        public int FlingHandleId
        {
            get { return _flingHandleId; }
            set { _flingHandleId = value; }
        }
        private int _flingHandleId;

        private bool _canDrag;

        private int _positionX;

        #endregion

        #region Constructor

        /// <summary>
        /// Calls {@link #DragSortController(DragSortListView, int)} with a
        /// 0 drag handle id, FLING_RIGHT_REMOVE remove mode,
        /// and ON_DOWN drag init. By default, sorting is enabled, and
        /// removal is disabled.
        /// </summary>
        /// <param name="dslv">The DSLV instance</param>
        public AppCompatDragSortController(AppCompatBaseDragSortListView dslv)
            : this(dslv, 0, DragInitModeEnum.OnDown, RemoveModeEnum.FlingRemove)
        {
        }

        public AppCompatDragSortController(AppCompatBaseDragSortListView dslv, int dragHandleId, DragInitModeEnum dragInitMode, RemoveModeEnum removeMode)
            : this(dslv, dragHandleId, dragInitMode, removeMode, 0)
        {
        }

        public AppCompatDragSortController(AppCompatBaseDragSortListView dslv, int dragHandleId, DragInitModeEnum dragInitMode, RemoveModeEnum removeMode, int clickRemoveId)
            : this(dslv, dragHandleId, dragInitMode, removeMode, clickRemoveId, 0)
        {
        }

        /// <summary>
        /// By default, sorting is enabled, and removal is disabled.
        /// </summary>
        /// <param name="dslv">The DSLV instance</param>
        /// <param name="dragHandleId">The resource id of the View that represents the drag handle in a list item.</param>
        /// <param name="dragInitMode"></param>
        /// <param name="removeMode"></param>
        /// <param name="clickRemoveId"></param>
        /// <param name="flingHandleId"></param>
        public AppCompatDragSortController(AppCompatBaseDragSortListView dslv, int dragHandleId, DragInitModeEnum dragInitMode, RemoveModeEnum removeMode, int clickRemoveId, int flingHandleId)
            : base(dslv)
        {
            _dslv = dslv;
            _detector = new GestureDetector(dslv.Context, this);
            _flingRemoveDetector = new GestureDetector(dslv.Context, new FlingListener(this, _dslv));
            _flingRemoveDetector.IsLongpressEnabled = false;
            _touchSlop = ViewConfiguration.Get(dslv.Context).ScaledTouchSlop;
            _dragHandleId = dragHandleId;
            _clickRemoveId = clickRemoveId;
            _flingHandleId = flingHandleId;
            _removeMode = removeMode;
            _dragInitMode = dragInitMode;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets flags to restrict certain motions of the floating View
        /// based on DragSortController settings (such as remove mode).
        /// Starts the drag on the DragSortListView.
        /// </summary>
        /// <param name="position">The list item position (includes headers).</param>
        /// <param name="deltaX">Touch x-coord minus left edge of floating View.</param>
        /// <param name="deltaY">Touch y-coord minus top edge of floating View.</param>
        /// <returns>True if drag started, false otherwise.</returns>
        public bool StartDrag(int position, int deltaX, int deltaY)
        {

            int dragFlags = 0;
            if (_sortEnabled && !_isRemoving)
            {
                dragFlags |= _dslv.DRAG_POS_Y | _dslv.DRAG_NEG_Y;
            }
            if (_removeEnabled && _isRemoving)
            {
                dragFlags |= _dslv.DRAG_POS_X;
                dragFlags |= _dslv.DRAG_NEG_X;
            }

            _dragging = _dslv.StartDrag(position - _dslv.HeaderViewsCount, dragFlags, deltaX, deltaY);

            return _dragging;
        }

        /// <summary>
        /// Overrides to provide fading when slide removal is enabled.
        /// </summary>
        /// <param name="floatView"></param>
        /// <param name="position"></param>
        /// <param name="touch"></param>
        public override void OnDragFloatView(View floatView, Android.Graphics.Point position, Android.Graphics.Point touch)
        {
            if (_removeEnabled && _isRemoving)
            {
                _positionX = position.X;
            }
        }

        /// <summary>
        /// Get the position to start dragging based on the ACTION_DOWN
        /// MotionEvent. This function simply calls
        /// {@link #dragHandleHitPosition(MotionEvent)}. Override
        /// to change drag handle behavior;
        /// this function is called internally when an ACTION_DOWN
        /// event is detected. 
        /// </summary>
        /// <param name="ev">The ACTION_DOWN MotionEvent.</param>
        /// <returns>The list position to drag if a drag-init gesture is detected; MISS if unsuccessful.</returns>
        public int StartDragPosition(MotionEvent ev)
        {
            return DragHandleHitPosition(ev);
        }

        public int StartFlingPosition(MotionEvent ev)
        {
            return _removeMode == RemoveModeEnum.FlingRemove ? FlingHandleHitPosition(ev) : MISS;
        }

        /// <summary>
        /// Checks for the touch of an item's drag handle (specified by
        /// {@link #setDragHandleId(int)}), and returns that item's position
        /// if a drag handle touch was detected.
        /// </summary>
        /// <param name="ev">The ACTION_DOWN MotionEvent.</param>
        /// <returns>The list position of the item whose drag handle was touched; MISS if unsuccessful.</returns>
        public int DragHandleHitPosition(MotionEvent ev)
        {
            return ViewIdHitPosition(ev, _dragHandleId);
        }

        public int FlingHandleHitPosition(MotionEvent ev)
        {
            return ViewIdHitPosition(ev, _flingHandleId);
        }

        public int ViewIdHitPosition(MotionEvent ev, int id)
        {
            int x = (int)ev.GetX();
            int y = (int)ev.GetY();

            int touchPos = _dslv.PointToPosition(x, y); // includes headers/footers

            int numHeaders = _dslv.HeaderViewsCount;
            int numFooters = _dslv.FooterViewsCount;
            int count = _dslv.Count;

            // We're only interested if the touch was on an
            // item that's not a header or footer.
            if (touchPos != AdapterView.InvalidPosition &&
                touchPos >= numHeaders &&
                touchPos < (count - numFooters))
            {
                View item = _dslv.GetChildAt(touchPos - _dslv.FirstVisiblePosition);
                int rawX = (int)ev.RawX;
                int rawY = (int)ev.RawY;

                View dragBox = id == 0 ? item : (View)item.FindViewById(id);
                if (dragBox != null)
                {
                    dragBox.GetLocationOnScreen(_tempLoc);

                    if (rawX > _tempLoc[0] && rawY > _tempLoc[1] &&
                        rawX < _tempLoc[0] + dragBox.Width &&
                        rawY < _tempLoc[1] + dragBox.Height)
                    {
                        _itemX = item.Left;
                        _itemY = item.Top;

                        return touchPos;
                    }
                }
            }

            return MISS;
        }

        #endregion

        #region GestureDetector.IOnGestureListener implementation

        public bool OnDown(MotionEvent e)
        {
            if (_removeEnabled && _removeMode == RemoveModeEnum.ClickRemove)
            {
                _clickRemoveHitPos = ViewIdHitPosition(e, _clickRemoveId);
            }

            _hitPos = StartDragPosition(e);
            if (_hitPos != MISS && _dragInitMode == DragInitModeEnum.OnDown)
            {
                StartDrag(_hitPos, (int)e.GetX() - _itemX, (int)e.GetY() - _itemY);
            }

            _isRemoving = false;
            _canDrag = true;
            _positionX = 0;
            _flingHitPos = StartFlingPosition(e);

            return true;
        }

        public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            return false;
        }

        public void OnLongPress(MotionEvent e)
        {
            if (_hitPos != MISS && _dragInitMode == DragInitModeEnum.OnLongPress)
            {
                _dslv.PerformHapticFeedback(FeedbackConstants.LongPress);
                StartDrag(_hitPos, _currX - _itemX, _currY - _itemY);
            }
        }

        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            int x1 = (int)e1.GetX();
            int y1 = (int)e1.GetY();
            int x2 = (int)e2.GetX();
            int y2 = (int)e2.GetY();
            int deltaX = x2 - _itemX;
            int deltaY = y2 - _itemY;

            if (_canDrag && !_dragging && (_hitPos != MISS || _flingHitPos != MISS))
            {
                if (_hitPos != MISS)
                {
                    if (_dragInitMode == DragInitModeEnum.OnDrag && Math.Abs(y2 - y1) > _touchSlop && _sortEnabled)
                    {
                        StartDrag(_hitPos, deltaX, deltaY);
                    }
                    else if (_dragInitMode != DragInitModeEnum.OnDown && Math.Abs(x2 - x1) > _touchSlop && _removeEnabled)
                    {
                        _isRemoving = true;
                        StartDrag(_flingHitPos, deltaX, deltaY);
                    }
                }
                else if (_flingHitPos != MISS)
                {
                    if (Math.Abs(x2 - x1) > _touchSlop && _removeEnabled)
                    {
                        _isRemoving = true;
                        StartDrag(_flingHitPos, deltaX, deltaY);
                    }
                    else if (Math.Abs(y2 - y1) > _touchSlop)
                    {
                        _canDrag = false; // if started to scroll the list then
                        // don't allow sorting nor fling-removing
                    }
                }
            }
            return false;
        }

        public void OnShowPress(MotionEvent e)
        {
            //do nothing
        }

        public bool OnSingleTapUp(MotionEvent e)
        {
            if (_removeEnabled && _removeMode == RemoveModeEnum.ClickRemove)
            {
                if (_clickRemoveHitPos != MISS)
                {
                    _dslv.RemoveItem(_clickRemoveHitPos - _dslv.HeaderViewsCount);
                }
            }
            return true;
        }

        #endregion

        #region View.IOnTouchListener implementation

        public bool OnTouch(View v, MotionEvent e)
        {
            if (!_dslv.DragEnabled || _dslv.ListViewIntercepted)
            {
                return false;
            }

            _detector.OnTouchEvent(e);
            if (RemoveEnabled && _dragging && _removeMode == RemoveModeEnum.FlingRemove)
            {
                _flingRemoveDetector.OnTouchEvent(e);
            }

            MotionEventActions action = e.Action & MotionEventActions.Mask;
            switch (action)
            {
                case MotionEventActions.Down:
                    _currX = (int)e.GetX();
                    _currY = (int)e.GetY();
                    break;

                case MotionEventActions.Up:
                    if (_removeEnabled && _isRemoving)
                    {
                        int x = _positionX >= 0 ? _positionX : -_positionX;
                        int removePoint = _dslv.Width / 2;
                        if (x > removePoint)
                        {
                            _dslv.StopDragWithVelocity(true, 0);
                        }
                    }
                    _isRemoving = false;
                    _dragging = false;
                    break;

                case MotionEventActions.Cancel:
                    _isRemoving = false;
                    _dragging = false;
                    break;
            }

            return false;
        }

        #endregion


        internal class FlingListener : Java.Lang.Object, GestureDetector.IOnGestureListener
        {
            #region Fields

            private readonly AppCompatDragSortController _dsc;
            private readonly AppCompatBaseDragSortListView _dslv;

            #endregion

            #region Constructor

            public FlingListener(AppCompatDragSortController dsc, AppCompatBaseDragSortListView dslv)
            {
                _dsc = dsc;
                _dslv = dslv;
            }

            #endregion

            #region GestureDetector.IOnGestureListener implementation

            public bool OnDown(MotionEvent e)
            {
                throw new NotImplementedException();
            }

            public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
            {
                if (_dsc.RemoveEnabled && _dsc._isRemoving)
                {
                    int w = _dslv.Width;
                    int minPos = w / 5;
                    if (velocityX > _dsc._flingSpeed)
                    {
                        if (_dsc._positionX > -minPos)
                        {
                            _dslv.StopDragWithVelocity(true, velocityX);
                        }
                    }
                    else if (velocityX < -_dsc._flingSpeed)
                    {
                        if (_dsc._positionX < minPos)
                        {
                            _dslv.StopDragWithVelocity(true, velocityX);
                        }
                    }
                    _dsc._isRemoving = false;
                }
                return false;
            }

            public void OnLongPress(MotionEvent e)
            {
                throw new NotImplementedException();
            }

            public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
            {
                throw new NotImplementedException();
            }

            public void OnShowPress(MotionEvent e)
            {
                throw new NotImplementedException();
            }

            public bool OnSingleTapUp(MotionEvent e)
            {
                throw new NotImplementedException();
            }

            #endregion
        }
    }
}