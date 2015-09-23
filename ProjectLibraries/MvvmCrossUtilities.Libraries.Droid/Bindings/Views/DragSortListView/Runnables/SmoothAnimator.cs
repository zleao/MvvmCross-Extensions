using Android.OS;
using Java.Lang;
using System;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views.DragSortListView.Runnables
{
    class SmoothAnimator : Java.Lang.Object, IRunnable
    {
        #region Fields

        protected long _startTime;
        private float _durationF;
        private float _alpha;
        private float _A, _B, _C, _D;
        private bool _canceled;

        protected readonly AbstractDragSortListView _dslv;

        #endregion

        #region Constructor

        public SmoothAnimator(AbstractDragSortListView dslv, float smoothness, int duration)
        {
            if (dslv == null)
                throw new ArgumentNullException("dslv");

            _dslv = dslv;

            _alpha = smoothness;
            _durationF = (float)duration;
            _A = _D = 1f / (2f * _alpha * (1f - _alpha));
            _B = _alpha / (2f * (_alpha - 1f));
            _C = 1f / (1f - _alpha);
        }

        #endregion

        #region Methods

        public float Transform(float frac)
        {
            if (frac < _alpha)
            {
                return _A * frac * frac;
            }
            else if (frac < 1f - _alpha)
            {
                return _B + _C * frac;
            }
            else
            {
                return 1f - _D * (frac - 1f) * (frac - 1f);
            }
        }

        public void Start()
        {
            _startTime = SystemClock.UptimeMillis();
            _canceled = false;
            OnStart();
            _dslv.Post(this);
        }

        public void Cancel()
        {
            _canceled = true;
        }

        public virtual void OnStart()
        {
            // stub
        }

        public virtual void OnUpdate(float frac, float smoothFrac)
        {
            // stub
        }

        public virtual void OnStop()
        {
            // stub
        }

        public void Run()
        {
            if (_canceled)
            {
                return;
            }

            float fraction = ((float)(SystemClock.UptimeMillis() - _startTime)) / _durationF;

            if (fraction >= 1f)
            {
                OnUpdate(1f, 1f);
                OnStop();
            }
            else
            {
                OnUpdate(fraction, Transform(fraction));
                _dslv.Post(this);
            }
        }
        
        #endregion
    }
}