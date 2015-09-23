using Android.OS;
using Android.Views;
using Java.Lang;
using System;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views.DragSortListView.Runnables
{
    class DragScroller : Java.Lang.Object, IRunnable
    {
        #region Constants

        public readonly static int STOP = -1;
        public readonly static int UP = 0;
        public readonly static int DOWN = 1;

        #endregion

        #region Fields

        private bool _abort;

        private long _prevTime;
        private long _currTime;

        private int _dy;
        private float _dt;
        private long _tStart;

        private float _scrollSpeed; // pixels per ms

        //private int _lastHeader;
        //private int _firstFooter;

        private readonly AbstractDragSortListView _dslv;

        #endregion

        #region Properties

        public bool IsScrolling
        {
            get { return _isScrolling; }
            set { _isScrolling = value; }
        }
        private bool _isScrolling = false;

        public int ScrollDir
        {
            get { return _isScrolling ? _scrollDir : STOP; }
            set { _scrollDir = value; }
        }
        private int _scrollDir;

        #endregion

        #region Constructor

        public DragScroller(AbstractDragSortListView dslv)
        {
            if (dslv == null)
                throw new ArgumentNullException("dslv");

            _dslv = dslv;
        }

        #endregion

        #region Methods

        public void StartScrolling(int dir)
        {
            if (!IsScrolling)
            {
                _abort = false;
                IsScrolling = true;
                _tStart = SystemClock.UptimeMillis();
                _prevTime = _tStart;
                ScrollDir = dir;
                _dslv.Post(this);
            }
        }

        public void StopScrolling(bool now)
        {
            if (now)
            {
                _dslv.RemoveCallbacks(this);
                IsScrolling = false;
            }
            else
            {
                _abort = true;
            }
        }

        public void Run()
        {
            if (_abort)
            {
                IsScrolling = false;
                return;
            }

            int first = _dslv.FirstVisiblePosition;
            int last = _dslv.LastVisiblePosition;
            int count = _dslv.Count;
            int padTop = _dslv.ListPaddingTop;
            int listHeight = _dslv.Height - padTop - _dslv.PaddingBottom;

            int minY = System.Math.Min(_dslv.TouchY, _dslv.FloatViewMid + _dslv.FloatViewHeightHalf);
            int maxY = System.Math.Max(_dslv.TouchY, _dslv.FloatViewMid - _dslv.FloatViewHeightHalf);

            if (ScrollDir == UP)
            {
                View v = _dslv.GetChildAt(0);
                if (v == null)
                {
                    IsScrolling = false;
                    return;
                }
                else
                {
                    if (first == 0 && v.Top == padTop)
                    {
                        IsScrolling = false;
                        return;
                    }
                }
                _scrollSpeed = _dslv.DragScrollProfile.GetSpeed((_dslv.UpScrollStartYF - maxY) / _dslv.DragUpScrollHeight, _prevTime);
            }
            else
            {
                View v = _dslv.GetChildAt(last - first);
                if (v == null)
                {
                    IsScrolling = false;
                    return;
                }
                else
                {
                    if (last == count - 1 && v.Bottom <= listHeight + padTop)
                    {
                        IsScrolling = false;
                        return;
                    }
                }
                _scrollSpeed = -_dslv.DragScrollProfile.GetSpeed((minY - _dslv.DownScrollStartYF) / _dslv.DragDownScrollHeight, _prevTime);
            }

            _currTime = SystemClock.UptimeMillis();
            _dt = (float)(_currTime - _prevTime);

            // dy is change in View position of a list item; i.e. positive dy
            // means user is scrolling up (list item moves down the screen,
            // remember
            // y=0 is at top of View).
            _dy = (int)System.Math.Round(_scrollSpeed * _dt);

            int movePos;
            if (_dy >= 0)
            {
                _dy = System.Math.Min(listHeight, _dy);
                movePos = first;
            }
            else
            {
                _dy = System.Math.Max(-listHeight, _dy);
                movePos = last;
            }

            View moveItem = _dslv.GetChildAt(movePos - first);
            int top = moveItem.Top + _dy;

            if (movePos == 0 && top > padTop)
            {
                top = padTop;
            }

            // always do scroll
            _dslv.BlockLayoutRequests = true;

            _dslv.SetSelectionFromTop(movePos, top - padTop);
            _dslv.RunLayoutChildren();
            _dslv.Invalidate();

            _dslv.BlockLayoutRequests = false;

            // scroll means relative float View movement
            _dslv.DoDragFloatView(movePos, moveItem, false);

            _prevTime = _currTime;

            _dslv.Post(this);
        }

        #endregion
    }
}