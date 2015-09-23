using Android.Views;
using System;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views.DragSortListView.Runnables
{
    /// <summary>
    /// Centers floating View over drop slot before destroying.
    /// </summary>
    class DropAnimator : SmoothAnimator
    {
        #region Fields

        private int _dropPos;
        private int _srcPos;
        private float _initDeltaY;
        private float _initDeltaX;

        #endregion

        #region Constructor

        public DropAnimator(AbstractDragSortListView dslv, float smoothness, int duration)
            : base(dslv, smoothness, duration)
        {
        }

        #endregion

        #region Methods

        public override void OnStart()
        {
            base.OnStart();

            _dropPos = _dslv.FloatPos;
            _srcPos = _dslv.SrcPos;
            _dslv.DragState = Enums.DragStateEnum.Dropping;
            _initDeltaY = _dslv.FloatLoc.Y - GetTargetY();
            _initDeltaX = _dslv.FloatLoc.X - _dslv.PaddingLeft;
        }

        public override void OnUpdate(float frac, float smoothFrac)
        {
            base.OnUpdate(frac, smoothFrac);

            int targetY = GetTargetY();
            int targetX = _dslv.PaddingLeft;
            float deltaY = _dslv.FloatLoc.Y - targetY;
            float deltaX = _dslv.FloatLoc.X - targetX;
            float f = 1f - smoothFrac;
            if (f < Math.Abs(deltaY / _initDeltaY) || f < Math.Abs(deltaX / _initDeltaX))
            {
                _dslv.FloatLoc.Y = targetY + (int)(_initDeltaY * f);
                _dslv.FloatLoc.X = _dslv.PaddingLeft + (int)(_initDeltaX * f);
                _dslv.DoDragFloatView(true);
            }
        }

        public override void OnStop()
        {
            base.OnStop();

            _dslv.DropFloatView();
        }

        private int GetTargetY()
        {
            int first = _dslv.FirstVisiblePosition;
            int otherAdjust = (_dslv.ItemHeightCollapsed + _dslv.DividerHeight) / 2;
            View v = _dslv.GetChildAt(_dropPos - first);
            int targetY = -1;
            if (v != null)
            {
                if (_dropPos == _srcPos)
                {
                    targetY = v.Top;
                }
                else if (_dropPos < _srcPos)
                {
                    // expanded down
                    targetY = v.Top - otherAdjust;
                }
                else
                {
                    // expanded up
                    targetY = v.Bottom + otherAdjust - _dslv.FloatViewHeight;
                }
            }
            else
            {
                // drop position is not on screen?? no animation
                Cancel();
            }

            return targetY;
        }

        #endregion
    }
}