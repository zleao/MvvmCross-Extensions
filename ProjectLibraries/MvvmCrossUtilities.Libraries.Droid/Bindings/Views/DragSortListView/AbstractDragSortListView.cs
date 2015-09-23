using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Binding;
using Cirrious.MvvmCross.Binding.Droid.ResourceHelpers;
using MvvmCrossUtilities.Libraries.Droid.Bindings.Views.DragSortListView.Enums;
using MvvmCrossUtilities.Libraries.Droid.Bindings.Views.DragSortListView.Interfaces;
using MvvmCrossUtilities.Libraries.Droid.Bindings.Views.DragSortListView.Runnables;
using System;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views.DragSortListView
{
    public abstract class AbstractDragSortListView : ListView
    {
        #region Constants

        // Drag flag bit. 
        // Floating View can move in the positive x direction.
        public readonly static int DRAG_POS_X = 0x1;

        // Drag flag bit. Floating View can move in the negative
        // x direction.
        public readonly static int DRAG_NEG_X = 0x2;

        // Drag flag bit. Floating View can move in the positive
        // y direction. This is subtle. What this actually means is
        // that, if enabled, the floating View can be dragged below its starting
        // position. Remove in favor of upper-bounding item position?
        public readonly static int DRAG_POS_Y = 0x4;

        // Drag flag bit. Floating View can move in the negative
        // y direction. This is subtle. What this actually means is
        // that the floating View can be dragged above its starting
        // position. Remove in favor of lower-bounding item position?
        public readonly static int DRAG_NEG_Y = 0x8;

        #endregion

        #region Default Values

        protected virtual int DEFAULT_ANIMATION_DURATION_MILISECONDS { get { return 150; } }
        protected virtual int DEFAULT_ITEM_HEIGHT_COLLAPSED { get { return 1; } }
        protected virtual int DEFAULT_CACHE_SIZE { get { return 3; } }

        protected virtual float DEFAULT_FLOAT_ALPHA { get { return 0.6f; } }
        protected virtual float DEFAULT_SLIDE_REGION_FRAC { get { return 0.25f; } }
        protected virtual float DEFAULT_SLIDE_SHUFFLE_SPEED { get { return 0.75f; } }
        protected virtual float DEFAULT_DRAG_UP_SCROLL_START_FRAC { get { return 1.0f / 3.0f; } }
        protected virtual float DEFAULT_DRAG_DOWN_SCROLL_START_FRAC { get { return 1.0f / 3.0f; } }
        protected virtual float DEFAULT_MAX_SCROLL_SPEED { get { return 0.5f; } }
        protected virtual float DEFAULT_SLIDE_FRAC { get { return 0.0f; } }
        protected virtual float DEFAULT_REMOVE_VELOCITY_X { get { return 0; } }

        protected virtual bool DEFAULT_DRAG_ENABLED { get { return true; } }
        protected virtual bool DEFAULT_TRACK_DRAG_SORT { get { return false; } }
        protected virtual bool DEFAULT_USE_DEFAULT_CONTROLLER { get { return true; } }
        protected virtual bool DEFAULT_BLOCK_LAYOUT_REQUESTS { get { return false; } }


        //DragSortController default values
        protected virtual bool DEFAULT_CONTROLLER_REMOVE_ENABLED { get { return false; } }
        protected virtual RemoveModeEnum DEFAULT_CONTROLLER_REMOVE_MODE { get { return RemoveModeEnum.FlingRemove; } }
        protected virtual bool DEFAULT_CONTROLLER_SORT_ENABLED { get { return true; } }
        protected virtual DragInitModeEnum DEFAULT_CONTROLLER_DRAG_INIT_MODE { get { return DragInitModeEnum.OnDown; } }
        protected virtual int DEFAULT_CONTROLLER_DRAG_HANDLE_ID { get { return 0; } }
        protected virtual int DEFAULT_CONTROLLER_FLING_HANDLE_ID { get { return 0; } }
        protected virtual int DEFAULT_CONTROLLER_CLICK_REMOVE_ID { get { return 0; } }
        protected virtual Color DEFAULT_CONTROLLER_BACKGROUND_COLOR { get { return Color.Black; } }

        #endregion

        #region Fields And Properties

        private int _removeAnimDuration; // ms
        private int _dropAnimDuration; // ms

        /// <summary>
        /// The View that floats above the ListView and represents the dragged item.
        /// </summary>
        private View _floatView;

        /// <summary>
        /// The float View location. First based on touch location
        /// and given deltaX and deltaY. Then restricted by callback
        /// to FloatViewManager.onDragFloatView(). Finally restricted
        /// by bounds of DSLV.
        /// </summary>
        public Point FloatLoc
        {
            get { return _floatLoc; }
        }
        private Point _floatLoc = new Point();

        private Point _touchLoc = new Point();

        /// <summary>
        /// The middle (in the y-direction) of the floating View.
        /// </summary>
        public int FloatViewMid
        {
            get { return _floatViewMid; }
        }
        private int _floatViewMid;

        /// <summary>
        /// Flag to make sure float View isn't measured twice
        /// </summary>
        private bool _floatViewOnMeasured = false;

        /// <summary>
        /// Transparency for the floating View (XML attribute).
        /// </summary>
        private float _floatAlpha;

        /// <summary>
        /// Usually called from a FloatViewManager. The float alpha
        /// will be reset to the xml-defined value every time a drag
        /// is stopped.
        /// </summary>
        public float CurrFloatAlpha
        {
            get { return _currFloatAlpha; }
            set { _currFloatAlpha = value; }
        }
        private float _currFloatAlpha;

        /// <summary>
        /// While drag-sorting, the current position of the floating
        /// View. If dropped, the dragged item will land in this position.
        /// </summary>
        public int FloatPos
        {
            get { return _floatPos; }
        }
        private int _floatPos;

        /// <summary>
        /// The first expanded ListView position that helps represent
        /// the drop slot tracking the floating View.
        /// </summary>
        public int FirstExpPos
        {
            get { return _firstExpPos; }
        }
        private int _firstExpPos;

        /// <summary>
        /// The second expanded ListView position that helps represent
        /// the drop slot tracking the floating View. This can equal
        /// _firstExpPos if there is no slide shuffle occurring; otherwise
        /// it is equal to _firstExpPos + 1.
        /// </summary>
        public int SecondExpPos
        {
            get { return _secondExpPos; }
        }
        private int _secondExpPos;

        /// <summary>
        /// Flag set if slide shuffling is enabled.
        /// </summary>
        private bool _animate;

        /// <summary>
        /// The user dragged from this position.
        /// </summary>
        public int SrcPos
        {
            get { return _srcPos; }
        }
        private int _srcPos;

        /// <summary>
        /// Offset (in x) within the dragged item at which the user
        /// picked it up (or first touched down with the digitalis).
        /// </summary>
        private int _dragDeltaX;

        /// <summary>
        /// Offset (in y) within the dragged item at which the user
        /// picked it up (or first touched down with the digitalis).
        /// </summary>
        public int DragDeltaY
        {
            get { return _dragDeltaY; }
            set { _dragDeltaY = value; }
        }
        private int _dragDeltaY;

        /// <summary>
        /// The difference (in x) between screen coordinates and coordinates in this view.
        /// </summary>
        private int _offsetX;

        /// <summary>
        /// The difference (in y) between screen coordinates and coordinates in this view.
        /// </summary>
        private int _offsetY;

        /// <summary>
        /// A listener that receives callbacks whenever the floating View
        /// hovers over a new position.
        /// </summary>
        public IDragListener DragListener
        {
            get { return _dragListener; }
            set { _dragListener = value; }
        }
        private IDragListener _dragListener;

        /// <summary>
        /// A listener that receives a callback when the floating View is dropped.
        /// This better reorder your ListAdapter! DragSortListView does not do this
        /// for you; doesn't make sense to. Make sure
        /// {@link BaseAdapter#notifyDataSetChanged()} or something like it is called
        /// in your implementation. Furthermore, if you have a choiceMode other than
        /// none and the ListAdapter does not return true for
        /// {@link ListAdapter#hasStableIds()}, you will need to call
        /// {@link #moveCheckState(int, int)} to move the check boxes along with the
        /// list items.
        /// </summary>
        public IDropListener DropListener
        {
            get { return _dropListener; }
            set { _dropListener = value; }
        }
        private IDropListener _dropListener;

        /// <summary>
        /// A listener that receives a callback when the floating View
        /// (or more precisely the originally dragged item) is removed
        /// by one of the provided gestures.
        /// </summary>
        public IRemoveListener RemoveListener
        {
            get { return _removeListener; }
            set { _removeListener = value; }
        }
        private IRemoveListener _removeListener;

        /// <summary>
        /// Enable/Disable item dragging
        /// Allows for easy toggling between a DragSortListView
        /// and a regular old ListView. If enabled, items are
        /// draggable, where the drag init mode determines how
        /// items are lifted (see {@link setDragInitMode(int)}).
        /// If disabled, items cannot be dragged.
        /// </summary>
        public bool DragEnabled
        {
            get { return _dragEnabled; }
            set { _dragEnabled = value; }
        }
        private bool _dragEnabled;

        /// <summary>
        /// Current drag state
        /// </summary>
        public DragStateEnum DragState
        {
            get { return _dragState; }
            set { _dragState = value; }
        }
        private DragStateEnum _dragState = DragStateEnum.Idle;

        /// <summary>
        /// Height in pixels to which the originally dragged item
        /// is collapsed during a drag-sort. Currently, this value
        /// must be greater than zero.
        /// </summary>
        public int ItemHeightCollapsed
        {
            get { return _itemHeightCollapsed; }
        }
        private int _itemHeightCollapsed;

        /// <summary>
        /// Height of the floating View. Stored for the purpose of
        /// providing the tracking drop slot.
        /// </summary>
        public int FloatViewHeight
        {
            get { return _floatViewHeight; }
        }
        private int _floatViewHeight;

        /// <summary>
        /// Convenience member. See above.
        /// </summary>
        public int FloatViewHeightHalf
        {
            get { return _floatViewHeightHalf; }
        }
        private int _floatViewHeightHalf;

        /// <summary>
        /// Save the given width spec for use in measuring children
        /// </summary>
        private int _widthMeasureSpec = 0;

        /// <summary>
        /// Sample Views ultimately used for calculating the height
        /// of ListView items that are off-screen.
        /// </summary>
        private View[] _sampleViewTypes = new View[1];

        /// <summary>
        /// Drag-scroll encapsulator!
        /// </summary>
        private DragScroller _dragScroller;

        /// <summary>
        /// Determines the start of the upward drag-scroll region
        /// at the top of the ListView. Specified by a fraction
        /// of the ListView height, thus screen resolution agnostic.
        /// </summary>
        private float _dragUpScrollStartFrac;

        /// <summary>
        /// Determines the start of the downward drag-scroll region
        /// at the bottom of the ListView. Specified by a fraction
        /// of the ListView height, thus screen resolution agnostic.
        /// </summary>
        private float _dragDownScrollStartFrac;

        /// <summary>
        /// The following are calculated from the above fracs.
        /// </summary>
        private int _upScrollStartY;
        private int _downScrollStartY;
        public float DownScrollStartYF
        {
            get { return _downScrollStartYF; }
        }
        private float _downScrollStartYF;
        public float UpScrollStartYF
        {
            get { return _upScrollStartYF; }
        }
        private float _upScrollStartYF;

        /// <summary>
        /// Calculated from above above and current ListView height.
        /// </summary>
        public float DragUpScrollHeight
        {
            get { return _dragUpScrollHeight; }
        }
        private float _dragUpScrollHeight;

        /// <summary>
        /// Calculated from above above and current ListView height.
        /// </summary>
        public float DragDownScrollHeight
        {
            get { return _dragDownScrollHeight; }
        }
        private float _dragDownScrollHeight;

        /// <summary>
        /// Maximum drag-scroll speed in pixels per ms. Only used with
        /// default linear drag-scroll profile.
        /// </summary>
        public float MaxScrollSpeed
        {
            get { return _maxScrollSpeed; }
            set { _maxScrollSpeed = value; }
        }
        private float _maxScrollSpeed;

        /// <summary>
        /// Defines the scroll speed during a drag-scroll.
        /// </summary>
        public IDragScrollProfile DragScrollProfile
        {
            get { return _scrollProfile; }
        }
        private IDragScrollProfile _scrollProfile;

        /// <summary>
        /// Current touch x.
        /// </summary>
        private int _touchX;

        /// <summary>
        /// Current touch y.
        /// </summary>
        public int TouchY
        {
            get { return _touchY; }
        }
        private int _touchY;

        /// <summary>
        /// Last touch x.
        /// </summary>
        private int _lastX;

        /// <summary>
        /// Last touch y.
        /// </summary>
        private int _lastY;

        /// <summary>
        /// The touch y-coord at which drag started
        /// </summary>
        private int _dragStartY;

        /// <summary>
        /// Flags that determine limits on the motion of the
        /// floating View. See flags above.
        /// </summary>
        private int _dragFlags = 0;

        /// <summary>
        /// Last call to an on*TouchEvent was a call to
        /// onInterceptTouchEvent.
        /// </summary>
        private bool _lastCallWasIntercept = false;

        /// <summary>
        /// A touch event is in progress.
        /// </summary>
        private bool _inTouchEvent = false;

        /// <summary>
        /// floating view manager.
        /// </summary>
        public IFloatViewManager FloatViewManager
        {
            get { return _floatViewManager; }
            set { _floatViewManager = value; }
        }
        private IFloatViewManager _floatViewManager = null;

        /// <summary>
        /// Given to ListView to cancel its action when a drag-sort begins.
        /// </summary>
        private MotionEvent _cancelEvent;

        /// <summary>
        /// Where to cancel the ListView action when a drag-sort begins
        /// </summary>
        private CancelMethodEnum _cancelMethod = CancelMethodEnum.NoCancel;

        /// <summary>
        /// Determines when a slide shuffle animation starts. That is,
        /// defines how close to the edge of the drop slot the floating
        /// View must be to initiate the slide.
        /// </summary>
        private float _slideRegionFrac;

        /// <summary>
        /// Number between 0 and 1 indicating the relative location of
        /// a sliding item (only used if drag-sort animations
        /// are turned on). Nearly 1 means the item is 
        /// at the top of the slide region (nearly full blank item
        /// is directly below).
        /// </summary>
        private float _slideFrac;

        /// <summary>
        /// Turn on custom debugger.
        /// </summary>
        private bool _trackDragSort;

        /// <summary>
        /// Debugging class
        /// </summary>
        private DragSortTracker _dragSortTracker = null;

        /// <summary>
        /// Needed for adjusting item heights from within layoutChildren
        /// </summary>
        public bool BlockLayoutRequests
        {
            get { return _blockLayoutRequests; }
            set { _blockLayoutRequests = value; }
        }
        private bool _blockLayoutRequests;

        /// <summary>
        /// Set to true when a down event happens during drag sort;
        /// for example, when drag finish animations are
        /// playing.
        /// </summary>
        private bool _ignoreTouchEvent = false;

        /// <summary>
        /// Caches DragSortItemView child heights. Sometimes DSLV has to
        /// know the height of an offscreen item. Since ListView virtualizes
        /// these, DSLV must get the item from the ListAdapter to obtain
        /// its height. That process can be expensive, but often the same
        /// offscreen item will be requested many times in a row. Once an
        /// offscreen item height is calculated, we cache it in this guy.
        /// Actually, we cache the height of the child of the
        /// DragSortItemView since the item height changes often during a
        /// drag-sort.
        /// </summary>
        private HeightCache _childHeightCache;

        private RemoveAnimator _removeAnimator;

		private LiftAnimator _liftAnimator = null;

        private DropAnimator _dropAnimator;

        public bool UseRemoveVelocity
        {
            get { return _useRemoveVelocity; }
        }
        private bool _useRemoveVelocity;

        public float RemoveVelocityX
        {
            get { return _removeVelocityX; }
            set { _removeVelocityX = value; }
        }
        private float _removeVelocityX;

        public bool ListViewIntercepted
        {
            get { return _listViewIntercepted; }
        }
        private bool _listViewIntercepted = false;

        #endregion

        #region Constructor

        public AbstractDragSortListView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            _dragUpScrollStartFrac = DEFAULT_DRAG_UP_SCROLL_START_FRAC;
            _dragDownScrollStartFrac = DEFAULT_DRAG_DOWN_SCROLL_START_FRAC;
            _slideFrac = DEFAULT_SLIDE_FRAC;
            _blockLayoutRequests = DEFAULT_BLOCK_LAYOUT_REQUESTS;
            _childHeightCache = new HeightCache(DEFAULT_CACHE_SIZE);
            _removeVelocityX = DEFAULT_REMOVE_VELOCITY_X;
            _scrollProfile = new LinearDragScrollProfile(this);

            ParseAttributes(context, attrs);

            _dragScroller = new DragScroller(this);

            float smoothness = 0.5f;
            if (_removeAnimDuration > 0)
            {
                _removeAnimator = new RemoveAnimator(this, smoothness, _removeAnimDuration);
            }
            if (_dropAnimDuration > 0)
            {
                _dropAnimator = new DropAnimator(this, smoothness, _dropAnimDuration);
            }

            _cancelEvent = MotionEvent.Obtain(0, 0, MotionEventActions.Cancel, 0f, 0f, 0f, 0f, 0, 0f, 0f, 0, 0);

            

            //// construct the dataset observer
            //mObserver = new DataSetObserver() {
            //    private void cancel() {
            //        if (_dragState == DRAGGING) {
            //            cancelDrag();
            //        }
            //    }

            //    @Override
            //    public void onChanged() {
            //        cancel();
            //    }

            //    @Override
            //    public void onInvalidated() {
            //        cancel();
            //    }
            //};
        }

        #endregion

        #region Methods

        private object SafeGetFieldValue(Type styleable, string fieldName)
        {
            return SafeGetFieldValue(styleable, fieldName, 0);
        }

        private object SafeGetFieldValue(Type styleable, string fieldName, object defaultValue)
        {
            var field = styleable.GetField(fieldName);
            if (field == null)
            {
                MvxBindingTrace.Trace(MvxTraceLevel.Error, "Missing stylable field {0}", fieldName);
                return defaultValue;
            }

            return field.GetValue(null);
        }

        private void ParseAttributes(Context context, IAttributeSet attrs)
        {
            if (attrs == null)
                throw new ArgumentNullException("attrs");
            
            var finder = Mvx.Resolve<IMvxAppResourceTypeFinder>();
            var resourceType = finder.Find();
            var styleable = resourceType.GetNestedType("Styleable");

            var dragSortListViewAttrsId = (int[])SafeGetFieldValue(styleable, "BindableDragSortListView", new int[0]);
            TypedArray a = context.ObtainStyledAttributes(attrs, dragSortListViewAttrsId, 0, 0);

            _trackDragSort = a.GetBoolean((int)SafeGetFieldValue(styleable, "BindableDragSortListView_track_drag_sort", 0), DEFAULT_TRACK_DRAG_SORT);
            if (_trackDragSort)
                _dragSortTracker = new DragSortTracker();

            _itemHeightCollapsed = Math.Max(1, a.GetDimensionPixelSize((int)SafeGetFieldValue(styleable, "BindableDragSortListView_collapsed_height" ,0), DEFAULT_ITEM_HEIGHT_COLLAPSED));

            // alpha between 0 and 255, 0=transparent, 255=opaque
            _floatAlpha = a.GetFloat((int)SafeGetFieldValue(styleable, "BindableDragSortListView_float_alpha" ,0), DEFAULT_FLOAT_ALPHA);
            _currFloatAlpha = _floatAlpha;

            _dragEnabled = a.GetBoolean((int)SafeGetFieldValue(styleable, "BindableDragSortListView_drag_enabled" ,0), DEFAULT_DRAG_ENABLED);

            _slideRegionFrac = Math.Max(0.0f, Math.Min(1.0f, 1.0f - a.GetFloat((int)SafeGetFieldValue(styleable, "BindableDragSortListView_slide_shuffle_speed" ,0), DEFAULT_SLIDE_SHUFFLE_SPEED)));
            _animate = _slideRegionFrac > 0.0f;

            SetDragScrollStart(a.GetFloat((int)SafeGetFieldValue(styleable, "BindableDragSortListView_drag_scroll_start" ,0), DEFAULT_DRAG_UP_SCROLL_START_FRAC));

            _maxScrollSpeed = a.GetFloat((int)SafeGetFieldValue(styleable, "BindableDragSortListView_max_drag_scroll_speed" ,0), DEFAULT_MAX_SCROLL_SPEED);

            _removeAnimDuration = a.GetInt((int)SafeGetFieldValue(styleable, "BindableDragSortListView_remove_animation_duration" ,0), DEFAULT_ANIMATION_DURATION_MILISECONDS);

            _dropAnimDuration = a.GetInt((int)SafeGetFieldValue(styleable, "BindableDragSortListView_drop_animation_duration" ,0), DEFAULT_ANIMATION_DURATION_MILISECONDS);

            bool useDefault = a.GetBoolean((int)SafeGetFieldValue(styleable, "BindableDragSortListView_use_default_controller" ,0), DEFAULT_USE_DEFAULT_CONTROLLER);
            if (useDefault)
            {
                bool removeEnabled = a.GetBoolean((int)SafeGetFieldValue(styleable, "BindableDragSortListView_remove_enabled" ,0), DEFAULT_CONTROLLER_REMOVE_ENABLED);
                RemoveModeEnum removeMode = (RemoveModeEnum)a.GetInt((int)SafeGetFieldValue(styleable, "BindableDragSortListView_remove_mode" ,0), (int)DEFAULT_CONTROLLER_REMOVE_MODE);
                bool sortEnabled = a.GetBoolean((int)SafeGetFieldValue(styleable, "BindableDragSortListView_sort_enabled" ,0), DEFAULT_CONTROLLER_SORT_ENABLED);
                DragInitModeEnum dragInitMode = (DragInitModeEnum)a.GetInt((int)SafeGetFieldValue(styleable, "BindableDragSortListView_drag_start_mode" ,0), (int)DEFAULT_CONTROLLER_DRAG_INIT_MODE);
                int dragHandleId = a.GetResourceId((int)SafeGetFieldValue(styleable, "BindableDragSortListView_drag_handle_id" ,0), DEFAULT_CONTROLLER_DRAG_HANDLE_ID);
                int flingHandleId = a.GetResourceId((int)SafeGetFieldValue(styleable, "BindableDragSortListView_fling_handle_id" ,0), DEFAULT_CONTROLLER_FLING_HANDLE_ID);
                int clickRemoveId = a.GetResourceId((int)SafeGetFieldValue(styleable, "BindableDragSortListView_click_remove_id" ,0), DEFAULT_CONTROLLER_CLICK_REMOVE_ID);
                Color bgColor = a.GetColor((int)SafeGetFieldValue(styleable, "BindableDragSortListView_float_background_color", 0), DEFAULT_CONTROLLER_BACKGROUND_COLOR);

                DragSortController controller = new DragSortController(this, dragHandleId, dragInitMode, removeMode, clickRemoveId, flingHandleId);
                controller.RemoveEnabled = removeEnabled;
                controller.SortEnabled = sortEnabled;
                controller.SetBackgroundColor(bgColor);

                _floatViewManager = controller;
                SetOnTouchListener(controller);
            }

            a.Recycle();
        }

        private void DrawDivider(int expPosition, Canvas canvas)
        {
            Drawable divider = Divider;
            int dividerHeight = DividerHeight;

            if (divider != null && dividerHeight != 0)
            {
                ViewGroup expItem = (ViewGroup)GetChildAt(expPosition - FirstVisiblePosition);
                if (expItem != null)
                {
                    int l = PaddingLeft;
                    int r = Width - PaddingRight;
                    int t;
                    int b;

                    int childHeight = expItem.GetChildAt(0).Height;

                    if (expPosition > _srcPos)
                    {
                        t = expItem.Top + childHeight;
                        b = t + dividerHeight;
                    }
                    else
                    {
                        b = expItem.Bottom - childHeight;
                        t = b - dividerHeight;
                    }

                    // Have to clip to support ColorDrawable on <= Gingerbread
                    canvas.Save();
                    canvas.ClipRect(l, t, r, b);
                    divider.SetBounds(l, t, r, b);
                    divider.Draw(canvas);
                    canvas.Restore();
                }
            }
        }

        protected override void DispatchDraw(Canvas canvas)
        {
            base.DispatchDraw(canvas);

            if (_dragState != DragStateEnum.Idle)
            {
                // draw the divider over the expanded item
                if (_firstExpPos != _srcPos)
                {
                    DrawDivider(_firstExpPos, canvas);
                }
                if (_secondExpPos != _firstExpPos && _secondExpPos != _srcPos)
                {
                    DrawDivider(_secondExpPos, canvas);
                }
            }

            if (_floatView != null)
            {
                // draw the float view over everything
                int w = _floatView.Width;
                int h = _floatView.Height;

                int x = _floatLoc.X;

                int width = Width;
                if (x < 0)
                    x = -x;
                float alphaMod;
                if (x < width)
                {
                    alphaMod = ((float)(width - x)) / ((float)width);
                    alphaMod *= alphaMod;
                }
                else
                {
                    alphaMod = 0;
                }

                int alpha = (int)(255f * _currFloatAlpha * alphaMod);

                canvas.Save();
                canvas.Translate(_floatLoc.X, _floatLoc.Y);
                canvas.ClipRect(0, 0, w, h);
                canvas.SaveLayerAlpha(0, 0, w, h, alpha, SaveFlags.All);
                _floatView.Draw(canvas);
                canvas.Restore();
                canvas.Restore();
            }
        }

        private int GetItemHeight(int position)
        {
            View v = GetChildAt(position - FirstVisiblePosition);

            if (v != null)
            {
                // item is onscreen, just get the height of the View
                return v.Height;
            }
            else
            {
                // item is offscreen. get child height and calculate
                // item height based on current shuffle state
                return CalcItemHeight(position, GetChildHeight(position));
            }
        }

        private void PrintPosData()
        {
            Log.Debug("mobeta", "_srcPos=" + _srcPos + " _firstExpPos=" + _firstExpPos + " _secondExpPos=" + _secondExpPos);
        }

        /// <summary>
        /// Get the shuffle edge for item at position when top of
        /// item is at y-coord top. Assumes that current item heights
        /// are consistent with current float view location and
        /// thus expanded positions and slide fraction. i.e. Should not be
        /// called between update of expanded positions/slide fraction
        /// and layoutChildren.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="top"></param>
        /// <returns>
        /// Shuffle line between position-1 and position (for
        /// the given view of the list; that is, for when top of item at
        /// position has y-coord of given `top`). If
        /// floating View (treated as horizontal line) is dropped
        /// immediately above this line, it lands in position-1. If
        /// dropped immediately below this line, it lands in position.
        /// </returns>
        private int GetShuffleEdge(int position, int top)
        {
            int numHeaders = HeaderViewsCount;
            int numFooters = FooterViewsCount;

            // shuffle edges are defined between items that can be
            // dragged; there are N-1 of them if there are N draggable
            // items.
            if (position <= numHeaders || (position >= Count - numFooters))
            {
                return top;
            }

            int divHeight = DividerHeight;

            int edge;

            int maxBlankHeight = _floatViewHeight - _itemHeightCollapsed;
            int childHeight = GetChildHeight(position);
            int itemHeight = GetItemHeight(position);

            // first calculate top of item given that floating View is
            // centered over src position
            int otop = top;
            if (_secondExpPos <= _srcPos)
            {
                // items are expanded on and/or above the source position

                if (position == _secondExpPos && _firstExpPos != _secondExpPos)
                {
                    if (position == _srcPos)
                    {
                        otop = top + itemHeight - _floatViewHeight;
                    }
                    else
                    {
                        int blankHeight = itemHeight - childHeight;
                        otop = top + blankHeight - maxBlankHeight;
                    }
                }
                else if (position > _secondExpPos && position <= _srcPos)
                {
                    otop = top - maxBlankHeight;
                }
            }
            else
            {
                // items are expanded on and/or below the source position
                if (position > _srcPos && position <= _firstExpPos)
                {
                    otop = top + maxBlankHeight;
                }
                else if (position == _secondExpPos && _firstExpPos != _secondExpPos)
                {
                    int blankHeight = itemHeight - childHeight;
                    otop = top + blankHeight;
                }
            }

            // otop is set
            if (position <= _srcPos)
            {
                edge = otop + (_floatViewHeight - divHeight - GetChildHeight(position - 1)) / 2;
            }
            else
            {
                edge = otop + (childHeight - divHeight - _floatViewHeight) / 2;
            }

            return edge;
        }

        private bool UpdatePositions()
        {
            int first = FirstVisiblePosition;
            int startPos = _firstExpPos;
            View startView = GetChildAt(startPos - first);

            if (startView == null)
            {
                startPos = first + ChildCount / 2;
                startView = GetChildAt(startPos - first);
            }
            int startTop = startView.Top;

            int itemHeight = startView.Height;

            int edge = GetShuffleEdge(startPos, startTop);
            int lastEdge = edge;

            int divHeight = DividerHeight;

            int itemPos = startPos;
            int itemTop = startTop;
            if (_floatViewMid < edge)
            {
                // scanning up for float position
                while (itemPos >= 0)
                {
                    itemPos--;
                    itemHeight = GetItemHeight(itemPos);

                    if (itemPos == 0)
                    {
                        edge = itemTop - divHeight - itemHeight;
                        break;
                    }

                    itemTop -= itemHeight + divHeight;
                    edge = GetShuffleEdge(itemPos, itemTop);

                    if (_floatViewMid >= edge)
                    {
                        break;
                    }

                    lastEdge = edge;
                }
            }
            else
            {
                // scanning down for float position
                int count = Count;
                while (itemPos < count)
                {
                    if (itemPos == count - 1)
                    {
                        edge = itemTop + divHeight + itemHeight;
                        break;
                    }

                    itemTop += divHeight + itemHeight;
                    itemHeight = GetItemHeight(itemPos + 1);
                    edge = GetShuffleEdge(itemPos + 1, itemTop);

                    // test for hit
                    if (_floatViewMid < edge)
                    {
                        break;
                    }

                    lastEdge = edge;
                    itemPos++;
                }
            }

            int numHeaders = HeaderViewsCount;
            int numFooters = FooterViewsCount;

            bool updated = false;

            int oldFirstExpPos = _firstExpPos;
            int oldSecondExpPos = _secondExpPos;
            float oldSlideFrac = _slideFrac;

            if (_animate)
            {
                int edgeToEdge = Math.Abs(edge - lastEdge);

                int edgeTop, edgeBottom;
                if (_floatViewMid < edge)
                {
                    edgeBottom = edge;
                    edgeTop = lastEdge;
                }
                else
                {
                    edgeTop = edge;
                    edgeBottom = lastEdge;
                }

                int slideRgnHeight = (int)(0.5f * _slideRegionFrac * edgeToEdge);
                float slideRgnHeightF = (float)slideRgnHeight;
                int slideEdgeTop = edgeTop + slideRgnHeight;
                int slideEdgeBottom = edgeBottom - slideRgnHeight;

                // Three regions
                if (_floatViewMid < slideEdgeTop)
                {
                    _firstExpPos = itemPos - 1;
                    _secondExpPos = itemPos;
                    _slideFrac = 0.5f * ((float)(slideEdgeTop - _floatViewMid)) / slideRgnHeightF;
                }
                else if (_floatViewMid < slideEdgeBottom)
                {
                    _firstExpPos = itemPos;
                    _secondExpPos = itemPos;
                }
                else
                {
                    _firstExpPos = itemPos;
                    _secondExpPos = itemPos + 1;
                    _slideFrac = 0.5f * (1.0f + ((float)(edgeBottom - _floatViewMid))
                            / slideRgnHeightF);
                }
            }
            else
            {
                _firstExpPos = itemPos;
                _secondExpPos = itemPos;
            }

            // correct for headers and footers
            if (_firstExpPos < numHeaders)
            {
                itemPos = numHeaders;
                _firstExpPos = itemPos;
                _secondExpPos = itemPos;
            }
            else if (_secondExpPos >= Count - numFooters)
            {
                itemPos = Count - numFooters - 1;
                _firstExpPos = itemPos;
                _secondExpPos = itemPos;
            }

            if (_firstExpPos != oldFirstExpPos ||
                _secondExpPos != oldSecondExpPos ||
                _slideFrac != oldSlideFrac)
            {
                updated = true;
            }

            if (itemPos != _floatPos)
            {
                if (_dragListener != null)
                {
                    _dragListener.Drag(_floatPos - numHeaders, itemPos - numHeaders);
                }

                _floatPos = itemPos;
                updated = true;
            }

            return updated;
        }

        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);

            if (_trackDragSort)
            {
                _dragSortTracker.appendState();
            }
        }

        public void RemoveItem(int which)
        {
            _useRemoveVelocity = false;
            RemoveItem(which, 0);
        }

        /// <summary>
        /// Removes an item from the list and animates the removal.
        /// </summary>
        /// <param name="which">Position to remove (NOTE: headers/footers ignored! this is a position in your input ListAdapter).</param>
        /// <param name="velocityX"></param>
        public void RemoveItem(int which, float velocityX)
        {
            if (_dragState == DragStateEnum.Idle || _dragState == DragStateEnum.Dragging)
            {
                if (_dragState == DragStateEnum.Idle)
                {
                    // called from outside drag-sort
                    _srcPos = HeaderViewsCount + which;
                    _firstExpPos = _srcPos;
                    _secondExpPos = _srcPos;
                    _floatPos = _srcPos;
                    View v = GetChildAt(_srcPos - FirstVisiblePosition);
                    if (v != null)
                    {
                        v.Visibility = ViewStates.Invisible;
                    }
                }

                _dragState = DragStateEnum.Removing;
                _removeVelocityX = velocityX;

                if (_inTouchEvent)
                {
                    switch (_cancelMethod)
                    {
                        case CancelMethodEnum.OnTouchEvent:
                            OnTouchEvent(_cancelEvent);
                            break;

                        case CancelMethodEnum.OnInterceptTouchEvent:
                            OnInterceptTouchEvent(_cancelEvent);
                            break;
                    }
                }

                if (_removeAnimator != null)
                {
                    _removeAnimator.Start();
                }
                else
                {
                    DoRemoveItem(which);
                }
            }
        }

        /// <summary>
        /// Cancel a drag. Calls {@link #stopDrag(boolean, boolean)} with
        /// <code>true</code> as the first argument.
        /// </summary>
        public void CancelDrag()
        {
            if (_dragState == DragStateEnum.Dragging)
            {
                _dragScroller.StopScrolling(true);
                DestroyFloatView();
                ClearPositions();
                AdjustAllItems();

                if (_inTouchEvent)
                {
                    _dragState = DragStateEnum.Stopped;
                }
                else
                {
                    _dragState = DragStateEnum.Idle;
                }
            }
        }

        private void ClearPositions()
        {
            _srcPos = -1;
            _firstExpPos = -1;
            _secondExpPos = -1;
            _floatPos = -1;
        }

        public void DropFloatView()
        {
            // must set to avoid cancelDrag being called from the
            // DataSetObserver
            _dragState = DragStateEnum.Dropping;

            if (_dropListener != null && _floatPos >= 0 && _floatPos < Count)
            {
                int numHeaders = HeaderViewsCount;
                _dropListener.Drop(_srcPos - numHeaders, _floatPos - numHeaders);
            }

            DestroyFloatView();

            AdjustOnReorder();
            ClearPositions();
            AdjustAllItems();

            // now the drag is done
            if (_inTouchEvent)
            {
                _dragState = DragStateEnum.Stopped;
            }
            else
            {
                _dragState = DragStateEnum.Idle;
            }
        }

        private void DoRemoveItem()
        {
            DoRemoveItem(_srcPos - HeaderViewsCount);
        }

        /// <summary>
        /// Removes dragged item from the list. Calls RemoveListener.
        /// </summary>
        /// <param name="which"></param>
        private void DoRemoveItem(int which)
        {
            // must set to avoid cancelDrag being called from the
            // DataSetObserver
            _dragState = DragStateEnum.Removing;

            // end it
            if (_removeListener != null)
            {
                _removeListener.Remove(which);
            }

            DestroyFloatView();

            AdjustOnReorder();
            ClearPositions();

            // now the drag is done
            if (_inTouchEvent)
            {
                _dragState = DragStateEnum.Stopped;
            }
            else
            {
                _dragState = DragStateEnum.Idle;
            }
        }

        private void AdjustOnReorder()
        {
            int firstPos = FirstVisiblePosition;
            if (_srcPos < firstPos)
            {
                // collapsed src item is off screen;
                // adjust the scroll after item heights have been fixed
                View v = GetChildAt(0);
                int top = 0;
                if (v != null)
                {
                    top = v.Top;
                }

                SetSelectionFromTop(firstPos - 1, top - PaddingTop);
            }
        }

        /// <summary>
        /// Stop a drag in progress. Pass <code>true</code> if you would
        /// like to remove the dragged item from the list.
        /// </summary>
        /// <param name="remove">
        /// Remove the dragged item from the list. Calls
        /// a registered RemoveListener, if one exists. Otherwise, calls
        /// the DropListener, if one exists.
        /// </param>
        /// <returns>True if the stop was successful. False if there is no floating View.</returns>
        public bool StopDrag(bool remove)
        {
            _useRemoveVelocity = false;
            return StopDrag(remove, 0);
        }

        public bool StopDragWithVelocity(bool remove, float velocityX)
        {

            _useRemoveVelocity = true;
            return StopDrag(remove, velocityX);
        }

        public bool StopDrag(bool remove, float velocityX)
        {
            if (_floatView != null)
            {
                _dragScroller.StopScrolling(true);

                if (remove)
                {
                    RemoveItem(_srcPos - HeaderViewsCount, velocityX);
                }
                else
                {
                    if (_dropAnimator != null)
                    {
                        _dropAnimator.Start();
                    }
                    else
                    {
                        DropFloatView();
                    }
                }

                if (_trackDragSort)
                {
                    _dragSortTracker.stopTracking();
                }

                return true;
            }
            else
            {
                // stop failed
                return false;
            }
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            if (_ignoreTouchEvent)
            {
                _ignoreTouchEvent = false;
                return false;
            }

            if (!_dragEnabled)
            {
                return base.OnTouchEvent(e);
            }

            bool more = false;

            bool lastCallWasIntercept = _lastCallWasIntercept;
            _lastCallWasIntercept = false;

            if (!lastCallWasIntercept)
            {
                SaveTouchCoords(e);
            }

            // if (_floatView != null) {
            if (_dragState == DragStateEnum.Dragging)
            {
                OnDragTouchEvent(e);
                more = true; // give us more!
            }
            else
            {
                // what if float view is null b/c we dropped in middle
                // of drag touch event?

                // if (_dragState != STOPPED) {
                if (_dragState == DragStateEnum.Idle)
                {
                    if (base.OnTouchEvent(e))
                    {
                        more = true;
                    }
                }

                MotionEventActions action = e.Action & MotionEventActions.Mask;

                switch (action)
                {
                    case MotionEventActions.Cancel:
                    case MotionEventActions.Up:
                        DoActionUpOrCancel();
                        break;

                    default:
                        if (more)
                        {
                            _cancelMethod = CancelMethodEnum.OnTouchEvent;
                        }
                        break;
                }
            }

            return more;
        }

        private void DoActionUpOrCancel()
        {
            _cancelMethod = CancelMethodEnum.NoCancel;
            _inTouchEvent = false;
            if (_dragState == DragStateEnum.Stopped)
            {
                _dragState = DragStateEnum.Idle;
            }
            _currFloatAlpha = _floatAlpha;
            _listViewIntercepted = false;
            _childHeightCache.Clear();
        }

        private void SaveTouchCoords(MotionEvent ev)
        {
            MotionEventActions action = ev.Action & MotionEventActions.Mask;
            if (action != MotionEventActions.Down)
            {
                _lastX = _touchX;
                _lastY = _touchY;
            }
            _touchX = (int)ev.GetX();
            _touchY = (int)ev.GetY();
            if (action == MotionEventActions.Down)
            {
                _lastX = _touchX;
                _lastY = _touchY;
            }
            _offsetX = (int)ev.RawX - _touchX;
            _offsetY = (int)ev.RawY - _touchY;
        }

        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            if (!_dragEnabled)
            {
                return base.OnInterceptTouchEvent(ev);
            }

            SaveTouchCoords(ev);
            _lastCallWasIntercept = true;

            MotionEventActions action = ev.Action & MotionEventActions.Mask;

            if (action == MotionEventActions.Down)
            {
                if (_dragState != DragStateEnum.Idle)
                {
                    // intercept and ignore
                    _ignoreTouchEvent = true;
                    return true;
                }
                _inTouchEvent = true;
            }

            bool intercept = false;

            // the following deals with calls to super.onInterceptTouchEvent
            if (_floatView != null)
            {
                // super's touch event canceled in startDrag
                intercept = true;
            }
            else
            {
                if (base.OnInterceptTouchEvent(ev))
                {
                    _listViewIntercepted = true;
                    intercept = true;
                }

                switch (action)
                {
                    case MotionEventActions.Cancel:
                    case MotionEventActions.Up:
                        DoActionUpOrCancel();
                        break;

                    default:
                        if (intercept)
                        {
                            _cancelMethod = CancelMethodEnum.OnTouchEvent;
                        }
                        else
                        {
                            _cancelMethod = CancelMethodEnum.OnInterceptTouchEvent;
                        }
                        break;
                }
            }

            if (action == MotionEventActions.Up || action == MotionEventActions.Cancel)
            {
                _inTouchEvent = false;
            }

            return intercept;
        }

        /// <summary>
        /// Set the width of each drag scroll region by specifying
        /// a fraction of the ListView height.
        /// </summary>
        /// <param name="heightFraction">Fraction of ListView height. Capped at 0.5f</param>
        public void SetDragScrollStart(float heightFraction)
        {
            SetDragScrollStarts(heightFraction, heightFraction);
        }

        /// <summary>
        /// Set the width of each drag scroll region by specifying
        /// a fraction of the ListView height.
        /// </summary>
        /// <param name="upperFrac">Fraction of ListView height for up-scroll bound. Capped at 0.5f.</param>
        /// <param name="lowerFrac">Fraction of ListView height for down-scroll bound. Capped at 0.5f.</param>
        public void SetDragScrollStarts(float upperFrac, float lowerFrac)
        {
            if (lowerFrac > 0.5f)
            {
                _dragDownScrollStartFrac = 0.5f;
            }
            else
            {
                _dragDownScrollStartFrac = lowerFrac;
            }

            if (upperFrac > 0.5f)
            {
                _dragUpScrollStartFrac = 0.5f;
            }
            else
            {
                _dragUpScrollStartFrac = upperFrac;
            }

            if (this.Height != 0)
            {
                UpdateScrollStarts();
            }
        }

        private void ContinueDrag(int x, int y)
        {
            // proposed position
            _floatLoc.X = x - _dragDeltaX;
            _floatLoc.Y = y - _dragDeltaY;

            DoDragFloatView(true);

            int minY = Math.Min(y, _floatViewMid + _floatViewHeightHalf);
            int maxY = Math.Max(y, _floatViewMid - _floatViewHeightHalf);

            // get the current scroll direction
            int currentScrollDir = _dragScroller.ScrollDir;

            if (minY > _lastY && minY > _downScrollStartY && currentScrollDir != DragScroller.DOWN)
            {
                // dragged down, it is below the down scroll start and it is not
                // scrolling up

                if (currentScrollDir != DragScroller.STOP)
                {
                    // moved directly from up scroll to down scroll
                    _dragScroller.StopScrolling(true);
                }

                // start scrolling down
                _dragScroller.StartScrolling(DragScroller.DOWN);
            }
            else if (maxY < _lastY && maxY < _upScrollStartY && currentScrollDir != DragScroller.UP)
            {
                // dragged up, it is above the up scroll start and it is not
                // scrolling up

                if (currentScrollDir != DragScroller.STOP)
                {
                    // moved directly from down scroll to up scroll
                    _dragScroller.StopScrolling(true);
                }

                // start scrolling up
                _dragScroller.StartScrolling(DragScroller.UP);
            }
            else if (maxY >= _upScrollStartY && minY <= _downScrollStartY
                    && _dragScroller.IsScrolling)
            {
                // not in the upper nor in the lower drag-scroll regions but it is
                // still scrolling

                _dragScroller.StopScrolling(true);
            }
        }

        private void UpdateScrollStarts()
        {
            int padTop = this.PaddingTop;
            int listHeight = this.Height - padTop - this.PaddingBottom;
            float heightF = (float)listHeight;

            _upScrollStartYF = padTop + _dragUpScrollStartFrac * heightF;
            _downScrollStartYF = padTop + (1.0f - _dragDownScrollStartFrac) * heightF;

            _upScrollStartY = (int)_upScrollStartYF;
            _downScrollStartY = (int)_downScrollStartYF;

            _dragUpScrollHeight = _upScrollStartYF - padTop;
            _dragDownScrollHeight = padTop + listHeight - _downScrollStartYF;
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            UpdateScrollStarts();
        }

        private void AdjustAllItems()
        {
            int first = FirstVisiblePosition;
            int last = LastVisiblePosition;

            int begin = Math.Max(0, HeaderViewsCount - first);
            int end = Math.Min(last - first, Count - 1 - FooterViewsCount - first);

            for (int i = begin; i <= end; ++i)
            {
                View v = GetChildAt(i);
                if (v != null)
                {
                    AdjustItem(first + i, v, false);
                }
            }
        }

        private void AdjustItem(int position)
        {
            View v = GetChildAt(position - FirstVisiblePosition);

            if (v != null)
            {
                AdjustItem(position, v, false);
            }
        }

        /// <summary>
        /// Sets layout param height, gravity, and visibility on wrapped item.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="v"></param>
        /// <param name="invalidChildHeight"></param>
        public void AdjustItem(int position, View v, bool invalidChildHeight)
        {
            // Adjust item height
            ViewGroup.LayoutParams lp = v.LayoutParameters;
            int height;
            if (position != _srcPos && position != _firstExpPos && position != _secondExpPos)
            {
                height = ViewGroup.LayoutParams.WrapContent;
            }
            else
            {
                height = CalcItemHeight(position, v, invalidChildHeight);
            }

            if (height != lp.Height)
            {
                lp.Height = height;
                v.LayoutParameters = lp;
            }

            // Adjust item gravity
            if (position == _firstExpPos || position == _secondExpPos)
            {
                if (position < _srcPos)
                {
                    ((DragSortItemView)v).Gravity = GravityFlags.Bottom;
                }
                else if (position > _srcPos)
                {
                    ((DragSortItemView)v).Gravity = GravityFlags.Top;
                }
            }

            // Finally adjust item visibility
            ViewStates oldVis = v.Visibility;
            ViewStates vis = ViewStates.Visible;

            if (position == _srcPos && _floatView != null)
            {
                vis = ViewStates.Invisible;
            }

            if (vis != oldVis)
            {
                v.Visibility = vis;
            }
        }

        public int GetChildHeight(int position)
        {
            if (position == _srcPos)
            {
                return 0;
            }

            View v = GetChildAt(position - FirstVisiblePosition);
            if (v != null)
            {
                // item is onscreen, therefore child height is valid,
                // hence the "true"
                return GetChildHeight(position, v, false);
            }
            else
            {
                // item is offscreen
                // first check cache for child height at this position
                int childHeight = _childHeightCache.Get(position);
                if (childHeight != -1)
                {
                    return childHeight;
                }

                int type = Adapter.GetItemViewType(position);

                // There might be a better place for checking for the following
                int typeCount = Adapter.ViewTypeCount;
                if (typeCount != _sampleViewTypes.Length)
                {
                    _sampleViewTypes = new View[typeCount];
                }

                if (type >= 0)
                {
                    if (_sampleViewTypes[type] == null)
                    {
                        v = Adapter.GetView(position, null, this);
                        _sampleViewTypes[type] = v;
                    }
                    else
                    {
                        v = Adapter.GetView(position, _sampleViewTypes[type], this);
                    }
                }
                else
                {
                    // type is HEADER_OR_FOOTER or IGNORE
                    v = Adapter.GetView(position, null, this);
                }

                // current child height is invalid, hence "true" below
                childHeight = GetChildHeight(position, v, true);

                // cache it because this could have been expensive
                _childHeightCache.Add(position, childHeight);

                return childHeight;
            }
        }

        public int GetChildHeight(int position, View item, bool invalidChildHeight)
        {
            if (position == _srcPos)
            {
                return 0;
            }

            View child;
            if (position < HeaderViewsCount || position >= Count - FooterViewsCount)
            {
                child = item;
            }
            else
            {
                child = ((ViewGroup)item).GetChildAt(0);
            }

            ViewGroup.LayoutParams lp = child.LayoutParameters;

            if (lp != null)
            {
                if (lp.Height > 0)
                {
                    return lp.Height;
                }
            }

            int childHeight = child.Height;

            if (childHeight == 0 || invalidChildHeight)
            {
                MeasureItem(child);
                childHeight = child.MeasuredHeight;
            }

            return childHeight;
        }

        private int CalcItemHeight(int position, View item, bool invalidChildHeight)
        {
            return CalcItemHeight(position, GetChildHeight(position, item, invalidChildHeight));
        }

        private int CalcItemHeight(int position, int childHeight)
        {
            int divHeight = DividerHeight;

            bool isSliding = _animate && _firstExpPos != _secondExpPos;
            int maxNonSrcBlankHeight = _floatViewHeight - _itemHeightCollapsed;
            int slideHeight = (int)(_slideFrac * maxNonSrcBlankHeight);

            int height;

            if (position == _srcPos)
            {
                if (_srcPos == _firstExpPos)
                {
                    if (isSliding)
                    {
                        height = slideHeight + _itemHeightCollapsed;
                    }
                    else
                    {
                        height = _floatViewHeight;
                    }
                }
                else if (_srcPos == _secondExpPos)
                {
                    // if gets here, we know an item is sliding
                    height = _floatViewHeight - slideHeight;
                }
                else
                {
                    height = _itemHeightCollapsed;
                }
            }
            else if (position == _firstExpPos)
            {
                if (isSliding)
                {
                    height = childHeight + slideHeight;
                }
                else
                {
                    height = childHeight + maxNonSrcBlankHeight;
                }
            }
            else if (position == _secondExpPos)
            {
                // we know an item is sliding (b/c 2ndPos != 1stPos)
                height = childHeight + maxNonSrcBlankHeight - slideHeight;
            }
            else
            {
                height = childHeight;
            }

            return height;
        }

        public override void RequestLayout()
        {
            if (!_blockLayoutRequests)
            {
                base.RequestLayout();
            }
        }

        private int AdjustScroll(int movePos, View moveItem, int oldFirstExpPos, int oldSecondExpPos)
        {
            int adjust = 0;

            int childHeight = GetChildHeight(movePos);

            int moveHeightBefore = moveItem.Height;
            int moveHeightAfter = CalcItemHeight(movePos, childHeight);

            int moveBlankBefore = moveHeightBefore;
            int moveBlankAfter = moveHeightAfter;
            if (movePos != _srcPos)
            {
                moveBlankBefore -= childHeight;
                moveBlankAfter -= childHeight;
            }

            int maxBlank = _floatViewHeight;
            if (_srcPos != _firstExpPos && _srcPos != _secondExpPos)
            {
                maxBlank -= _itemHeightCollapsed;
            }

            if (movePos <= oldFirstExpPos)
            {
                if (movePos > _firstExpPos)
                {
                    adjust += maxBlank - moveBlankAfter;
                }
            }
            else if (movePos == oldSecondExpPos)
            {
                if (movePos <= _firstExpPos)
                {
                    adjust += moveBlankBefore - maxBlank;
                }
                else if (movePos == _secondExpPos)
                {
                    adjust += moveHeightBefore - moveHeightAfter;
                }
                else
                {
                    adjust += moveBlankBefore;
                }
            }
            else
            {
                if (movePos <= _firstExpPos)
                {
                    adjust -= maxBlank;
                }
                else if (movePos == _secondExpPos)
                {
                    adjust -= moveBlankAfter;
                }
            }

            return adjust;
        }

        private void MeasureItem(View item)
        {
            ViewGroup.LayoutParams lp = item.LayoutParameters;
            if (lp == null)
            {
                lp = new AbsListView.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
                item.LayoutParameters = lp;
            }
            int wspec = ViewGroup.GetChildMeasureSpec(_widthMeasureSpec, ListPaddingLeft + ListPaddingRight, lp.Width);
            int hspec;
            if (lp.Height > 0)
            {
                hspec = MeasureSpec.MakeMeasureSpec(lp.Height, MeasureSpecMode.Exactly);
            }
            else
            {
                hspec = MeasureSpec.MakeMeasureSpec(0, MeasureSpecMode.Unspecified);
            }
            item.Measure(wspec, hspec);
        }

        private void MeasureFloatView()
        {
            if (_floatView != null)
            {
                MeasureItem(_floatView);
                _floatViewHeight = _floatView.MeasuredHeight;
                _floatViewHeightHalf = _floatViewHeight / 2;
            }
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            if (_floatView != null)
            {
                if (_floatView.IsLayoutRequested)
                {
                    MeasureFloatView();
                }
                _floatViewOnMeasured = true; // set to false after layout
            }
            _widthMeasureSpec = widthMeasureSpec;
        }

        public void RunLayoutChildren() { LayoutChildren(); }
        protected override void LayoutChildren()
        {
            base.LayoutChildren();

            if (_floatView != null)
            {
                if (_floatView.IsLayoutRequested && !_floatViewOnMeasured)
                {
                    // Have to measure here when usual android measure
                    // pass is skipped. This happens during a drag-sort
                    // when layoutChildren is called directly.
                    MeasureFloatView();
                }
                _floatView.Layout(0, 0, _floatView.MeasuredWidth, _floatView.MeasuredHeight);
                _floatViewOnMeasured = false;
            }
        }

        protected bool OnDragTouchEvent(MotionEvent ev)
        {
            // we are in a drag
            MotionEventActions action = ev.Action & MotionEventActions.Mask;

            switch (ev.Action & MotionEventActions.Mask)
            {
                case MotionEventActions.Cancel:
                    if (_dragState == DragStateEnum.Dragging)
                    {
                        CancelDrag();
                    }
                    DoActionUpOrCancel();
                    break;

                case MotionEventActions.Up:
                    // Log.d("mobeta", "calling stopDrag from onDragTouchEvent");
                    if (_dragState == DragStateEnum.Dragging)
                    {
                        StopDrag(false);
                    }
                    DoActionUpOrCancel();
                    break;

                case MotionEventActions.Move:
                    ContinueDrag((int)ev.GetX(), (int)ev.GetY());
                    break;
            }

            return true;
        }

        /// <summary>
        /// Start a drag of item at <code>position</code> using the
        /// registered FloatViewManager. Calls through
        /// to {@link #startDrag(int,View,int,int,int)} after obtaining
        /// the floating View from the FloatViewManager. 
        /// </summary>
        /// <param name="position">Item to drag.</param>
        /// <param name="dragFlags">
        /// Flags that restrict some movements of the
        /// floating View. For example, set <code>dragFlags |= 
        /// ~{@link #DRAG_NEG_X}</code> to allow dragging the floating
        /// View in all directions except off the screen to the left.
        /// </param>
        /// <param name="deltaX">
        /// Offset in x of the touch coordinate from the
        /// left edge of the floating View (i.e. touch-x minus float View
        /// left).
        /// </param>
        /// <param name="deltaY">
        /// Offset in y of the touch coordinate from the
        /// top edge of the floating View (i.e. touch-y minus float View
        /// top).
        /// </param>
        /// <returns>
        /// True if the drag was started, false otherwise. This
        /// <code>startDrag</code> will fail if we are not currently in
        /// a touch event, there is no registered FloatViewManager,
        /// or the FloatViewManager returns a null View.
        /// </returns>
        public bool StartDrag(int position, int dragFlags, int deltaX, int deltaY)
        {
            if (!_inTouchEvent || _floatViewManager == null)
            {
                return false;
            }

            View v = _floatViewManager.OnCreateFloatView(position);

            if (v == null)
            {
                return false;
            }
            else
            {
                return StartDrag(position, v, dragFlags, deltaX, deltaY);
            }

        }

        /// <summary>
        /// Start a drag of item at <code>position</code> without using a FloatViewManager.
        /// </summary>
        /// <param name="position">Item to drag.</param>
        /// <param name="floatView">Floating View.</param>
        /// <param name="dragFlags">
        /// Flags that restrict some movements of the
        /// floating View. For example, set <code>dragFlags |= 
        /// ~{@link #DRAG_NEG_X}</code> to allow dragging the floating
        /// View in all directions except off the screen to the left.
        /// </param>
        /// <param name="deltaX">
        /// Offset in x of the touch coordinate from the
        /// left edge of the floating View (i.e. touch-x minus float View
        /// left).
        /// </param>
        /// <param name="deltaY">
        ///  Offset in y of the touch coordinate from the
        /// top edge of the floating View (i.e. touch-y minus float View
        /// top).
        /// </param>
        /// <returns>
        /// True if the drag was started, false otherwise. This
        /// <code>startDrag</code> will fail if we are not currently in
        /// a touch event, <code>floatView</code> is null, or there is
        /// a drag in progress.
        /// </returns>
        public bool StartDrag(int position, View floatView, int dragFlags, int deltaX, int deltaY)
        {
            if (_dragState != DragStateEnum.Idle || !_inTouchEvent || _floatView != null || floatView == null || !_dragEnabled)
            {
                return false;
            }

            if (Parent != null)
            {
                Parent.RequestDisallowInterceptTouchEvent(true);
            }

            int pos = position + HeaderViewsCount;
            _firstExpPos = pos;
            _secondExpPos = pos;
            _srcPos = pos;
            _floatPos = pos;

            // _dragState = dragType;
            _dragState = DragStateEnum.Dragging;
            _dragFlags = 0;
            _dragFlags |= dragFlags;

            _floatView = floatView;
            MeasureFloatView(); // sets _floatViewHeight

            _dragDeltaX = deltaX;
            _dragDeltaY = deltaY;
            _dragStartY = _touchY;

            // updateFloatView(_touchX - _dragDeltaX, mY - _dragDeltaY);
            _floatLoc.X = _touchX - _dragDeltaX;
            _floatLoc.Y = _touchY - _dragDeltaY;

            // set src item invisible
            View srcItem = GetChildAt(_srcPos - FirstVisiblePosition);

            if (srcItem != null)
            {
                srcItem.Visibility = ViewStates.Invisible;
            }

            if (_trackDragSort)
            {
                _dragSortTracker.startTracking();
            }

            // once float view is created, events are no longer passed
            // to ListView
            switch (_cancelMethod)
            {
                case CancelMethodEnum.OnTouchEvent:
                    OnTouchEvent(_cancelEvent);
                    break;

                case CancelMethodEnum.OnInterceptTouchEvent:
                    OnInterceptTouchEvent(_cancelEvent);
                    break;
            }

            RequestLayout();

            if (_liftAnimator != null)
            {
                _liftAnimator.Start();
            }

            return true;
        }

        public void DoDragFloatView(bool forceInvalidate)
        {
            int movePos = FirstVisiblePosition + ChildCount / 2;
            View moveItem = GetChildAt(ChildCount / 2);

            if (moveItem == null)
            {
                return;
            }

            DoDragFloatView(movePos, moveItem, forceInvalidate);
        }

        public void DoDragFloatView(int movePos, View moveItem, bool forceInvalidate)
        {
            _blockLayoutRequests = true;

            UpdateFloatView();

            int oldFirstExpPos = _firstExpPos;
            int oldSecondExpPos = _secondExpPos;

            bool updated = UpdatePositions();

            if (updated)
            {
                AdjustAllItems();
                int scroll = AdjustScroll(movePos, moveItem, oldFirstExpPos, oldSecondExpPos);

                SetSelectionFromTop(movePos, moveItem.Top + scroll - PaddingTop);
                LayoutChildren();
            }

            if (updated || forceInvalidate)
            {
                Invalidate();
            }

            _blockLayoutRequests = false;
        }

        /// <summary>
        /// Sets float View location based on suggested values and
        /// constraints set in _dragFlags.
        /// </summary>
        private void UpdateFloatView()
        {
            if (_floatViewManager != null)
            {
                _touchLoc.Set(_touchX, _touchY);
                _floatViewManager.OnDragFloatView(_floatView, _floatLoc, _touchLoc);
            }

            int floatX = _floatLoc.X;
            int floatY = _floatLoc.Y;

            // restrict x motion
            int padLeft = PaddingLeft;
            if ((_dragFlags & DRAG_POS_X) == 0 && floatX > padLeft)
            {
                _floatLoc.X = padLeft;
            }
            else if ((_dragFlags & DRAG_NEG_X) == 0 && floatX < padLeft)
            {
                _floatLoc.X = padLeft;
            }

            // keep floating view from going past bottom of last header view
            int numHeaders = HeaderViewsCount;
            int numFooters = FooterViewsCount;
            int firstPos = FirstVisiblePosition;
            int lastPos = LastVisiblePosition;

            // "nHead="+numHeaders+" nFoot="+numFooters+" first="+firstPos+" last="+lastPos);
            int topLimit = PaddingTop;
            if (firstPos < numHeaders)
            {
                topLimit = GetChildAt(numHeaders - firstPos - 1).Bottom;
            }
            if ((_dragFlags & DRAG_NEG_Y) == 0)
            {
                if (firstPos <= _srcPos)
                {
                    topLimit = Math.Max(GetChildAt(_srcPos - firstPos).Top, topLimit);
                }
            }
            // bottom limit is top of first footer View or
            // bottom of last item in list
            int bottomLimit = Height - PaddingBottom;
            if (lastPos >= Count - numFooters - 1)
            {
                bottomLimit = GetChildAt(Count - numFooters - 1 - firstPos).Bottom;
            }
            if ((_dragFlags & DRAG_POS_Y) == 0)
            {
                if (lastPos >= _srcPos)
                {
                    bottomLimit = Math.Min(GetChildAt(_srcPos - firstPos).Bottom, bottomLimit);
                }
            }

            if (floatY < topLimit)
            {
                _floatLoc.Y = topLimit;
            }
            else if (floatY + _floatViewHeight > bottomLimit)
            {
                _floatLoc.Y = bottomLimit - _floatViewHeight;
            }

            // get y-midpoint of floating view (constrained to ListView bounds)
            _floatViewMid = _floatLoc.Y + _floatViewHeightHalf;
        }

        public void DestroyFloatView()
        {
            if (_floatView != null)
            {
                _floatView.Visibility = ViewStates.Gone;
                if (_floatViewManager != null)
                {
                    _floatViewManager.OnDestroyFloatView(_floatView);
                }
                _floatView = null;
                Invalidate();
            }
        }

        public void SetDragSortListener(IDragSortListener l)
        {
            DropListener = l;
            DragListener = l;
            RemoveListener = l;
        }

        /// <summary>
        /// Completely custom scroll speed profile. Default increases linearly
        /// with position and is constant in time. Create your own by implementing
        /// {@link DragSortListView.DragScrollProfile}.
        /// </summary>
        /// <param name="ssp"></param>
        public void SetDragScrollProfile(IDragScrollProfile ssp)
        {
            if (ssp != null)
            {
                _scrollProfile = ssp;
            }
        }

        #endregion
    }
}