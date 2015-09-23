using MvvmCrossUtilities.Libraries.Droid.Bindings.Views.DragSortListView.Enums;

namespace MvvmCrossUtilities.Libraries.Droid.Bindings.Views.DragSortListView.Runnables
{
    /// <summary>
    /// Centers floating View under touch point.
    /// </summary>
    class LiftAnimator : SmoothAnimator
    {
        #region Fields

        private float _initDragDeltaY;
        private float _finalDragDeltaY;
        
        #endregion

        #region Constructor

        public LiftAnimator(AbstractDragSortListView dslv, float smoothness, int duration)
            : base(dslv, smoothness, duration)
        {
        }
        
        #endregion

        #region Methods

        public override void OnStart()
        {
            base.OnStart();

            _initDragDeltaY = _dslv.DragDeltaY;
            _finalDragDeltaY = _dslv.FloatViewHeightHalf;
        }

        public override void OnUpdate(float frac, float smoothFrac)
        {
            base.OnUpdate(frac, smoothFrac);

            if (_dslv.DragState != DragStateEnum.Dragging)
            {
                Cancel();
            }
            else
            {
                _dslv.DragDeltaY = (int)(smoothFrac * _finalDragDeltaY + (1f - smoothFrac)* _initDragDeltaY);
                _dslv.FloatLoc.Y = _dslv.TouchY - _dslv.DragDeltaY;
                _dslv.DoDragFloatView(true);
            }
        }
        
        #endregion
    }
}