using Android.OS;
using Android.Views;
using MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.DragSortListView.Enums;
using System;

namespace MvxExtensions.Libraries.Droid.Core.Support.V7.Components.Controls.DragSortListView.Runnables
{
    /// <summary>
    /// Collapses expanded items.
    /// </summary>
    class AppCompatRemoveAnimator : AppCompatSmoothAnimator
    {
        #region Fields

        private float mFloatLocX;
        private float mFirstStartBlank;
        private float mSecondStartBlank;

        private int mFirstChildHeight = -1;
        private int mSecondChildHeight = -1;

        private int mFirstPos;
        private int mSecondPos;
        private int srcPos;

        #endregion

        #region Constructor

        public AppCompatRemoveAnimator(AppCompatBaseDragSortListView dslv, float smoothness, int duration)
            : base(dslv, smoothness, duration)
        {
        }

        #endregion

        #region Methods

        public override void OnStart()
        {
            base.OnStart();

            mFirstChildHeight = -1;
            mSecondChildHeight = -1;
            mFirstPos = _dslv.FirstExpPos;
            mSecondPos = _dslv.SecondExpPos;
            srcPos = _dslv.SrcPos;
            _dslv.DragState = DragStateEnum.Removing;

            mFloatLocX = _dslv.FloatLoc.X;
            if (_dslv.UseRemoveVelocity)
            {
                float minVelocity = 2f * _dslv.Width;
                if (_dslv.RemoveVelocityX == 0)
                {
                    _dslv.RemoveVelocityX = (mFloatLocX < 0 ? -1 : 1) * minVelocity;
                }
                else
                {
                    minVelocity *= 2;
                    if (_dslv.RemoveVelocityX < 0 && _dslv.RemoveVelocityX > -minVelocity)
                        _dslv.RemoveVelocityX = -minVelocity;
                    else if (_dslv.RemoveVelocityX > 0 && _dslv.RemoveVelocityX < minVelocity)
                        _dslv.RemoveVelocityX = minVelocity;
                }
            }
            else
            {
                _dslv.DestroyFloatView();
            }
        }

        public override void OnUpdate(float frac, float smoothFrac)
        {
            base.OnUpdate(frac, smoothFrac);

            float f = 1f - smoothFrac;

            int firstVis = _dslv.FirstVisiblePosition;
            View item = _dslv.GetChildAt(mFirstPos - firstVis);
            ViewGroup.LayoutParams lp;
            int blank;

            if (_dslv.UseRemoveVelocity)
            {
                float dt = (float)(SystemClock.UptimeMillis() - _startTime) / 1000;
                if (dt == 0)
                    return;
                float dx = _dslv.RemoveVelocityX * dt;
                int w = _dslv.Width;
                _dslv.RemoveVelocityX += (_dslv.RemoveVelocityX > 0 ? 1 : -1) * dt * w;
                mFloatLocX += dx;
                _dslv.FloatLoc.X = (int)mFloatLocX;
                if (mFloatLocX < w && mFloatLocX > -w)
                {
                    _startTime = SystemClock.UptimeMillis();
                    _dslv.DoDragFloatView(true);
                    return;
                }
            }

            if (item != null)
            {
                if (mFirstChildHeight == -1)
                {
                    mFirstChildHeight = _dslv.GetChildHeight(mFirstPos, item, false);
                    mFirstStartBlank = (float)(item.Height - mFirstChildHeight);
                }
                blank = Math.Max((int)(f * mFirstStartBlank), 1);
                lp = item.LayoutParameters;
                lp.Height = mFirstChildHeight + blank;
                item.LayoutParameters = lp;
            }
            if (mSecondPos != mFirstPos)
            {
                item = _dslv.GetChildAt(mSecondPos - firstVis);
                if (item != null)
                {
                    if (mSecondChildHeight == -1)
                    {
                        mSecondChildHeight = _dslv.GetChildHeight(mSecondPos, item, false);
                        mSecondStartBlank = (float)(item.Height - mSecondChildHeight);
                    }
                    blank = Math.Max((int)(f * mSecondStartBlank), 1);
                    lp = item.LayoutParameters;
                    lp.Height = mSecondChildHeight + blank;
                    item.LayoutParameters = lp;
                }
            }
        }

        //    @Override
        //    public void onStop() {
        //        doRemoveItem();
        //    }

        #endregion
    }
}